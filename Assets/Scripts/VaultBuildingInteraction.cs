using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VaultBuildingInteraction : MonoBehaviour
{
    public GameObject popupMenu;        // Reference to the UI Panel pop-up
    public Button cancelButton;         // UI button to cancel and close the menu
    public Button proceedButton;        // UI button to transition scenes
    public Volume globalVolume;         // Post-Processing Global Volume
    private Vignette vignetteEffect;    // Reference to the vignette effect
    private bool isTransitioning = false;

    void Start()
    {
        // Ensure popup menu is hidden initially
        popupMenu.SetActive(false);

        // Add listeners to buttons
        cancelButton.onClick.AddListener(ClosePopup);
        proceedButton.onClick.AddListener(StartTransition);

        // Access the Vignette effect from Global Volume
        if (globalVolume.profile.TryGet(out Vignette vignette))
        {
            vignetteEffect = vignette;
            vignetteEffect.intensity.value = 0; // Initial vignette intensity
        }
    }

    void OnMouseDown()
    {
        // When Vault Avenue building is clicked, show the pop-up menu
        if (!isTransitioning) popupMenu.SetActive(true);
    }

    public void ClosePopup()
    {
        // Close the pop-up menu
        popupMenu.SetActive(false);
    }

    public void StartTransition()
    {
        // Public method for OnClick; starts the vignette transition coroutine
        if (!isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(AnimateVignetteAndLoadScene());
        }
    }

    private System.Collections.IEnumerator AnimateVignetteAndLoadScene()
    {
        float duration = 1.5f; // Transition duration in seconds
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float intensity = Mathf.Lerp(0, 1, elapsed / duration);
            vignetteEffect.intensity.value = intensity;
            yield return null;
        }

        // Load the detailed Vault Avenue scene
        SceneManager.LoadScene("VaultAvenueDetailedScene");
    }
}
