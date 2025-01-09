using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class UIGenerator : EditorWindow
{
    [MenuItem("Tools/Generate Zone UI")]
    public static void GenerateZoneUI()
    {
        CreateZoneUI();
    }

    [MenuItem("Tools/Generate Building Popup")]
    public static void GenerateBuildingPopup()
    {
        CreateBuildingPopup();
    }

    static void CreateZoneUI()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("ZoneUICanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(360, 640);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObj.AddComponent<GraphicRaycaster>();

        // Create Top Panel
        GameObject topPanel = new GameObject("TopPanel");
        topPanel.transform.SetParent(canvasObj.transform, false);
        RectTransform topPanelRect = topPanel.AddComponent<RectTransform>();
        topPanelRect.anchorMin = new Vector2(0, 1);
        topPanelRect.anchorMax = new Vector2(1, 1);
        topPanelRect.pivot = new Vector2(0.5f, 1);
        topPanelRect.sizeDelta = new Vector2(0, 70);
        
        // Add ScrollRect for slots
        GameObject scrollView = new GameObject("SlotsScrollView");
        scrollView.transform.SetParent(topPanel.transform, false);
        ScrollRect scrollRect = scrollView.AddComponent<ScrollRect>();
        RectTransform scrollRectTransform = scrollView.GetComponent<RectTransform>();
        scrollRectTransform.anchorMin = new Vector2(0, 0);
        scrollRectTransform.anchorMax = new Vector2(1, 1);
        scrollRectTransform.sizeDelta = Vector2.zero;
        
        // Create Content for ScrollRect
        GameObject content = new GameObject("Content");
        content.transform.SetParent(scrollView.transform, false);
        RectTransform contentRect = content.AddComponent<RectTransform>();
        HorizontalLayoutGroup layoutGroup = content.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.spacing = 10;
        layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        ContentSizeFitter contentFitter = content.AddComponent<ContentSizeFitter>();
        contentFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        scrollRect.content = contentRect;
        scrollRect.horizontal = true;
        scrollRect.vertical = false;

        // Create 10 Zone Slots
        for (int i = 0; i < 10; i++)
        {
            CreateZoneSlot(content.transform, i);
        }

        // Create Return Button
        GameObject returnBtn = CreateButton("ReturnButton", canvasObj.transform, "Return to Map");
        RectTransform returnBtnRect = returnBtn.GetComponent<RectTransform>();
        returnBtnRect.anchorMin = new Vector2(0, 1);
        returnBtnRect.anchorMax = new Vector2(0, 1);
        returnBtnRect.pivot = new Vector2(0, 1);
        returnBtnRect.anchoredPosition = new Vector2(10, -80);
        returnBtnRect.sizeDelta = new Vector2(120, 40);

        // Create Zone Name Text
        GameObject zoneName = new GameObject("ZoneName");
        zoneName.transform.SetParent(canvasObj.transform, false);
        TextMeshProUGUI zoneText = zoneName.AddComponent<TextMeshProUGUI>();
        zoneText.text = "Vault Avenue";
        zoneText.fontSize = 24;
        zoneText.alignment = TextAlignmentOptions.Center;
        RectTransform zoneNameRect = zoneName.GetComponent<RectTransform>();
        zoneNameRect.anchorMin = new Vector2(0.5f, 1);
        zoneNameRect.anchorMax = new Vector2(0.5f, 1);
        zoneNameRect.pivot = new Vector2(0.5f, 1);
        zoneNameRect.anchoredPosition = new Vector2(0, -80);
        zoneNameRect.sizeDelta = new Vector2(200, 40);

        Debug.Log("Zone UI Created!");
    }

    static void CreateBuildingPopup()
    {
        GameObject canvasObj = GameObject.Find("ZoneUICanvas");
        if (canvasObj == null)
        {
            Debug.LogError("Please create Zone UI first!");
            return;
        }

        // Main popup panel with safe area margins
        GameObject popupObj = new GameObject("BuildingPopupPanel");
        popupObj.transform.SetParent(canvasObj.transform, false);
        Image panelImage = popupObj.AddComponent<Image>();
        panelImage.color = new Color(1f, 0.843f, 0.808f, 1f); // Peach/salmon color
        RectTransform popupRect = popupObj.GetComponent<RectTransform>();
        popupRect.anchorMin = new Vector2(0.1f, 0.1f);
        popupRect.anchorMax = new Vector2(0.9f, 0.9f);
        popupRect.sizeDelta = Vector2.zero;
        
        // Content container with vertical layout
        GameObject contentContainer = new GameObject("ContentContainer");
        contentContainer.transform.SetParent(popupObj.transform, false);
        VerticalLayoutGroup verticalLayout = contentContainer.AddComponent<VerticalLayoutGroup>();
        verticalLayout.padding = new RectOffset(20, 20, 20, 20);
        verticalLayout.spacing = 10;
        verticalLayout.childAlignment = TextAnchor.UpperCenter;
        verticalLayout.childControlHeight = true;
        verticalLayout.childControlWidth = true;
        verticalLayout.childForceExpandHeight = false;
        verticalLayout.childForceExpandWidth = true;
        RectTransform contentRect = contentContainer.GetComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.sizeDelta = Vector2.zero;

        // Close button (outside layout group)
        GameObject closeBtn = CreateButton("CloseButton", popupObj.transform, "X");
        RectTransform closeBtnRect = closeBtn.GetComponent<RectTransform>();
        closeBtnRect.anchorMin = new Vector2(1, 1);
        closeBtnRect.anchorMax = new Vector2(1, 1);
        closeBtnRect.pivot = new Vector2(1, 1);
        closeBtnRect.sizeDelta = new Vector2(40, 40);
        closeBtnRect.anchoredPosition = new Vector2(-10, -10);
        
        // Title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(contentContainer.transform, false);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "Vault";
        titleText.fontSize = 32;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.black;
        LayoutElement titleLayout = titleObj.AddComponent<LayoutElement>();
        titleLayout.minHeight = 50;
        titleLayout.flexibleWidth = 1;

        // Timer text
        GameObject timerObj = new GameObject("TimerText");
        timerObj.transform.SetParent(contentContainer.transform, false);
        TextMeshProUGUI timerText = timerObj.AddComponent<TextMeshProUGUI>();
        timerText.text = "Timer - swapping to collect\nbutton once done";
        timerText.fontSize = 18;
        timerText.alignment = TextAlignmentOptions.Center;
        timerText.color = Color.black;
        LayoutElement timerLayout = timerObj.AddComponent<LayoutElement>();
        timerLayout.minHeight = 50;
        timerLayout.flexibleWidth = 1;

        // Main content area (Building + Info)
        GameObject mainContentArea = new GameObject("MainContentArea");
        mainContentArea.transform.SetParent(contentContainer.transform, false);
        HorizontalLayoutGroup mainContentLayout = mainContentArea.AddComponent<HorizontalLayoutGroup>();
        mainContentLayout.spacing = 20;
        mainContentLayout.childAlignment = TextAnchor.UpperCenter;
        mainContentLayout.childControlHeight = true;
        mainContentLayout.childControlWidth = true;
        LayoutElement mainContentElement = mainContentArea.AddComponent<LayoutElement>();
        mainContentElement.flexibleHeight = 1;
        mainContentElement.flexibleWidth = 1;

        // Building 3D image area
        GameObject buildingImageArea = new GameObject("BuildingImageArea");
        buildingImageArea.transform.SetParent(mainContentArea.transform, false);
        Image buildingImage = buildingImageArea.AddComponent<Image>();
        buildingImage.color = new Color(0.9f, 0.9f, 0.9f, 0.5f);
        LayoutElement buildingLayout = buildingImageArea.AddComponent<LayoutElement>();
        buildingLayout.flexibleHeight = 1;
        buildingLayout.flexibleWidth = 0.5f;

        // Information text
        GameObject infoObj = new GameObject("InfoText");
        infoObj.transform.SetParent(mainContentArea.transform, false);
        TextMeshProUGUI infoText = infoObj.AddComponent<TextMeshProUGUI>();
        infoText.text = "Information about\nproduction\n\nlike boosts from\nNFTs, upgrade\nstatus, etc.\n\nno details for now";
        infoText.fontSize = 16;
        infoText.alignment = TextAlignmentOptions.Left;
        infoText.color = Color.black;
        LayoutElement infoLayout = infoObj.AddComponent<LayoutElement>();
        infoLayout.flexibleHeight = 1;
        infoLayout.flexibleWidth = 0.5f;

        // NFT slots container
        GameObject slotsContainer = new GameObject("NFTSlotsContainer");
        slotsContainer.transform.SetParent(contentContainer.transform, false);
        HorizontalLayoutGroup slotsLayout = slotsContainer.AddComponent<HorizontalLayoutGroup>();
        slotsLayout.spacing = 10;
        slotsLayout.childAlignment = TextAnchor.LowerCenter;
        slotsLayout.childControlHeight = true;
        slotsLayout.childControlWidth = true;
        slotsLayout.childForceExpandWidth = false;
        LayoutElement slotsContainerLayout = slotsContainer.AddComponent<LayoutElement>();
        slotsContainerLayout.minHeight = 100;
        slotsContainerLayout.flexibleWidth = 1;

        // Create 3 NFT slots
        for (int i = 0; i < 3; i++)
        {
            GameObject slot = new GameObject($"NFTSlot_{i}");
            slot.transform.SetParent(slotsContainer.transform, false);
            Image slotImage = slot.AddComponent<Image>();
            slotImage.color = new Color(0.8f, 0.8f, 0.8f, 0.5f);
            AspectRatioFitter aspectFitter = slot.AddComponent<AspectRatioFitter>();
            aspectFitter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
            aspectFitter.aspectRatio = 1;
            LayoutElement slotLayout = slot.AddComponent<LayoutElement>();
            slotLayout.flexibleHeight = 1;
            slotLayout.flexibleWidth = 1;
        }

        Debug.Log("Building Popup Created!");
    }

    static GameObject CreateButton(string name, Transform parent, string text)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        
        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.2f, 0.2f, 0.2f, 1);
        
        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImage;
        
        GameObject txtObj = new GameObject("Text");
        txtObj.transform.SetParent(btnObj.transform, false);
        
        TextMeshProUGUI tmp = txtObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.color = Color.white;
        tmp.fontSize = 16;
        tmp.alignment = TextAlignmentOptions.Center;
        
        RectTransform txtRect = txtObj.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.sizeDelta = Vector2.zero;

        return btnObj;
    }

    static GameObject CreateZoneSlot(Transform parent, int index)
    {
        GameObject slot = new GameObject($"ZoneSlot_{index}");
        slot.transform.SetParent(parent, false);
        
        Image background = slot.AddComponent<Image>();
        background.color = index == 0 ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
        RectTransform slotRect = slot.GetComponent<RectTransform>();
        slotRect.sizeDelta = new Vector2(50, 50);

        GameObject iconObj = new GameObject("NFTIcon");
        iconObj.transform.SetParent(slot.transform, false);
        Image iconImage = iconObj.AddComponent<Image>();
        iconImage.color = new Color(1, 1, 1, 0.5f);
        RectTransform iconRect = iconObj.GetComponent<RectTransform>();
        iconRect.anchorMin = Vector2.zero;
        iconRect.anchorMax = Vector2.one;
        iconRect.sizeDelta = Vector2.zero;
        iconObj.SetActive(false);

        GameObject stakeBtn = CreateButton("StakeButton", slot.transform, "Stake");
        RectTransform stakeBtnRect = stakeBtn.GetComponent<RectTransform>();
        stakeBtnRect.anchorMin = Vector2.zero;
        stakeBtnRect.anchorMax = Vector2.one;
        stakeBtnRect.sizeDelta = Vector2.zero;
        stakeBtn.SetActive(index == 0);

        GameObject unstakeBtn = CreateButton("UnstakeButton", slot.transform, "Unstake");
        RectTransform unstakeBtnRect = unstakeBtn.GetComponent<RectTransform>();
        unstakeBtnRect.anchorMin = Vector2.zero;
        unstakeBtnRect.anchorMax = Vector2.one;
        unstakeBtnRect.sizeDelta = Vector2.zero;
        unstakeBtn.SetActive(false);

        return slot;
    }

    static GameObject CreateCharacterSlot(Transform parent)
    {
        GameObject slot = new GameObject("CharacterSlot");
        slot.transform.SetParent(parent, false);
        
        Image background = slot.AddComponent<Image>();
        background.color = new Color(0.2f, 0.2f, 0.2f, 1);
        RectTransform slotRect = slot.GetComponent<RectTransform>();
        slotRect.sizeDelta = new Vector2(0, 80);

        GameObject iconObj = new GameObject("CharacterIcon");
        iconObj.transform.SetParent(slot.transform, false);
        Image iconImage = iconObj.AddComponent<Image>();
        iconImage.color = new Color(0.5f, 0.5f, 0.5f, 1);
        RectTransform iconRect = iconObj.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.5f);
        iconRect.anchorMax = new Vector2(0, 0.5f);
        iconRect.pivot = new Vector2(0, 0.5f);
        iconRect.sizeDelta = new Vector2(60, 60);
        iconRect.anchoredPosition = new Vector2(10, 0);

        GameObject stakeBtn = CreateButton("StakeButton", slot.transform, "Stake");
        RectTransform stakeBtnRect = stakeBtn.GetComponent<RectTransform>();
        stakeBtnRect.anchorMin = new Vector2(1, 0.5f);
        stakeBtnRect.anchorMax = new Vector2(1, 0.5f);
        stakeBtnRect.pivot = new Vector2(1, 0.5f);
        stakeBtnRect.sizeDelta = new Vector2(80, 40);
        stakeBtnRect.anchoredPosition = new Vector2(-10, 0);

        GameObject unstakeBtn = CreateButton("UnstakeButton", slot.transform, "Unstake");
        RectTransform unstakeBtnRect = unstakeBtn.GetComponent<RectTransform>();
        unstakeBtnRect.anchorMin = new Vector2(1, 0.5f);
        unstakeBtnRect.anchorMax = new Vector2(1, 0.5f);
        unstakeBtnRect.pivot = new Vector2(1, 0.5f);
        unstakeBtnRect.sizeDelta = new Vector2(80, 40);
        unstakeBtnRect.anchoredPosition = new Vector2(-10, 0);
        unstakeBtn.SetActive(false);

        return slot;
    }
} 