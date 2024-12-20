using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class VignettingSceneTransition : MonoBehaviour
{
    public Volume postProcessingVolume; // The Post-Processing Volume for controlling effects
    private Vignette vignette;          // Reference to the Vignette effect
    private bool isTransitioning = false;

    private void Start()
    {
        // Ensure the Volume and Vignette effect are set up properly
        if (postProcessingVolume == null)
        {
            Debug.LogError("Post-Processing Volume is not assigned.");
            return;
        }

        if (postProcessingVolume.profile.TryGet(out Vignette vignetteEffect))
        {
            vignette = vignetteEffect;
            vignette.intensity.value = 0f; // Ensure it starts fully transparent
        }
        else
        {
            Debug.LogError("No Vignette effect found in the Volume profile.");
        }
    }

    public void FadeToScene(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(FadeOutAndLoadScene(sceneName));
        }
    }

    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        if (vignette == null)
        {
            Debug.LogError("Vignette effect is not set up.");
            yield break;
        }

        isTransitioning = true;

        // Fade out with Vignette effect
        float fadeDuration = 1.5f; // Duration of the fade
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            vignette.intensity.value = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        vignette.intensity.value = 1f; // Ensure it reaches full intensity

        // Load the new scene
        SceneManager.LoadScene(sceneName);

        // Wait for one frame after the scene loads
        yield return null;

        // Fade in after loading
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            vignette.intensity.value = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        vignette.intensity.value = 0f; // Ensure it resets to fully transparent

        isTransitioning = false;
    }
}
