using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class VaultBuildingInteraction : MonoBehaviour
{
    public GameObject popupMenu;        // Reference to the UI Panel pop-up (e.g., VAPanel)
    public Button cancelButton;         // UI button to cancel and close the menu
    public Button proceedButton;        // UI button to transition scenes
    public Volume globalVolume;         // Post-Processing Global Volume
    public GameObject transitionTextObject; // Text GameObject to show messages during transition
    private TMP_Text transitionText;     // Reference to the TMP_Text component
    private Vignette vignetteEffect;    // Reference to the vignette effect
    private bool isTransitioning = false;

    private float touchStartTime = 0f;  // Time when the touch starts
    private bool isTouching = false;    // Flag to check if the touch is in progress
    private const float quickTapThreshold = 0.3f; // Threshold time for a quick tap

    void Start()
    {
        // Ensure popup menu is hidden initially
        if (popupMenu != null)
        {
            popupMenu.SetActive(false);
        }

        // Add listeners to buttons
        if (cancelButton != null)
            cancelButton.onClick.AddListener(ClosePopup);

        if (proceedButton != null)
            proceedButton.onClick.AddListener(() => StartTransition("ModelsTest3", "Loading Models Test Scene..."));

        // Access the Vignette effect from Global Volume
        if (globalVolume != null && globalVolume.profile.TryGet(out Vignette vignette))
        {
            vignetteEffect = vignette;
            vignetteEffect.intensity.value = 0; // Initial vignette intensity
        }

        // Access the TMP_Text component from the transitionTextObject
        if (transitionTextObject != null)
        {
            transitionText = transitionTextObject.GetComponent<TMP_Text>();
            transitionTextObject.SetActive(false); // Hide text GameObject at the start
        }

        // Start a fade-out effect if this is the VaultAvenueDetailedScene
        if (SceneManager.GetActiveScene().name == "VaultAvenueDetailedScene")
        {
            StartCoroutine(FadeOutVignetteQuickly());
        }
    }

    void OnMouseDown()
    {
        touchStartTime = Time.time;
        isTouching = true;
    }

    void OnMouseUp()
    {
        if (isTouching)
        {
            float touchDuration = Time.time - touchStartTime;

            if (touchDuration <= quickTapThreshold && !isTransitioning)
            {
                OpenPopup();
            }

            isTouching = false;
        }
    }

    private void OpenPopup()
    {
        if (popupMenu != null)
        {
            Debug.Log("Popup menu opened.");
            popupMenu.SetActive(true);
        }
    }

    public void ClosePopup()
    {
        if (popupMenu != null)
        {
            Debug.Log("Popup menu closed.");
            popupMenu.SetActive(false);
        }
    }

    public void StartTransition(string sceneName, string message)
    {
        if (!isTransitioning)
        {
            isTransitioning = true;

            if (popupMenu != null)
                popupMenu.SetActive(false);

            if (transitionTextObject != null && transitionText != null)
            {
                transitionTextObject.SetActive(true);
                transitionText.text = message;
            }

            StartCoroutine(AnimateVignetteAndLoadScene(sceneName));
        }
    }

    private System.Collections.IEnumerator AnimateVignetteAndLoadScene(string sceneName)
    {
        float duration = 1.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float intensity = Mathf.Lerp(0, 1, elapsed / duration);
            vignetteEffect.intensity.value = intensity;
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }

    private System.Collections.IEnumerator FadeOutVignetteQuickly()
    {
        float duration = 0.5f; // Short fade-out duration
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float intensity = Mathf.Lerp(1, 0, elapsed / duration);
            vignetteEffect.intensity.value = intensity;
            yield return null;
        }

        vignetteEffect.intensity.value = 0;
    }
}
