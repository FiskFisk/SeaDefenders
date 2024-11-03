using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10; // Amount of HP to deduct on collision

    private InGameMoney inGameMoney; // Reference to the InGameMoney component

    private void Start()
    {
        // Find the InGameMoney component in the scene
        inGameMoney = FindObjectOfType<InGameMoney>();
        if (inGameMoney == null)
        {
            Debug.LogError("InGameMoney component not found in the scene.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object has the tag "End"
        if (other.CompareTag("End"))
        {
            if (inGameMoney != null)
            {
                // Deduct HP from the player
                inGameMoney.TakeDamage(damageAmount); // Correct method to reduce HP

                // Optionally destroy the enemy object
                Destroy(gameObject);
            }
        }
    }
}
