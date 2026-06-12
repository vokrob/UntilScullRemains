using UnityEngine;

[CreateAssetMenu()]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    public int enemyHealth;
    public int enemyDamageAmount;

    public bool isRangedEnemy;
    public float rangedAttackDistance = 6f;
    public int projectileDamage = 1;
    public float projectileSpeed = 5f;
    public float projectileLifetime = 3f;
}
