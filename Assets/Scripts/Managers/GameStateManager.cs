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
            
            // Delay initialization to ensure other managers are ready
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

            currentState = new GameState
            {
                MarketplaceState = new MarketplaceState
                {
                    ActiveListings = MarketplaceManager.Instance.GetActiveListings(),
                    PlayerInventory = MarketplaceManager.Instance.GetPlayerInventory()
                }
            };
        }

        public void SaveGameState()
        {
            if (currentState == null || MarketplaceManager.Instance == null)
            {
                Debug.LogWarning("Cannot save game state: dependencies not initialized");
                return;
            }

            // Update marketplace state before saving
            currentState.MarketplaceState.ActiveListings = MarketplaceManager.Instance.GetActiveListings();
            currentState.MarketplaceState.PlayerInventory = MarketplaceManager.Instance.GetPlayerInventory();

            // TODO: Implement actual save to disk/cloud
            Debug.Log("Game state saved");
        }

        public GameState GetCurrentState()
        {
            return currentState;
        }

        public void UpdateMarketplaceState(MarketplaceState state)
        {
            if (currentState != null && MarketplaceManager.Instance != null)
            {
                currentState.MarketplaceState = state;
                MarketplaceManager.Instance.RestoreState(state);
            }
        }
    }

    [System.Serializable]
    public class GameState
    {
        public MarketplaceState MarketplaceState;
    }
} 