using UnityEngine;

public class SwordVisual : MonoBehaviour
{
    [SerializeField] private Sword sword;

    private Animator animator;
    private const string ATTACK = "isAttack";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Sword.Instance.IsAttack() != animator.GetBool(ATTACK))
            animator.SetBool(ATTACK, Sword.Instance.IsAttack());
    }

    public void EndAttackAnimation()
    {
            Sword.Instance.FinishAttack(); 
    }

    //private void Start()
    //{
    //    sword.OnSwordSwing += Sword_OnSwordSwing;
    //}

    //private void Sword_OnSwordSwing(object sender, System.EventArgs e)
    //{
    //    animator.SetBool(ATTACK, Sword.Instance.IsAttack());
    //}
}
