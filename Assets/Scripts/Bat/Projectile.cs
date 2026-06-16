using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    private Vector3 moveDirection;
    private int damage;
    private float speed;
    private float lifetime;
    private float spawnTime;

    private readonly HashSet<Collider2D> spawnOverlap = new HashSet<Collider2D>();

    public void Initialize(Vector3 direction, int damage, float speed, float lifetime)
    {
        moveDirection = direction.normalized;
        this.damage = damage;
        this.speed = speed;
        this.lifetime = lifetime;
        spawnTime = Time.time;

        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider != null)
        {
            ContactFilter2D filter = new ContactFilter2D();
            filter.useTriggers = false;

            List<Collider2D> overlapping = new List<Collider2D>();
            myCollider.Overlap(filter, overlapping);

            foreach (Collider2D col in overlapping)
            {
                spawnOverlap.Add(col);
            }
        }
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;

        if (Time.time - spawnTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<Player>() is Player player)
        {
            player.TakeDamage(transform, damage);
            Destroy(gameObject);
            return;
        }

        if (collision.GetComponentInParent<EnemyEntity>() != null) return;
        if (collision.GetComponentInParent<MushroomEntity>() != null) return;
        if (collision.isTrigger) return;
        if (spawnOverlap.Contains(collision)) return;

        Destroy(gameObject);
    }
}
