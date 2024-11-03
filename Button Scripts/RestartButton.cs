using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour
{
    public Button restartButton; // Reference to the button in the UI

    private void Start()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartScene);
        }
    }

    // Method to restart the current scene
    private void RestartScene()
    {
        // Get the current active scene and reload it
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
