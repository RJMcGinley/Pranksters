using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PrankCard
{
    public string title;
    public List<PranksterType> requiredPranksters;
    public int renownPoints;
    public int favorMultiplier;
    public Sprite cardSprite;

    public bool IsFourOfSameType(out PranksterType type)
    {
        type = PranksterType.Thief;

        if (requiredPranksters == null || requiredPranksters.Count != 4)
            return false;

        PranksterType first = requiredPranksters[0];

        for (int i = 1; i < requiredPranksters.Count; i++)
        {
            if (requiredPranksters[i] != first)
                return false;
        }

        type = first;
        return true;
    }
}