using System;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(PolygonCollider2D))]
public class LongSword : MonoBehaviour
{
    [SerializeField] private int damageAmount = 5;
    public static LongSword Instance { get; private set; }

    public event EventHandler OnLongSwordSwing;

    private PolygonCollider2D polygonCollider2D;

    private bool isAttack = false;

    private void Awake()
    {
        Instance = this;
        polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    public void Start()
    {
        AttackColliderTurnOff();
    }

    public void Attack()
    {
        AttackColliderTurnOn();
        if (isAttack) return;
        isAttack = true;
        OnLongSwordSwing?.Invoke(this, EventArgs.Empty);
    }

    public bool IsAttack()
    {
        return isAttack;
    }

    public void FinishAttack()
    {
        isAttack = false;
        AttackColliderTurnOff();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyEntity enemyEntity))
        {
            enemyEntity.TakeDamage(damageAmount);
        }
        else if (collision.TryGetComponent(out MushroomEntity mushroomEntity))
        {
            mushroomEntity.TakeDamage(damageAmount);
        }
    }

    private void AttackColliderTurnOff()
    {
        polygonCollider2D.enabled = false;
    }

    private void AttackColliderTurnOn()
    {
        polygonCollider2D.enabled = true;
    }
}
