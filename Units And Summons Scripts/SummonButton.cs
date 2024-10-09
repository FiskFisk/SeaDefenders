using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SummonButton : MonoBehaviour
{
    public Button summonButton; // Button for summoning
    public Button secondButton; // Second button to deactivate during summon
    public Button thirdButton; // Third button to deactivate during summon
    public int costInSeaCoins = 100; // Cost for summoning

    public GameObject[] commonPrefabs;
    public GameObject[] rarePrefabs;
    public GameObject[] epicPrefabs;
    public GameObject[] legendaryPrefabs;
    public GameObject[] secretPrefabs;
    public GameObject[] godlyPrefabs;
    public GameObject[] otherworldlyPrefabs;
    public GameObject[] fishPrefabs;

    [Header("Content Parent Settings")]
    public string contentParentName; // Name of the parent to spawn the prefabs under
    public Transform imageParent; // Parent for spawning the image
    public GameObject imagePrefab; // Image to show after summon

    [Header("Rarity Chances (in %)")]
    public float commonChance = 40f;
    public float rareChance = 30f;
    public float epicChance = 15f;
    public float legendaryChance = 1f;
    public float secretChance = 0.5f;
    public float godlyChance = 0.1f;
    public float otherworldlyChance = 0.01f;
    public float fishChance = 0.001f;

    [Header("Timers")]
    public float imageSpawnDelay = 0f; // Duration to wait before spawning image
    public float imageDuration = 3f; // How long to show the image

    [Header("Animation")]
    public Animator summonAnimator;
    public string summonTrigger = "Summon";

    [Header("Options")]
    public bool skipAnimation = false; // Toggle in Inspector to skip animation

    public Transform contentParent; // Cached reference for content parent

    private void Start()
    {
        // Set up the button listener
        if (summonButton != null)
        {
            summonButton.onClick.AddListener(TrySummon);
        }

        // Find the content parent by name, even if it's deactivated
        GameObject foundContentParent = FindInactiveObjectByName(contentParentName);
        if (foundContentParent != null)
        {
            contentParent = foundContentParent.transform;
        }
        else
        {
            Debug.LogWarning($"Content parent with name '{contentParentName}' not found.");
        }
    }

    private void TrySummon()
    {
        if (CurrencyManager.Instance.seaCoins >= costInSeaCoins)
        {
            // Deduct the currency
            CurrencyManager.Instance.SpendSeaCoins(costInSeaCoins);

            // Disable buttons to prevent further summons
            summonButton.interactable = false;
            if (secondButton != null)
            {
                secondButton.interactable = false;
            }
            if (thirdButton != null)
            {
                thirdButton.interactable = false;
            }

            HandleSingleSummon();
        }
        else
        {
            Debug.LogWarning("Not enough SeaCoins!");
        }
    }

    private void HandleSingleSummon()
    {
        if (skipAnimation)
        {
            // Immediately spawn without waiting for the delay or playing animation
            SpawnPrefabWithImage(null);
            StartCoroutine(ReactivateButtonsAfterDelay());
        }
        else
        {
            // Play the animation and spawn after the delay
            if (summonAnimator != null)
            {
                summonAnimator.SetTrigger(summonTrigger);
            }
            StartCoroutine(DelayedSingleSummon());
        }
    }

    private IEnumerator DelayedSingleSummon()
    {
        // Wait for the image spawn delay before spawning the unit
        yield return new WaitForSeconds(imageSpawnDelay);
        SpawnPrefabWithImage(null);

        // Wait for the image duration before reactivating the buttons
        yield return new WaitForSeconds(imageDuration);

        // Re-enable buttons after single summon is completed
        StartCoroutine(ReactivateButtonsAfterDelay());
    }

    private IEnumerator ReactivateButtonsAfterDelay()
    {
        // Wait for the image duration to ensure it is fully displayed
        yield return new WaitForSeconds(imageDuration);
        summonButton.interactable = true; 
        if (secondButton != null)
        {
            secondButton.interactable = true;
        }
        if (thirdButton != null)
        {
            thirdButton.interactable = true;
        }
    }

    private void SpawnPrefabWithImage(GameObject prefabToSpawn)
    {
        // Get a random prefab if none is specified
        if (prefabToSpawn == null)
        {
            prefabToSpawn = GetRandomPrefab();
        }

        if (prefabToSpawn != null && contentParent != null)
        {
            GameObject instance = Instantiate(prefabToSpawn, contentParent);
            instance.name = prefabToSpawn.name;
            Debug.Log($"Summoned and instantiated: {instance.name}");
            AdjustInstance(instance);

            StartCoroutine(HandleImage(instance));
        }
    }

    private IEnumerator HandleImage(GameObject prefabInstance)
    {
        if (imagePrefab != null && imageParent != null)
        {
            GameObject imageInstance = Instantiate(imagePrefab, imageParent);
            Image imageComponent = imageInstance.GetComponent<Image>();

            SpriteRenderer prefabSpriteRenderer = prefabInstance.GetComponent<SpriteRenderer>();
            if (prefabSpriteRenderer != null && imageComponent != null)
            {
                imageComponent.sprite = prefabSpriteRenderer.sprite;
            }
            else
            {
                Debug.LogWarning("Prefab SpriteRenderer or Image component is missing.");
            }

            Destroy(imageInstance, imageDuration); // Destroy after the image duration
        }
        else
        {
            Debug.LogWarning("Image prefab or image parent is missing.");
        }

        yield return new WaitForSeconds(imageDuration); // Wait for image duration
    }

    // Method to find the parent GameObject even if it's inactive
    private GameObject FindInactiveObjectByName(string name)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == name)
            {
                return obj;
            }
        }
        return null;
    }

    // Make this method public to allow Summon10Units script to access it
    public GameObject GetRandomPrefab()
    {
        float randomValue = Random.Range(0f, 100f);

        if (randomValue < commonChance)
        {
            return GetRandomFromArray(commonPrefabs);
        }
        else if (randomValue < commonChance + rareChance)
        {
            return GetRandomFromArray(rarePrefabs);
        }
        else if (randomValue < commonChance + rareChance + epicChance)
        {
            return GetRandomFromArray(epicPrefabs);
        }
        else if (randomValue < commonChance + rareChance + epicChance + legendaryChance)
        {
            return GetRandomFromArray(legendaryPrefabs);
        }
        else if (randomValue < commonChance + rareChance + epicChance + legendaryChance + secretChance)
        {
            return GetRandomFromArray(secretPrefabs);
        }
        else if (randomValue < commonChance + rareChance + epicChance + legendaryChance + secretChance + godlyChance)
        {
            return GetRandomFromArray(godlyPrefabs);
        }
        else if (randomValue < commonChance + rareChance + epicChance + legendaryChance + secretChance + godlyChance + otherworldlyChance)
        {
            return GetRandomFromArray(otherworldlyPrefabs);
        }
        else
        {
            return GetRandomFromArray(fishPrefabs);
        }
    }

    private GameObject GetRandomFromArray(GameObject[] array)
    {
        if (array.Length > 0)
        {
            return array[Random.Range(0, array.Length)];
        }
        return null;
    }

    // Make this method public to allow Summon10Units script to access it
    public void AdjustInstance(GameObject instance)
    {
        // Custom logic to adjust the instance if needed
        instance.transform.localScale = new Vector3(1, 1, 1);
        // You can add more adjustments here as needed
    }
}
