using TMPro; // Make sure to include this for TextMeshPro usage
using UnityEngine;
using UnityEngine.UI; // Include this for Button usage

public class TowerUpgradeManager : MonoBehaviour
{
    public float damage;
    public float range;
    public float attackSpeed;
    public int gameLevel;
    public int upgradeCost;
    public int maxGameLevel;

    public TextMeshProUGUI damageText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI attackSpeedText;
    public TextMeshProUGUI upgradeCostText; // TextMeshProUGUI field to show upgrade cost
    public TextMeshProUGUI levelText; // New TextMeshProUGUI field to show tower level

    public Button upgradeButton; // Button to trigger the upgrade
    private InGameMoney inGameMoney; // Reference to the InGameMoney script

    public GameObject objectToDelete; // GameObject to delete if stats are 0

    // Event that notifies when the stats are changed
    public delegate void StatsChanged(float damage, float range, float attackSpeed, int gameLevel, int upgradeCost);
    public event StatsChanged OnStatsChanged; // Declare the event

    private void Start()
    {
        // Get the InGameMoney instance (assumed to be on the same GameObject or elsewhere)
        inGameMoney = FindObjectOfType<InGameMoney>(); // Adjust if necessary

        // Set up the button listener
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(UpgradeTower);
        }

        // Initial update of the TMP text fields
        UpdateTMPText();

        // Check if the stats are all 0 and delete the object if true
        CheckAndDeleteIfZero();
    }

    // Method to update the TMP text fields with the current stats
    public void UpdateTMPText()
    {
        // Update the TMP text fields with the current stats
        if (damageText != null) damageText.text = "Damage: " + damage.ToString("F2"); // Format to 2 decimal places
        if (rangeText != null) rangeText.text = "Range: " + range.ToString("F2"); // Format to 2 decimal places
        if (attackSpeedText != null) attackSpeedText.text = "Attack Speed: " + attackSpeed.ToString("F2"); // Format to 2 decimal places
        
        // Update the level text with the current game level
        if (levelText != null) levelText.text = gameLevel.ToString();

        // Check if the tower is at max level
        if (gameLevel >= maxGameLevel)
        {
            if (upgradeCostText != null) upgradeCostText.text = "Max Upgraded"; // Display "Max Upgraded" if at max level
        }
        else
        {
            if (upgradeCostText != null) upgradeCostText.text = "Upgrade Cost: " + upgradeCost.ToString(); // Display upgrade cost otherwise
        }
    }

    // Example method to trigger when upgrading stats
    public void UpgradeTower()
    {
        // Debug the current stats
        Debug.Log("Current Stats - Damage: " + damage + ", Range: " + range + ", Attack Speed: " + attackSpeed);

        // Check if we can upgrade (not at max level and enough currency)
        if (gameLevel < maxGameLevel && inGameMoney.GetMoney() >= upgradeCost)
        {
            // Spend the upgrade cost
            inGameMoney.SpendMoney(upgradeCost);

            // Upgrade stats
            damage *= 1.35f; // Damage increases by 35%
            range *= 1.15f;  // Range increases by 15%
            attackSpeed *= 1.1f; // Attack speed increases by 10%
            gameLevel++; // Increase the game level
            upgradeCost = Mathf.CeilToInt(upgradeCost * 1.8f); // Increase upgrade cost by 80%

            // Notify that the stats have changed
            OnStatsChanged?.Invoke(damage, range, attackSpeed, gameLevel, upgradeCost);

            // Update the TMP text after upgrading
            UpdateTMPText();

            // Check again if stats are all 0 after upgrade
            CheckAndDeleteIfZero();
        }
        else
        {
            // Check if the tower cannot be upgraded (e.g., not enough currency or at max level)
            if (gameLevel >= maxGameLevel)
            {
                Debug.Log("Tower is already at max level.");
            }
            else
            {
                Debug.Log("Not enough currency to upgrade.");
            }
        }
    }

    // Method to check if damage, range, and attack speed are all 0
    private void CheckAndDeleteIfZero()
    {
        if (damage == 0 && range == 0 && attackSpeed == 0)
        {
            Debug.Log("Stats are all 0, deleting object.");
            if (objectToDelete != null)
            {
                Destroy(objectToDelete);
            }
            else
            {
                Destroy(gameObject); // Delete this game object if no other object is specified
            }
        }
    }
}
