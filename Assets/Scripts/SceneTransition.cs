using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    // Public fields to assign in the Inspector
    public CanvasGroup loadingScreen;    // The panel to fade in/out
    public GameObject loadingScreenText; // The text to display during transitions
    public Volume globalVolume;          // Post-Processing Global Volume for vignette control
    public GameObject menu;              // The menu that should disappear on "Proceed"

    public float fadeDuration = 1.5f;    // Duration for fade animations
    private Vignette vignetteEffect;     // Reference to the vignette effect

    /// <summary>
    /// Initialize and configure components.
    /// </summary>
    private void Start()
    {
        // Ensure loading screen starts fully disabled
        if (loadingScreen != null)
        {
            loadingScreen.gameObject.SetActive(false);
            loadingScreen.alpha = 0f;
        }

        // Ensure the loading text is hidden initially
        if (loadingScreenText != null)
        {
            loadingScreenText.SetActive(false);
        }

        // Access the Vignette effect from Global Volume
        if (globalVolume != null && globalVolume.profile.TryGet(out Vignette vignette))
        {
            vignetteEffect = vignette;
            vignetteEffect.intensity.value = 0; // Set initial vignette intensity
        }

        // Ensure the menu is active at the start
        if (menu != null)
        {
            menu.SetActive(true);
        }
    }

    /// <summary>
    /// Triggered when the Proceed button is clicked.
    /// </summary>
    /// <param name="sceneIndex">Index of the target scene in Build Settings.</param>
    public void ProceedAndTransition(int sceneIndex)
    {
        if (menu != null)
        {
            menu.SetActive(false); // Hide the menu when proceeding
        }

        // Enable the loading screen and text
        if (loadingScreen != null)
        {
            loadingScreen.gameObject.SetActive(true);
        }

        if (loadingScreenText != null)
        {
            loadingScreenText.SetActive(false); // Initially hidden, will show after fade-in
        }

        StartCoroutine(FadeOutAndLoadScene(sceneIndex));
    }

    private IEnumerator FadeOutAndLoadScene(int sceneIndex)
    {
        if (loadingScreen == null)
        {
            Debug.LogError("Loading Screen (CanvasGroup) is not assigned.");
            yield break;
        }

        loadingScreen.blocksRaycasts = true; // Block interactions during fade

        // Fade in the loading screen and increase vignette intensity
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;

            // Animate loading screen alpha
            loadingScreen.alpha = Mathf.Lerp(0f, 1f, normalizedTime);

            // Animate vignette intensity
            if (vignetteEffect != null)
            {
                vignetteEffect.intensity.value = Mathf.Lerp(0f, 1f, normalizedTime);
            }

            yield return null;
        }

        // Ensure fully opaque and vignette at max intensity
        loadingScreen.alpha = 1f;
        if (vignetteEffect != null) vignetteEffect.intensity.value = 1f;

        // Display loading screen text after fade-in
        if (loadingScreenText != null)
        {
            loadingScreenText.SetActive(true);
        }

        // Wait for 2 seconds to simulate loading condition
        yield return new WaitForSeconds(2f);

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
            float normalizedTime = t / fadeDuration;

            // Animate loading screen alpha
            loadingScreen.alpha = Mathf.Lerp(1f, 0f, normalizedTime);

            // Animate vignette intensity
            if (vignetteEffect != null)
            {
                vignetteEffect.intensity.value = Mathf.Lerp(1f, 0f, normalizedTime);
            }

            yield return null;
        }

        // Ensure fully transparent and vignette intensity reset
        loadingScreen.alpha = 0f;
        if (vignetteEffect != null) vignetteEffect.intensity.value = 0f;

        loadingScreen.blocksRaycasts = false; // Allow interactions again

        // Deactivate loading screen and text
        if (loadingScreen != null)
        {
            loadingScreen.gameObject.SetActive(false);
        }

        if (loadingScreenText != null)
        {
            loadingScreenText.SetActive(false);
        }

        Debug.Log("Scene fade-in completed.");
    }
}
