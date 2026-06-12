using System;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class EnemyEntity : MonoBehaviour
{
    [SerializeField] private EnemySO enemySO;

    public event EventHandler OnTakeHit;
    public event EventHandler OnDeath;

    private int currentHealth;

    private PolygonCollider2D polygonCollider2D;
    private BoxCollider2D boxCollider2D;
    private EnemyAI enemyAI;

    private void Awake()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        enemyAI = GetComponent<EnemyAI>();
    }

    public EnemyAI GetEnemyAI()
    {
        return enemyAI;
    }

    public EnemySO GetEnemySO()
    {
        return enemySO;
    }

    private void Start()
    {
        currentHealth = enemySO.enemyHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            player.TakeDamage(enemySO.enemyDamageAmount);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        OnTakeHit?.Invoke(this, EventArgs.Empty);
        DetectDeath();
    }

    public void PolygonColliderTurnOn()
    {
        polygonCollider2D.enabled = true;
    }

    public void PolygonColliderTurnOff()
    {
        polygonCollider2D.enabled = false;
    }

    public void DeathAnimation()
    {
        Destroy(gameObject);
    }

    private void DetectDeath()
    {
        if (currentHealth <= 0)
        {
            boxCollider2D.enabled = false;
            polygonCollider2D.enabled = false;

            if (enemyAI != null)
            {
                enemyAI.SetDeathState();
            }
            OnDeath?.Invoke(this, EventArgs.Empty);
        }
    }
}
