using UnityEngine;
using UnityEngine.AI;
using ChoosingDirection.Utils;
using System;

public class BatAI : MonoBehaviour
{
    [SerializeField] private EnemySO enemySO;
    [SerializeField] private State startingState;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Roaming")]
    [SerializeField] private float roamingDistanceMin = 3f;
    [SerializeField] private float roamingDistanceMax = 7f;
    [SerializeField] private float roamingTimerMax = 2f;

    [Header("Chasing")]
    [SerializeField] private bool isChaisingEnemy = true;
    [SerializeField] private float chasingDistance = 8f;
    [SerializeField] private float chasingSpeedMultiplier = 2.5f;

    [Header("Melee Attack")]
    [SerializeField] private bool isAttackingEnemy = true;
    [SerializeField] private float meleeAttackDistance = 2.5f;

    [Header("Ranged Attack")]
    [SerializeField] private bool isRangedEnemy = true;
    [SerializeField] private float rangedAttackDistance = 7f;

    [Header("Attack")]
    [SerializeField] private float attackRate = 2f;
    private float nextAttackTime = 0f;

    private NavMeshAgent navMeshAgent;
    private State currenState;
    private float roamingTimer;
    private Vector3 roamingPosition;
    private Vector3 startPosition;

    private float roamingSpeed;
    private float chasingSpeed;

    private float nextCheckDirectionTime = 0f;
    private float checkDirectionDuration = 0.1f;
    private Vector3 lastPosition;

    public event EventHandler OnMeleeAttack;
    public event EventHandler OnRangedAttack;

    public bool IsRunning
    {
        get
        {
            if (navMeshAgent.velocity == Vector3.zero)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    private enum State
    {
        Idle,
        Roaming,
        Chasing,
        Attacking,
        Death
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        currenState = startingState;

        roamingSpeed = navMeshAgent.speed;
        chasingSpeed = navMeshAgent.speed * chasingSpeedMultiplier;
    }

    private void Update()
    {
        StateHadler();
        MovementDirectionHandler();
    }

    public void SetDeathState()
    {
        navMeshAgent.ResetPath();
        currenState = State.Death;
    }

    private void StateHadler()
    {
        switch (currenState)
        {
            case State.Roaming:
                roamingTimer -= Time.deltaTime;
                if (roamingTimer < 0)
                {
                    Roaming();
                    roamingTimer = roamingTimerMax;
                }
                CheckCurrentState();
                break;
            case State.Chasing:
                ChasingTagret();
                CheckCurrentState();
                break;
            case State.Attacking:
                AttackingTarget();
                CheckCurrentState();
                break;
            case State.Death:
                break;
            default:
            case State.Idle:
                break;
        }
    }

    private void CheckCurrentState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);
        State newState = State.Roaming;

        if (isChaisingEnemy)
        {
            if (distanceToPlayer <= chasingDistance)
            {
                newState = State.Chasing;
            }
        }

        float attackDistance = 0f;
        if (isAttackingEnemy) attackDistance = Mathf.Max(attackDistance, meleeAttackDistance);
        if (isRangedEnemy) attackDistance = Mathf.Max(attackDistance, rangedAttackDistance);

        if ((isAttackingEnemy || isRangedEnemy) && distanceToPlayer <= attackDistance)
        {
            newState = State.Attacking;
        }

        if (newState != currenState)
        {
            if (newState == State.Chasing)
            {
                navMeshAgent.ResetPath();
                navMeshAgent.speed = chasingSpeed;
            }
            else if (newState == State.Roaming)
            {
                roamingTimer = 0f;
                navMeshAgent.speed = roamingSpeed;
            }
            else if (newState == State.Attacking)
            {
                navMeshAgent.ResetPath();
            }
            currenState = newState;
        }
    }

    private void AttackingTarget()
    {
        if (Time.time > nextAttackTime)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);

            if (isAttackingEnemy && distanceToPlayer <= meleeAttackDistance)
            {
                OnMeleeAttack?.Invoke(this, EventArgs.Empty);
            }
            else if (isRangedEnemy)
            {
                SpawnProjectile();
                OnRangedAttack?.Invoke(this, EventArgs.Empty);
            }

            nextAttackTime = Time.time + attackRate;
        }
    }

    private void SpawnProjectile()
    {
        if (projectilePrefab == null) return;

        Vector3 spawnPosition = transform.position;
        Vector3 direction = (Player.Instance.transform.position - transform.position).normalized;

        GameObject projectileObj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(direction, enemySO.projectileDamage, enemySO.projectileSpeed, enemySO.projectileLifetime);
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectileObj.transform.rotation = Quaternion.Euler(0, 0, angle + 180f);
    }

    private void ChasingTagret()
    {
        navMeshAgent.SetDestination(Player.Instance.transform.position);
    }

    public float GetRoamingAnimationSpeed()
    {
        return navMeshAgent.speed / roamingSpeed;
    }

    private void MovementDirectionHandler()
    {
        if (Time.time > nextCheckDirectionTime)
        {
            if (IsRunning)
            {
                ChangeFacingDirection(lastPosition, transform.position);
            }
            else if (currenState == State.Attacking)
            {
                ChangeFacingDirection(transform.position, Player.Instance.transform.position);
            }

            lastPosition = transform.position;
            nextCheckDirectionTime = Time.time + checkDirectionDuration;
        }
    }

    private void Roaming()
    {
        startPosition = transform.position;
        roamingPosition = GetRoamingPosition();
        navMeshAgent.SetDestination(roamingPosition);
    }

    private Vector3 GetRoamingPosition()
    {
        return startPosition + DirectionUtils.GetRandomDir() * UnityEngine.Random.Range(roamingDistanceMin, roamingDistanceMax);
    }

    private void ChangeFacingDirection(Vector3 soursePos, Vector3 targetPos)
    {
        if (soursePos.x > targetPos.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
