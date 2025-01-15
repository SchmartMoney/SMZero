using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.Reflection;

namespace SMZero
{
    public class BuildingSlotPrefab
    {
        #if UNITY_EDITOR
        [MenuItem("GameObject/UI/SMZero/Building Slot")]
        public static void CreateBuildingSlotPrefab()
        {
            // Create main slot object
            GameObject slotObj = new GameObject("BuildingSlot");
            RectTransform slotRect = slotObj.AddComponent<RectTransform>();
            slotRect.sizeDelta = new Vector2(120, 180);
            
            // Add Canvas and required components
            Canvas canvas = slotObj.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 1;
            slotObj.AddComponent<GraphicRaycaster>(); // Add this to enable UI interaction
            
            // Add CanvasGroup for opacity control
            CanvasGroup canvasGroup = slotObj.AddComponent<CanvasGroup>();
            
            // Add BuildingSlotUI component
            BuildingSlotUI slotUI = slotObj.AddComponent<BuildingSlotUI>();
            
            // Create background
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(slotObj.transform);
            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // Create building icon
            GameObject iconObj = new GameObject("BuildingIcon");
            iconObj.transform.SetParent(slotObj.transform);
            Image buildingIcon = iconObj.AddComponent<Image>();
            buildingIcon.color = Color.white;
            RectTransform iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.1f, 0.2f);
            iconRect.anchorMax = new Vector2(0.9f, 0.8f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;
            
            // Create NFT text
            GameObject nftTextObj = new GameObject("NFTText");
            nftTextObj.transform.SetParent(slotObj.transform);
            TextMeshProUGUI nftText = nftTextObj.AddComponent<TextMeshProUGUI>();
            nftText.fontSize = 14;
            nftText.color = Color.white;
            nftText.alignment = TextAlignmentOptions.Center;
            RectTransform nftTextRect = nftTextObj.GetComponent<RectTransform>();
            nftTextRect.anchorMin = new Vector2(0, 0.8f);
            nftTextRect.anchorMax = new Vector2(1, 0.95f);
            nftTextRect.offsetMin = Vector2.zero;
            nftTextRect.offsetMax = Vector2.zero;
            
            // Create stake button
            GameObject stakeObj = new GameObject("StakeButton");
            stakeObj.transform.SetParent(slotObj.transform);
            Button stakeButton = stakeObj.AddComponent<Button>();
            Image stakeImage = stakeObj.AddComponent<Image>();
            stakeImage.color = new Color(0.2f, 0.7f, 0.2f, 0.8f);
            RectTransform stakeRect = stakeObj.GetComponent<RectTransform>();
            stakeRect.anchorMin = new Vector2(0.1f, 0.05f);
            stakeRect.anchorMax = new Vector2(0.9f, 0.15f);
            stakeRect.offsetMin = Vector2.zero;
            stakeRect.offsetMax = Vector2.zero;
            
            GameObject stakeTextObj = new GameObject("StakeText");
            stakeTextObj.transform.SetParent(stakeObj.transform);
            TextMeshProUGUI stakeText = stakeTextObj.AddComponent<TextMeshProUGUI>();
            stakeText.text = "Stake";
            stakeText.fontSize = 12;
            stakeText.color = Color.white;
            stakeText.alignment = TextAlignmentOptions.Center;
            RectTransform stakeTextRect = stakeTextObj.GetComponent<RectTransform>();
            stakeTextRect.anchorMin = Vector2.zero;
            stakeTextRect.anchorMax = Vector2.one;
            stakeTextRect.offsetMin = Vector2.zero;
            stakeTextRect.offsetMax = Vector2.zero;
            
            // Create unstake button (initially hidden)
            GameObject unstakeObj = new GameObject("UnstakeButton");
            unstakeObj.transform.SetParent(slotObj.transform);
            Button unstakeButton = unstakeObj.AddComponent<Button>();
            Image unstakeImage = unstakeObj.AddComponent<Image>();
            unstakeImage.color = new Color(0.7f, 0.2f, 0.2f, 0.8f);
            RectTransform unstakeRect = unstakeObj.GetComponent<RectTransform>();
            unstakeRect.anchorMin = new Vector2(0.1f, 0.05f);
            unstakeRect.anchorMax = new Vector2(0.9f, 0.15f);
            unstakeRect.offsetMin = Vector2.zero;
            unstakeRect.offsetMax = Vector2.zero;
            
            GameObject unstakeTextObj = new GameObject("UnstakeText");
            unstakeTextObj.transform.SetParent(unstakeObj.transform);
            TextMeshProUGUI unstakeText = unstakeTextObj.AddComponent<TextMeshProUGUI>();
            unstakeText.text = "Unstake";
            unstakeText.fontSize = 12;
            unstakeText.color = Color.white;
            unstakeText.alignment = TextAlignmentOptions.Center;
            RectTransform unstakeTextRect = unstakeTextObj.GetComponent<RectTransform>();
            unstakeTextRect.anchorMin = Vector2.zero;
            unstakeTextRect.anchorMax = Vector2.one;
            unstakeTextRect.offsetMin = Vector2.zero;
            unstakeTextRect.offsetMax = Vector2.zero;
            
            // Initialize UI references
            slotUI.Initialize(true);
            
            // Position the prefab in the hierarchy
            if (Selection.activeGameObject != null)
            {
                slotObj.transform.SetParent(Selection.activeGameObject.transform, false);
            }
            
            Debug.Log("Building slot prefab created successfully!");
        }
        #endif
    }
} 