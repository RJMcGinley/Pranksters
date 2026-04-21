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

    public List<PrankCompletionEntry> prankCompletions = new List<PrankCompletionEntry>();
    public List<FavorPointsEntry> favorPointsByType = new List<FavorPointsEntry>();
    public List<DiscardCountEntry> discardCountsByType = new List<DiscardCountEntry>();
    public List<PranksterUnlockEntry> pranksterUnlocks = new List<PranksterUnlockEntry>();
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
public class PranksterUnlockEntry
{
    public string pranksterType;
    public int tier;
    public bool earned;
    public int unlockOrder;
}