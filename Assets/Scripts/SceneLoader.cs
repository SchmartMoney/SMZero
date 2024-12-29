using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public GameObject fadeCanvas;      // Reference to the FadeCanvas (parent of the LoadingScreen)
    public GameObject loadingScreen;  // Reference to the LoadingScreen (child of FadeCanvas)
    public Slider loadingBar;         // Reference to the progress bar slider

    /// <summary>
    /// Public method to load a scene by its Build Index.
    /// </summary>
    /// <param name="levelIndex">Build Index of the target scene</param>
    public void LoadScene(int levelIndex)
    {
        // Check if levelIndex is valid
        if (levelIndex < 0 || levelIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError("Invalid scene index: " + levelIndex);
            return;
        }

        // Activate the FadeCanvas and LoadingScreen if they are disabled
        if (fadeCanvas != null)
        {
            fadeCanvas.SetActive(true);
        }
        else
        {
            Debug.LogError("FadeCanvas is not assigned!");
        }

        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }
        else
        {
            Debug.LogError("LoadingScreen is not assigned!");
        }

        // Start loading the scene
        StartCoroutine(LoadSceneAsynchronously(levelIndex));
    }

    private IEnumerator LoadSceneAsynchronously(int levelIndex)
    {
        // Begin async loading
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelIndex);

        // Update the progress bar while loading
        while (!operation.isDone)
        {
            if (loadingBar != null)
            {
                // Normalize progress value (0.0 - 1.0)
                loadingBar.value = Mathf.Clamp01(operation.progress / 0.9f);
            }
            else
            {
                Debug.LogWarning("LoadingBar slider is not assigned!");
            }

            yield return null;
        }
    }
}
