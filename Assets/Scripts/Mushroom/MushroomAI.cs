using System;
using UnityEngine;

public class MushroomAI : MonoBehaviour
{
    [SerializeField] private EnemySO enemySO;
    [SerializeField] private State startingState;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Ranged Attack")]
    [SerializeField] private float rangedAttackDistance = 6f;
    [SerializeField] private float rangedAttackRate = 2.5f;

    private State currentState;
    private float nextAttackTime = 0f;
    private float nextCheckDirectionTime = 0f;
    private float checkDirectionDuration = 0.1f;

    public event EventHandler OnRangedAttack;

    private enum State
    {
        Idle,
        RangedAttacking,
        Death
    }

    private void Awake()
    {
        currentState = startingState;
    }

    private void Update()
    {
        StateHandler();
        FacingDirectionHandler();
    }

    public void SetDeathState()
    {
        currentState = State.Death;
    }

    private void StateHandler()
    {
        switch (currentState)
        {
            case State.RangedAttacking:
                RangedAttackingTarget();
                CheckCurrentState();
                break;
            case State.Death:
                break;
            default:
            case State.Idle:
                CheckCurrentState();
                break;
        }
    }

    private void CheckCurrentState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);
        State newState = State.Idle;

        if (distanceToPlayer <= rangedAttackDistance)
        {
            newState = State.RangedAttacking;
        }

        if (newState != currentState)
        {
            currentState = newState;
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
        if (projectilePrefab == null)
        {
            return;
        }

        Vector3 spawnPosition = transform.position;
        Vector3 direction = (Player.Instance.transform.position - transform.position).normalized;

        GameObject projectileObj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(direction, enemySO.projectileDamage, enemySO.projectileSpeed, enemySO.projectileLifetime);
        }
        else
        {
            Debug.LogWarning("[MushroomAI] Projectile component not found on spawned projectile!");
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectileObj.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void FacingDirectionHandler()
    {
        if (Time.time > nextCheckDirectionTime)
        {
            if (currentState == State.RangedAttacking)
            {
                ChangeFacingDirection(transform.position, Player.Instance.transform.position);
            }

            nextCheckDirectionTime = Time.time + checkDirectionDuration;
        }
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
