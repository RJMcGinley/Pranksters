public static class AvailableServicesUnlockRules
{
    public const int UnlockPanelAt = 50;
    public const int UnlockScoringAt = 50;
    public const int UnlockImmediateAt = 150;
    public const int UnlockOngoingAt = 300;

    public static bool CanUsePanel(PlayerProgressSave save)
    {
        return save != null && save.lifetimeFinalScorePoints >= UnlockPanelAt;
    }

    public static bool CanUseScoring(PlayerProgressSave save)
    {
        return save != null && save.lifetimeFinalScorePoints >= UnlockScoringAt;
    }

    public static bool CanUseImmediate(PlayerProgressSave save)
    {
        return save != null && save.lifetimeFinalScorePoints >= UnlockImmediateAt;
    }

    public static bool CanUseOngoing(PlayerProgressSave save)
    {
        return save != null && save.lifetimeFinalScorePoints >= UnlockOngoingAt;
    }
}