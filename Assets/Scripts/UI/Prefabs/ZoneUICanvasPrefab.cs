using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace SMZero
{
    public class ZoneUICanvasPrefab
    {
        #if UNITY_EDITOR
        [MenuItem("GameObject/UI/SMZero/Zone UI Canvas")]
        public static void CreateZoneUICanvas()
        {
            // Create main canvas
            GameObject canvasObj = new GameObject("ZoneUICanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1;
            
            // Add required components
            canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObj.AddComponent<GraphicRaycaster>(); // Add this to enable UI interaction
            
            // Create main container
            GameObject containerObj = new GameObject("ContentContainer");
            containerObj.transform.SetParent(canvasObj.transform);
            RectTransform containerRect = containerObj.AddComponent<RectTransform>();
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.offsetMin = Vector2.zero;
            containerRect.offsetMax = Vector2.zero;
            
            // Create top panel
            GameObject topPanelObj = new GameObject("TopPanel");
            topPanelObj.transform.SetParent(containerObj.transform);
            Image topPanelImage = topPanelObj.AddComponent<Image>();
            topPanelImage.color = new Color(0, 0, 0, 0.8f);
            RectTransform topPanelRect = topPanelObj.GetComponent<RectTransform>();
            topPanelRect.anchorMin = new Vector2(0, 0.9f);
            topPanelRect.anchorMax = Vector2.one;
            topPanelRect.offsetMin = Vector2.zero;
            topPanelRect.offsetMax = Vector2.zero;
            
            // Add zone name text
            GameObject zoneNameObj = new GameObject("ZoneName");
            zoneNameObj.transform.SetParent(topPanelObj.transform);
            TextMeshProUGUI zoneNameText = zoneNameObj.AddComponent<TextMeshProUGUI>();
            zoneNameText.text = "Zone Name";
            zoneNameText.fontSize = 24;
            zoneNameText.color = Color.white;
            zoneNameText.alignment = TextAlignmentOptions.Center;
            RectTransform zoneNameRect = zoneNameObj.GetComponent<RectTransform>();
            zoneNameRect.anchorMin = Vector2.zero;
            zoneNameRect.anchorMax = Vector2.one;
            zoneNameRect.offsetMin = new Vector2(120, 0); // Make room for return button
            zoneNameRect.offsetMax = new Vector2(-120, 0);
            
            // Add return button
            GameObject returnButtonObj = new GameObject("ReturnButton");
            returnButtonObj.transform.SetParent(topPanelObj.transform);
            Button returnButton = returnButtonObj.AddComponent<Button>();
            Image returnButtonImage = returnButtonObj.AddComponent<Image>();
            returnButtonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            RectTransform returnButtonRect = returnButtonObj.GetComponent<RectTransform>();
            returnButtonRect.anchorMin = new Vector2(0, 0.5f);
            returnButtonRect.anchorMax = new Vector2(0, 0.5f);
            returnButtonRect.pivot = new Vector2(0, 0.5f);
            returnButtonRect.sizeDelta = new Vector2(100, 30);
            returnButtonRect.anchoredPosition = new Vector2(10, 0);
            
            GameObject returnTextObj = new GameObject("ReturnText");
            returnTextObj.transform.SetParent(returnButtonObj.transform);
            TextMeshProUGUI returnText = returnTextObj.AddComponent<TextMeshProUGUI>();
            returnText.text = "← Return";
            returnText.fontSize = 16;
            returnText.color = Color.white;
            returnText.alignment = TextAlignmentOptions.Center;
            RectTransform returnTextRect = returnTextObj.GetComponent<RectTransform>();
            returnTextRect.anchorMin = Vector2.zero;
            returnTextRect.anchorMax = Vector2.one;
            returnTextRect.offsetMin = Vector2.zero;
            returnTextRect.offsetMax = Vector2.zero;
            
            // Create building slots container
            GameObject slotsObj = new GameObject("BuildingSlotsContainer");
            slotsObj.transform.SetParent(containerObj.transform);
            GridLayoutGroup gridLayout = slotsObj.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(120, 180);
            gridLayout.spacing = new Vector2(10, 10);
            gridLayout.padding = new RectOffset(10, 10, 10, 10);
            gridLayout.childAlignment = TextAnchor.UpperCenter;
            ContentSizeFitter sizeFitter = slotsObj.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            RectTransform slotsRect = slotsObj.GetComponent<RectTransform>();
            slotsRect.anchorMin = new Vector2(0, 0);
            slotsRect.anchorMax = new Vector2(1, 0.9f);
            slotsRect.offsetMin = Vector2.zero;
            slotsRect.offsetMax = Vector2.zero;
            
            // Add ZoneUI component
            ZoneUI zoneUI = canvasObj.AddComponent<ZoneUI>();
            
            // Position in hierarchy
            if (Selection.activeGameObject != null)
            {
                canvasObj.transform.SetParent(Selection.activeGameObject.transform, false);
            }
            
            Debug.Log("Zone UI Canvas created successfully!");
        }
        #endif
    }
} 