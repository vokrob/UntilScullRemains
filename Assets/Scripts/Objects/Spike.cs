using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Spike : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other is CapsuleCollider2D && other.GetComponentInParent<Player>() is Player player)
        {
            player.TakeDamage(transform, damage);
        }
    }
}
