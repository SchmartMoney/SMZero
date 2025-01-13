using UnityEngine;
using System.Runtime.InteropServices;

public static class WebGLPlugins
{
    [DllImport("__Internal")]
    public static extern bool InitTelegramWebApp();

    [DllImport("__Internal")]
    public static extern string GetTelegramUserId();

    [DllImport("__Internal")]
    public static extern string GetTelegramUsername();

    [DllImport("__Internal")]
    public static extern string GetTelegramFirstName();

    [DllImport("__Internal")]
    public static extern string GetTelegramLastName();

    [DllImport("__Internal")]
    public static extern string GetTelegramLanguage();

    [DllImport("__Internal")]
    public static extern bool IsTelegramPremium();

    [DllImport("__Internal")]
    public static extern bool IsTelegramWebApp();
} 