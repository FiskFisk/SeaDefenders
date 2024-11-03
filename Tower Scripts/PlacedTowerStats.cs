using UnityEngine;

public class PlacedTowerStats : MonoBehaviour
{
    public float damage;        // Amount of damage the tower does
    public float range;         // Range of the tower's attack
    public float attackSpeed;   // Attack speed of the tower
    public int cost;            // Cost to place the tower
    public int gameLevel = 1;   // Current level of the tower, starts at 1
    public int upgradeCost;     // Cost to upgrade the tower to the next level
    public int maxGameLevel = 5; // Maximum level this tower can be upgraded to

    private TowerStats towerStats; // Reference to TowerStats

    public void InitializeTower()
    {
        // Set the stats from the TowerStats
        towerStats = GetComponent<TowerStats>();
        if (towerStats != null)
        {
            damage = towerStats.damage;
            range = towerStats.range;
            attackSpeed = towerStats.attackSpeed;
            cost = towerStats.cost;
            upgradeCost = towerStats.upgradeCost; // Ensure upgradeCost is initialized
        }
        else
        {
            Debug.LogError("TowerStats component is missing from the tower prefab.");
        }
    }

    public bool CanUpgrade()
    {
        // Return true if the tower hasn't reached the max game level
        return gameLevel < maxGameLevel;
    }

    public void UpdateStats(float newDamage, float newRange, float newAttackSpeed, int newGameLevel, int newUpgradeCost)
    {
        // Update stats from the TowerUpgradeManager
        damage = newDamage;
        range = newRange;
        attackSpeed = newAttackSpeed;
        gameLevel = newGameLevel; // Ensure the game level is updated
        upgradeCost = newUpgradeCost; // Ensure upgrade cost is updated

        Debug.Log($"Tower stats updated: Damage: {damage}, Range: {range}, Attack Speed: {attackSpeed}, Game Level: {gameLevel}, Upgrade Cost: {upgradeCost}");
    }

    public float GetDamage() // Method to retrieve current damage
    {
        return damage;
    }

    public float GetRange() // Method to retrieve current range
    {
        return range;
    }

    public float GetAttackSpeed() // Method to retrieve current attack speed
    {
        return attackSpeed;
    }
}
