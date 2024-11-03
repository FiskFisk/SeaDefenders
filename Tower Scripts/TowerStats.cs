using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class TowerStats : MonoBehaviour
{
    public float damage;        // Amount of damage the tower does
    public float range;         // Range of the tower's attack
    public float attackSpeed;   // Attack speed of the tower
    public int cost;            // Cost to place the tower
    public int unitCount;       // Number of units (e.g., total copies of this tower)
    public int upgradeCost; // Add this line to define upgradeCost

    public TMP_Text costText;   // Reference to the TMP Text for displaying cost

    private void Start()
    {
        UpdateCostText();       // Ensure the text is updated at the start
    }

    // Optional: Method to display tower stats in a readable format
    public string GetStatsDescription()
    {
        return $"Damage: {damage}\nRange: {range}\nAttack Speed: {attackSpeed}\nCost: {cost}\nUnit Count: {unitCount}";
    }

    // Method to increase the unit count
    public void AddUnit()
    {
        unitCount++;
    }

    // Method to decrease the unit count (if greater than 0)
    public void RemoveUnit()
    {
        if (unitCount > 0)
        {
            unitCount--;
        }
    }

    // Method to update the cost text on the TMP object
    private void UpdateCostText()
    {
        if (costText != null)
        {
            costText.text = $"Cost: {cost}";
        }
        else
        {
            Debug.LogWarning("No TMP_Text assigned for displaying cost.");
        }
    }

    // Example method if you need to update cost dynamically
    public void SetCost(int newCost)
    {
        cost = newCost;
        UpdateCostText(); // Update the TMP text when cost changes
    }
}
