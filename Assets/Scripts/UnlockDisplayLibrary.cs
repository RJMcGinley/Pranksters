using UnityEngine;

public static class UnlockDisplayLibrary
{
    public static string GetTitle(int tier)
    {
        switch (tier)
        {
            case 1: return "Crew Leader";
            case 2: return "Expert";
            case 3: return "Master";
            default: return "Unknown";
        }
    }

    public static string GetFlavor(int tier)
    {
        switch (tier)
        {
            case 1: return "+1 Prank points when used to complete a prank";
            case 2: return "+3 Prank points when used to complete a prank";
            case 3: return "+5 Prank points when used to complete a prank";
            default: return "";
        }
    }

    public static Sprite GetSprite(string pranksterType, int tier)
    {
        string path = "";

        // normalize (important since your save uses string)
        pranksterType = pranksterType.ToLower();

        if (tier == 1)
            path = $"UnlockCards/{pranksterType}_crewleader";
        else if (tier == 2)
            path = $"UnlockCards/{pranksterType}_expert";
        else if (tier == 3)
            path = $"UnlockCards/{pranksterType}_master";

        return Resources.Load<Sprite>(path);
    }
}