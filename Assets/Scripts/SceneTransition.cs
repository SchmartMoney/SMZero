using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public CanvasGroup fadePanel;       // The panel to fade in/out
    public GameObject transitionText;  // The text GameObject to show during transitions
    public float fadeDuration = 1.5f;  // Duration for the fade animation

    private void Start()
    {
        // Ensure the fade panel starts fully transparent and transitionText is hidden
        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
            fadePanel.blocksRaycasts = false; // Allow interactions when transparent
        }

        if (transitionText != null)
        {
            transitionText.SetActive(false); // Ensure the text is hidden at start
        }
    }

    /// <summary>
    /// Method to transition to a scene by its build index.
    /// </summary>
    /// <param name="sceneIndex">The index of the scene in Build Settings.</param>
    public void TransitionToScene(int sceneIndex)
    {
        // Validate the scene index
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"Invalid scene index: {sceneIndex}. Please verify Build Settings.");
            return;
        }

        Debug.Log($"Starting transition to scene index {sceneIndex}");
        StartCoroutine(FadeOutAndLoadScene(sceneIndex));
    }

    private IEnumerator FadeOutAndLoadScene(int sceneIndex)
    {
        if (fadePanel == null)
        {
            Debug.LogError("Fade Panel is not assigned.");
            yield break;
        }

        fadePanel.blocksRaycasts = true; // Block interactions during the fade

        // Activate the transition text if available
        if (transitionText != null)
        {
            transitionText.SetActive(true);
        }

        // Fade to black
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            fadePanel.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        fadePanel.alpha = 1f; // Ensure it's fully opaque

        // Log the scene index being loaded
        Debug.Log($"Loading scene index: {sceneIndex} ({SceneManager.GetSceneByBuildIndex(sceneIndex).name})");

        // Load the new scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Fade back in after the new scene is loaded
        StartCoroutine(FadeInScene());
    }

    private IEnumerator FadeInScene()
    {
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            fadePanel.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        fadePanel.alpha = 0f; // Ensure it's fully transparent

        fadePanel.blocksRaycasts = false; // Allow interactions again

        // Deactivate the transition text after the fade-in
        if (transitionText != null)
        {
            transitionText.SetActive(false);
        }

        Debug.Log("Scene fade-in completed.");
    }
}
