using UnityEngine;

public class ESCMenuController : MonoBehaviour
{
    public GameObject mainMenuObject; // Drag in the main menu GameObject
    public GameObject altMenuObject;  // Drag in the alternative GameObject to activate if no "NotMainMenu" objects are active

    void Update()
    {
        // Check if ESC key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Find all game objects with the tag "NotMainMenu"
            GameObject[] notMainMenuObjects = GameObject.FindGameObjectsWithTag("NotMainMenu");

            // Check if any of those objects are active
            bool anyActive = false;
            foreach (GameObject obj in notMainMenuObjects)
            {
                if (obj.activeSelf)
                {
                    anyActive = true;
                    // Deactivate the found object
                    obj.SetActive(false);
                    // Activate the main menu or any GameObject you drag in
                    if (mainMenuObject != null)
                    {
                        mainMenuObject.SetActive(true);
                    }
                    break; // Exit the loop after deactivating one object
                }
            }

            // If no "NotMainMenu" objects were active, activate the alternative object
            if (!anyActive)
            {
                if (altMenuObject != null)
                {
                    altMenuObject.SetActive(true);
                }
            }
        }
    }
}
