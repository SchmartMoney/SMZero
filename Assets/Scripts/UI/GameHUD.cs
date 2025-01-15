using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace SMZero
{
    public class GameHUD : MonoBehaviour
    {
        private static GameHUD instance;
        public static GameHUD Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameManagers.Instance.HUD;
                }
                return instance;
            }
        }

        [Header("UI References")]
        [SerializeField] private Image playerPortrait;
        [SerializeField] private TextMeshProUGUI playerIdText;
        [SerializeField] private Slider progressBar;
        [SerializeField] private TextMeshProUGUI locationText;
        [SerializeField] private Image fdIcon;
        [SerializeField] private TextMeshProUGUI balanceText;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            Debug.Log("GameHUD Awake - Starting initialization");

            // Create UI if not set
            if (playerPortrait == null)
            {
                CreateHUD();
            }

            // Wait a frame to ensure PlayerProgress is initialized
            StartCoroutine(InitializeBalance());
        }

        private System.Collections.IEnumerator InitializeBalance()
        {
            yield return null; // Wait one frame

            if (PlayerProgress.Instance != null)
            {
                Debug.Log($"Initializing balance from PlayerProgress: {PlayerProgress.Instance.GetFortuneDollars()}");
                PlayerProgress.Instance.OnBalanceChanged += UpdateBalance;
                UpdateBalance(PlayerProgress.Instance.GetFortuneDollars());
            }
            else
            {
                Debug.LogWarning("PlayerProgress.Instance is null during balance initialization!");
            }
        }

        private void CreateHUD()
        {
            // Create Canvas
            GameObject canvasObj = new GameObject("GameHUD");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1; // Lower than debug overlay
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvasObj.transform.SetParent(transform);

            // Create top bar panel
            GameObject topPanel = CreatePanel(canvasObj.transform, new Vector2(Screen.width, 80), Vector2.zero, new Vector2(0, 1));
            topPanel.name = "TopPanel";
            Image topPanelImage = topPanel.GetComponent<Image>();
            topPanelImage.color = new Color(0, 0, 0, 0.8f);

            // Create player portrait
            GameObject portraitObj = new GameObject("PlayerPortrait");
            portraitObj.transform.SetParent(topPanel.transform);
            playerPortrait = portraitObj.AddComponent<Image>();
            playerPortrait.color = Color.white;
            RectTransform portraitRect = portraitObj.GetComponent<RectTransform>();
            portraitRect.anchorMin = new Vector2(0, 0.5f);
            portraitRect.anchorMax = new Vector2(0, 0.5f);
            portraitRect.pivot = new Vector2(0, 0.5f);
            portraitRect.sizeDelta = new Vector2(60, 60);
            portraitRect.anchoredPosition = new Vector2(10, 0);

            // Create player ID text
            GameObject playerIdObj = new GameObject("PlayerID");
            playerIdObj.transform.SetParent(topPanel.transform);
            playerIdText = playerIdObj.AddComponent<TextMeshProUGUI>();
            playerIdText.text = "Player #1234";
            playerIdText.fontSize = 16;
            playerIdText.color = Color.white;
            RectTransform playerIdRect = playerIdObj.GetComponent<RectTransform>();
            playerIdRect.anchorMin = new Vector2(0, 0.5f);
            playerIdRect.anchorMax = new Vector2(0, 0.5f);
            playerIdRect.pivot = new Vector2(0, 0.5f);
            playerIdRect.sizeDelta = new Vector2(120, 30);
            playerIdRect.anchoredPosition = new Vector2(80, 15);

            // Create progress bar
            GameObject progressObj = new GameObject("ProgressBar");
            progressObj.transform.SetParent(topPanel.transform);
            progressBar = progressObj.AddComponent<Slider>();
            progressBar.minValue = 0;
            progressBar.maxValue = 100;
            progressBar.value = 50; // Fake progress for now
            RectTransform progressRect = progressObj.GetComponent<RectTransform>();
            progressRect.anchorMin = new Vector2(0, 0.5f);
            progressRect.anchorMax = new Vector2(0, 0.5f);
            progressRect.pivot = new Vector2(0, 0.5f);
            progressRect.sizeDelta = new Vector2(120, 10);
            progressRect.anchoredPosition = new Vector2(80, -10);

            // Create location text
            GameObject locationObj = new GameObject("Location");
            locationObj.transform.SetParent(topPanel.transform);
            locationText = locationObj.AddComponent<TextMeshProUGUI>();
            locationText.text = "Willow's Creek";
            locationText.fontSize = 20;
            locationText.color = Color.white;
            locationText.alignment = TextAlignmentOptions.Center;
            RectTransform locationRect = locationObj.GetComponent<RectTransform>();
            locationRect.anchorMin = new Vector2(0.5f, 0.5f);
            locationRect.anchorMax = new Vector2(0.5f, 0.5f);
            locationRect.pivot = new Vector2(0.5f, 0.5f);
            locationRect.sizeDelta = new Vector2(200, 30);
            locationRect.anchoredPosition = Vector2.zero;

            // Create balance container
            GameObject balanceContainer = new GameObject("BalanceContainer");
            balanceContainer.transform.SetParent(topPanel.transform);
            RectTransform containerRect = balanceContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(1, 0.5f);
            containerRect.anchorMax = new Vector2(1, 0.5f);
            containerRect.pivot = new Vector2(1, 0.5f);
            containerRect.sizeDelta = new Vector2(240, 30);
            containerRect.anchoredPosition = new Vector2(-20, -10);

            // Create FD icon
            GameObject fdIconObj = new GameObject("FDIcon");
            fdIconObj.transform.SetParent(balanceContainer.transform);
            fdIcon = fdIconObj.AddComponent<Image>();
            fdIcon.sprite = Resources.Load<Sprite>("UI/fd");
            fdIcon.preserveAspect = true;
            RectTransform iconRect = fdIconObj.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0, 0.5f);
            iconRect.anchorMax = new Vector2(0, 0.5f);
            iconRect.pivot = new Vector2(0, 0.5f);
            iconRect.sizeDelta = new Vector2(24, 24);
            iconRect.anchoredPosition = new Vector2(0, 0);

            // Create balance text (updated to be in container)
            GameObject balanceObj = new GameObject("Balance");
            balanceObj.transform.SetParent(balanceContainer.transform);
            balanceText = balanceObj.AddComponent<TextMeshProUGUI>();
            balanceText.text = "1,000,000.00 FD";
            balanceText.fontSize = 20;
            balanceText.color = Color.white;
            balanceText.alignment = TextAlignmentOptions.Right;
            RectTransform balanceRect = balanceObj.GetComponent<RectTransform>();
            balanceRect.anchorMin = new Vector2(1, 0.5f);
            balanceRect.anchorMax = new Vector2(1, 0.5f);
            balanceRect.pivot = new Vector2(1, 0.5f);
            balanceRect.sizeDelta = new Vector2(200, 30);
            balanceRect.anchoredPosition = new Vector2(0, 0);

            // Initialize values
            UpdateBalance(PlayerProgress.Instance?.GetFortuneDollars() ?? 0);
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
            rect.pivot = anchor;
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
            
            return panel;
        }

        public void UpdateBalance(float balance)
        {
            if (balanceText != null)
            {
                Debug.Log($"Updating HUD balance to: {balance}");
                balanceText.text = $"{balance:N2} FD";
            }
            else
            {
                Debug.LogWarning("balanceText is null during UpdateBalance!");
            }
        }

        public void SetLocation(string locationName)
        {
            if (locationText != null)
            {
                locationText.text = locationName;
            }
        }

        private void OnDestroy()
        {
            if (PlayerProgress.Instance != null)
            {
                PlayerProgress.Instance.OnBalanceChanged -= UpdateBalance;
            }
        }
    }
} 