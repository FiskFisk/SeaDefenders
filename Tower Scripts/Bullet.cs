using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 0f; // Set to 0 because the bullet should not move
    private Vector2 direction;
    private float damage;
    public string[] barrierTags; // Array of tags for barriers
    public string enemyTag = "Enemy"; // Tag specifically for enemies

    private bool isNewlySpawned; // Flag to check if the bullet is newly spawned
    private float spawnTime; // The time when the bullet was spawned
    public float thresholdTime = 0.5f; // Time limit to determine if the bullet is newly spawned

    public void SetDirection(Vector2 bulletDirection, float bulletDamage)
    {
        direction = bulletDirection;
        damage = bulletDamage;
        // Ensure bullet starts off facing the correct direction
        transform.up = direction;
        
        // Mark the bullet as newly spawned
        isNewlySpawned = true;
        spawnTime = Time.time; // Record the time of spawn
    }

    private void Update()
    {
        // Move the bullet in the specified direction (if needed) or not at all if speed is zero
        if (speed > 0f)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }

        // Check if the bullet is no longer newly spawned
        if (isNewlySpawned && Time.time - spawnTime > thresholdTime)
        {
            isNewlySpawned = false; // Set to false after threshold time
        }
    }

    public bool IsNewlySpawned() // Method to check if the bullet is newly spawned
    {
        return isNewlySpawned;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider belongs to a barrier or enemy
        if (IsBarrier(other))
        {
            HandleBarrierCollision();
        }
        else if (other.CompareTag(enemyTag))
        {
            HandleEnemyCollision(other);
        }
    }

    private bool IsBarrier(Collider2D collider)
    {
        // Check if the collider's tag is in the list of barrierTags
        foreach (string tag in barrierTags)
        {
            if (collider.CompareTag(tag))
            {
                return true;
            }
        }
        return false;
    }

    private void HandleBarrierCollision()
    {
        // Destroy the bullet upon hitting a barrier
        Destroy(gameObject);
    }

    private void HandleEnemyCollision(Collider2D enemyCollider)
    {
        // Apply damage to the target using the EnemyInformation script
        EnemyInformation enemyInfo = enemyCollider.GetComponent<EnemyInformation>();
        if (enemyInfo != null)
        {
            enemyInfo.TakeDamage(damage);
        }

        // Destroy the bullet after it hits the target
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        // Draw a visual representation of the collider in the scene view for debugging
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
        }
    }
}
