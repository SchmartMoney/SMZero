using UnityEngine;

public static class SceneNames
{
    public const string MAIN_SCENE = "MainScene";
    public const string VAULT_SCENE = "VaultAvenueDetailedScene";
    // Add other scene names as constants here
}

[System.Serializable]
public class LocationData
{
    public string locationId;
    public string displayName;
    public string description;
    [Tooltip("Use SceneNames constants for scene paths")]
    public string sceneToLoad;  // Example: SceneNames.VAULT_SCENE
    public bool isInteractable = false;  // Whether this location can be entered
    public bool isMarketplace = false;  // Whether this location has a marketplace

    // Helper method to validate scene name
    public bool ValidateScenePath()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError($"Scene path is empty for location: {displayName}");
            return false;
        }

        // Check if scene exists in build settings
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneName == sceneToLoad)
            {
                return true;
            }
        }

        Debug.LogError($"Scene '{sceneToLoad}' not found in build settings for location: {displayName}");
        return false;
    }
}

[System.Serializable]
public class LocationUIData
{
    public string title;
    public string description;
    public bool showEnterButton = true;
    public bool showLeaveButton = true;
} 