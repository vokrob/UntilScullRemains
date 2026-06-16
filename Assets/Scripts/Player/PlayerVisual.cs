using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.WSA;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(FlashBlink))]
public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private FlashBlink flashBlink;

    private const string IS_RUNNING = "IsRunnig";
    private const string IS_DIE = "IsDie";
    private const string LOSE_LIMBS = "LoseLimbs";
    private const string IS_HANDS = "IsHands";
    private const string IS_LEGS = "IsLegs";
    private const string IS_HEAD = "IsHead";
    private const string IS_KICK = "IsKick";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        flashBlink = GetComponent<FlashBlink>();
    }

    private void Start()
    {
        Player.Instance.OnPlayerDeath += Player_OnPlayerDeath;
        Player.Instance.OnMinusLimb += Player_OnMinusLimb;
        Kick.Instance.OnKick += Kick_OnKick;
    }

    private void Kick_OnKick(object sender, EventArgs e)
    {
        animator.SetTrigger(IS_KICK);
    }

    private void Player_OnMinusLimb(object sender, System.EventArgs e)
    {
        animator.SetTrigger(LOSE_LIMBS);

        if (Player.Instance.IsHands())
            animator.SetBool(IS_HANDS, true);

        if (Player.Instance.IsLegs())
            animator.SetBool(IS_LEGS, true);

        if (Player.Instance.IsHead())
            animator.SetBool(IS_HEAD, true);
    }

    private void Player_OnPlayerDeath(object sender, System.EventArgs e)
    {
        animator.SetBool(IS_DIE, true);
        flashBlink.StopBlinking();
    }

    private void Update()
    {
        animator.SetBool(IS_RUNNING, Player.Instance.IsRunning());
        if (Player.Instance.IsAlive())
            KeybordFacingDirection();
    }

    private void KeybordFacingDirection()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput < 0f)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveInput > 0f)
        {
            spriteRenderer.flipX = false;
        }
    }

    private void AdjustFacingDirection()
    {
        Vector3 mousePos = GameInput.Instance.GetMousePosition();
        Vector3 playerPos = Player.Instance.GetPlayerScreenPosition();

        if (mousePos.x < playerPos.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    public void FinishKickAnimation()
    {
        Kick.Instance.FinishKick();
    }

    private void OnDestroy()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnPlayerDeath -= Player_OnPlayerDeath;
            Player.Instance.OnMinusLimb -= Player_OnMinusLimb;
            Kick.Instance.OnKick -= Kick_OnKick;
        }
    }
}
