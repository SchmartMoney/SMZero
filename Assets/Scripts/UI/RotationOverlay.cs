using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RotationOverlay : MonoBehaviour
{
    private GameObject rotatePromptPanel;
    
    private int lastWidth;
    private int lastHeight;
    private bool lastOrientationWasPortrait;
    private bool isInitialized;

    private void Awake()
    {
        // Try to find components if not set up externally
        if (!isInitialized)
        {
            var panel = transform.Find("RotatePromptPanel")?.gameObject;
            var container = transform.Find("RotatePromptPanel/RotateIconContainer")?.GetComponent<RectTransform>();
            var debug = transform.Find("DebugText")?.GetComponent<TextMeshProUGUI>();
            
            if (panel != null && container != null && debug != null)
            {
                Setup(panel, container, debug);
            }
        }
    }

    public void Setup(GameObject panel, RectTransform imageRect, TextMeshProUGUI debugTextComponent)
    {
        if (panel == null || imageRect == null || debugTextComponent == null)
        {
            Debug.LogError("RotationOverlay: Setup called with null components!");
            return;
        }

        rotatePromptPanel = panel;
        isInitialized = true;

        lastWidth = Screen.width;
        lastHeight = Screen.height;
        lastOrientationWasPortrait = Screen.height > Screen.width;
        
        CheckOrientation(true);
    }

    private void Update()
    {
        if (!isInitialized) return;

        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            CheckOrientation(false);
        }

    }

    private void CheckOrientation(bool forceUpdate)
    {
        if (!isInitialized || rotatePromptPanel == null) return;

        bool isPortrait = Screen.height > Screen.width;
        
        if (forceUpdate || isPortrait != lastOrientationWasPortrait)
        {
            lastOrientationWasPortrait = isPortrait;
            rotatePromptPanel.SetActive(isPortrait);
        }
    }

  

    private void OnEnable()
    {
        if (isInitialized)
        {
            CheckOrientation(true);
        }
    }
}