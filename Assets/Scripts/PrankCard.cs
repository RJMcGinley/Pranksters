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
}