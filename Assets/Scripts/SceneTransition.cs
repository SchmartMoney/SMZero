using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public CanvasGroup fadePanel;       // The panel to fade in/out (set to 100% black in the Inspector)
    public TMP_Text fadeText;           // The text element for messages during transitions

    private void Start()
    {
        // Ensure the panel starts fully transparent and disabled
        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
            fadePanel.blocksRaycasts = false;
        }

        if (fadeText != null)
        {
            fadeText.text = ""; // Clear any text
        }
    }

    public void FadeToScene(string sceneName, string message)
    {
        // Start the fade transition with the given scene name and message
        Debug.Log("Starting fade to scene: " + sceneName);
        StartCoroutine(FadeOutAndLoadScene(sceneName, message));
    }

    // Helper method for "Proceed" button to transition to Vault Avenue
    public void FadeToVaultAvenueScene()
    {
        Debug.Log("Proceeding to Vault Avenue...");
        FadeToScene("VaultAvenueDetailedScene", "Loading Vault Avenue...");
    }

    // Helper method for "Return Now" button to transition back to City Map
    public void FadeToCityMapScene()
    {
        Debug.Log("Returning to City Map...");
        FadeToScene("CityMapScene", "Returning to City Map...");
    }

    private IEnumerator FadeOutAndLoadScene(string sceneName, string message)
    {
        if (fadePanel == null)
        {
            Debug.LogError("Fade Panel is not assigned.");
            yield break;
        }

        fadePanel.blocksRaycasts = true; // Block interactions during the fade

        // Display the message (if provided)
        if (fadeText != null)
        {
            fadeText.text = message;
        }

        // Fade to black
        float fadeDuration = 1.5f; // Adjust fade duration as needed
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            fadePanel.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        fadePanel.alpha = 1f; // Ensure it's fully opaque

        // Log for debugging
        Debug.Log("Fading completed. Loading scene: " + sceneName);

        // Load the scene
        SceneManager.LoadScene(sceneName);

        // Wait for a frame to allow scene loading
        yield return null;

        // Fade back to transparent
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            fadePanel.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        fadePanel.alpha = 0f; // Ensure it's fully transparent

        fadePanel.blocksRaycasts = false; // Allow interactions again

        Debug.Log("Scene transition completed.");
    }
}
