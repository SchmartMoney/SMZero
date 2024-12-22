using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering; // For Volume
using UnityEngine.Rendering.Universal; // For URP-specific effects
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public CanvasGroup fadePanel; // Panel for fade effect
    public TMP_Text fadeText;     // Text for messages during transitions
    public float fadeDuration = 1.5f; // Duration of fade effect

    public Volume globalVolume; // Reference to Global Volume
    private Vignette vignette; // Vignette effect

    [Header("Default Loading Message Settings")]
    [TextArea]
    public string defaultTransitionMessage = "Loading...";
    public Color textColor = Color.white;      // Default text color
    public int fontSize = 36;                  // Default text size
    public Vector2 textPosition = new Vector2(0, -200); // Default text position (relative to the canvas center)

    private void Start()
    {
        // Initialize fade panel
        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
            fadePanel.blocksRaycasts = false;
        }

        // Initialize fade text appearance
        if (fadeText != null)
        {
            fadeText.text = "";
            fadeText.color = textColor;
            fadeText.fontSize = fontSize;
            fadeText.rectTransform.anchoredPosition = textPosition;
        }

        // Retrieve Vignette from the Global Volume
        if (globalVolume != null && globalVolume.profile != null)
        {
            if (globalVolume.profile.TryGet<Vignette>(out var vignetteEffect))
            {
                vignette = vignetteEffect;
                vignette.intensity.Override(0f); // Start with vignette disabled
            }
            else
            {
                Debug.LogError("No Vignette override found in the Global Volume profile.");
            }
        }
        else
        {
            Debug.LogError("Global Volume or its profile is not assigned.");
        }
    }

    public void FadeToScene(string sceneName, string transitionMessage = null)
    {
        string messageToShow = string.IsNullOrEmpty(transitionMessage) ? defaultTransitionMessage : transitionMessage;
        StartCoroutine(FadeOutAndLoadScene(sceneName, messageToShow));
    }

    private IEnumerator FadeOutAndLoadScene(string sceneName, string message)
    {
        fadePanel.blocksRaycasts = true;

        // Set transition message
        if (fadeText != null)
        {
            fadeText.text = message;
        }

        // Enable vignette
        if (vignette != null)
        {
            StartCoroutine(AdjustVignette(true));
        }

        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            fadePanel.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        fadePanel.alpha = 1f;

        // Load the new scene
        SceneManager.LoadScene(sceneName);

        // Wait a frame to ensure the scene loads
        yield return null;

        // Start fade-in
        StartCoroutine(FadeInScene());
    }

    private IEnumerator FadeInScene()
    {
        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            fadePanel.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        fadePanel.alpha = 0f;
        fadePanel.blocksRaycasts = false;

        // Disable vignette
        if (vignette != null)
        {
            StartCoroutine(AdjustVignette(false));
        }

        // Clear transition message
        if (fadeText != null)
        {
            fadeText.text = "";
        }
    }

    private IEnumerator AdjustVignette(bool enable)
    {
        if (vignette == null) yield break;

        float targetIntensity = enable ? 0.5f : 0f; // Adjust the vignette intensity
        float initialIntensity = vignette.intensity.value;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            vignette.intensity.Override(Mathf.Lerp(initialIntensity, targetIntensity, t / fadeDuration));
            yield return null;
        }

        vignette.intensity.Override(targetIntensity);
    }
}
