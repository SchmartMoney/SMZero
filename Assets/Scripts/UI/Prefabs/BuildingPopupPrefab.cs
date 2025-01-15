using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace SMZero
{
    public class BuildingPopupPrefab
    {
        #if UNITY_EDITOR
        [MenuItem("GameObject/UI/SMZero/Building Popup")]
        public static void CreateBuildingPopup()
        {
            // Create main popup object
            GameObject popupObj = new GameObject("BuildingPopupPanel");
            BuildingPopup popup = popupObj.AddComponent<BuildingPopup>();
            CanvasGroup canvasGroup = popupObj.AddComponent<CanvasGroup>();
            
            // Add Canvas and Raycaster components
            Canvas canvas = popupObj.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 100; // Ensure it's above other UI
            popupObj.AddComponent<GraphicRaycaster>();
            
            // Add background panel
            GameObject bgPanel = new GameObject("ContentContainer");
            bgPanel.transform.SetParent(popupObj.transform, false);
            Image bgImage = bgPanel.AddComponent<Image>();
            bgImage.color = new Color(0, 0, 0, 0.9f);
            
            RectTransform bgRect = bgPanel.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // Add vertical layout for content
            VerticalLayoutGroup contentLayout = bgPanel.AddComponent<VerticalLayoutGroup>();
            contentLayout.padding = new RectOffset(20, 20, 20, 20);
            contentLayout.spacing = 10;
            contentLayout.childAlignment = TextAnchor.UpperCenter;
            contentLayout.childControlWidth = true;
            contentLayout.childControlHeight = false;
            
            // Title
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(bgPanel.transform, false);
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.fontSize = 24;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.white;
            LayoutElement titleLayout = titleObj.AddComponent<LayoutElement>();
            titleLayout.minHeight = 40;
            
            // Game Object container
            GameObject gameObjContainer = new GameObject("GameObject");
            gameObjContainer.transform.SetParent(bgPanel.transform, false);
            HorizontalLayoutGroup gameObjLayout = gameObjContainer.AddComponent<HorizontalLayoutGroup>();
            gameObjLayout.childAlignment = TextAnchor.MiddleCenter;
            gameObjLayout.spacing = 20;
            LayoutElement gameObjContainerLayout = gameObjContainer.AddComponent<LayoutElement>();
            gameObjContainerLayout.minHeight = 200;
            
            // Left side
            GameObject leftSide = new GameObject("Left");
            leftSide.transform.SetParent(gameObjContainer.transform, false);
            VerticalLayoutGroup leftLayout = leftSide.AddComponent<VerticalLayoutGroup>();
            leftLayout.spacing = 10;
            leftLayout.childAlignment = TextAnchor.MiddleCenter;
            
            // Timer text
            GameObject timerObj = new GameObject("TimerText");
            timerObj.transform.SetParent(leftSide.transform, false);
            TextMeshProUGUI timerText = timerObj.AddComponent<TextMeshProUGUI>();
            timerText.fontSize = 16;
            timerText.alignment = TextAlignmentOptions.Center;
            timerText.color = Color.white;
            
            // Progress bar
            GameObject progressObj = new GameObject("ProgressBar");
            progressObj.transform.SetParent(leftSide.transform, false);
            Image progressBg = progressObj.AddComponent<Image>();
            progressBg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            RectTransform progressRect = progressObj.GetComponent<RectTransform>();
            progressRect.sizeDelta = new Vector2(200, 20);
            
            // Building model/image
            GameObject buildingObj = new GameObject("BuildingModel");
            buildingObj.transform.SetParent(gameObjContainer.transform, false);
            Image buildingImage = buildingObj.AddComponent<Image>();
            RectTransform buildingRect = buildingObj.GetComponent<RectTransform>();
            buildingRect.sizeDelta = new Vector2(200, 200);
            
            // Right side (info)
            GameObject rightSide = new GameObject("Right");
            rightSide.transform.SetParent(gameObjContainer.transform, false);
            VerticalLayoutGroup rightLayout = rightSide.AddComponent<VerticalLayoutGroup>();
            rightLayout.spacing = 10;
            rightLayout.childAlignment = TextAnchor.MiddleLeft;
            
            // Info text
            GameObject infoObj = new GameObject("InfoText");
            infoObj.transform.SetParent(rightSide.transform, false);
            TextMeshProUGUI infoText = infoObj.AddComponent<TextMeshProUGUI>();
            infoText.fontSize = 16;
            infoText.alignment = TextAlignmentOptions.Left;
            infoText.color = Color.white;
            
            // NFT slots container
            GameObject slotsObj = new GameObject("NFTSlotsContainer");
            slotsObj.transform.SetParent(rightSide.transform, false);
            HorizontalLayoutGroup slotsLayout = slotsObj.AddComponent<HorizontalLayoutGroup>();
            slotsLayout.spacing = 10;
            slotsLayout.childAlignment = TextAnchor.MiddleCenter;
            LayoutElement slotsContainerLayout = slotsObj.AddComponent<LayoutElement>();
            slotsContainerLayout.minHeight = 120;
            
            // Buttons
            GameObject buttonsObj = new GameObject("Buttons");
            buttonsObj.transform.SetParent(bgPanel.transform, false);
            HorizontalLayoutGroup buttonsLayout = buttonsObj.AddComponent<HorizontalLayoutGroup>();
            buttonsLayout.spacing = 20;
            buttonsLayout.childAlignment = TextAnchor.LowerCenter;
            buttonsLayout.childControlWidth = false;
            LayoutElement buttonsContainerLayout = buttonsObj.AddComponent<LayoutElement>();
            buttonsContainerLayout.minHeight = 40;
            
            // Close button
            GameObject closeObj = new GameObject("CloseButton");
            closeObj.transform.SetParent(buttonsObj.transform, false);
            Button closeButton = closeObj.AddComponent<Button>();
            Image closeImage = closeObj.AddComponent<Image>();
            closeImage.color = new Color(0.8f, 0.2f, 0.2f, 0.8f);
            RectTransform closeRect = closeObj.GetComponent<RectTransform>();
            closeRect.sizeDelta = new Vector2(120, 40);
            
            GameObject closeTextObj = new GameObject("Text");
            closeTextObj.transform.SetParent(closeObj.transform, false);
            TextMeshProUGUI closeText = closeTextObj.AddComponent<TextMeshProUGUI>();
            closeText.text = "Close";
            closeText.fontSize = 16;
            closeText.alignment = TextAlignmentOptions.Center;
            closeText.color = Color.white;
            
            RectTransform closeTextRect = closeTextObj.GetComponent<RectTransform>();
            closeTextRect.anchorMin = Vector2.zero;
            closeTextRect.anchorMax = Vector2.one;
            closeTextRect.offsetMin = Vector2.zero;
            closeTextRect.offsetMax = Vector2.zero;
            
            // Collect button
            GameObject collectObj = new GameObject("CollectButton");
            collectObj.transform.SetParent(buttonsObj.transform, false);
            Button collectButton = collectObj.AddComponent<Button>();
            Image collectImage = collectObj.AddComponent<Image>();
            collectImage.color = new Color(0.2f, 0.8f, 0.2f, 0.8f);
            RectTransform collectRect = collectObj.GetComponent<RectTransform>();
            collectRect.sizeDelta = new Vector2(120, 40);
            
            GameObject collectTextObj = new GameObject("Text");
            collectTextObj.transform.SetParent(collectObj.transform, false);
            TextMeshProUGUI collectText = collectTextObj.AddComponent<TextMeshProUGUI>();
            collectText.text = "Collect";
            collectText.fontSize = 16;
            collectText.alignment = TextAlignmentOptions.Center;
            collectText.color = Color.white;
            
            RectTransform collectTextRect = collectTextObj.GetComponent<RectTransform>();
            collectTextRect.anchorMin = Vector2.zero;
            collectTextRect.anchorMax = Vector2.one;
            collectTextRect.offsetMin = Vector2.zero;
            collectTextRect.offsetMax = Vector2.zero;
            
            // Setup references using SerializedObject
            var serializedObject = new SerializedObject(popup);
            
            var backgroundProp = serializedObject.FindProperty("backgroundPanel");
            backgroundProp.objectReferenceValue = bgPanel;
            
            var nameProp = serializedObject.FindProperty("nameText");
            nameProp.objectReferenceValue = titleText;
            
            var descriptionProp = serializedObject.FindProperty("descriptionText");
            descriptionProp.objectReferenceValue = infoText;
            
            var imageProp = serializedObject.FindProperty("buildingImage");
            imageProp.objectReferenceValue = buildingImage;
            
            var timerProp = serializedObject.FindProperty("timerText");
            timerProp.objectReferenceValue = timerText;
            
            var slotsProp = serializedObject.FindProperty("characterSlotsContainer");
            slotsProp.objectReferenceValue = slotsObj;
            
            var closeProp = serializedObject.FindProperty("closeButton");
            closeProp.objectReferenceValue = closeButton;
            
            var collectProp = serializedObject.FindProperty("collectButton");
            collectProp.objectReferenceValue = collectButton;
            
            var canvasGroupProp = serializedObject.FindProperty("canvasGroup");
            canvasGroupProp.objectReferenceValue = canvasGroup;
            
            serializedObject.ApplyModifiedProperties();
            
            // Hide by default
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            
            // Position in hierarchy
            if (Selection.activeGameObject != null)
            {
                popupObj.transform.SetParent(Selection.activeGameObject.transform, false);
            }
            
            Selection.activeGameObject = popupObj;
            
            Debug.Log("Building popup prefab created");
        }
        #endif
    }
} 