using UnityEngine;

[RequireComponent(typeof(Animator))]
public class KickVisual : MonoBehaviour
{
    [SerializeField] private Kick kick;
    private Animator animator;
    private const string IS_KICK = "IsKick";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        kick.OnKick += Kick_OnKick;
    }

    private void Kick_OnKick(object sender, System.EventArgs e)
    {
        animator.SetTrigger(IS_KICK);
    }

    public void EndTorsoAnimation()
    {
        kick.FinishKick();
    }

    private void OnDestroy()
    {
        kick.OnKick -= Kick_OnKick;
    }
}
