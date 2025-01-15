using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace SMZero
{
    public class CharacterSlotPrefab
    {
        #if UNITY_EDITOR
        [MenuItem("GameObject/UI/SMZero/Character Slot")]
        public static void CreateCharacterSlot()
        {
            // Create main slot object
            GameObject slotObj = new GameObject("CharacterSlot");
            CharacterSlotUI slotUI = slotObj.AddComponent<CharacterSlotUI>();
            
            // Setup RectTransform
            RectTransform slotRect = slotObj.GetComponent<RectTransform>();
            slotRect.sizeDelta = new Vector2(120, 180);
            
            // Empty state
            GameObject emptyState = new GameObject("EmptyState");
            emptyState.transform.SetParent(slotObj.transform);
            VerticalLayoutGroup emptyLayout = emptyState.AddComponent<VerticalLayoutGroup>();
            emptyLayout.padding = new RectOffset(10, 10, 10, 10);
            emptyLayout.spacing = 5;
            emptyLayout.childAlignment = TextAnchor.MiddleCenter;
            emptyLayout.childControlHeight = true;
            emptyLayout.childControlWidth = true;
            
            // Empty state background
            Image emptyBg = emptyState.AddComponent<Image>();
            emptyBg.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            
            // Empty icon
            GameObject emptyIconObj = new GameObject("EmptyIcon");
            emptyIconObj.transform.SetParent(emptyState.transform);
            Image emptyIcon = emptyIconObj.AddComponent<Image>();
            emptyIcon.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            LayoutElement emptyIconLayout = emptyIconObj.AddComponent<LayoutElement>();
            emptyIconLayout.preferredWidth = 60;
            emptyIconLayout.preferredHeight = 60;
            emptyIconLayout.flexibleWidth = 0;
            emptyIconLayout.flexibleHeight = 0;
            
            // Stake button
            GameObject stakeObj = new GameObject("StakeButton");
            stakeObj.transform.SetParent(emptyState.transform);
            Button stakeButton = stakeObj.AddComponent<Button>();
            Image stakeImage = stakeObj.AddComponent<Image>();
            stakeImage.color = new Color(0.2f, 0.8f, 0.2f, 0.8f);
            LayoutElement stakeLayout = stakeObj.AddComponent<LayoutElement>();
            stakeLayout.preferredHeight = 30;
            stakeLayout.flexibleWidth = 1;
            
            GameObject stakeTextObj = new GameObject("Text");
            stakeTextObj.transform.SetParent(stakeObj.transform);
            TextMeshProUGUI stakeText = stakeTextObj.AddComponent<TextMeshProUGUI>();
            stakeText.text = "Stake";
            stakeText.fontSize = 14;
            stakeText.color = Color.white;
            stakeText.alignment = TextAlignmentOptions.Center;
            
            RectTransform stakeTextRect = stakeTextObj.GetComponent<RectTransform>();
            stakeTextRect.anchorMin = Vector2.zero;
            stakeTextRect.anchorMax = Vector2.one;
            stakeTextRect.offsetMin = Vector2.zero;
            stakeTextRect.offsetMax = Vector2.zero;
            
            // Filled state
            GameObject filledState = new GameObject("FilledState");
            filledState.transform.SetParent(slotObj.transform);
            VerticalLayoutGroup filledLayout = filledState.AddComponent<VerticalLayoutGroup>();
            filledLayout.padding = new RectOffset(10, 10, 10, 10);
            filledLayout.spacing = 5;
            filledLayout.childAlignment = TextAnchor.MiddleCenter;
            filledLayout.childControlHeight = true;
            filledLayout.childControlWidth = true;
            
            // Filled state background
            Image filledBg = filledState.AddComponent<Image>();
            filledBg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            // Character icon
            GameObject iconObj = new GameObject("CharacterIcon");
            iconObj.transform.SetParent(filledState.transform);
            Image characterIcon = iconObj.AddComponent<Image>();
            LayoutElement iconLayout = iconObj.AddComponent<LayoutElement>();
            iconLayout.preferredWidth = 60;
            iconLayout.preferredHeight = 60;
            iconLayout.flexibleWidth = 0;
            iconLayout.flexibleHeight = 0;
            
            // Character info
            GameObject infoObj = new GameObject("Info");
            infoObj.transform.SetParent(filledState.transform);
            VerticalLayoutGroup infoLayout = infoObj.AddComponent<VerticalLayoutGroup>();
            infoLayout.spacing = 2;
            infoLayout.childAlignment = TextAnchor.MiddleCenter;
            infoLayout.childControlHeight = true;
            infoLayout.childControlWidth = true;
            
            // Character name
            GameObject nameObj = new GameObject("NameText");
            nameObj.transform.SetParent(infoObj.transform);
            TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
            nameText.fontSize = 14;
            nameText.color = Color.white;
            nameText.alignment = TextAlignmentOptions.Center;
            
            // Character rarity
            GameObject rarityObj = new GameObject("RarityText");
            rarityObj.transform.SetParent(infoObj.transform);
            TextMeshProUGUI rarityText = rarityObj.AddComponent<TextMeshProUGUI>();
            rarityText.fontSize = 12;
            rarityText.color = Color.gray;
            rarityText.alignment = TextAlignmentOptions.Center;
            
            // Unstake button
            GameObject unstakeObj = new GameObject("UnstakeButton");
            unstakeObj.transform.SetParent(filledState.transform);
            Button unstakeButton = unstakeObj.AddComponent<Button>();
            Image unstakeImage = unstakeObj.AddComponent<Image>();
            unstakeImage.color = new Color(0.8f, 0.2f, 0.2f, 0.8f);
            LayoutElement unstakeLayout = unstakeObj.AddComponent<LayoutElement>();
            unstakeLayout.preferredHeight = 30;
            unstakeLayout.flexibleWidth = 1;
            
            GameObject unstakeTextObj = new GameObject("Text");
            unstakeTextObj.transform.SetParent(unstakeObj.transform);
            TextMeshProUGUI unstakeText = unstakeTextObj.AddComponent<TextMeshProUGUI>();
            unstakeText.text = "Unstake";
            unstakeText.fontSize = 14;
            unstakeText.color = Color.white;
            unstakeText.alignment = TextAlignmentOptions.Center;
            
            RectTransform unstakeTextRect = unstakeTextObj.GetComponent<RectTransform>();
            unstakeTextRect.anchorMin = Vector2.zero;
            unstakeTextRect.anchorMax = Vector2.one;
            unstakeTextRect.offsetMin = Vector2.zero;
            unstakeTextRect.offsetMax = Vector2.zero;
            
            // Get the CharacterSlotUI component
            CharacterSlotUI slotComponent = slotObj.GetComponent<CharacterSlotUI>();
            if (slotComponent == null)
            {
                slotComponent = slotObj.AddComponent<CharacterSlotUI>();
            }

            // Assign references directly
            slotComponent.characterIcon = characterIcon;
            slotComponent.nameText = nameText;
            slotComponent.rarityText = rarityText;
            slotComponent.stakeButton = stakeButton;
            slotComponent.unstakeButton = unstakeButton;
            slotComponent.canvasGroup = slotObj.GetComponent<CanvasGroup>();
            
            // Position in hierarchy
            if (Selection.activeGameObject != null)
            {
                slotObj.transform.SetParent(Selection.activeGameObject.transform, false);
            }
            
            Selection.activeGameObject = slotObj;
            
            Debug.Log("Character slot prefab created");
        }
        #endif
    }
} 