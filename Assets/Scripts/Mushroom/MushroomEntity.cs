using System;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(MushroomAI))]
public class MushroomEntity : MonoBehaviour
{
    [SerializeField] private EnemySO enemySO;

    public event EventHandler OnTakeHit;
    public event EventHandler OnDeath;

    private int currentHealth;

    private PolygonCollider2D polygonCollider2D;
    private BoxCollider2D boxCollider2D;
    private MushroomAI mushroomAI;

    private void Awake()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        mushroomAI = GetComponent<MushroomAI>();
    }

    public MushroomAI GetMushroomAI()
    {
        return mushroomAI;
    }

    public EnemySO GetEnemySO()
    {
        return enemySO;
    }

    private void Start()
    {
        currentHealth = enemySO.enemyHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        OnTakeHit?.Invoke(this, EventArgs.Empty);
        DetectDeath();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            player.TakeDamage(enemySO.enemyDamageAmount);
        }
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

            mushroomAI.SetDeathState();
            OnDeath?.Invoke(this, EventArgs.Empty);
        }
    }
}
