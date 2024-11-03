using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown screenSizeDropdown; // Reference to your TMP_Dropdown
    [SerializeField] private Toggle fullScreenToggle; // Reference to the fullscreen toggle
    [SerializeField] private Toggle windowedToggle; // Reference to the windowed toggle

    private void Start()
    {
        // Initialize the dropdown and toggle states
        if (fullScreenToggle != null) fullScreenToggle.onValueChanged.AddListener(OnFullScreenToggle);
        if (windowedToggle != null) windowedToggle.onValueChanged.AddListener(OnWindowedToggle);
        if (screenSizeDropdown != null) screenSizeDropdown.onValueChanged.AddListener(OnScreenSizeChange);
        
        // Load initial settings
        LoadInitialSettings();
    }

    private void LoadInitialSettings()
    {
        // Load the last selected resolution from PlayerPrefs
        int lastResolutionIndex = PlayerPrefs.GetInt("LastResolutionIndex", -1);
        if (lastResolutionIndex >= 0)
        {
            screenSizeDropdown.value = lastResolutionIndex; // Set the dropdown to the last selected resolution
            OnScreenSizeChange(lastResolutionIndex); // Apply the last resolution
        }
        else
        {
            // Default to screen size on first run
            Resolution currentResolution = Screen.currentResolution;
            screenSizeDropdown.value = GetClosestResolutionIndex(currentResolution.width, currentResolution.height);
            OnScreenSizeChange(screenSizeDropdown.value); // Apply the default resolution
        }

        // Set initial toggle states
        fullScreenToggle.isOn = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;
        windowedToggle.isOn = !fullScreenToggle.isOn;
    }

    private void OnFullScreenToggle(bool isOn)
    {
        if (isOn)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            if (windowedToggle != null) windowedToggle.isOn = false; // Ensure the windowed toggle is off
        }
    }

    private void OnWindowedToggle(bool isOn)
    {
        if (isOn)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            if (fullScreenToggle != null) fullScreenToggle.isOn = false; // Ensure the fullscreen toggle is off
        }
    }

    private void OnScreenSizeChange(int index)
    {
        // Update this array with the 10 most common resolutions
        Resolution[] resolutions = {
            new Resolution { width = 640, height = 480 },    // 4:3 VGA
            new Resolution { width = 800, height = 600 },    // 4:3 SVGA
            new Resolution { width = 1024, height = 768 },   // 4:3 XGA
            new Resolution { width = 1280, height = 720 },   // 16:9 HD
            new Resolution { width = 1280, height = 800 },   // 16:10 WXGA
            new Resolution { width = 1366, height = 768 },   // 16:9 WXGA
            new Resolution { width = 1600, height = 900 },   // 16:9 HD+
            new Resolution { width = 1920, height = 1080 },  // 16:9 Full HD
            new Resolution { width = 2560, height = 1440 },  // 16:9 QHD
            new Resolution { width = 3840, height = 2160 }   // 16:9 4K UHD
        };

        if (index >= 0 && index < resolutions.Length)
        {
            Resolution selectedResolution = resolutions[index];
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);

            // Save the last selected resolution index
            PlayerPrefs.SetInt("LastResolutionIndex", index);
            PlayerPrefs.Save(); // Ensure the changes are saved
        }
    }

    private int GetClosestResolutionIndex(int width, int height)
    {
        // Updated list of 10 resolutions
        Resolution[] resolutions = {
            new Resolution { width = 640, height = 480 },    // 4:3 VGA
            new Resolution { width = 800, height = 600 },    // 4:3 SVGA
            new Resolution { width = 1024, height = 768 },   // 4:3 XGA
            new Resolution { width = 1280, height = 720 },   // 16:9 HD
            new Resolution { width = 1280, height = 800 },   // 16:10 WXGA
            new Resolution { width = 1366, height = 768 },   // 16:9 WXGA
            new Resolution { width = 1600, height = 900 },   // 16:9 HD+
            new Resolution { width = 1920, height = 1080 },  // 16:9 Full HD
            new Resolution { width = 2560, height = 1440 },  // 16:9 QHD
            new Resolution { width = 3840, height = 2160 }   // 16:9 4K UHD
        };

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == width && resolutions[i].height == height)
            {
                return i; // Return the index of the closest match
            }
        }

        return 0; // Return the first index if no match is found
    }

    private void Update()
    {
        // Optional: Handle additional logic if needed during resizing
        // For example, you can check for specific window resizing behavior here
    }
}
