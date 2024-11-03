using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GetSeaCoinsManager : MonoBehaviour
{
    public static GetSeaCoinsManager Instance; // Singleton instance
    public int seaCoins; // Currency amount
    public TextMeshProUGUI seaCoinsText; // Reference to the TMP GameObject for displaying SeaCoins
    public string currencyManagerGameObjectName = "CurrencyManager"; // Name of the GameObject with the CurrencyManager script

    private int displayedSeaCoins; // Separate field to store the displayed SeaCoins amount

    private void Awake()
    {
        // Implement Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        displayedSeaCoins = seaCoins; // Initialize the displayed SeaCoins value
        UpdateSeaCoinsText(); // Update the text at the start
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to scene loaded event
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from scene loaded event
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            TransferSeaCoinsToCurrencyManager(); // Transfer coins when MainMenu loads
        }
    }

    // Method to transfer SeaCoins to CurrencyManager by looking up the GameObject by name
    public void TransferSeaCoinsToCurrencyManager()
    {
        GameObject currencyManagerObject = GameObject.Find(currencyManagerGameObjectName);

        if (currencyManagerObject != null)
        {
            CurrencyManager currencyManager = currencyManagerObject.GetComponent<CurrencyManager>();
            if (currencyManager != null)
            {
                currencyManager.AddSeaCoins(seaCoins); // Add SeaCoins to CurrencyManager
                seaCoins = 0; // Reset SeaCoins after transfer
                Debug.Log($"Transferred SeaCoins to CurrencyManager, current SeaCoins in manager: {seaCoins}");
            }
            else
            {
                Debug.LogWarning("CurrencyManager component not found on the specified GameObject!");
            }
        }
        else
        {
            Debug.LogWarning($"CurrencyManager GameObject '{currencyManagerGameObjectName}' not found!");
        }
    }

    // Method to add SeaCoins from WaveManager
    public void AddSeaCoins(int amount)
    {
        seaCoins += amount;
        displayedSeaCoins = seaCoins; // Update the displayed SeaCoins to match the actual SeaCoins
        UpdateSeaCoinsText(); // Update the text whenever SeaCoins are added
        Debug.Log($"Added {amount} SeaCoins. Total now: {seaCoins}");
    }

    // Method to update the TMP text displaying the SeaCoins amount (uses displayedSeaCoins)
    private void UpdateSeaCoinsText()
    {
        if (seaCoinsText != null)
        {
            seaCoinsText.text = $"SeaCoins: {displayedSeaCoins}"; // Show the displayed SeaCoins amount
        }
        else
        {
            Debug.LogWarning("SeaCoins Text TMP reference is not assigned!"); // Warn if TMP reference is not set
        }
    }
}
