using UnityEngine;

[RequireComponent (typeof(Animator))]
public class TorsoAttackVisual : MonoBehaviour
{
    [SerializeField] private TorsoAttack torsoAttack;
    private Animator animator;
    private const string IS_TORSO = "IsTorso";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {    
            torsoAttack.OnTorsoAttack += Torso_OnTorso;
    }

    private void Torso_OnTorso(object sender, System.EventArgs e)
    {
        animator.SetTrigger(IS_TORSO);
    }

    public void EndTorsoAnimation()
    {
        torsoAttack.FinishTorso();
    }

    private void OnDestroy()
    {
        torsoAttack.OnTorsoAttack -= Torso_OnTorso;
    }
}
