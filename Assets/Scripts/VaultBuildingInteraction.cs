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
        popupMenu.SetActive(false);

        // Add listeners to buttons
        cancelButton.onClick.AddListener(ClosePopup);
        proceedButton.onClick.AddListener(() => StartTransition("VaultAvenueDetailedScene", "Going to Vault Avenue..."));

        // Access the Vignette effect from Global Volume
        if (globalVolume.profile.TryGet(out Vignette vignette))
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
    }

    void OnMouseDown()
    {
        // Record the time when the mouse is pressed (or touch starts)
        touchStartTime = Time.time;
        isTouching = true;
    }

    void OnMouseUp()
    {
        // When the mouse is released (or touch ends)
        if (isTouching)
        {
            float touchDuration = Time.time - touchStartTime;

            // Check if it was a quick tap
            if (touchDuration <= quickTapThreshold && !isTransitioning)
            {
                OpenPopup(); // Open the popup menu
            }

            // Reset the touch flag
            isTouching = false;
        }
    }

    private void OpenPopup()
    {
        Debug.Log("Popup menu opened on Vault Avenue object."); // Debug for verification
        popupMenu.SetActive(true); // Show the popup menu
    }

    public void ClosePopup()
    {
        Debug.Log("Popup menu closed."); // Debug for verification
        popupMenu.SetActive(false); // Close the popup menu
    }

    public void StartTransition(string sceneName, string message)
    {
        // Ensure the menu disappears before starting the vignette transition
        popupMenu.SetActive(false);

        if (!isTransitioning)
        {
            isTransitioning = true;

            if (transitionTextObject != null && transitionText != null)
            {
                transitionTextObject.SetActive(true); // Show the text GameObject
                transitionText.text = message; // Set the transition message
            }

            StartCoroutine(AnimateVignetteAndLoadScene(sceneName));
        }
    }

    private System.Collections.IEnumerator AnimateVignetteAndLoadScene(string sceneName)
    {
        float duration = 1.5f; // Transition duration in seconds
        float elapsed = 0f;

        // Fade the vignette intensity to full
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float intensity = Mathf.Lerp(0, 1, elapsed / duration);
            vignetteEffect.intensity.value = intensity;
            yield return null;
        }

        // Load the next scene
        SceneManager.LoadScene(sceneName);

        // Wait for the scene to load (small delay to ensure scene is ready)
        yield return null;

        // Reset elapsed time for fade-in
        elapsed = 0f;

        // Fade the vignette intensity back to zero
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float intensity = Mathf.Lerp(1, 0, elapsed / duration);
            vignetteEffect.intensity.value = intensity;
            yield return null;
        }

        // Ensure vignette effect is fully reset
        vignetteEffect.intensity.value = 0;

        // Hide the transition text
        if (transitionTextObject != null)
        {
            transitionTextObject.SetActive(false);
        }

        // Reset transition state
        isTransitioning = false;
    }
}
