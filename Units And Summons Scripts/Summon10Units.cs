using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Summon10Units : MonoBehaviour
{
    public SummonButton summonButtonScript; // Reference to the original SummonButton script
    public Button summon10Button; // New summon button specific to this script
    public Button thirdButton; // New third button specific to this script
    public int costInSeaCoins = 500; // Different cost for summoning 10 units

    public bool summonByTen = true; // This script will always summon by 10 units
    public bool skipAnimation; // This is shared with the original SummonButton script

    private void Start()
    {
        // Set up the summon button listener
        if (summon10Button != null)
        {
            summon10Button.onClick.AddListener(TrySummon10);
        }

        // Ensure the skip animation checkbox is synced with the original SummonButton script
        skipAnimation = summonButtonScript.skipAnimation;
    }

    private void TrySummon10()
    {
        int finalCost = costInSeaCoins; // Use the cost specific to summoning 10 units

        // Check if the player has enough currency
        if (CurrencyManager.Instance.seaCoins >= finalCost)
        {
            // Deduct the currency
            CurrencyManager.Instance.SpendSeaCoins(finalCost);

            // Disable buttons to prevent further summons during the summoning process
            summon10Button.interactable = false;
            if (thirdButton != null)
            {
                thirdButton.interactable = false; // Deactivate the third button
            }

            // Start the summon coroutine to summon 10 units
            StartCoroutine(SummonMultiple());
        }
        else
        {
            Debug.LogWarning("Not enough SeaCoins!");
        }
    }

    private IEnumerator SummonMultiple()
    {
        for (int i = 0; i < 10; i++)
        {
            // For the first summon, play the animation if "skipAnimation" is not checked
            if (i == 0 && !skipAnimation)
            {
                if (summonButtonScript.summonAnimator != null)
                {
                    summonButtonScript.summonAnimator.SetTrigger(summonButtonScript.summonTrigger);
                }
                yield return new WaitForSeconds(summonButtonScript.imageSpawnDelay); // Wait for the animation
            }

            // Spawn the prefab for the current iteration
            SpawnPrefabWithImage(null);

            // Wait for the image duration before the next spawn
            yield return new WaitForSeconds(summonButtonScript.imageDuration);
        }

        // Wait for the last image to be destroyed before reactivating the buttons
        yield return new WaitForSeconds(summonButtonScript.imageDuration); // Wait for the last image duration

        // Re-enable buttons after all 10 summons
        summon10Button.interactable = true;
        if (thirdButton != null)
        {
            thirdButton.interactable = true; // Reactivate the third button
        }
    }

    private void SpawnPrefabWithImage(GameObject prefabToSpawn)
    {
        // Get a random prefab if none is specified
        if (prefabToSpawn == null)
        {
            prefabToSpawn = summonButtonScript.GetRandomPrefab();
        }

        if (prefabToSpawn != null && summonButtonScript.contentParent != null)
        {
            GameObject instance = Instantiate(prefabToSpawn, summonButtonScript.contentParent);
            instance.name = prefabToSpawn.name;
            Debug.Log($"Summoned and instantiated: {instance.name}");
            summonButtonScript.AdjustInstance(instance);
            
            StartCoroutine(HandleImage(instance)); // Wait delay for image
        }
    }

    private IEnumerator HandleImage(GameObject prefabInstance)
    {
        if (summonButtonScript.imagePrefab != null && summonButtonScript.imageParent != null)
        {
            GameObject imageInstance = Instantiate(summonButtonScript.imagePrefab, summonButtonScript.imageParent);
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

            Destroy(imageInstance, summonButtonScript.imageDuration); // Destroy after the image duration
        }
        else
        {
            Debug.LogWarning("Image prefab or image parent is missing.");
        }

        yield return new WaitForSeconds(summonButtonScript.imageDuration); // Wait for image duration
    }
}