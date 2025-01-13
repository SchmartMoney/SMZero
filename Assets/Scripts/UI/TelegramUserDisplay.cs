using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class TelegramUserDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI userIdText;
        [SerializeField] private TextMeshProUGUI usernameText;
        [SerializeField] private TextMeshProUGUI firstNameText;
        [SerializeField] private TextMeshProUGUI lastNameText;
        [SerializeField] private TextMeshProUGUI languageText;
        [SerializeField] private TextMeshProUGUI isPremiumText;

        void Start()
        {
            // Call this to initialize Telegram WebApp
            TelegramInit();
        }

        private void TelegramInit()
        {
    #if !UNITY_EDITOR && UNITY_WEBGL
            WebGLPlugins.InitTelegramWebApp();
            UpdateUserInfo();
    #endif
        }

        public void UpdateUserInfo()
        {
            if (userIdText) userIdText.text = $"User ID: {WebGLPlugins.GetTelegramUserId()}";
            if (usernameText) usernameText.text = $"Username: {WebGLPlugins.GetTelegramUsername()}";
            if (firstNameText) firstNameText.text = $"First Name: {WebGLPlugins.GetTelegramFirstName()}";
            if (lastNameText) lastNameText.text = $"Last Name: {WebGLPlugins.GetTelegramLastName()}";
            if (languageText) languageText.text = $"Language: {WebGLPlugins.GetTelegramLanguage()}";
            if (isPremiumText) isPremiumText.text = $"Premium: {(WebGLPlugins.IsTelegramPremium() ? "Yes" : "No")}";
        }
    }
} 