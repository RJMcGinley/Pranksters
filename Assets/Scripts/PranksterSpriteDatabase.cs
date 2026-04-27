using UnityEngine;

public class PranksterSpriteDatabase : MonoBehaviour
{
    private const string BasePath = "UnlockCards/";

    // ===== STATIC REFERENCES =====

    public static Sprite thief;
    public static Sprite wizard;
    public static Sprite engineer;
    public static Sprite beastMaster;
    public static Sprite laborer;
    public static Sprite scribe;

    public static Sprite thiefCrewLeader;
    public static Sprite wizardCrewLeader;
    public static Sprite engineerCrewLeader;
    public static Sprite beastMasterCrewLeader;
    public static Sprite laborerCrewLeader;
    public static Sprite scribeCrewLeader;

    public static Sprite thiefExpert;
    public static Sprite wizardExpert;
    public static Sprite engineerExpert;
    public static Sprite beastMasterExpert;
    public static Sprite laborerExpert;
    public static Sprite scribeExpert;

    public static Sprite thiefMaster;
    public static Sprite wizardMaster;
    public static Sprite engineerMaster;
    public static Sprite beastMasterMaster;
    public static Sprite laborerMaster;
    public static Sprite scribeMaster;

    // ===== PUBLIC API =====

    public static Sprite GetSprite(PranksterType type)
    {
        return GetSprite(type, 0);
    }

    public static Sprite GetSprite(PranksterType type, int tier)
    {
        // Use assigned sprites only (stable gameplay path)
        return GetAssignedSprite(type, tier);
    }

    public static Sprite GetSprite(string pranksterType, int tier)
    {
        return LoadFromResources(pranksterType, tier);
    }

    // ===== ASSIGNED SPRITE LOGIC =====

    private static Sprite GetAssignedSprite(PranksterType type, int tier)
    {
        switch (type)
        {
            case PranksterType.Thief:
                return GetTierSprite(thief, thiefCrewLeader, thiefExpert, thiefMaster, tier);

            case PranksterType.Wizard:
                return GetTierSprite(wizard, wizardCrewLeader, wizardExpert, wizardMaster, tier);

            case PranksterType.Engineer:
                return GetTierSprite(engineer, engineerCrewLeader, engineerExpert, engineerMaster, tier);

            case PranksterType.BeastMaster:
                return GetTierSprite(beastMaster, beastMasterCrewLeader, beastMasterExpert, beastMasterMaster, tier);

            case PranksterType.Laborer:
                return GetTierSprite(laborer, laborerCrewLeader, laborerExpert, laborerMaster, tier);

            case PranksterType.Scribe:
                return GetTierSprite(scribe, scribeCrewLeader, scribeExpert, scribeMaster, tier);
        }

        return null;
    }

    private static Sprite GetTierSprite(Sprite baseSprite, Sprite t1, Sprite t2, Sprite t3, int tier)
    {
        switch (tier)
        {
            case 1: return t1 != null ? t1 : baseSprite;
            case 2: return t2 != null ? t2 : baseSprite;
            case 3: return t3 != null ? t3 : baseSprite;
            default: return baseSprite;
        }
    }

    // ===== RESOURCE LOADING (ONLY FOR UNLOCKS / EXTRA) =====

    private static Sprite LoadFromResources(string pranksterType, int tier)
    {
        string normalized = pranksterType == "BeastMaster" ? "Beastmaster" : pranksterType;

        string resourceName = normalized;

        if (tier == 1) resourceName += "Crewleader";
        else if (tier == 2) resourceName += "Expert";
        else if (tier == 3) resourceName += "Master";

        Sprite sprite = Resources.Load<Sprite>(BasePath + resourceName);

        if (sprite == null && tier > 0)
        {
            Debug.LogWarning("Missing unlock sprite: " + resourceName + " → fallback to base");
            sprite = Resources.Load<Sprite>(BasePath + normalized);
        }

        if (sprite == null)
        {
            Debug.LogError("Missing base sprite: " + normalized);
        }

        return sprite;
    }

    // ===== CATEGORY-BASED SPRITES (UNLOCK SYSTEM) =====

    public static Sprite GetSprite(PranksterType type, int tier, PranksterUnlockCategory category)
    {
        string pranksterName = type.ToString() == "BeastMaster" ? "Beastmaster" : type.ToString();

        string suffix = "";

        if (category == PranksterUnlockCategory.FavorOffer)
        {
            if (tier == 1) suffix = "Assistant";
            else if (tier == 2) suffix = "Strategist";
            else if (tier == 3) suffix = "Advisor";
        }
        else if (category == PranksterUnlockCategory.Discard)
        {
            if (tier == 1) suffix = "Hustler";
            else if (tier == 2) suffix = "Opportunist";
            else if (tier == 3) suffix = "Manipulator";
        }
        else // PrankCompletion
        {
            if (tier == 1) suffix = "Crewleader";
            else if (tier == 2) suffix = "Expert";
            else if (tier == 3) suffix = "Master";
        }

        string resourceName = pranksterName + suffix;

        Sprite sprite = Resources.Load<Sprite>("UnlockCards/" + resourceName);

        if (sprite == null)
        {
            Debug.LogWarning("SPRITE LOAD FAILED | " + resourceName + " → fallback to base");
            sprite = Resources.Load<Sprite>("UnlockCards/" + pranksterName);
        }

        return sprite;
    }

    public static string GetTierTitle(int tier)
{
    switch (tier)
    {
        case 1: return "Crew Leader";
        case 2: return "Expert";
        case 3: return "Master";
        default: return "Base";
    }
}

public static string GetTierFlavorText(int tier)
{
    switch (tier)
    {
        case 1: return "+1 Prank points when used to complete a prank";
        case 2: return "+3 Prank points when used to complete a prank";
        case 3: return "+5 Prank points when used to complete a prank";
        default: return "";
    }
}

public static string GetFavorTierTitle(int tier)
{
    switch (tier)
    {
        case 1: return "Assistant";
        case 2: return "Strategist";
        case 3: return "Advisor";
        default: return "Base";
    }
}

public static string GetFavorTierFlavorText(int tier)
{
    switch (tier)
    {
        case 1: return "+1 Favor point when offered as favor";
        case 2: return "+3 Favor points when offered as favor";
        case 3: return "+5 Favor points when offered as favor";
        default: return "";
    }
}

public static string GetDiscardTierTitle(int tier)
{
    switch (tier)
    {
        case 1: return "Hustler";
        case 2: return "Opportunist";
        case 3: return "Manipulator";
        default: return "Base";
    }
}

public static string GetDiscardTierFlavorText(int tier)
{
    switch (tier)
    {
        case 1: return "+1 Favor point when discarded";
        case 2: return "+1 Favor point and +1 Prank point when discarded";
        case 3: return "+2 Favor points and +2 Prank points when discarded";
        default: return "";
    }
}
}