using UnityEngine;
using System;

namespace SMZero
{
    [Serializable]
    public class PlayerProgressData
    {
        public float fortuneDollars;
        public int totalAssetsProduced;
        public float totalIncome;
        public float dailyIncome;

        public PlayerProgressData(PlayerProgress progress)
        {
            fortuneDollars = progress.GetFortuneDollars();
            totalAssetsProduced = progress.GetTotalAssetsProduced();
            totalIncome = progress.GetTotalIncome();
            dailyIncome = progress.GetDailyIncome();
        }
    }

    public class PlayerProgress : MonoBehaviour
    {
        private static PlayerProgress instance;
        private const string SAVE_KEY = "PlayerProgress";
        
        public static PlayerProgress Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameManagers.Instance.Progress;
                }
                return instance;
            }
        }

        [SerializeField] private float fortuneDollars = 0f;
        private int totalAssetsProduced;
        private float totalIncome;
        private float dailyIncome;
        public System.Action<float> OnBalanceChanged;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            LoadProgress();
        }

        private void OnApplicationQuit()
        {
            SaveProgress();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveProgress();
            }
        }

        private void SaveProgress()
        {
            var data = new PlayerProgressData(this);
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
        }

        private void LoadProgress()
        {
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                var data = JsonUtility.FromJson<PlayerProgressData>(json);
                
                SetFortuneDollars(data.fortuneDollars);
                totalAssetsProduced = data.totalAssetsProduced;
                totalIncome = data.totalIncome;
                dailyIncome = data.dailyIncome;
                
            }
        }

        public float GetFortuneDollars()
        {
            return fortuneDollars;
        }

        public void AddFortuneDollars(float amount)
        {
            fortuneDollars += amount;
            totalIncome += amount;
            dailyIncome += amount;
            OnBalanceChanged?.Invoke(fortuneDollars);
            SaveProgress();
        }

        public bool SpendFortuneDollars(float amount)
        {
            if (fortuneDollars >= amount)
            {
                fortuneDollars -= amount;
                OnBalanceChanged?.Invoke(fortuneDollars);
                SaveProgress();
                return true;
            }
            return false;
        }

        public void SetFortuneDollars(float amount)
        {
            fortuneDollars = amount;
            OnBalanceChanged?.Invoke(fortuneDollars);
            SaveProgress();
        }

        public void ResetDailyStats()
        {
            dailyIncome = 0f;
            Debug.Log("Daily stats reset");
            SaveProgress();
        }

        public void IncrementAssetsProduced()
        {
            totalAssetsProduced++;
            SaveProgress();
        }

        public float GetDailyIncome() => dailyIncome;
        public float GetTotalIncome() => totalIncome;
        public int GetTotalAssetsProduced() => totalAssetsProduced;
    }
} 