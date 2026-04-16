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

    public List<PrankCompletionEntry> prankCompletions = new List<PrankCompletionEntry>();
    public List<FavorPointsEntry> favorPointsByType = new List<FavorPointsEntry>();
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