using UnityEngine;

[RequireComponent(typeof(Animator))]
public class StaffProjectile : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float chargeSpeed = 25f;
    [SerializeField] private float lifetime = 1.5f;
    [SerializeField] private float yOffset = 1.2f;

    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool isActive = false;
    private const string IS_CAST = "IsCastStaff";

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.gravityScale = 0;
    }
    private void Update()
    {
        if (isActive)
        {
            transform.Translate(moveDirection * chargeSpeed * Time.deltaTime);
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Launch(Vector2 direction)
    {
        if (isActive) return;

        isActive = true;

        transform.SetParent(null);

        transform.position = Player.Instance.transform.position + new Vector3(0, yOffset, 0);

        moveDirection = direction.normalized;
        gameObject.SetActive(true);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.linearVelocity = moveDirection * chargeSpeed;
        }

        if (animator != null)
        {
            animator.SetTrigger(IS_CAST);
        }

        Invoke(nameof(Deactivate), lifetime);
    }


    public void FinishHeadAttack()
    {
        FinishProjectile();
    }

    public void FinishProjectile()
    {
        Deactivate();
        CancelInvoke(nameof(Deactivate));
    }

    private void Deactivate()
    {
        isActive = false;
        gameObject.SetActive(false);
        if (rb != null) rb.linearVelocity = Vector2.zero;

        if (Staff.Instance != null)
        {
            transform.SetParent(Staff.Instance.transform);
        }
        transform.localPosition = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyEntity enemyEntity))
        {
            enemyEntity.TakeDamage(damageAmount);
            FinishProjectile();
        }
    }
}
