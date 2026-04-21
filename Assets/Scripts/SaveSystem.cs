using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class SaveSystem
{
    private static readonly string SaveFileName = "player1save.json";

    private static bool FullGamePurchased => false;
    private static List<PranksterUnlockEntry> sessionNewUnlocks = new List<PranksterUnlockEntry>();

    private static string SavePath
    {
        get { return Path.Combine(Application.persistentDataPath, SaveFileName); }
    }

    public static void Save(PlayerProgressSave data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("SAVE WRITTEN: " + SavePath);
    }

    public static PlayerProgressSave Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("NO SAVE FOUND. Creating new PlayerProgressSave.");
            return CreateDefaultSave();
        }

        string json = File.ReadAllText(SavePath);
        PlayerProgressSave data = JsonUtility.FromJson<PlayerProgressSave>(json);

        if (data == null)
        {
            Debug.LogWarning("SAVE FILE WAS NULL OR INVALID. Creating new PlayerProgressSave.");
            return CreateDefaultSave();
        }

        EnsureDefaults(data);

        Debug.Log("SAVE LOADED: " + SavePath);
        return data;
    }

    public static bool HasSave()
    {
        return File.Exists(SavePath);
    }

    private static PlayerProgressSave CreateDefaultSave()
    {
        PlayerProgressSave data = new PlayerProgressSave();

        foreach (PranksterType type in System.Enum.GetValues(typeof(PranksterType)))
        {
            data.favorPointsByType.Add(new FavorPointsEntry
            {
                pranksterType = type.ToString(),
                totalFavorPointsGained = 0
            });

            data.discardCountsByType.Add(new DiscardCountEntry
            {
                pranksterType = type.ToString(),
                totalDiscards = 0
            });

            for (int tier = 1; tier <= 3; tier++)
            {
                data.pranksterUnlocks.Add(new PranksterUnlockEntry
                {
                    pranksterType = type.ToString(),
                    tier = tier,
                    earned = false,
                    unlockOrder = 0
                });
            }
        }

        return data;
    }

    private static void EnsureDefaults(PlayerProgressSave data)
    {
        if (data.prankCompletions == null)
            data.prankCompletions = new List<PrankCompletionEntry>();

        if (data.favorPointsByType == null)
            data.favorPointsByType = new List<FavorPointsEntry>();

        if (data.discardCountsByType == null)
            data.discardCountsByType = new List<DiscardCountEntry>();

        if (data.pranksterUnlocks == null)
            data.pranksterUnlocks = new List<PranksterUnlockEntry>();

        foreach (PranksterType type in System.Enum.GetValues(typeof(PranksterType)))
        {
            bool favorFound = false;

            for (int i = 0; i < data.favorPointsByType.Count; i++)
            {
                if (data.favorPointsByType[i].pranksterType == type.ToString())
                {
                    favorFound = true;
                    break;
                }
            }

            if (!favorFound)
            {
                data.favorPointsByType.Add(new FavorPointsEntry
                {
                    pranksterType = type.ToString(),
                    totalFavorPointsGained = 0
                });
            }

            bool discardFound = false;

            for (int i = 0; i < data.discardCountsByType.Count; i++)
            {
                if (data.discardCountsByType[i].pranksterType == type.ToString())
                {
                    discardFound = true;
                    break;
                }
            }

            if (!discardFound)
            {
                data.discardCountsByType.Add(new DiscardCountEntry
                {
                    pranksterType = type.ToString(),
                    totalDiscards = 0
                });
            }

            for (int tier = 1; tier <= 3; tier++)
            {
                bool unlockFound = false;

                for (int i = 0; i < data.pranksterUnlocks.Count; i++)
                {
                    if (data.pranksterUnlocks[i].pranksterType == type.ToString() &&
                        data.pranksterUnlocks[i].tier == tier)
                    {
                        unlockFound = true;
                        break;
                    }
                }

                if (!unlockFound)
                {
                    data.pranksterUnlocks.Add(new PranksterUnlockEntry
                    {
                        pranksterType = type.ToString(),
                        tier = tier,
                        earned = false,
                        unlockOrder = 0
                    });
                }
            }
        }
    }

    public static int GetPrankCompletionCount(string prankTitle)
    {
        PlayerProgressSave data = Load();

        for (int i = 0; i < data.prankCompletions.Count; i++)
        {
            if (data.prankCompletions[i].prankTitle == prankTitle)
                return data.prankCompletions[i].timesCompleted;
        }

        return 0;
    }

    public static PranksterUnlockEntry GetPranksterUnlock(string pranksterType, int tier)
    {
    PlayerProgressSave data = Load();

    for (int i = 0; i < data.pranksterUnlocks.Count; i++)
    {
        if (data.pranksterUnlocks[i].pranksterType == pranksterType &&
            data.pranksterUnlocks[i].tier == tier)
        {
            return data.pranksterUnlocks[i];
        }
    }

    return null;
    }

    public static bool IsPranksterUnlockEarned(string pranksterType, int tier)
    {
    PranksterUnlockEntry entry = GetPranksterUnlock(pranksterType, tier);

    if (entry == null)
        return false;

    return entry.earned;
    }

    public static int GetPranksterUnlockOrder(string pranksterType, int tier)
    {
    PranksterUnlockEntry entry = GetPranksterUnlock(pranksterType, tier);

    if (entry == null)
        return 0;

    return entry.unlockOrder;
    }

    public static int GetTotalEarnedUnlockCount()
    {
    PlayerProgressSave data = Load();
    int count = 0;

    for (int i = 0; i < data.pranksterUnlocks.Count; i++)
    {
        if (data.pranksterUnlocks[i].earned)
            count++;
    }

    return count;
    }

    public static bool IsPranksterUnlockUsable(string pranksterType, int tier)
    {
    PranksterUnlockEntry entry = GetPranksterUnlock(pranksterType, tier);

    if (entry == null)
        return false;

    if (!entry.earned)
        return false;

    if (FullGamePurchased)
        return true;

    return entry.unlockOrder > 0 && entry.unlockOrder <= 5;
    }

    public static bool EarnPranksterUnlock(string pranksterType, int tier)
    {
    PlayerProgressSave data = Load();

    PranksterUnlockEntry entry = null;

    for (int i = 0; i < data.pranksterUnlocks.Count; i++)
    {
        if (data.pranksterUnlocks[i].pranksterType == pranksterType &&
            data.pranksterUnlocks[i].tier == tier)
        {
            entry = data.pranksterUnlocks[i];
            break;
        }
    }

    if (entry == null)
    {
        Debug.LogWarning("SaveSystem: Could not find unlock entry for " + pranksterType + " tier " + tier);
        return false;
    }

    if (entry.earned)
    {
        Debug.Log("SaveSystem: Unlock already earned for " + pranksterType + " tier " + tier);
        return false;
    }

    int nextOrder = 1;

    for (int i = 0; i < data.pranksterUnlocks.Count; i++)
    {
        if (data.pranksterUnlocks[i].earned && data.pranksterUnlocks[i].unlockOrder >= nextOrder)
        {
            nextOrder = data.pranksterUnlocks[i].unlockOrder + 1;
        }
    }

    entry.earned = true;
    entry.unlockOrder = nextOrder;

    Save(data);

    Debug.Log("UNLOCK EARNED: " + pranksterType + " tier " + tier + " | order = " + nextOrder);
    return true;
    }

    public static int GetUnlockTierFromCompletionCount(int completionCount)
    {
    if (completionCount >= 10)
        return 3;

    if (completionCount >= 5)
        return 2;

    if (completionCount >= 1)
        return 1;

    return 0;
    }

    public static List<PranksterUnlockEntry> EvaluateAndAwardUnlocksFromSavedProgress()
    {

    sessionNewUnlocks.Clear();
        
    PlayerProgressSave data = Load();
    List<PranksterUnlockEntry> newlyEarned = new List<PranksterUnlockEntry>();

    List<PrankCard> prankDeck = PrankDatabase.CreatePrankDeck();

    for (int i = 0; i < data.prankCompletions.Count; i++)
    {
        PrankCompletionEntry completionEntry = data.prankCompletions[i];
        if (completionEntry == null || string.IsNullOrWhiteSpace(completionEntry.prankTitle))
            continue;

        PrankCard matchingPrank = null;

        for (int j = 0; j < prankDeck.Count; j++)
        {
            if (prankDeck[j].title == completionEntry.prankTitle)
            {
                matchingPrank = prankDeck[j];
                break;
            }
        }

        if (matchingPrank == null)
            continue;

        if (!matchingPrank.IsFourOfSameType(out PranksterType pranksterType))
            continue;

        int highestTier = GetUnlockTierFromCompletionCount(completionEntry.timesCompleted);

        for (int tier = 1; tier <= highestTier; tier++)
        {
            bool earnedNow = EarnPranksterUnlock(pranksterType.ToString(), tier);

            if (earnedNow)
            {
                PranksterUnlockEntry unlockedEntry = GetPranksterUnlock(pranksterType.ToString(), tier);

                if (unlockedEntry != null)
                {
                    newlyEarned.Add(unlockedEntry);
                    sessionNewUnlocks.Add(unlockedEntry);
                }
            }
        }
    }

    return newlyEarned;
    }   

    public static List<PranksterUnlockEntry> GetSessionNewUnlocks()
    {
    return sessionNewUnlocks;
    }

}