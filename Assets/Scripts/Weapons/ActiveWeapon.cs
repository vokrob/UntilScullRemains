using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    public static ActiveWeapon Instance { get; private set; }

    [SerializeField] private Sword sword;
    [SerializeField] private TorsoAttack torsoAttack;
    [SerializeField] private Kick kick;
    [SerializeField] private HeadAttack headAttack;
    [SerializeField] private LongSword longSword;
    [SerializeField] private Staff staff;

    public void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Player.Instance.IsAlive())
            WeaponPosition();
    }

    public Sword GetActiveSword()
    {
        return sword;
    }

    public TorsoAttack GetActiveTorsoAttack()
    {
        return torsoAttack;
    }

    public Kick GetActiveKick()
    {
        return kick;
    }

    public HeadAttack GetActiveHeadAttack()
    {
        return headAttack;
    }

    public LongSword GetActiveLongSword()
    {
        return longSword;
    }

    public Staff GetActiveStaff()
    {
        return staff;
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