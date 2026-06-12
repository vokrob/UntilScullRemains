using System;
using UnityEngine;
using UnityEngine.AI;
using ChoosingDirection.Utils;

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
    [SerializeField] private float chasingDistance = 8f;
    [SerializeField] private float chasingSpeedMultiplier = 2f;

    [Header("Melee Attack")]
    [SerializeField] private float meleeAttackDistance = 2f;
    [SerializeField] private float meleeAttackRate = 1.5f;

    [Header("Ranged Attack")]
    [SerializeField] private float rangedAttackDistance = 6f;
    [SerializeField] private float rangedAttackRate = 2.5f;

    private NavMeshAgent navMeshAgent;
    private State currentState;
    private float roamingTimer;
    private Vector3 roamingPosition;
    private Vector3 startPosition;

    private float roamingSpeed;
    private float chasingSpeed;

    private float nextAttackTime = 0f;
    private float nextCheckDirectionTime = 0f;
    private float checkDirectionDuration = 0.1f;
    private Vector3 lastPosition;

    public event EventHandler OnMeleeAttack;
    public event EventHandler OnRangedAttack;

    public bool IsRunning
    {
        get
        {
            return navMeshAgent.velocity != Vector3.zero;
        }
    }

    private enum State
    {
        Idle,
        Roaming,
        Chasing,
        MeleeAttacking,
        RangedAttacking,
        Death
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        currentState = startingState;

        roamingSpeed = navMeshAgent.speed;
        chasingSpeed = navMeshAgent.speed * chasingSpeedMultiplier;
    }

    private void Update()
    {
        StateHandler();
        MovementDirectionHandler();
    }

    public void SetDeathState()
    {
        navMeshAgent.ResetPath();
        currentState = State.Death;
    }

    private void StateHandler()
    {
        switch (currentState)
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
                ChasingTarget();
                CheckCurrentState();
                break;
            case State.MeleeAttacking:
                MeleeAttackingTarget();
                CheckCurrentState();
                break;
            case State.RangedAttacking:
                RangedAttackingTarget();
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

        if (distanceToPlayer <= meleeAttackDistance)
        {
            newState = State.MeleeAttacking;
        }
        else if (distanceToPlayer <= rangedAttackDistance)
        {
            newState = State.RangedAttacking;
        }
        else if (distanceToPlayer <= chasingDistance)
        {
            newState = State.Chasing;
        }

        if (newState != currentState)
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
            else if (newState == State.MeleeAttacking || newState == State.RangedAttacking)
            {
                navMeshAgent.ResetPath();
            }
            currentState = newState;
        }
    }

    private void MeleeAttackingTarget()
    {
        if (Time.time > nextAttackTime)
        {
            OnMeleeAttack?.Invoke(this, EventArgs.Empty);
            nextAttackTime = Time.time + meleeAttackRate;
        }
    }

    private void RangedAttackingTarget()
    {
        if (Time.time > nextAttackTime)
        {
            SpawnProjectile();
            OnRangedAttack?.Invoke(this, EventArgs.Empty);
            nextAttackTime = Time.time + rangedAttackRate;
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
        projectileObj.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void ChasingTarget()
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
            else if (currentState == State.MeleeAttacking || currentState == State.RangedAttacking)
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

    private void ChangeFacingDirection(Vector3 sourcePos, Vector3 targetPos)
    {
        if (sourcePos.x > targetPos.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
