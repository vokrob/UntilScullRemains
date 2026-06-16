using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class MushroomVisual : MonoBehaviour
{
    [SerializeField] private MushroomAI mushroomAI;
    [SerializeField] private MushroomEntity mushroomEntity;
    [SerializeField] private GameObject enemyShadow;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private const string TAKE_HIT = "TakeHit";
    private const string IS_DIE = "IsDie";
    private const string ATTACK = "Attack";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        mushroomAI.OnRangedAttack += MushroomAI_OnRangedAttack;
        mushroomEntity.OnTakeHit += MushroomEntity_OnTakeHit;
        mushroomEntity.OnDeath += MushroomEntity_OnDeath;
    }

    public void TrigerAttackAnimationTurnOn()
    {
        mushroomEntity.PolygonColliderTurnOn();
    }

    public void TrigerAttackAnimationTurnOff()
    {
        mushroomEntity.PolygonColliderTurnOff();
    }

    public void TrigerDeathAnimation()
    {
        mushroomEntity.DeathAnimation();
    }

    private void MushroomEntity_OnDeath(object sender, System.EventArgs e)
    {
        animator.SetBool(IS_DIE, true);
        enemyShadow.SetActive(false);
    }

    private void MushroomEntity_OnTakeHit(object sender, System.EventArgs e)
    {
        animator.SetTrigger(TAKE_HIT);
    }

    private void OnDestroy()
    {
        mushroomAI.OnRangedAttack -= MushroomAI_OnRangedAttack;
        mushroomEntity.OnTakeHit -= MushroomEntity_OnTakeHit;
        mushroomEntity.OnDeath -= MushroomEntity_OnDeath;
    }

    private void MushroomAI_OnRangedAttack(object sender, System.EventArgs e)
    {
        animator.SetTrigger(ATTACK);
    }
}
