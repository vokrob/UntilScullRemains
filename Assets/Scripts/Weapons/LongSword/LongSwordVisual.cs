using UnityEngine;

[RequireComponent (typeof(Animator))]
public class LongSwordVisual : MonoBehaviour
{
    [SerializeField] private LongSword longSword;

    private Animator animator;
    private const string ATTACK = "isAttack";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (LongSword.Instance.IsAttack() != animator.GetBool(ATTACK))
            animator.SetBool(ATTACK, LongSword.Instance.IsAttack());
    }

    public void EndAttackAnimation()
    {
        LongSword.Instance.FinishAttack();
    }
}
