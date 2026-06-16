using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    public event EventHandler OnPlayerDeath;
    public event EventHandler OnFlashBlink;
    public event EventHandler OnMinusLimb;

    [SerializeField] private float movingSpeed = 10f;
    [SerializeField] private float damageRecoveryTime = 0.5f;

    [SerializeField] private int maxHealth = 10;
    public Image[] hearts;
    public Sprite Heart;
    public Sprite BrokenHeart;

    Vector2 inputVector;

    private Rigidbody2D rb;
    private KnockBack knockBack;
    private float minMovingSpeed = 0.1f;
    private bool isRunnig = false;

    private int currentHealth;
    private bool canTakeDamage;
    private bool isAlive;

    private bool isHands = false;
    private bool isLegs = false;
    private bool isHead = false;

    [SerializeField] private Collider2D fullBodyCollider;
    [SerializeField] private Collider2D noHandsCollider;
    [SerializeField] private Collider2D noLegsCollider;
    [SerializeField] private Collider2D headCollider;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        knockBack = GetComponent<KnockBack>();
        currentHealth = maxHealth;
    }

    public void Start()
    {
        InitializeHearts();
        UpdateHearts();
        canTakeDamage = true;
        isAlive = true;

        Sword.Instance.gameObject.SetActive(true);
        LongSword.Instance.gameObject.SetActive(false);
        Staff.Instance.gameObject.SetActive(false);

        SwitchCollider(fullBodyCollider);
        GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;
    }

    private void Update()
    {
        inputVector = GameInput.Instance.GetMovemantVector();
    }
    private void FixedUpdate()
    {
        if (knockBack.IsGettingKnockedBack) return;
        HandleMovement();
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    public bool IsRunning()
    {
        return isRunnig;
    }
    public bool IsHands()
    {
        return isHands;
    }
    public bool IsLegs()
    {
        return isLegs;
    }

    public bool IsHead()
    {
        return isHead;
    }

    public Vector3 GetPlayerScreenPosition()
    {
        Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(transform.position);
        return playerScreenPos;
    }

    public void TakeDamage(Transform damageSource, int damage)
    {
        if (canTakeDamage && isAlive)
        {
            canTakeDamage = false;
            currentHealth = Mathf.Max(0, currentHealth -= damage);
            UpdateHearts();
            knockBack.GetKnockedBack(damageSource);

            OnFlashBlink?.Invoke(this, EventArgs.Empty);

            if (currentHealth <= 0)
            {
                if (!isHead)
                {
                    NextLimb();
                }
                else
                {
                    DetectDeath();
                }
            }

            StartCoroutine(DamageRecoveryRoutine());
        }
    }
    private void GameInput_OnPlayerAttack(object sender, System.EventArgs e)
    {
        if (!isAlive) return;

        if (isHead)
        {
            Vector2 direction = inputVector != Vector2.zero ? inputVector : Vector2.right;
            HeadAttack.Instance.Attack(direction);
        }
        else if (isLegs)
        {
            ActiveWeapon.Instance.GetActiveTorsoAttack().Attack();
        }
        else if (isHands)
        {
            ActiveWeapon.Instance.GetActiveKick().Attack();
        }
        else if (!isHands)
        {
            Sword.Instance.Attack();
        }

    }

    private void NextLimb()
    {
        if (!isHands)
        {
            isHands = true;
            SwitchCollider(noHandsCollider);
            Sword.Instance.gameObject.SetActive(false);
            LongSword.Instance.gameObject.SetActive(false);
            Staff.Instance.gameObject.SetActive(false);
            Kick.Instance.gameObject.SetActive(true);
            currentHealth = maxHealth;
            UpdateHearts();
        }
        else if (!isLegs)
        {
            isLegs = true;
            SwitchCollider(noLegsCollider);
            Kick.Instance.gameObject.SetActive(false);
            TorsoAttack.Instance.gameObject.SetActive(true);
            currentHealth = maxHealth;
            UpdateHearts();
        }
        else if (!isHead)
        {
            isHead = true;
            SwitchCollider(headCollider);
            TorsoAttack.Instance.gameObject.SetActive(false);
            currentHealth = maxHealth;
            UpdateHearts();
        }

        OnMinusLimb?.Invoke(this, EventArgs.Empty);
    }

    private void SwitchCollider(Collider2D newCollider)
    {
        fullBodyCollider.enabled = false;
        noHandsCollider.enabled = false;
        noLegsCollider.enabled = false;
        headCollider.enabled = false;

        if (newCollider != null) newCollider.enabled = true;
    }

    private void DetectDeath()
    {
        if (currentHealth == 0 && isAlive)
        {
            isAlive = false;
            knockBack.StopKnockBackMovement();
            GameInput.Instance.DisableMovement();

            fullBodyCollider.enabled = false;
            noHandsCollider.enabled = false;
            noLegsCollider.enabled = false;
            headCollider.enabled = false;

            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
        }
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    private void HandleMovement()
    {
        bool isAttacking = Kick.Instance.IsKicking();
        float debuffSpeed = isAttacking ? 0f : 1f;
        rb.MovePosition(rb.position + inputVector * (movingSpeed * debuffSpeed * Time.fixedDeltaTime));

        if (Mathf.Abs(inputVector.x) > minMovingSpeed || Mathf.Abs(inputVector.y) > minMovingSpeed)
        {
            isRunnig = true;
        }
        else
        {
            isRunnig = false;
        }
    }

    private void InitializeHearts()
    {
        if (hearts == null || hearts.Length == 0)
        {
            return;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < maxHealth)
            {
                hearts[i].enabled = true;
                hearts[i].sprite = Heart;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

    private void UpdateHearts()
    {
        if (hearts == null) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = Heart;
            }
            else
            {
                hearts[i].sprite = BrokenHeart;
            }

            if (i < maxHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

    private void OnDestroy()
    {
        if (GameInput.Instance != null)
            GameInput.Instance.OnPlayerAttack -= GameInput_OnPlayerAttack;

        if (Instance == this) Instance = null;
    }
}