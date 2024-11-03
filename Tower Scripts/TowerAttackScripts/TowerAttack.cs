using System.Collections;
using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    public GameObject bulletPrefab; // Prefab of the bullet to shoot
    public Transform firePoint; // The point from where the bullet will be spawned
    public LayerMask enemyLayer; // Layer mask to identify enemies
    private float damage;
    private float range;
    private float attackSpeed;
    private bool isAttacking;

    // Shooting patterns
    public bool shoot1Bullet = true;
    public bool shoot4Bullets = false;
    public bool shoot8Bullets = false;

    private void Start()
    {
        PlacedTowerStats stats = GetComponent<PlacedTowerStats>();
        if (stats != null)
        {
            damage = stats.GetDamage(); // Use the method to get the current damage
            range = stats.GetRange();     // Use the method to get the current range
            attackSpeed = stats.GetAttackSpeed(); // Use the method to get the current attack speed
        }
        else
        {
            Debug.LogError("PlacedTowerStats component is missing from this tower.");
        }

        // Draw the range circle and remove the CircleCollider2D afterwards
        DrawAttackRange();
    }

    private void Update()
    {
        // Update range and attack speed dynamically
        PlacedTowerStats stats = GetComponent<PlacedTowerStats>();
        if (stats != null)
        {
            damage = stats.GetDamage();
            range = stats.GetRange();
            attackSpeed = stats.GetAttackSpeed();
        }

        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);

        if (enemiesInRange.Length > 0 && !isAttacking)
        {
            StartCoroutine(Attack(enemiesInRange[0].transform)); // Attack the first enemy in range
        }
    }

    private IEnumerator Attack(Transform enemy)
    {
        isAttacking = true;
        while (enemy != null && Vector2.Distance(transform.position, enemy.position) <= range)
        {
            Shoot(enemy); // Shoot at the enemy
            yield return new WaitForSeconds(1f / attackSpeed); // Wait based on the attack speed
        }
        isAttacking = false;
    }

    private void Shoot(Transform enemy)
    {
        if (bulletPrefab != null && firePoint != null)
        {
            if (shoot1Bullet)
            {
                FireBulletInDirection(enemy.position, 0); // Shoot straight at the enemy
            }
            else if (shoot4Bullets)
            {
                // 0, 90, 180, and 270 degrees relative to the direction to the enemy
                FireBulletInDirection(enemy.position, 0);   // Straight
                FireBulletInDirection(enemy.position, 90);  // 90 degrees
                FireBulletInDirection(enemy.position, 180); // 180 degrees
                FireBulletInDirection(enemy.position, 270); // 270 degrees
            }
            else if (shoot8Bullets)
            {
                for (int i = 0; i < 8; i++)
                {
                    FireBulletInDirection(enemy.position, i * 45); // 45 degrees increment
                }
            }
        }
        else
        {
            Debug.LogError("BulletPrefab or FirePoint is not assigned.");
        }
    }

    private void FireBulletInDirection(Vector3 targetPosition, float angle)
    {
        // Instantiate the bullet and set its direction
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            // Calculate direction to the target
            Vector2 direction = (targetPosition - firePoint.position).normalized;

            // Rotate the direction by the specified angle
            direction = RotateDirection(direction, angle);

            // Pass the direction and damage to the bullet
            bulletScript.SetDirection(direction, damage);
        }
    }

    private Vector2 RotateDirection(Vector2 direction, float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        float cosAngle = Mathf.Cos(radians);
        float sinAngle = Mathf.Sin(radians);
        return new Vector2(cosAngle * direction.x - sinAngle * direction.y,
                           sinAngle * direction.x + cosAngle * direction.y);
    }

    // Visualize the attack range in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    private void DrawAttackRange()
    {
        CircleCollider2D rangeCollider = gameObject.AddComponent<CircleCollider2D>();
        rangeCollider.isTrigger = true;
        rangeCollider.radius = range;

        // Remove the collider after a frame
        Destroy(rangeCollider, 0f);
    }
}
