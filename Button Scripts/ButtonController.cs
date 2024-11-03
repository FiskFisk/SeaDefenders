using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public string[] objectsToActivate;    // List of object names to activate
    public string[] objectsToDeactivate;  // List of object names to deactivate

    // Public method to be called by the button's OnClick event in the Inspector
    public void ToggleObjects()
    {
        // Activate objects
        foreach (string objectName in objectsToActivate)
        {
            GameObject obj = FindObjectInHierarchy(objectName);
            if (obj != null)
            {
                obj.SetActive(true); // Activate the object
                Debug.Log($"Activated: {objectName}"); // Debug message for activation
            }
            else
            {
                Debug.LogWarning($"Object '{objectName}' not found in the scene!"); // Warning if not found
            }
        }

        // Deactivate objects
        foreach (string objectName in objectsToDeactivate)
        {
            GameObject obj = FindObjectInHierarchy(objectName);
            if (obj != null)
            {
                obj.SetActive(false); // Deactivate the object
                Debug.Log($"Deactivated: {objectName}"); // Debug message for deactivation
            }
            else
            {
                Debug.LogWarning($"Object '{objectName}' not found in the scene!"); // Warning if not found
            }
        }
    }

    // Method to find a GameObject by name in the hierarchy, even if it's inactive
    private GameObject FindObjectInHierarchy(string name)
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>(true); // Find all objects, including inactive ones
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == name)
            {
                return obj; // Return the GameObject if its name matches
            }
        }
        return null; // Return null if no matching GameObject is found
    }
}
