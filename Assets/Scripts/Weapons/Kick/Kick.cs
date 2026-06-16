using System;
using UnityEngine;

public class Kick : MonoBehaviour
{
    [SerializeField] private int damageAmount = 3;
    public static Kick Instance { get; private set; }
    public event EventHandler OnKick;

    private PolygonCollider2D polygonCollider2D;
    private bool isKicking = false;

    private void Awake()
    {
        Instance = this;
        polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    public void Start()
    {
        KickColliderTurnOff();
    }

    public void Attack()
    {
        if (isKicking) return;
        isKicking = true;

        KickColliderTurnOn();
        OnKick?.Invoke(this, EventArgs.Empty);
    }

    public bool IsKicking()
    {
        return isKicking;
    }

    public void FinishKick()
    {
        isKicking = false;
        KickColliderTurnOff();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyEntity enemyEntity))
        {
            enemyEntity.TakeDamage(damageAmount);
        }
    }

    private void KickColliderTurnOff()
    {
        polygonCollider2D.enabled = false;
    }

    private void KickColliderTurnOn()
    {
        polygonCollider2D.enabled = true;
    }
}
