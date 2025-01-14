using UnityEngine;
using System;
using System.Collections.Generic;

namespace SMZero
{
    public class GameStateManager : MonoBehaviour
    {
        private static GameStateManager instance;
        public static GameStateManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameStateManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("GameStateManager");
                        instance = go.AddComponent<GameStateManager>();
                    }
                }
                return instance;
            }
        }

        private GameState currentState;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Initialize with default state
            currentState = new GameState();
            
            // Initialize marketplace state after a short delay
            Invoke(nameof(InitializeState), 0.1f);
        }

        private void InitializeState()
        {
            if (MarketplaceManager.Instance == null)
            {
                Debug.LogWarning("MarketplaceManager not found, retrying initialization...");
                Invoke(nameof(InitializeState), 0.1f);
                return;
            }

            if (currentState.MarketplaceState == null)
            {
                currentState.MarketplaceState = new MarketplaceState
                {
                    ActiveListings = MarketplaceManager.Instance.GetActiveListings(),
                    PlayerInventory = MarketplaceManager.Instance.GetPlayerInventory()
                };
            }

            // Apply the state to the marketplace
            MarketplaceManager.Instance.RestoreState(currentState.MarketplaceState);
        }

        public string ExportGameState()
        {
            if (currentState == null || MarketplaceManager.Instance == null)
            {
                Debug.LogWarning("Cannot export game state: dependencies not initialized");
                return "{}";
            }

            try
            {
                // Update marketplace state before exporting
                currentState.MarketplaceState.ActiveListings = MarketplaceManager.Instance.GetActiveListings();
                currentState.MarketplaceState.PlayerInventory = MarketplaceManager.Instance.GetPlayerInventory();
                currentState.PlayerBalance = PlayerProgress.Instance.GetFortuneDollars();

                // Convert to formatted JSON
                string json = JsonUtility.ToJson(currentState, true);
                Debug.Log($"Current Game State:\n{json}");
                return json;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error exporting game state: {e.Message}");
                return "{}";
            }
        }

        public GameState GetCurrentState()
        {
            if (currentState != null)
            {
                currentState.PlayerBalance = PlayerProgress.Instance.GetFortuneDollars();
            }
            return currentState;
        }

        public void UpdateMarketplaceState(MarketplaceState state)
        {
            if (currentState != null && MarketplaceManager.Instance != null)
            {
                currentState.MarketplaceState = state;
                MarketplaceManager.Instance.RestoreState(state);
                // Log the updated state
                Debug.Log($"Updated Game State:\n{ExportGameState()}");
            }
        }

        private void OnApplicationQuit()
        {
            // Export final state when quitting
            Debug.Log($"Final Game State:\n{ExportGameState()}");
        }
    }

    [System.Serializable]
    public class GameState
    {
        public MarketplaceState MarketplaceState;
        public float PlayerBalance;
    }
} 