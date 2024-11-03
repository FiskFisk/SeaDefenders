using System.Collections.Generic;
using UnityEngine;

public class EquippedManager : MonoBehaviour
{
    public static EquippedManager Instance;

    [System.Serializable]
    public class EquippedUnit
    {
        public string unitName;   // Save unit by name
        public string unitSlotName; // Save slot by name
        [System.NonSerialized] public GameObject unit;
        [System.NonSerialized] public GameObject unitSlot;
    }

    public List<EquippedUnit> equippedUnits = new List<EquippedUnit>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EquipUnit(GameObject unit, GameObject slot)
    {
        if (unit == null || slot == null)
        {
            Debug.LogWarning("Cannot equip unit: unit or slot is null.");
            return;
        }

        // Check if the unit is already equipped
        foreach (var equipped in equippedUnits)
        {
            if (equipped.unit == unit)
            {
                Debug.LogWarning("Unit is already equipped!");
                return;
            }
        }

        equippedUnits.Add(new EquippedUnit { unit = unit, unitSlot = slot, unitName = unit.name, unitSlotName = slot.name });
        Debug.Log($"Equipped unit: {unit.name} to slot: {slot.name}");
    }

    public void UnequipUnit(GameObject unit)
    {
        if (unit == null)
        {
            Debug.LogWarning("Cannot unequip unit: unit is null.");
            return;
        }

        EquippedUnit equippedToRemove = null;
        foreach (var equipped in equippedUnits)
        {
            if (equipped.unit == unit)
            {
                equippedToRemove = equipped;
                break;
            }
        }

        if (equippedToRemove != null)
        {
            equippedUnits.Remove(equippedToRemove);
            Debug.Log($"Unequipped unit: {unit.name}");
        }
        else
        {
            Debug.LogWarning("Unit is not equipped!");
        }
    }

    public GameObject GetSlotForUnit(GameObject unit)
    {
        if (unit == null)
        {
            Debug.LogWarning("Cannot get slot: unit is null.");
            return null;
        }

        foreach (var equipped in equippedUnits)
        {
            if (equipped.unit == unit)
            {
                return equipped.unitSlot;
            }
        }

        Debug.LogWarning("Unit is not equipped!");
        return null;
    }

    // Save equipped units
    public void SaveEquippedUnits()
    {
        for (int i = 0; i < equippedUnits.Count; i++)
        {
            PlayerPrefs.SetString("EquippedUnit_" + i, equippedUnits[i].unitName);
            PlayerPrefs.SetString("EquippedSlot_" + i, equippedUnits[i].unitSlotName);
        }

        PlayerPrefs.SetInt("EquippedUnitCount", equippedUnits.Count);
        PlayerPrefs.Save();
        Debug.Log("Equipped units saved.");
    }

    // Load equipped units
    public void LoadEquippedUnits()
    {
        int count = PlayerPrefs.GetInt("EquippedUnitCount", 0);
        equippedUnits.Clear();

        for (int i = 0; i < count; i++)
        {
            string unitName = PlayerPrefs.GetString("EquippedUnit_" + i, "");
            string slotName = PlayerPrefs.GetString("EquippedSlot_" + i, "");

            GameObject unit = GameObject.Find(unitName);
            GameObject slot = GameObject.Find(slotName);

            if (unit != null && slot != null)
            {
                equippedUnits.Add(new EquippedUnit { unit = unit, unitSlot = slot, unitName = unitName, unitSlotName = slotName });
                Debug.Log($"Loaded equipped unit: {unitName} into slot: {slotName}");
            }
            else
            {
                Debug.LogWarning($"Failed to find unit or slot: {unitName}, {slotName}");
            }
        }
    }

    // Optional: Cleanup invalid references
    private void Update()
    {
        for (int i = equippedUnits.Count - 1; i >= 0; i--)
        {
            if (equippedUnits[i].unit == null || equippedUnits[i].unitSlot == null)
            {
                equippedUnits.RemoveAt(i);
                Debug.LogWarning("Removed invalid equipped unit reference.");
            }
        }
    }
}
