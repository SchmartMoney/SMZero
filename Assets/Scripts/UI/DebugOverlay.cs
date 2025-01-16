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
                    instance = GameManagers.Instance.Debug;
                }
                return instance;
            }
        }

        private Queue<string> logLines = new Queue<string>();
        private const int MaxLines = 50;
        private bool isVisible = false;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            isVisible = false;
        }

        public void Log(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            logLines.Enqueue($"[{DateTime.Now.ToString("HH:mm:ss")}] {message}");
            while (logLines.Count > MaxLines)
            {
                logLines.Dequeue();
            }

            // Only log to console now that we don't have UI
            Debug.Log($"[Debug Overlay] {message}");
        }

        public void Clear()
        {
            logLines.Clear();
        }

        public void Show()
        {
            isVisible = true;
        }

        public void Hide()
        {
            isVisible = false;
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
            }
            
            Log("\n=== End of State ===");
        }
    }
} 