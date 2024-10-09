using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Add this to use the Button component

public class SpawnEquippedUnits : MonoBehaviour
{
    public string[] unitSlotNames; // Array of unit slot names

    private void Start()
    {
        SpawnUnitsAtSpawnPoints();
    }

    private void SpawnUnitsAtSpawnPoints()
    {
        // Get the list of equipped units from EquippedManager
        List<EquippedManager.EquippedUnit> equippedUnits = EquippedManager.Instance.equippedUnits;

        if (equippedUnits == null || equippedUnits.Count == 0)
        {
            Debug.LogWarning("No equipped units found.");
            return;
        }

        // Create a dictionary to map slot names to transforms
        Dictionary<string, Transform> unitSlotMap = new Dictionary<string, Transform>();

        // Loop through the unitSlotNames and find corresponding transforms
        foreach (var slotName in unitSlotNames)
        {
            Transform slotTransform = GameObject.Find(slotName)?.transform;

            if (slotTransform == null)
            {
                Debug.LogWarning($"Spawn point with name {slotName} not found.");
            }
            else
            {
                unitSlotMap[slotName] = slotTransform;
                Debug.Log($"Found slot: {slotName} at position {slotTransform.position}");
            }
        }

        // Spawn units at their respective slots
        foreach (var equippedUnit in equippedUnits)
        {
            if (equippedUnit.unit == null)
            {
                Debug.LogWarning("No unit assigned to this slot.");
                continue;
            }

            Transform slotTransform;
            if (unitSlotMap.TryGetValue(equippedUnit.unitSlot.name, out slotTransform))
            {
                // Instantiate the unit
                GameObject instance = Instantiate(equippedUnit.unit, slotTransform.position, slotTransform.rotation);

                // Set the parent to the unit slot
                instance.transform.SetParent(slotTransform);

                Debug.Log($"Spawned {equippedUnit.unit.name} as child of {slotTransform.name}");

                // Check if the prefab has a Renderer
                Renderer renderer = instance.GetComponent<Renderer>();
                if (renderer == null)
                {
                    Debug.LogWarning($"{instance.name} does not have a Renderer component.");
                }
                else if (!renderer.enabled)
                {
                    Debug.LogWarning($"{instance.name}'s Renderer is disabled.");
                }

                // Activate any deactivated TowerPlacement scripts on the instance
                TowerPlacement towerPlacement = instance.GetComponent<TowerPlacement>();
                if (towerPlacement != null && !towerPlacement.enabled)
                {
                    towerPlacement.enabled = true;
                    Debug.Log($"Activated TowerPlacement script on {instance.name}");
                }

                // Activate any deactivated Button components on the instance
                Button button = instance.GetComponent<Button>();
                if (button != null && !button.enabled)
                {
                    button.enabled = true;
                    Debug.Log($"Activated Button component on {instance.name}");
                }

                // Activate the HoverActivate script if it exists and is deactivated
                HoverActivate hoverActivate = instance.GetComponent<HoverActivate>();
                if (hoverActivate != null && !hoverActivate.enabled)
                {
                    hoverActivate.enabled = true;
                    Debug.Log($"Activated HoverActivate script on {instance.name}");
                }

                // Activate the HoverToSpawnPrefab script if it exists and is deactivated
                HoverToSpawnPrefab hoverToSpawnPrefab = instance.GetComponent<HoverToSpawnPrefab>();
                if (hoverToSpawnPrefab != null && !hoverToSpawnPrefab.enabled)
                {
                    hoverToSpawnPrefab.enabled = true;
                    Debug.Log($"Activated HoverToSpawnPrefab script on {instance.name}");
                }
            }
            else
            {
                Debug.LogWarning($"No slot found for unit: {equippedUnit.unit.name}");
            }
        }
    }
}