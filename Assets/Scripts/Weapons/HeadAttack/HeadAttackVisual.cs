using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HeadAttackVisual : MonoBehaviour
{
    [SerializeField] private HeadAttack headAttack;
    private Animator animator;
    private const string HEADATTACK = "HeadAttack";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        headAttack.OnAttack += HeadAttack_OnAttack;
    }

    public void FinishHeadAttack()
    {
        headAttack.FinishHeadAttack();
    }

    private void HeadAttack_OnAttack(object sender, EventArgs e)
    {
        animator.SetTrigger(HEADATTACK);
    }

    private void OnDestroy()
    {
        headAttack.OnAttack -= HeadAttack_OnAttack;
    }
}
