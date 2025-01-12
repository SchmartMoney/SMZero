using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RotationOverlaySetup : MonoBehaviour
{
    #if UNITY_EDITOR
    [MenuItem("GameObject/UI/Create Rotation Overlay")]
    public static void CreateFromEditor()
    {
        CreateRotationOverlay();
        Debug.Log("Rotation Overlay created successfully!");
    }
    #endif

    public static GameObject CreateRotationOverlay()
    {
        // Check if overlay already exists
        if (GameObject.Find("RotationCanvas"))
        {
            Debug.LogWarning("Rotation Overlay already exists in the scene!");
            return null;
        }

        // Create Canvas
        GameObject canvasObj = new GameObject("RotationCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        // Create prompt panel
        GameObject panelObj = new GameObject("RotatePromptPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.9f);
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;

        // Create debug text (always visible, on top of everything)
        GameObject debugTextObj = new GameObject("DebugText");
        debugTextObj.transform.SetParent(canvasObj.transform, false);
        TextMeshProUGUI debugText = debugTextObj.AddComponent<TextMeshProUGUI>();
        debugText.text = "Initializing...";
        debugText.fontSize = 24;
        debugText.color = Color.yellow;
        debugText.alignment = TextAlignmentOptions.Top;
        RectTransform debugTextRect = debugTextObj.GetComponent<RectTransform>();
        debugTextRect.anchorMin = new Vector2(0, 1);
        debugTextRect.anchorMax = new Vector2(1, 1);
        debugTextRect.pivot = new Vector2(0.5f, 1);
        debugTextRect.sizeDelta = new Vector2(0, 100);
        debugTextRect.anchoredPosition = Vector2.zero;

        // Create rotate icon container
        GameObject iconContainer = new GameObject("RotateIconContainer");
        iconContainer.transform.SetParent(panelObj.transform, false);
        RectTransform containerRect = iconContainer.AddComponent<RectTransform>();
        containerRect.sizeDelta = new Vector2(200, 200);
        containerRect.anchoredPosition = new Vector2(0, 50);

        // Create rotate icon
        GameObject iconObj = new GameObject("RotateIcon");
        iconObj.transform.SetParent(iconContainer.transform, false);
        Image iconImage = iconObj.AddComponent<Image>();
        iconImage.sprite = CreateDefaultPhoneSprite();
        iconImage.preserveAspect = true;
        RectTransform iconRect = iconObj.GetComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(150, 150);
        iconRect.anchoredPosition = Vector2.zero;

        // Create instruction text
        GameObject textObj = new GameObject("InstructionText");
        textObj.transform.SetParent(panelObj.transform, false);
        TMP_Text text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "Please rotate your device";
        text.fontSize = 36;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(600, 100);
        textRect.anchoredPosition = new Vector2(0, -100);

        // Add rotation overlay component
        RotationOverlay overlay = canvasObj.AddComponent<RotationOverlay>();
        overlay.Setup(panelObj, containerRect, debugText);

        return canvasObj;
    }

    private static Sprite CreateDefaultPhoneSprite()
    {
        // Create a simple phone-shaped sprite
        Texture2D tex = new Texture2D(64, 64);
        Color[] colors = new Color[64 * 64];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.clear;
        }

        // Draw phone outline
        for (int x = 10; x < 54; x++)
        {
            for (int y = 5; y < 59; y++)
            {
                if (x == 10 || x == 53 || y == 5 || y == 58)
                {
                    colors[y * 64 + x] = Color.white;
                }
            }
        }

        tex.SetPixels(colors);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
    }
}