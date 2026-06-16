using System;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class TorsoAttack : MonoBehaviour
{
    [SerializeField] private int damageAmount = 3;
    public static TorsoAttack Instance { get; private set; }
    public event EventHandler OnTorsoAttack;

    private PolygonCollider2D polygonCollider2D;
    private bool isTorso = false;

    private void Awake()
    {
        Instance = this;
        polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Attack()
    {
        if (isTorso) return;
        isTorso = true;
        gameObject.SetActive(true);
        TorsoColliderTurnOn();
        OnTorsoAttack?.Invoke(this, EventArgs.Empty);
    }

    public bool IsTorso()
    {
        return isTorso;
    }

    public void FinishTorso()
    {
        isTorso = false;
        gameObject.SetActive(false);
        TorsoColliderTurnOff();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyEntity enemyEntity))
        {
            enemyEntity.TakeDamage(damageAmount);
        }
    }


    private void TorsoColliderTurnOff()
    {
        polygonCollider2D.enabled = false;
    }

    private void TorsoColliderTurnOn()
    {
        polygonCollider2D.enabled = true;
    }
}
