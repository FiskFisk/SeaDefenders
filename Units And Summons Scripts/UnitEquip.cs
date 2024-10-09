using UnityEngine;

public class UnitEquip : MonoBehaviour
{
    public GameObject toggleObject; // The GameObject to be toggled

    private GameObject[] unitSlots;
    private GameObject currentSpawnedClone; // Reference to the currently spawned clone
    private bool isSpawned = false; // Track whether the unit is currently spawned
    private int clickCount = 0; // Track the number of clicks

    private void Start()
    {
        // Find all objects tagged as "UnitSlot"
        unitSlots = GameObject.FindGameObjectsWithTag("UnitSlot");

        // Log a warning if no unit slots are found
        if (unitSlots.Length == 0)
        {
            Debug.LogWarning("No UnitSlots found in the scene!");
        }

        // Ensure the toggleObject is initially deactivated
        if (toggleObject != null)
        {
            toggleObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Check for left mouse button click
        if (Input.GetMouseButtonDown(0))
        {
            // Check if the click is on this UI object
            if (IsMouseOverUIObject())
            {
                HandleClick();
            }
        }
    }

    private bool IsMouseOverUIObject()
    {
        // Check if the mouse is over this UI Image's RectTransform
        RectTransform rectTransform = GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, null);
    }

    private void HandleClick()
    {
        clickCount++;

        if (isSpawned)
        {
            // Remove the currently spawned clone
            if (currentSpawnedClone != null)
            {
                // Inform EquippedManager that we are unequipping the unit
                EquippedManager.Instance.UnequipUnit(currentSpawnedClone);
                
                Destroy(currentSpawnedClone);
                currentSpawnedClone = null;
                isSpawned = false;
                Debug.Log($"Removed previous clone {gameObject.name}");
            }
        }
        else
        {
            // Check if there is at least one available unit slot
            bool slotAvailable = false;
            foreach (GameObject unit in unitSlots)
            {
                if (unit.transform.childCount == 0) // Check if the slot has no children
                {
                    slotAvailable = true;
                    break;
                }
            }

            if (slotAvailable)
            {
                // Spawn a new clone in an available unit slot
                foreach (GameObject unit in unitSlots)
                {
                    if (unit.transform.childCount == 0) // Check if the slot has no children
                    {
                        currentSpawnedClone = SpawnInUnit(unit);
                        isSpawned = true;

                        // Inform EquippedManager that we are equipping the unit
                        EquippedManager.Instance.EquipUnit(currentSpawnedClone, unit);
                        break; // Stop once we've spawned the object
                    }
                }
            }
            else
            {
                Debug.LogWarning("All unit slots are full!");
                return; // Do not activate the toggleObject if no slots are available
            }
        }

        // Handle toggleObject activation/deactivation
        if (toggleObject != null)
        {
            if (clickCount % 2 == 1) // 1st, 3rd, 5th, etc. clicks
            {
                toggleObject.SetActive(true);
            }
            else // 2nd, 4th, 6th, etc. clicks
            {
                toggleObject.SetActive(false);
            }
        }
    }

    private GameObject SpawnInUnit(GameObject unit)
    {
        // Instantiate a copy of this object and make it a child of the unit
        GameObject clone = Instantiate(gameObject, unit.transform);

        // Set local position to zero to fit within the parent
        clone.transform.localPosition = Vector3.zero;

        // Ensure the scale is correct
        clone.transform.localScale = Vector3.one;

        // Remove the UnitEquip script from the cloned object
        UnitEquip unitEquipScript = clone.GetComponent<UnitEquip>();
        if (unitEquipScript != null)
        {
            Destroy(unitEquipScript); // Remove the UnitEquip script
        }

        // Optionally adjust RectTransform properties
        RectTransform rectTransform = clone.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(100, 100); // Adjust size as needed
        }

        // Assign a unique identifier to the clone with "Equipped" suffix
        clone.name = $"{gameObject.name} Equipped"; // Example of unique name

        return clone; // Return the created clone for reference
    }
}
