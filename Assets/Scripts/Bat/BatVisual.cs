using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class BatVisual : MonoBehaviour
{
    [SerializeField] private BatAI batAI;
    [SerializeField] private EnemyEntity enemyEntity;
    [SerializeField] private GameObject enemyShadow;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private const string IS_RUNNING = "IsRunning";
    private const string TAKE_HIT = "TakeHit";
    private const string IS_DIE = "IsDie";
    private const string CHASING_SPEED_MULTIPLIER = "ChasingSpeedMultiplier";
    private const string MELEE_ATTACK = "MeleeAttack";
    private const string RANGED_ATTACK = "RangedAttack";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        batAI.OnMeleeAttack += BatAI_OnMeleeAttack;
        batAI.OnRangedAttack += BatAI_OnRangedAttack;
        enemyEntity.OnTakeHit += EnemyEntity_OnTakeHit;
        enemyEntity.OnDeath += EnemyEntity_OnDeath;
    }

    public void TrigerAttackAnimationTurnOff()
    {
        enemyEntity.PolygonColliderTurnOff();
    }

    public void TrigerAttackAnimationTurnOn()
    {
        enemyEntity.PolygonColliderTurnOn();
    }

    public void TrigerDeathAnimation()
    {
        enemyEntity.DeathAnimation();
    }

    private void EnemyEntity_OnDeath(object sender, System.EventArgs e)
    {
        animator.SetBool(IS_DIE, true);
        spriteRenderer.sortingOrder = -1;
        enemyShadow.SetActive(false);
    }

    private void EnemyEntity_OnTakeHit(object sender, System.EventArgs e)
    {
        animator.SetTrigger(TAKE_HIT);
    }

    private void OnDestroy()
    {
        batAI.OnMeleeAttack -= BatAI_OnMeleeAttack;
        batAI.OnRangedAttack -= BatAI_OnRangedAttack;
    }

    private void Update()
    {
        animator.SetBool(IS_RUNNING, batAI.IsRunning);
        animator.SetFloat(CHASING_SPEED_MULTIPLIER, batAI.GetRoamingAnimationSpeed());
    }

    private void BatAI_OnMeleeAttack(object sender, System.EventArgs e)
    {
        animator.SetTrigger(MELEE_ATTACK);
    }

    private void BatAI_OnRangedAttack(object sender, System.EventArgs e)
    {
        animator.SetTrigger(RANGED_ATTACK);
    }
}
