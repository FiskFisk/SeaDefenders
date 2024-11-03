using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

[System.Serializable]
public class Wave
{
    public List<PrefabSpawnInfo> prefabSpawnInfos; // List of prefabs to spawn in this wave
}

[System.Serializable]
public class PrefabSpawnInfo
{
    public GameObject prefab; // Prefab to spawn
    public int amount; // Amount of this prefab to spawn
}

public class WaveManager : MonoBehaviour
{
    public Button waveButton; // Button to start waves
    public Transform[] spawnPoints; // Spawn points for prefabs
    public Transform parentTransform; // Parent object for spawned prefabs
    public Wave[] waves; // Array of waves
    public float spawnCooldown = 0.5f; // Cooldown between spawns
    public string prefabTag = "Enemy"; // Tag to identify prefabs
    public float waveTransitionCooldown = 5f; // Cooldown between waves
    public bool autoSkip = false; // Toggle for auto skip
    public InGameMoney moneyManager; // Reference to the InGameMoney script
    public GetSeaCoinsManager seaCoinsManager; // Reference to the GetSeaCoinsManager script
    public TextMeshProUGUI waveButtonText; // Button text for Play/Skip
    public GameObject youWinGameObject; // GameObject to activate when the player wins
    public int bonusSeaCoins = 100; // Bonus SeaCoins for completing the last wave

    // New fields for wave progress and enemy count display
    public TextMeshProUGUI waveProgressText; // Text to display current wave progress (e.g., 4/10)
    public TextMeshProUGUI enemyCountText; // Text to display remaining enemy count

    private int currentWaveIndex = 0; // Current wave index
    private int baseReward = 350; // Starting reward for completing the wave
    private Coroutine waveCoroutine; // Reference to the coroutine for spawning waves
    private bool waveOngoing = false; // Flag to check if a wave is ongoing

    private void Start()
    {
        if (waveButton != null)
        {
            waveButton.onClick.AddListener(OnWaveButtonClicked); // Add listener to wave button
        }
        else
        {
            Debug.LogWarning("Wave Button is not assigned."); // Warn if button is not assigned
        }

        waveButtonText.text = "Play"; // Initialize wave button text
        UpdateWaveProgressText(); // Initialize the wave progress text
        UpdateEnemyCountText(); // Initialize the enemy count text
    }

    private void Update()
    {
        // Continuously update the enemy count text while a wave is ongoing
        if (waveOngoing)
        {
            UpdateEnemyCountText();
        }
    }

    private void OnWaveButtonClicked()
    {
        if (waveOngoing) // Skip the wave if it's ongoing
        {
            if (waveCoroutine != null)
            {
                StopCoroutine(waveCoroutine); // Stop spawning enemies
            }
            CompleteWave(); // Skip the wave and complete it
        }
        else // Start a new wave if none is ongoing
        {
            StartNextWave();
        }
    }

    private void StartNextWave()
    {
        if (currentWaveIndex < waves.Length)
        {
            HideWaveButton(); // Hide the button at the start of the wave
            waveOngoing = true; // Set the flag indicating a wave is ongoing
            waveCoroutine = StartCoroutine(SpawnWave(waves[currentWaveIndex])); // Start spawning enemies
            currentWaveIndex++; // Move to the next wave
            UpdateWaveProgressText(); // Update wave progress when a new wave starts

            // Show skip button only if not on the last wave
            if (currentWaveIndex < waves.Length)
            {
                ShowSkipButtonAfterDelay(5f); // Show the skip button after a delay
            }
        }
        else
        {
            Debug.Log("All waves completed!"); // Log if all waves are completed
            waveButtonText.text = "Waves Completed"; // Update button text
        }
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        // Spawn all prefabs in the wave
        foreach (PrefabSpawnInfo info in wave.prefabSpawnInfos)
        {
            for (int i = 0; i < info.amount; i++)
            {
                SpawnPrefab(info.prefab); // Spawn the prefab
                yield return new WaitForSeconds(spawnCooldown); // Wait between spawns
            }
        }

        // Wait until all prefabs with the specified tag are removed
        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag(prefabTag).Length == 0);

        CompleteWave(); // Call CompleteWave when the wave is naturally completed
    }

    private void SpawnPrefab(GameObject prefab)
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned."); // Warn if no spawn points are assigned
            return;
        }

        if (parentTransform == null)
        {
            Debug.LogWarning("No parent transform assigned."); // Warn if no parent transform is assigned
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)]; // Pick a random spawn point
        GameObject instance = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation, parentTransform); // Spawn the prefab
    }

    // Hide the wave button (make it invisible and non-interactable)
    private void HideWaveButton()
    {
        waveButton.gameObject.SetActive(false); // Deactivates the entire button
    }

    // Show the skip button after a delay
    private void ShowSkipButtonAfterDelay(float delay)
    {
        StartCoroutine(ShowSkipButtonCoroutine(delay)); // Start the coroutine for showing the skip button
    }

    private IEnumerator ShowSkipButtonCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        ShowSkipButton(); // Show the skip button after delay
    }

    // Show the wave button (make it visible and set it to Skip)
    private void ShowSkipButton()
    {
        waveButton.gameObject.SetActive(true); // Reactivate the button after 10 seconds
        waveButtonText.text = "Skip"; // Set the text to "Skip"
    }

    // Complete the wave (called either after all enemies are defeated or when skipping)
    private void CompleteWave()
{
    // Reward the player with money for finishing or skipping the wave
    int reward = baseReward + ((currentWaveIndex - 1) * 100); // Incremental reward for each wave
    moneyManager.AddMoney(reward); // Add money to the player
    Debug.Log($"Wave {currentWaveIndex} completed! Player rewarded with {reward} money.");

    // Give 50 SeaCoins to the GetSeaCoinsManager every wave
    seaCoinsManager.AddSeaCoins(50); // Add SeaCoins to the GetSeaCoinsManager
    Debug.Log($"Player rewarded with 50 SeaCoins for completing wave {currentWaveIndex}.");

    // Check if this is the last wave
    if (currentWaveIndex >= waves.Length)
    {
        youWinGameObject.SetActive(true); // Activate the YouWin GameObject
        seaCoinsManager.AddSeaCoins(bonusSeaCoins); // Add bonus SeaCoins for completing the last wave
        Debug.Log($"Player rewarded with {bonusSeaCoins} SeaCoins for completing the final wave.");
        
        // Call TransferSeaCoinsToCurrencyManager from the GetSeaCoinsManager
        seaCoinsManager.TransferSeaCoinsToCurrencyManager();
    }

    // Set up for the next wave
    waveOngoing = false; // Reset the wave ongoing flag
    waveButtonText.text = "Play"; // Reset button text
    HideWaveButton(); // Hide the button again before the next wave

    // Start the next wave after a cooldown period
    StartCoroutine(StartNextWaveAfterCooldown(waveTransitionCooldown));
}


    private IEnumerator StartNextWaveAfterCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown); // Wait for the cooldown
        StartNextWave(); // Start the next wave
    }

    // Update the wave progress text (e.g., "4/10")
    private void UpdateWaveProgressText()
    {
        if (waveProgressText != null)
        {
            waveProgressText.text = $"{currentWaveIndex}/{waves.Length}"; // Update wave progress text
        }
    }

    // Update the enemy count text (display how many enemies are still alive)
    private void UpdateEnemyCountText()
    {
        if (enemyCountText != null)
        {
            int enemyCount = GameObject.FindGameObjectsWithTag(prefabTag).Length; // Count remaining enemies
            enemyCountText.text = $"Enemies left: {enemyCount}"; // Update enemy count text
        }
    }
}
