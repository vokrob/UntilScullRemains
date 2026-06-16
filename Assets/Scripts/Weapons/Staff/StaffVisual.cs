using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class StaffVisual : MonoBehaviour
{
    [SerializeField] private Staff staff;
    private Animator animator;
    private const string IS_CAST = "IsCast";
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (staff != null)
        {
            staff.OnStaffCast += Staff_OnStaffCast;
        }
    }

    private void Staff_OnStaffCast(object sender, EventArgs e)
    {
        animator.SetTrigger(IS_CAST);
    }

    private void OnDestroy()
    {
        if (staff != null)
        {
            staff.OnStaffCast -= Staff_OnStaffCast;
        }
    }
}
