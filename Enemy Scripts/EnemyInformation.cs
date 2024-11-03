using UnityEngine;
using System.Collections.Generic;


public class EnemyInformation : MonoBehaviour
{
    public float speed = 5f; // Movement speed of the enemy
    public float health = 100f; // Health of the enemy
    public GameObject explosionPrefab; // Reference to the explosion prefab
    public int giveMoney = 50; // Amount of money this enemy gives when destroyed

    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypointIndex = 0;
    private WaypointManager waypointManager;
    private InGameMoney inGameMoney; // Reference to InGameMoney script

    private void Start()
    {
        waypointManager = FindObjectOfType<WaypointManager>();
        inGameMoney = FindObjectOfType<InGameMoney>();

        if (waypointManager == null)
        {
            Debug.LogError("WaypointManager not found in the scene.");
            return;
        }

        // Retrieve the waypoint names and corresponding transforms from WaypointManager
        string[] waypointNames = waypointManager.GetWaypointNames(); // Implement this method in WaypointManager
        foreach (var name in waypointNames)
        {
            Transform waypoint = waypointManager.GetWaypointTransformByName(name);
            if (waypoint != null)
            {
                waypoints.Add(waypoint);
            }
            else
            {
                Debug.LogWarning($"Waypoint with name {name} not found.");
            }
        }

        if (waypoints.Count == 0)
        {
            Debug.LogError("No waypoints found for the enemy.");
        }
    }

    private void Update()
    {
        if (waypoints.Count == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        MoveTowardsWaypoint(targetWaypoint);

        // Check if the enemy has reached the current waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            // Move to the next waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        }
    }

    private void MoveTowardsWaypoint(Transform target)
    {
        // Calculate the direction to the target waypoint
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    // Method to handle taking damage
    public void TakeDamage(float damage)
    {
        health -= damage;

        // Check if health is depleted
        if (health <= 0)
        {
            Die();
        }
    }

    // Method to handle enemy death
    private void Die()
    {
        // Check if the explosionPrefab is assigned
        if (explosionPrefab != null)
        {
            // Instantiate the explosion at the enemy's position
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("ExplosionPrefab is not assigned in the Inspector.");
        }

        // Add money to the player through the InGameMoney script
        if (inGameMoney != null)
        {
            inGameMoney.AddMoney(giveMoney);
        }

        // Destroy the enemy GameObject after the explosion
        Destroy(gameObject);
    }
}
