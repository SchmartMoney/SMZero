using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SMZero
{
    public class MarketplaceUISetup : MonoBehaviour
    {
        #if UNITY_EDITOR
        [MenuItem("GameObject/UI/Create Marketplace UI")]
        public static void CreateFromEditor()
        {
            CreateMarketplaceUI();
            Debug.Log("Marketplace UI created successfully!");
        }
        #endif

        public static GameObject CreateMarketplaceUI()
        {
            // Check if marketplace UI already exists
            if (GameObject.Find("MarketplaceCanvas"))
            {
                Debug.LogWarning("Marketplace UI already exists in the scene!");
                return null;
            }

            // Create Canvas
            GameObject canvasObj = new GameObject("MarketplaceCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();

            // Create main panel
            GameObject panelObj = new GameObject("MarketplacePanel");
            panelObj.transform.SetParent(canvasObj.transform, false);
            Image panelImage = panelObj.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.95f);
            RectTransform panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.1f, 0.1f);
            panelRect.anchorMax = new Vector2(0.9f, 0.9f);
            panelRect.sizeDelta = Vector2.zero;

            // Create title
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(panelObj.transform, false);
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "Employment Agency";
            titleText.fontSize = 48;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.white;
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.sizeDelta = new Vector2(0, 80);
            titleRect.anchoredPosition = new Vector2(0, -40);

            // Create close button
            GameObject closeBtn = new GameObject("CloseButton");
            closeBtn.transform.SetParent(panelObj.transform, false);
            Image closeBtnImage = closeBtn.AddComponent<Image>();
            closeBtnImage.color = new Color(0.2f, 0.2f, 0.2f, 1);
            Button closeButton = closeBtn.AddComponent<Button>();
            RectTransform closeBtnRect = closeBtn.GetComponent<RectTransform>();
            closeBtnRect.anchorMin = new Vector2(1, 1);
            closeBtnRect.anchorMax = new Vector2(1, 1);
            closeBtnRect.sizeDelta = new Vector2(60, 60);
            closeBtnRect.anchoredPosition = new Vector2(-30, -30);

            // Create close button text
            GameObject closeBtnTextObj = new GameObject("CloseButtonText");
            closeBtnTextObj.transform.SetParent(closeBtn.transform, false);
            TextMeshProUGUI closeBtnText = closeBtnTextObj.AddComponent<TextMeshProUGUI>();
            closeBtnText.text = "X";
            closeBtnText.fontSize = 36;
            closeBtnText.alignment = TextAlignmentOptions.Center;
            closeBtnText.color = Color.white;
            RectTransform closeBtnTextRect = closeBtnTextObj.GetComponent<RectTransform>();
            closeBtnTextRect.anchorMin = Vector2.zero;
            closeBtnTextRect.anchorMax = Vector2.one;
            closeBtnTextRect.sizeDelta = Vector2.zero;

            // Create scroll view for listings
            GameObject scrollView = new GameObject("ListingsScrollView");
            scrollView.transform.SetParent(panelObj.transform, false);
            ScrollRect scrollRect = scrollView.AddComponent<ScrollRect>();
            RectTransform scrollRectTransform = scrollView.GetComponent<RectTransform>();
            scrollRectTransform.anchorMin = new Vector2(0, 0);
            scrollRectTransform.anchorMax = new Vector2(1, 1);
            scrollRectTransform.sizeDelta = new Vector2(-40, -120);
            scrollRectTransform.anchoredPosition = new Vector2(0, -60);

            // Create viewport
            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollView.transform, false);
            Image viewportImage = viewport.AddComponent<Image>();
            viewportImage.color = new Color(0.1f, 0.1f, 0.1f, 0.5f);
            Mask viewportMask = viewport.AddComponent<Mask>();
            RectTransform viewportRect = viewport.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.sizeDelta = Vector2.zero;
            scrollRect.viewport = viewportRect;

            // Create content container with grid layout
            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);
            GridLayoutGroup grid = content.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(200, 300);
            grid.spacing = new Vector2(20, 20);
            grid.padding = new RectOffset(20, 20, 20, 20);
            grid.childAlignment = TextAnchor.UpperCenter;
            ContentSizeFitter sizeFitter = content.AddComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            RectTransform contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.sizeDelta = new Vector2(0, 0);
            scrollRect.content = contentRect;

            // Create listing item prefab
            GameObject listingPrefab = CreateListingItemPrefab();
            listingPrefab.transform.SetParent(content.transform, false);

            #if UNITY_EDITOR
            // Save the prefab
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs/UI"))
                AssetDatabase.CreateFolder("Assets/Prefabs", "UI");
            PrefabUtility.SaveAsPrefabAsset(listingPrefab, "Assets/Prefabs/UI/ListingItemPrefab.prefab");
            DestroyImmediate(listingPrefab);
            #endif

            return canvasObj;
        }

        private static GameObject CreateListingItemPrefab()
        {
            GameObject listingItem = new GameObject("ListingItemPrefab");
            Image itemBg = listingItem.AddComponent<Image>();
            itemBg.color = new Color(0.15f, 0.15f, 0.15f, 1);

            // Character/Zone image
            GameObject imageObj = new GameObject("ItemImage");
            imageObj.transform.SetParent(listingItem.transform, false);
            Image itemImage = imageObj.AddComponent<Image>();
            itemImage.color = Color.white;
            RectTransform imageRect = imageObj.GetComponent<RectTransform>();
            imageRect.anchorMin = new Vector2(0.1f, 0.4f);
            imageRect.anchorMax = new Vector2(0.9f, 0.9f);
            imageRect.sizeDelta = Vector2.zero;

            // Name text
            GameObject nameObj = new GameObject("NameText");
            nameObj.transform.SetParent(listingItem.transform, false);
            TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
            nameText.text = "Name";
            nameText.fontSize = 24;
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.color = Color.white;
            RectTransform nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.3f);
            nameRect.anchorMax = new Vector2(1, 0.4f);
            nameRect.sizeDelta = Vector2.zero;

            // Rarity text
            GameObject rarityObj = new GameObject("RarityText");
            rarityObj.transform.SetParent(listingItem.transform, false);
            TextMeshProUGUI rarityText = rarityObj.AddComponent<TextMeshProUGUI>();
            rarityText.text = "(common)";
            rarityText.fontSize = 18;
            rarityText.alignment = TextAlignmentOptions.Center;
            rarityText.color = new Color(0.7f, 0.7f, 0.7f, 1);
            RectTransform rarityRect = rarityObj.GetComponent<RectTransform>();
            rarityRect.anchorMin = new Vector2(0, 0.2f);
            rarityRect.anchorMax = new Vector2(1, 0.3f);
            rarityRect.sizeDelta = Vector2.zero;

            // Price text
            GameObject priceObj = new GameObject("PriceText");
            priceObj.transform.SetParent(listingItem.transform, false);
            TextMeshProUGUI priceText = priceObj.AddComponent<TextMeshProUGUI>();
            priceText.text = "400";
            priceText.fontSize = 24;
            priceText.alignment = TextAlignmentOptions.Center;
            priceText.color = Color.yellow;
            RectTransform priceRect = priceObj.GetComponent<RectTransform>();
            priceRect.anchorMin = new Vector2(0, 0.1f);
            priceRect.anchorMax = new Vector2(1, 0.2f);
            priceRect.sizeDelta = Vector2.zero;

            // Buy button
            GameObject buyBtn = new GameObject("BuyButton");
            buyBtn.transform.SetParent(listingItem.transform, false);
            Image buyBtnImage = buyBtn.AddComponent<Image>();
            buyBtnImage.color = new Color(0.2f, 0.6f, 1f, 1);
            Button buyButton = buyBtn.AddComponent<Button>();
            RectTransform buyBtnRect = buyBtn.GetComponent<RectTransform>();
            buyBtnRect.anchorMin = new Vector2(0.2f, 0);
            buyBtnRect.anchorMax = new Vector2(0.8f, 0.1f);
            buyBtnRect.sizeDelta = Vector2.zero;

            // Buy button text
            GameObject buyBtnTextObj = new GameObject("BuyButtonText");
            buyBtnTextObj.transform.SetParent(buyBtn.transform, false);
            TextMeshProUGUI buyBtnText = buyBtnTextObj.AddComponent<TextMeshProUGUI>();
            buyBtnText.text = "Buy now";
            buyBtnText.fontSize = 20;
            buyBtnText.alignment = TextAlignmentOptions.Center;
            buyBtnText.color = Color.white;
            RectTransform buyBtnTextRect = buyBtnTextObj.GetComponent<RectTransform>();
            buyBtnTextRect.anchorMin = Vector2.zero;
            buyBtnTextRect.anchorMax = Vector2.one;
            buyBtnTextRect.sizeDelta = Vector2.zero;

            return listingItem;
        }
    }
} 