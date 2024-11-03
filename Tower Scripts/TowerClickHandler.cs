using UnityEngine;

public class TowerClickHandler : MonoBehaviour
{
    public GameObject upgradeManagerPrefab; // Prefab with TowerUpgradeManager script
    public string parentObjectName; // The name of the object under which the prefab will be spawned

    private PlacedTowerStats placedTowerStats; // Reference to the PlacedTowerStats script
    private GameObject spawnedUpgradeManager;  // Reference to the spawned upgrade manager

    private bool isClickLocked = false; // Prevents immediate click after spawning

    private void Start()
    {
        // Get the PlacedTowerStats component attached to this game object
        placedTowerStats = GetComponent<PlacedTowerStats>();
        if (placedTowerStats == null)
        {
            Debug.LogError("PlacedTowerStats script is missing on this GameObject.");
        }
    }

    private void OnMouseDown()
    {
        // If clicks are locked, return and don't process the click
        if (isClickLocked)
        {
            return;
        }

        // Check if we already spawned the upgrade manager
        if (spawnedUpgradeManager == null)
        {
            SpawnUpgradeManager();
        }
    }

    // Function to spawn the upgrade manager prefab and transfer stats
    private void SpawnUpgradeManager()
    {
        // Find the parent object by name
        GameObject parentObject = GameObject.Find(parentObjectName);
        if (parentObject == null)
        {
            Debug.LogError("Parent object with the name '" + parentObjectName + "' not found.");
            return;
        }

        // Spawn the prefab as a child of the found parentObject
        spawnedUpgradeManager = Instantiate(upgradeManagerPrefab, parentObject.transform);

        // Get the TowerUpgradeManager component from the spawned prefab
        TowerUpgradeManager upgradeManager = spawnedUpgradeManager.GetComponent<TowerUpgradeManager>();

        if (upgradeManager != null)
        {
            // Transfer stats from PlacedTowerStats to TowerUpgradeManager
            upgradeManager.damage = placedTowerStats.damage;
            upgradeManager.range = placedTowerStats.range;
            upgradeManager.attackSpeed = placedTowerStats.attackSpeed;
            upgradeManager.gameLevel = placedTowerStats.gameLevel; // Pass game level
            upgradeManager.upgradeCost = placedTowerStats.upgradeCost; // Pass upgrade cost
            upgradeManager.maxGameLevel = placedTowerStats.maxGameLevel;

            // Start listening for updates in the upgrade manager
            upgradeManager.OnStatsChanged += UpdatePlacedTowerStats;

            // Immediately update the TMP text for the upgrade manager
            upgradeManager.UpdateTMPText(); // This will update the text with current stats
        }
        else
        {
            Debug.LogError("TowerUpgradeManager script is missing on the upgrade manager prefab.");
        }

        // Lock clicks for a short period to prevent double click right after placement
        StartCoroutine(UnlockClickAfterDelay(0.5f)); // Add a delay of 0.5 seconds or adjust as needed
    }

    // Function to update PlacedTowerStats whenever TowerUpgradeManager stats are changed
    private void UpdatePlacedTowerStats(float damage, float range, float attackSpeed, int gameLevel, int upgradeCost)
    {
        // Update the placed tower stats with the new values
        placedTowerStats.UpdateStats(damage, range, attackSpeed, gameLevel, upgradeCost);

        Debug.Log("PlacedTowerStats updated from TowerUpgradeManager.");
    }

    // Coroutine to unlock clicks after a short delay
    private System.Collections.IEnumerator UnlockClickAfterDelay(float delay)
    {
        isClickLocked = true;
        yield return new WaitForSeconds(delay);
        isClickLocked = false;
    }
}
