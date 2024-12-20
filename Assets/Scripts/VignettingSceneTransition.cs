using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class VignettingSceneTransition : MonoBehaviour
{
    [SerializeField] private Volume globalVolume; // Post-processing global volume
    [SerializeField] private TMP_Text transitionText; // Text for transition messages

    private Vignette vignetteEffect;

    // This is the Start method that runs when the scene starts
    private void Start()
    {
        // Access vignette effect from the volume profile
        if (globalVolume.profile.TryGet(out Vignette vignette))
        {
            vignetteEffect = vignette;
            vignetteEffect.intensity.value = 1f; // Start with full vignette (darkened screen)
            StartCoroutine(FadeInVignette()); // Start fading in vignette effect
        }

        // Optionally, add logic for transition text
        if (transitionText != null)
        {
            transitionText.text = "Scene Transition..."; // Set the initial transition text
        }
    }

    // Coroutine to handle the fading in of vignette intensity
    private System.Collections.IEnumerator FadeInVignette()
    {
        float duration = 1.5f; // Duration for fade-in
        float elapsed = 0f;

        // Fade vignette intensity from 1 to 0 over time
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            vignetteEffect.intensity.value = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        vignetteEffect.intensity.value = 0f; // Ensure vignette ends at 0
    }
}
