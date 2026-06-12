using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private const string IS_RUNNING = "IsRunnig";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        animator.SetBool(IS_RUNNING, Player.Instance.IsRunning());
        KeybordFacingDirection();
    }

    // Отражение по клавишам
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

    // Отражение по мышке и позиции персонажа
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


}
