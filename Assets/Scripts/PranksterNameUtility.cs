public static class PranksterNameUtility
{
    public static string GetPranksterDisplayName(PranksterType type)
    {
        switch (type)
        {
            case PranksterType.Scribe: return "Scholar";
            case PranksterType.Laborer: return "Hard Hand";
            case PranksterType.Engineer: return "Planner";
            case PranksterType.BeastMaster: return "Wrangler";
            case PranksterType.Wizard: return "Mystic";
            case PranksterType.Thief: return "Covert";
            default: return type.ToString();
        }
    }
}