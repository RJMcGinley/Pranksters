public static class PranksterUnlockRules
{
    public static int GetRenownBonusForTier(int tier)
    {
        switch (tier)
        {
            case 1: return 1; // Crew Leader
            case 2: return 3; // Expert
            case 3: return 5; // Master
            default: return 0; // Base
        }
    }

    public static int GetFavorBonusForTier(int tier)
    {
        switch (tier)
        {
            case 1: return 1; // Assistant
            case 2: return 3; // Strategist
            case 3: return 5; // Advisor
            default: return 0; // Base
        }
    }

    public static int GetRenownBonus(PranksterDeckEntry entry)
    {
        if (entry == null)
            return 0;

        return GetRenownBonusForTier(entry.tier);
    }

    public static int GetFavorBonus(PranksterDeckEntry entry)
    {
        if (entry == null)
            return 0;

        return GetFavorBonusForTier(entry.tier);
    }
}