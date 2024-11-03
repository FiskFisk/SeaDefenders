using System.Collections;
using UnityEngine;

public class SquidTowerAttack : MonoBehaviour
{
    public LayerMask enemyLayer; // Layer mask to identify enemies
    private float damage;
    private float range;
    private float attackSpeed;
    private bool isAttacking;

    private void Start()
    {
        // Fetch tower stats
        PlacedTowerStats stats = GetComponent<PlacedTowerStats>();
        if (stats != null)
        {
            damage = stats.GetDamage();
            range = stats.GetRange();
            attackSpeed = stats.GetAttackSpeed();
        }
        else
        {
            Debug.LogError("PlacedTowerStats component is missing from this tower.");
        }

        // Visualize range
        DrawAttackRange();
    }

    private void Update()
    {
        // Update dynamic stats
        PlacedTowerStats stats = GetComponent<PlacedTowerStats>();
        if (stats != null)
        {
            damage = stats.GetDamage();
            range = stats.GetRange();
            attackSpeed = stats.GetAttackSpeed();
        }

        // Find enemies in range
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);

        if (enemiesInRange.Length > 0 && !isAttacking)
        {
            StartCoroutine(Attack(enemiesInRange[0].GetComponent<EnemyInformation>())); // Attack the first enemy in range
        }
    }

    private IEnumerator Attack(EnemyInformation enemy)
    {
        isAttacking = true;
        while (enemy != null && Vector2.Distance(transform.position, enemy.transform.position) <= range)
        {
            enemy.TakeDamage(damage); // Direct damage to the enemy
            yield return new WaitForSeconds(1f / attackSpeed); // Wait based on attack speed
        }
        isAttacking = false;
    }

    // Visualize attack range in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    private void DrawAttackRange()
    {
        CircleCollider2D rangeCollider = gameObject.AddComponent<CircleCollider2D>();
        rangeCollider.isTrigger = true;
        rangeCollider.radius = range;
    }
}
