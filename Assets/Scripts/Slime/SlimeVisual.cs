using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent (typeof(SpriteRenderer))]
public class SlimeVisual : MonoBehaviour
{
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private EnemyEntity enemyEntity;
    [SerializeField] private GameObject enemyShadow;

    private Animator animator;

    private const string IS_RUNNING = "IsRunning";
    private const string TAKE_HIT = "TakeHit";
    private const string IS_DIE = "IsDie";
    private const string CHASING_SPEED_MULTIPLIER = "ChasingSpeedMultiplier";
    private const string ATTACK = "Attack";

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        enemyAI.OnEnemyAttack += EnemyAI_OnEnemyAttack;
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
        enemyAI.OnEnemyAttack -= EnemyAI_OnEnemyAttack;
    }

    private void Update()
    {
        animator.SetBool(IS_RUNNING, enemyAI.IsRunning);
        animator.SetFloat(CHASING_SPEED_MULTIPLIER,enemyAI.GetRoamingAnimationSpeed());
    }

    private void EnemyAI_OnEnemyAttack(object sender, System.EventArgs e)
    {
        animator.SetTrigger(ATTACK);
    }
}
