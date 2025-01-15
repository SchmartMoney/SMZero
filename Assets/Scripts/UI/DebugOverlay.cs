using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

namespace SMZero
{
    public class DebugOverlay : MonoBehaviour
    {
        private static DebugOverlay instance;
        public static DebugOverlay Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<DebugOverlay>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("DebugOverlay");
                        instance = go.AddComponent<DebugOverlay>();
                    }
                }
                return instance;
            }
        }

        [SerializeField] private GameObject overlayPanel;
        [SerializeField] private TextMeshProUGUI logText;
        [SerializeField] private Button toggleButton;
        [SerializeField] private Button showStateButton;
        private Queue<string> logLines = new Queue<string>();
        private const int MaxLines = 50;
        private bool isVisible = true;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            // Create UI if not set
            if (overlayPanel == null)
            {
                CreateDebugUI();
            }
        }

        private void CreateDebugUI()
        {
            // Create Canvas
            GameObject canvasObj = new GameObject("DebugCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999; // Ensure it's on top
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvasObj.transform.SetParent(transform);

            // Create Panel
            overlayPanel = CreatePanel(canvasObj.transform, new Vector2(600, 400), new Vector2(20, 20), new Vector2(0, 0));
            overlayPanel.name = "DebugPanel";

            // Create Log Text
            GameObject textObj = new GameObject("LogText");
            textObj.transform.SetParent(overlayPanel.transform);
            logText = textObj.AddComponent<TextMeshProUGUI>();
            logText.fontSize = 14;
            logText.color = Color.white;
            logText.alignment = TextAlignmentOptions.TopLeft;
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 0);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.offsetMin = new Vector2(10, 10);
            textRect.offsetMax = new Vector2(-10, -40);

            // Create Toggle Button
            GameObject toggleObj = new GameObject("ToggleButton");
            toggleObj.transform.SetParent(canvasObj.transform);
            toggleButton = toggleObj.AddComponent<Button>();
            Image toggleImage = toggleObj.AddComponent<Image>();
            toggleImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            GameObject toggleTextObj = new GameObject("Text");
            toggleTextObj.transform.SetParent(toggleObj.transform);
            TextMeshProUGUI toggleText = toggleTextObj.AddComponent<TextMeshProUGUI>();
            toggleText.text = "Toggle Debug";
            toggleText.fontSize = 14;
            toggleText.color = Color.white;
            toggleText.alignment = TextAlignmentOptions.Center;
            
            RectTransform toggleRect = toggleObj.GetComponent<RectTransform>();
            toggleRect.anchorMin = new Vector2(1, 1);
            toggleRect.anchorMax = new Vector2(1, 1);
            toggleRect.pivot = new Vector2(1, 1);
            toggleRect.sizeDelta = new Vector2(100, 30);
            toggleRect.anchoredPosition = new Vector2(-10, -10);
            
            RectTransform toggleTextRect = toggleTextObj.GetComponent<RectTransform>();
            toggleTextRect.anchorMin = Vector2.zero;
            toggleTextRect.anchorMax = Vector2.one;
            toggleTextRect.offsetMin = Vector2.zero;
            toggleTextRect.offsetMax = Vector2.zero;

            // Create Show State Button
            GameObject showStateObj = new GameObject("ShowStateButton");
            showStateObj.transform.SetParent(canvasObj.transform);
            showStateButton = showStateObj.AddComponent<Button>();
            Image showStateImage = showStateObj.AddComponent<Image>();
            showStateImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            GameObject showStateTextObj = new GameObject("Text");
            showStateTextObj.transform.SetParent(showStateObj.transform);
            TextMeshProUGUI showStateText = showStateTextObj.AddComponent<TextMeshProUGUI>();
            showStateText.text = "Show State";
            showStateText.fontSize = 14;
            showStateText.color = Color.white;
            showStateText.alignment = TextAlignmentOptions.Center;
            
            RectTransform showStateRect = showStateObj.GetComponent<RectTransform>();
            showStateRect.anchorMin = new Vector2(1, 1);
            showStateRect.anchorMax = new Vector2(1, 1);
            showStateRect.pivot = new Vector2(1, 1);
            showStateRect.sizeDelta = new Vector2(100, 30);
            showStateRect.anchoredPosition = new Vector2(-120, -10);
            
            RectTransform showStateTextRect = showStateTextObj.GetComponent<RectTransform>();
            showStateTextRect.anchorMin = Vector2.zero;
            showStateTextRect.anchorMax = Vector2.one;
            showStateTextRect.offsetMin = Vector2.zero;
            showStateTextRect.offsetMax = Vector2.zero;

            // Create Reset State Button
            GameObject resetStateObj = new GameObject("ResetStateButton");
            resetStateObj.transform.SetParent(canvasObj.transform);
            Button resetStateButton = resetStateObj.AddComponent<Button>();
            Image resetStateImage = resetStateObj.AddComponent<Image>();
            resetStateImage.color = new Color(0.8f, 0.2f, 0.2f, 0.8f); // Red color to indicate danger
            
            GameObject resetStateTextObj = new GameObject("Text");
            resetStateTextObj.transform.SetParent(resetStateObj.transform);
            TextMeshProUGUI resetStateText = resetStateTextObj.AddComponent<TextMeshProUGUI>();
            resetStateText.text = "Reset State";
            resetStateText.fontSize = 14;
            resetStateText.color = Color.white;
            resetStateText.alignment = TextAlignmentOptions.Center;
            
            RectTransform resetStateRect = resetStateObj.GetComponent<RectTransform>();
            resetStateRect.anchorMin = new Vector2(1, 1);
            resetStateRect.anchorMax = new Vector2(1, 1);
            resetStateRect.pivot = new Vector2(1, 1);
            resetStateRect.sizeDelta = new Vector2(100, 30);
            resetStateRect.anchoredPosition = new Vector2(-230, -10); // Position it to the left of show state button
            
            RectTransform resetStateTextRect = resetStateTextObj.GetComponent<RectTransform>();
            resetStateTextRect.anchorMin = Vector2.zero;
            resetStateTextRect.anchorMax = Vector2.one;
            resetStateTextRect.offsetMin = Vector2.zero;
            resetStateTextRect.offsetMax = Vector2.zero;

            // Add button listeners
            toggleButton.onClick.AddListener(ToggleVisibility);
            showStateButton.onClick.AddListener(ShowCurrentState);
            resetStateButton.onClick.AddListener(ResetState);
        }

        private GameObject CreatePanel(Transform parent, Vector2 size, Vector2 position, Vector2 anchor)
        {
            GameObject panel = new GameObject("Panel");
            panel.transform.SetParent(parent);
            
            Image image = panel.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0.8f);
            
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = Vector2.zero;
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
            
            return panel;
        }

        public void Log(string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            logLines.Enqueue($"[{DateTime.Now.ToString("HH:mm:ss")}] {message}");
            while (logLines.Count > MaxLines)
            {
                logLines.Dequeue();
            }

            if (logText != null)
            {
                logText.text = string.Join("\n", logLines);
            }
        }

        public void Clear()
        {
            logLines.Clear();
            if (logText != null)
            {
                logText.text = string.Empty;
            }
        }

        public void Show()
        {
            isVisible = true;
            if (overlayPanel != null)
            {
                overlayPanel.SetActive(true);
            }
        }

        public void Hide()
        {
            isVisible = false;
            if (overlayPanel != null)
            {
                overlayPanel.SetActive(false);
            }
        }

        private void ToggleVisibility()
        {
            if (isVisible)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private void ShowCurrentState()
        {
            Clear(); // Clear current logs
            Log("=== Current Game State ===");
            
            var state = GameStateManager.Instance.GetCurrentState();
            if (state == null)
            {
                Log("No game state available!");
                return;
            }

            // Show Balance
            Log($"Fortune Dollars: {state.PlayerBalance}");
            
            // Show Inventory
            if (state.PlayerInventory != null)
            {
                Log("\nInventory:");
                Log($"- Owned Characters ({state.PlayerInventory.OwnedCharacterIds?.Count ?? 0}):");
                foreach (var id in state.PlayerInventory.OwnedCharacterIds ?? new List<string>())
                {
                    Log($"  • {id}");
                }
                
                Log($"- Owned Zones ({state.PlayerInventory.OwnedZoneIds?.Count ?? 0}):");
                foreach (var id in state.PlayerInventory.OwnedZoneIds ?? new List<string>())
                {
                    Log($"  • {id}");
                }
                
                if (state.PlayerInventory.ActiveListingIds?.Count > 0)
                {
                    Log($"- Active Listings ({state.PlayerInventory.ActiveListingIds.Count}):");
                    foreach (var id in state.PlayerInventory.ActiveListingIds)
                    {
                        Log($"  • {id}");
                    }
                }
            }
            
            // Show Zones State
            if (state.ZonesState?.ZonesList != null)
            {
                Log($"\nZones ({state.ZonesState.ZonesList.Count}):");
                foreach (var zone in state.ZonesState.ZonesList)
                {
                    Log($"- Zone {zone.Id}:");
                    Log($"  • Name: {zone.DisplayName}");
                    Log($"  • Active: {zone.IsActive}");
                    
                    if (zone.Buildings?.Count > 0)
                    {
                        Log($"  • Buildings ({zone.Buildings.Count}):");
                        foreach (var building in zone.Buildings)
                        {
                            Log($"    - Building {building.Id}:");
                            Log($"      • Active: {building.IsActive}");
                            Log($"      • Staked Characters: {building.StakedCharacterIds?.Count ?? 0}");
                            
                            if (building.Production != null)
                            {
                                var startTime = DateTimeOffset.FromUnixTimeSeconds(building.Production.StartTimestamp);
                                var endTime = DateTimeOffset.FromUnixTimeSeconds(building.Production.EndTimestamp);
                                Log($"      • Production:");
                                Log($"        - Start: {startTime:MM/dd HH:mm:ss}");
                                Log($"        - End: {endTime:MM/dd HH:mm:ss}");
                                Log($"        - Value: {building.Production.AssetValue}");
                            }
                        }
                    }
                }
            }
            
            Log("\n=== End of State ===");
        }

        private void ResetState()
        {
            if (GameStateManager.Instance != null)
            {
                // Create confirmation dialog
                Log("=== Resetting Game State ===");
                
                // Delete local storage
                PlayerPrefs.DeleteKey(GameStateManager.LOCAL_SAVE_KEY);
                PlayerPrefs.Save();
                
                // Reset player progress
                if (PlayerProgress.Instance != null)
                {
                    PlayerProgress.Instance.SetFortuneDollars(1000f);
                }
                
                // Create new game state
                GameStateManager.Instance.ResetState();
                
                Log("State has been reset to defaults");
                Log("Please restart the game to apply changes");
            }
        }
    }
} 