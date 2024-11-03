using UnityEngine;
using UnityEngine.UI;

public class TowerPlacement : MonoBehaviour
{
    public Button placementButton; // UI Button to start placement
    public Texture2D cursorTexture; // Custom cursor texture for placement
    public Texture2D redCursorTexture; // Cursor texture for obstacles or invalid areas
    public Texture2D defaultCursorTexture; // Default cursor texture
    public GameObject towerPrefab; // The prefab to spawn
    public LayerMask placableLayer; // Layer mask for placable objects
    public LayerMask obstacleLayers; // Layer mask for obstacles

    private TowerStats buttonTowerStats; // Reference to the TowerStats component on the button
    private bool isPlacing = false; // State to check if we are placing a tower
    private InGameMoney inGameMoney; // Reference to the InGameMoney component
    private int placedTowers = 0; // Counter for placed towers
    private float towerRadius = 0.5f; // Default radius for obstacle checking
    private GameObject ghostTower; // Reference to the ghost (preview) tower

    private void Start()
    {
        // Set up the button listener
        if (placementButton != null)
        {
            buttonTowerStats = placementButton.GetComponent<TowerStats>();
            if (buttonTowerStats == null)
            {
                Debug.LogError("TowerStats component is missing from the placement button.");
            }
            placementButton.onClick.AddListener(OnPlacementButtonClicked);
        }
        else
        {
            Debug.LogError("PlacementButton is not assigned.");
        }

        // Ensure defaultCursorTexture is assigned
        if (defaultCursorTexture == null)
        {
            Debug.LogWarning("Default cursor texture is not assigned.");
        }

        // Find the InGameMoney component
        inGameMoney = FindObjectOfType<InGameMoney>();
        if (inGameMoney == null)
        {
            Debug.LogError("InGameMoney component not found in the scene.");
        }

        // Check the size of the collider on the tower prefab and set the appropriate radius
        if (towerPrefab != null)
        {
            BoxCollider2D boxCollider = towerPrefab.GetComponent<BoxCollider2D>();
            CircleCollider2D circleCollider = towerPrefab.GetComponent<CircleCollider2D>();

            if (boxCollider != null)
            {
                // Calculate the "radius" of the BoxCollider2D (half of the longest side)
                towerRadius = Mathf.Max(boxCollider.size.x, boxCollider.size.y) / 2f;
            }
            else if (circleCollider != null)
            {
                // Use the radius of the CircleCollider2D
                towerRadius = circleCollider.radius;
            }
            else
            {
                Debug.LogWarning("TowerPrefab does not have a CircleCollider2D or BoxCollider2D. Defaulting to radius 0.5.");
            }
        }
        else
        {
            Debug.LogError("TowerPrefab is not assigned.");
        }
    }

    private void Update()
    {
        if (isPlacing && ghostTower != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Ensure the z-position is 0 for 2D

            // Update the ghost tower's position to follow the mouse
            ghostTower.transform.position = mousePosition;

            UpdateCursor(mousePosition);

            // Check if the right mouse button is clicked to cancel placement
            if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
                return;
            }

            // Check if the left mouse button is clicked
            if (Input.GetMouseButtonDown(0))
            {
                if (!IsNearObstacle(mousePosition))
                {
                    // Perform a raycast to check if the click is on a Placable GameObject
                    if (Physics2D.OverlapPoint(mousePosition, placableLayer) != null)
                    {
                        // Ensure unit count has not been exceeded
                        if (placedTowers < buttonTowerStats.unitCount)
                        {
                            if (buttonTowerStats != null)
                            {
                                // Check if player has enough currency
                                if (inGameMoney != null && inGameMoney.GetMoney() >= buttonTowerStats.cost)
                                {
                                    // Deduct the cost
                                    inGameMoney.SpendMoney(buttonTowerStats.cost);

                                    // Instantiate the tower at the mouse position
                                    GameObject newTower = Instantiate(towerPrefab, mousePosition, Quaternion.identity);
                                    // Do not change the layer of the new tower; it will keep its original layer

                                    // Initialize the placed tower with data from the button
                                    PlacedTowerStats placedTowerStats = newTower.GetComponent<PlacedTowerStats>();
                                    if (placedTowerStats != null)
                                    {
                                        placedTowerStats.damage = buttonTowerStats.damage;
                                        placedTowerStats.range = buttonTowerStats.range;
                                        placedTowerStats.attackSpeed = buttonTowerStats.attackSpeed;
                                        placedTowerStats.cost = buttonTowerStats.cost;
                                    }
                                    else
                                    {
                                        Debug.LogError("PlacedTowerStats component is missing from the instantiated tower prefab.");
                                    }

                                    // Increment the placed towers count
                                    placedTowers++;

                                    // Destroy the ghost tower after placing
                                    Destroy(ghostTower);

                                    // Reset the cursor to default after placement
                                    isPlacing = false;
                                    ResetCursor();
                                }
                                else
                                {
                                    Debug.Log("Not enough currency to place the tower.");
                                }
                            }
                            else
                            {
                                Debug.LogError("ButtonTowerStats reference is missing.");
                            }
                        }
                        else
                        {
                            Debug.Log("Max unit count reached for this tower type.");
                        }
                    }
                }
                else
                {
                    Debug.Log("Cannot place tower here. Obstacle detected.");
                }
            }
        }
    }

    private void OnPlacementButtonClicked()
    {
        // Toggle the placing state
        isPlacing = !isPlacing;

        // Reset the placed towers count when placing a new tower type
        if (isPlacing && placedTowers >= buttonTowerStats.unitCount)
        {
            Debug.Log("Max unit count reached for this tower type.");
            isPlacing = false;
            return;
        }

        if (isPlacing)
        {
            // Change the cursor texture when starting placement
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);

            // Spawn a ghost version of the tower to follow the mouse
            ghostTower = Instantiate(towerPrefab);
            ghostTower.layer = LayerMask.NameToLayer("Default"); // Set the ghost to the "Default" layer

            // Make sure the ghost tower is semi-transparent
            SpriteRenderer ghostRenderer = ghostTower.GetComponent<SpriteRenderer>();
            if (ghostRenderer != null)
            {
                Color color = ghostRenderer.color;
                color.a = 0.5f; // Set alpha to 50%
                ghostRenderer.color = color;
            }
        }
        else
        {
            // Reset the cursor and destroy the ghost tower when placement is canceled
            ResetCursor();
            if (ghostTower != null)
            {
                Destroy(ghostTower);
            }
        }
    }

    private bool IsNearObstacle(Vector3 position)
    {
        // Use OverlapCircle to check for obstacles, matching the size of the tower's collider
        return Physics2D.OverlapCircle(position, towerRadius, obstacleLayers) != null;
    }

    private void UpdateCursor(Vector3 mousePosition)
    {
        // Check if hovering over an obstacle
        if (IsNearObstacle(mousePosition))
        {
            // Change the cursor to red when hovering near an obstacle
            Cursor.SetCursor(redCursorTexture, Vector2.zero, CursorMode.Auto);
        }
        // Check if hovering over a placable object
        else if (Physics2D.OverlapPoint(mousePosition, placableLayer) != null)
        {
            // Change the cursor to the placement texture when hovering over a placable object
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            // If not hovering over placable or obstacle layers, set the cursor to red (cannot place)
            Cursor.SetCursor(redCursorTexture, Vector2.zero, CursorMode.Auto);
        }
    }

    private void ResetCursor()
    {
        // Reset the cursor to the default texture if provided, or system default if not
        if (defaultCursorTexture != null)
        {
            Cursor.SetCursor(defaultCursorTexture, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    private void CancelPlacement()
    {
        // Reset the cursor and placement state without deducting money
        ResetCursor();
        isPlacing = false;

        // Destroy the ghost tower if placement is canceled
        if (ghostTower != null)
        {
            Destroy(ghostTower);
        }
    }
}
