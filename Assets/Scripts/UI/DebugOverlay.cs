using UnityEngine;
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

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        public void Log(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            // Only log critical game state changes and errors
            if (message.Contains("Error") || message.Contains("Failed") || 
                message.Contains("State") || message.Contains("Balance"))
            {
                Debug.Log($"[Game] {message}");
            }
        }
    }
} 