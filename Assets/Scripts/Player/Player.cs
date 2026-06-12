using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private float movingSpeed = 10f;
    Vector2 inputVector;

    private Rigidbody2D rb;
    private float minMovingSpeed = 0.1f;
    private bool isRunnig = false;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
    }

    public void Start()
    {
        GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;
    }

    private void GameInput_OnPlayerAttack(object sender, System.EventArgs e)
    {
        ActiveWeapon.Instance.GetActiveWeapon().Attack();
    }

    private void Update()
    {
        inputVector = GameInput.Instance.GetMovemantVector();
    }
    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        rb.MovePosition(rb.position + inputVector * (movingSpeed * Time.fixedDeltaTime));

        if (Mathf.Abs(inputVector.x) > minMovingSpeed || Mathf.Abs(inputVector.y) > minMovingSpeed)
        {
            isRunnig = true;
        }
        else
        {
            isRunnig = false;
        }
    }

    public bool IsRunning()
    {
        return isRunnig;
    }

    public Vector3 GetPlayerScreenPosition()
    {
        Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(transform.position);
        return playerScreenPos;
    }
}