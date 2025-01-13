using UnityEngine;
using System.Collections.Generic;

public enum AchievementType 
{ 
    Easy, 
    Medium, 
    Hard 
}

[System.Serializable]
public class Achievement
{
    public string id;
    public string name;
    public string description;
    public float reward;             // FD reward
    public AchievementType type;
    public bool isCompleted;
    
    [System.NonSerialized]
    public Dictionary<string, float> progress = new Dictionary<string, float>();

    public Achievement(string id, string name, string description, float reward, AchievementType type)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.reward = reward;
        this.type = type;
        this.isCompleted = false;
    }
}

[System.Serializable]
public class PlayerProgress
{
    // Currency
    public float fortuneDollars = 1000f;  // Starting money
    
    // Production stats
    public int totalAssetsProduced;
    public float totalEarnings;
    public float characterIncomeCollected;
    
    // Daily tracking
    public int assetsProducedToday;
    [System.NonSerialized]
    public System.DateTime lastDailyReset;
    
    // Achievements
    [System.NonSerialized]
    public Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>();

    public void ResetDaily()
    {
        if (System.DateTime.Now.Date > lastDailyReset.Date)
        {
            assetsProducedToday = 0;
            lastDailyReset = System.DateTime.Now;
        }
    }

    public void Initialize()
    {
        lastDailyReset = System.DateTime.Now;
        achievements = new Dictionary<string, Achievement>();
        InitializeAchievements();
    }

    private void InitializeAchievements()
    {
        // Easy Achievements
        AddAchievement("FIRST_STEP", "First Step", "Activate your first zone", 50, AchievementType.Easy);
        AddAchievement("WORKER_RECRUITED", "Worker Recruited", "Assign Richard to the building", 50, AchievementType.Easy);
        AddAchievement("FIRST_PRODUCTION", "First Production", "Collect your first asset", 50, AchievementType.Easy);
        AddAchievement("FULL_HOUSE", "Full House", "Assign all 3 characters to the building", 150, AchievementType.Easy);
        // Add other achievements as needed
    }

    private void AddAchievement(string id, string name, string description, float reward, AchievementType type)
    {
        achievements[id] = new Achievement(id, name, description, reward, type);
    }
} 