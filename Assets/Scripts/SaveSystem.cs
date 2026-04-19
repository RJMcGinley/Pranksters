using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class SaveSystem
{
    private static readonly string SaveFileName = "player1save.json";

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


}