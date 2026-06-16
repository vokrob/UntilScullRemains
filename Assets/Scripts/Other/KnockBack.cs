using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KnockBack : MonoBehaviour
{
    [SerializeField] private float knockBackForce = 3f;
    [SerializeField] private float knockBackMovingTimerMax = 0.3f;

    private float knockBackMovingTimer;

    private Rigidbody2D rb;

    public bool IsGettingKnockedBack { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        knockBackMovingTimer -= Time.deltaTime;
        if (knockBackMovingTimer < 0)
        {
            StopKnockBackMovement();
        }
    }

    public void GetKnockedBack(Transform damageSource)
    {
        IsGettingKnockedBack = true;
        knockBackMovingTimer = knockBackMovingTimerMax;
        Vector2 difference = (transform.position - damageSource.position).normalized * knockBackForce;
        rb.AddForce(difference, ForceMode2D.Impulse);
    }

    public void StopKnockBackMovement()
    {
        rb.linearVelocity = Vector2.zero;
        IsGettingKnockedBack = false;
    }
}
