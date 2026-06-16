using UnityEngine;
using System;

public class HeadAttack : MonoBehaviour
{
    [SerializeField] private int damageAmount = 2;
    [SerializeField] private float chargeSpeed = 25f;
    [SerializeField] private float lifetime = 1.5f;
    [SerializeField] private float attackCooldown = 2.5f;

    public static HeadAttack Instance { get; private set; }
    public event EventHandler OnAttack;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool isActive = false;
    private float nextAttackTime = 0f;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.gravityScale = 0;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Attack(Vector2 direction)
    {
        if (Time.time < nextAttackTime) return;
        if (isActive) return;

        isActive = true;

        transform.SetParent(null);
        transform.position = Player.Instance.transform.position;

        moveDirection = direction.normalized;
        nextAttackTime = Time.time + attackCooldown;

        gameObject.SetActive(true);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.linearVelocity = moveDirection * chargeSpeed;
        }

        OnAttack?.Invoke(this, EventArgs.Empty);
        Invoke(nameof(Deactivate), lifetime);
    }

    private void Update()
    {
        if (isActive)
        {
            transform.Translate(moveDirection * chargeSpeed * Time.deltaTime);
        }
    }

    public bool IsActive()
    {
        return isActive;
    }

    public bool CanAttack()
    {
        return Time.time >= nextAttackTime && !isActive;
    }

    public void FinishHeadAttack()
    {
        Deactivate();
        CancelInvoke(nameof(Deactivate));
    }

    private void Deactivate()
    {
        isActive = false;
        gameObject.SetActive(false);
        if (rb != null) rb.linearVelocity = Vector2.zero;
        transform.SetParent(Player.Instance.transform);
        transform.localPosition = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyEntity enemyEntity))
        {
            enemyEntity.TakeDamage(damageAmount);
            FinishHeadAttack();
        }
    }
}
