public static class PranksterNameUtility
{
    public static string GetPranksterDisplayName(PranksterType type)
    {
        switch (type)
        {
            case PranksterType.Scribe: return "Scholar";
            case PranksterType.Laborer: return "Roughneck";
            case PranksterType.Engineer: return "Tinkerer";
            case PranksterType.BeastMaster: return "Wrangler";
            case PranksterType.Wizard: return "Mystic";
            case PranksterType.Thief: return "Rogue";
            default: return type.ToString();
        }
    }
}