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
        Debug.Log("Awake");
        // Try to find components if not set up externally
        if (!isInitialized)
        {
            var panel = transform.Find("RotatePromptPanel")?.gameObject;
            var container = transform.Find("RotatePromptPanel/RotateIconContainer")?.GetComponent<RectTransform>();
            var debug = transform.Find("DebugText")?.GetComponent<TextMeshProUGUI>();
            
            if (panel != null && container != null)
            {
                Setup(panel, container, debug);
            }
        }
    }

    public void Setup(GameObject panel, RectTransform imageRect, TextMeshProUGUI debugTextComponent)
    {
        if (panel == null || imageRect == null)
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
        
        Debug.Log("isPortrait: " + isPortrait);
        Debug.Log("screen size: " + Screen.width + "x" + Screen.height);    


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

    // Editor tips as tooltip
    [Header("Testing Tips")]
    [Tooltip("In Unity Editor:\n" +
             "1. Use Game window's aspect ratio dropdown\n" +
             "2. Or drag Game window edges\n" +
             "3. Portrait mode when height > width")]
    [SerializeField] private bool editorTips;
}