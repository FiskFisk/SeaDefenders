using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoadPrefabManager : MonoBehaviour
{
    [SerializeField] private string parentObjectName; // Name of the parent GameObject
    [SerializeField] private string objectToDeleteName; // Name of the GameObject to delete after loading
    private GameObject parentObject;
    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "prefabData.dat"); // Changed to .dat for binary files
        Debug.Log($"Save file path: {saveFilePath}");

        // Search for the parent object, even if it's inactive
        parentObject = FindInactiveGameObjectByName(parentObjectName);
        
        if (parentObject == null)
        {
            Debug.LogError($"Parent object with the name '{parentObjectName}' not found.");
        }
    }

    // Method to find inactive GameObjects by name
    private GameObject FindInactiveGameObjectByName(string name)
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

    public void Save()
    {
        if (parentObject == null)
        {
            Debug.LogError("Cannot save. Parent object is not set.");
            return;
        }

        List<PrefabData> prefabDataList = new List<PrefabData>();

        foreach (Transform child in parentObject.transform)
        {
            IDScript idScript = child.GetComponent<IDScript>();
            if (idScript != null)
            {
                PrefabData data = new PrefabData
                {
                    id = idScript.id,
                    starCount = idScript.starCount,
                    levelCount = idScript.levelCount,
                    rarityID = idScript.rarityID
                };

                prefabDataList.Add(data);
            }
        }

        // Save to binary file
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(saveFilePath, FileMode.Create);
        formatter.Serialize(stream, prefabDataList);
        stream.Close();

        Debug.Log($"Saved {prefabDataList.Count} prefabs to {saveFilePath}");
        Application.Quit();
        Debug.Log("Application has been closed.");
    }

    public void Load()
    {
        Debug.Log("Load method called.");

        // Clear existing children of the parent object
        foreach (Transform child in parentObject.transform)
        {
            Destroy(child.gameObject);
        }

        // Load from binary file
        if (File.Exists(saveFilePath))
        {
            Debug.Log("Save file found. Attempting to read...");
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(saveFilePath, FileMode.Open);

            List<PrefabData> prefabDataList = (List<PrefabData>)formatter.Deserialize(stream);
            stream.Close();

            foreach (PrefabData prefabData in prefabDataList)
            {
                string prefabPath = $"Towers/UI/{prefabData.rarityID}/{prefabData.id}"; // Construct the path
                GameObject prefab = Resources.Load<GameObject>(prefabPath);

                if (prefab != null)
                {
                    GameObject instance = Instantiate(prefab, parentObject.transform);

                    IDScript idScript = instance.GetComponent<IDScript>();
                    if (idScript != null)
                    {
                        idScript.starCount = prefabData.starCount;
                        idScript.levelCount = prefabData.levelCount;
                        Debug.Log($"Loaded prefab: {prefabData.id} with Star Count: {idScript.starCount}, Level Count: {idScript.levelCount}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Prefab with ID {prefabData.id} not found in Resources/Towers/UI/{prefabData.rarityID}.");
                }
            }

            Debug.Log("All prefabs have been loaded.");
        }
        else
        {
            Debug.LogWarning($"Save file not found at {saveFilePath}. Creating an empty prefab data file.");
            List<PrefabData> emptyPrefabDataList = new List<PrefabData>();
            // Create empty binary file
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(saveFilePath, FileMode.Create);
            formatter.Serialize(stream, emptyPrefabDataList);
            stream.Close();
            Debug.Log("Empty prefab data file created.");
        }

        // Check the HaveClicked script and set deleteObject to true
        HaveClicked haveClicked = FindObjectOfType<HaveClicked>();
        if (haveClicked != null)
        {
            // Set the checkbox to true
            haveClicked.deleteObject = true; // This makes the checkbox TRUE
            
            Debug.Log("Checkbox in HaveClicked script has been set to true.");
        }
        else
        {
            Debug.LogWarning("HaveClicked script not found in the scene.");
        }

        // Find and delete the specified GameObject by name if it exists
        GameObject objectToDelete = GameObject.Find(objectToDeleteName);
        if (objectToDelete != null)
        {
            Destroy(objectToDelete);
            Debug.Log($"{objectToDelete.name} has been deleted.");
        }
        else
        {
            Debug.LogWarning($"No object found with the name '{objectToDeleteName}' for deletion.");
        }
    }

    [System.Serializable]
    public class PrefabData
    {
        public string id;
        public int starCount;
        public int levelCount;
        public string rarityID;
    }
}
