using System.Collections.Generic;

[System.Serializable]
public class PrankCard
{
    public string title;
    public List<PranksterType> requiredPranksters = new List<PranksterType>();
    public int renownPoints;
    public int favorMultiplier;
}