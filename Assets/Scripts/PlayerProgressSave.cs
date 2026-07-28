using System;
using System.Collections.Generic;

[Serializable]
public class PlayerProgressSave
{
    public int wins2P;
    public int losses2P;

    public int wins3P;
    public int losses3P;

    public int wins4P;
    public int losses4P;

    public int lifetimeFinalScorePoints;
    public int highestSingleGameScore;
    public bool fullGamePurchased = false;
    public bool hasUnlockedTwinMayor = false;
    public bool hasUnlockedTripletMayor = false;
    public bool hasPlayedTutorialGame;

    public List<PrankCompletionEntry> prankCompletions = new List<PrankCompletionEntry>();
    public List<FavorPointsEntry> favorPointsByType = new List<FavorPointsEntry>();
    public List<DiscardCountEntry> discardCountsByType = new List<DiscardCountEntry>();
    public List<AvailableServiceUseCountEntry> availableServiceUseCountsByType = new List<AvailableServiceUseCountEntry>();
    public List<PranksterUnlockEntry> pranksterUnlocks = new List<PranksterUnlockEntry>();

    public float musicVolume = .4f;
    public float sfxVolume = .7f;
    public bool fartSoundsEnabled = true;
    public bool tutorialTipsEnabled = true;
}

[Serializable]
public class PrankCompletionEntry
{
    public string prankTitle;
    public int timesCompleted;
}

[Serializable]
public class FavorPointsEntry
{
    public string pranksterType;
    public int totalFavorPointsGained;
}

[Serializable]
public class DiscardCountEntry
{
    public string pranksterType;
    public int totalDiscards;
}

[Serializable]
public class AvailableServiceUseCountEntry
{
    public string pranksterType;
    public int totalAvailableServiceUses;
}

[Serializable]
public class PranksterUnlockEntry
{
    public string pranksterType;
    public int tier;
    public PranksterUnlockCategory category; // ← ADD THIS LINE
    public bool earned;
    public int unlockOrder;
}