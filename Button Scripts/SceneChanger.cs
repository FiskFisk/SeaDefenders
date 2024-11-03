using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    public Button changeSceneButton; // The Button to attach this script to
    public string sceneToLoad; // Name of the scene to load

    private void Start()
    {
        if (changeSceneButton != null)
        {
            // Add a listener to the button click event
            changeSceneButton.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogWarning("Change Scene Button is not assigned!");
        }
    }

    private void OnButtonClick()
    {
        // Check if the scene name is not empty
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            // Load the specified scene
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Scene name is not set!");
        }
    }
}
