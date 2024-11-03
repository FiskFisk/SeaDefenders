using UnityEngine;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; // Import binary formatter for serialization
using UnityEngine.SceneManagement;

[System.Serializable]
public class CurrencyData
{
    public int seaCoins;
}

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance; // Singleton instance
    public int seaCoins; // Currency amount
    public string seaCoinsTextObjectName = "SeaCoinsText"; // Name of the TMP UI element to show the currency
    private TextMeshProUGUI seaCoinsText; // TMP UI element reference

    private string savePath;
    private string prefabDataPath; // Path for the prefab data file

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

        // Set the path for saving the data
        savePath = Path.Combine(Application.persistentDataPath, "currencyData.dat"); // Change extension to .dat for binary
        prefabDataPath = Path.Combine(Application.persistentDataPath, "prefabData.dat"); // Change extension to .dat for binary
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event
        FindSeaCoinsText(); // Look for the text at the start
        LoadCurrency(); // Load currency data when the script is enabled
        UpdateCurrencyUI(); // Update UI whenever this script is enabled
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from the event
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the loaded scene is MainMenu
        if (scene.name == "MainMenu") // Ensure this matches your MainMenu scene name
        {
            FindSeaCoinsText(); // Look for the SeaCoinsText when MainMenu is loaded
            UpdateCurrencyUI(); // Update the UI to reflect the current currency
        }
    }

    // Call this method to update the UI when currency changes
    public void UpdateCurrencyUI()
    {
        if (seaCoinsText != null)
        {
            seaCoinsText.text = $"SeaCoins: {seaCoins}";
        }
        else
        {
            Debug.LogWarning("SeaCoinsText reference is missing!");
        }
    }

    // Method to add coins
    public void AddSeaCoins(int amount)
    {
        seaCoins += amount;
        UpdateCurrencyUI();
        SaveCurrency(); // Save after updating the amount
    }

    // Method to spend coins
    public void SpendSeaCoins(int amount)
    {
        if (seaCoins >= amount)
        {
            seaCoins -= amount;
            UpdateCurrencyUI();
            SaveCurrency(); // Save after updating the amount
        }
        else
        {
            Debug.LogWarning("Not enough SeaCoins!");
        }
    }

    // Method to save currency data in binary format
    public void SaveCurrency()
    {
        CurrencyData data = new CurrencyData { seaCoins = seaCoins };
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(savePath, FileMode.Create))
        {
            formatter.Serialize(stream, data);
        }
        Debug.Log("Currency data saved to " + savePath);
    }

    // Method to load currency data from binary file
    public void LoadCurrency()
    {
        if (File.Exists(savePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(savePath, FileMode.Open))
            {
                CurrencyData data = (CurrencyData)formatter.Deserialize(stream);
                seaCoins = data.seaCoins;
                UpdateCurrencyUI();
                Debug.Log("Currency data loaded from " + savePath);
            }
        }
        else
        {
            // If no currency data file found, delete the prefabData file if it exists
            if (File.Exists(prefabDataPath))
            {
                File.Delete(prefabDataPath);
                Debug.Log("Prefab data file deleted: " + prefabDataPath);
            }

            // Initialize currency to 1000 seaCoins if no data exists
            Debug.LogWarning("No currency data found at " + savePath + ". Initializing with 1000 SeaCoins.");
            seaCoins = 1000; // Set initial seaCoins to 1000 if no data exists
            UpdateCurrencyUI();
            SaveCurrency(); // Save the initial currency state
        }
    }

    private void FindSeaCoinsText()
    {
        seaCoinsText = FindInactiveTextByName(seaCoinsTextObjectName);
    }

    private TextMeshProUGUI FindInactiveTextByName(string name)
    {
        TMP_Text[] allTexts = Resources.FindObjectsOfTypeAll<TMP_Text>(); // Finds all TMP_Text objects, including inactive ones
        foreach (TMP_Text text in allTexts)
        {
            if (text.name == name)
            {
                return text.GetComponent<TextMeshProUGUI>(); // Return the first matching TMP object
            }
        }
        return null; // Return null if no matching text is found
    }
}
