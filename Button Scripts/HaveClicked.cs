using UnityEngine;
using UnityEngine.SceneManagement;

public class HaveClicked : MonoBehaviour
{
    public bool deleteObject; // Checkbox to indicate whether to delete an object
    public string objectNameToDelete; // Name of the object to delete

    private void Awake()
    {
        // Prevent this GameObject from being destroyed when loading a new scene
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event when this object is destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the loaded scene is the "MainMenu"
        if (scene.name == "MainMenu")
        {
            // Attempt to delete the object if the checkbox is checked
            CheckAndDeleteObject();
        }
    }

    private void CheckAndDeleteObject()
    {
        // Check if the checkbox is checked
        if (deleteObject)
        {
            // Find the GameObject by name and delete it
            GameObject objectToDelete = GameObject.Find(objectNameToDelete);
            if (objectToDelete != null)
            {
                Destroy(objectToDelete);
                Debug.Log($"{objectToDelete.name} has been deleted.");
            }
            else
            {
                Debug.LogWarning($"Object with the name '{objectNameToDelete}' not found.");
            }
        }
        else
        {
            Debug.Log("Checkbox is unchecked, no object will be deleted.");
        }
    }

    // Optional: Update function to visualize the checkbox state in the inspector
    private void OnValidate()
    {
        // This method will be called whenever the script is loaded or a value is changed in the inspector
        Debug.Log($"Delete Object: {deleteObject}, Object to Delete: {objectNameToDelete}");

        // Call CheckAndDeleteObject whenever the checkbox is changed
        CheckAndDeleteObject();
    }
}