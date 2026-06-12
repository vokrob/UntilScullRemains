using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    public static ActiveWeapon Instance { get; private set; }

    [SerializeField] private Sword sword;

    public void Awake()
    {
        Instance = this;
    }

    public Sword GetActiveWeapon()
    {
        return sword;
    }

    private void Update()
    {
        WeaponPosition();
    }

    private void WeaponPosition()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput < 0f)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (moveInput > 0f)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
}