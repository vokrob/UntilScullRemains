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

    public void Initialize(Vector3 direction, int damage, float speed, float lifetime)
    {
        moveDirection = direction.normalized;
        this.damage = damage;
        this.speed = speed;
        this.lifetime = lifetime;
        spawnTime = Time.time;
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
        if (collision.TryGetComponent(out Player player))
        {
            Debug.Log($"Projectile hit player for {damage} damage");
            Destroy(gameObject);
        }
    }
}
