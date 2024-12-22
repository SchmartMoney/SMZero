using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class VaultBuildingInteraction : MonoBehaviour
{
    public GameObject popupMenu;          // Reference to the UI Panel pop-up (e.g., VAPanel)
    public Button cancelButton;           // UI button to cancel and close the menu
    public Volume globalVolume;           // Post-Processing Global Volume
    private Vignette vignetteEffect;      // Reference to the vignette effect
    private TMP_Text transitionText;      // Reference to the TMP_Text component
    private float touchStartTime = 0f;    // Time when the touch starts
    private bool isTouching = false;      // Flag to check if the touch is in progress
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
        {
            cancelButton.onClick.AddListener(ClosePopup);
        }

        // Access the Vignette effect from Global Volume
        if (globalVolume != null && globalVolume.profile.TryGet(out Vignette vignette))
        {
            vignetteEffect = vignette;
            vignetteEffect.intensity.value = 0; // Initial vignette intensity
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

            if (touchDuration <= quickTapThreshold)
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
}
