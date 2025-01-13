using UnityEngine;

namespace SMZero
{
    public class PlayerProgress : MonoBehaviour
    {
        private static PlayerProgress instance;
        public static PlayerProgress Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PlayerProgress>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("PlayerProgress");
                        instance = go.AddComponent<PlayerProgress>();
                    }
                }
                return instance;
            }
        }

        [SerializeField] private float fortuneDollars;
        private int totalAssetsProduced;
        private float totalIncome;
        private float dailyIncome;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void AddFortuneDollars(float amount)
        {
            fortuneDollars += amount;
            totalIncome += amount;
            dailyIncome += amount;
            Debug.Log($"Added {amount} fortune dollars. New balance: {fortuneDollars}");
        }

        public bool SpendFortuneDollars(float amount)
        {
            if (fortuneDollars >= amount)
            {
                fortuneDollars -= amount;
                Debug.Log($"Spent {amount} fortune dollars. New balance: {fortuneDollars}");
                return true;
            }
            Debug.LogWarning($"Insufficient funds. Required: {amount}, Available: {fortuneDollars}");
            return false;
        }

        public void ResetDailyStats()
        {
            dailyIncome = 0f;
            Debug.Log("Daily stats reset");
        }

        public void IncrementAssetsProduced()
        {
            totalAssetsProduced++;
            Debug.Log($"Total assets produced: {totalAssetsProduced}");
        }

        public float GetFortuneDollars() => fortuneDollars;
        public float GetDailyIncome() => dailyIncome;
        public float GetTotalIncome() => totalIncome;
        public int GetTotalAssetsProduced() => totalAssetsProduced;
    }
} 