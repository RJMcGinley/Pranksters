using System;

[Serializable]
public class PranksterDeckEntry
{
    public PranksterType pranksterType;
    public int tier; // 0 = base
    public PranksterUnlockCategory category; // PrankCompletion or FavorOffer
}