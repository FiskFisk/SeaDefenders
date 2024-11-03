using UnityEngine;
using UnityEngine.UI;

public class DeleteGameObjectOnClick : MonoBehaviour
{
    public Button deleteButton; // Reference to the Button in the UI
    public GameObject objectToDelete; // The GameObject to be deleted

    private void Start()
    {
        // Add the listener to the button to call DeleteObject method on click
        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener(DeleteObject);
        }
    }

    private void DeleteObject()
    {
        if (objectToDelete != null)
        {
            Destroy(objectToDelete); // Deletes the referenced GameObject
            Debug.Log($"{objectToDelete.name} has been deleted.");
        }
        else
        {
            Debug.LogWarning("No GameObject is assigned to delete.");
        }
    }
}
