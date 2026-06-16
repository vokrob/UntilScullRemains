using System;
using UnityEngine;

public class Staff : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float manaCost = 20f;
    [SerializeField] private StaffProjectile staffProjectile;

    public static Staff Instance { get; private set; }
    public event EventHandler OnStaffCast;

    private float nextAttackTime = 0f;
    private bool isCasting = false;

    private void Awake()
    {
        Instance = this;
    }

    public void Attack(Vector2 direction)
    {
        if (!CanAttack()) return;

        // Проверяем и тратим ману
        if (!ManaSystem.Instance.TryUseMana(manaCost)) return;

        isCasting = true;
        nextAttackTime = Time.time + attackCooldown;

        OnStaffCast?.Invoke(this, EventArgs.Empty);

        if (staffProjectile != null)
        {
            staffProjectile.Launch(direction);
        }

        Invoke(nameof(ResetCast), 0.3f);
    }

    private void ResetCast()
    {
        isCasting = false;
    }

    public bool CanAttack()
    {
        return Time.time >= nextAttackTime && !isCasting && ManaSystem.Instance.GetCurrentMana() >= manaCost;
    }
}
