using UnityEngine;

public class PranksterSpriteDatabase : MonoBehaviour
{
    private const string BasePath = "UnlockCards/";

    // ===== EXISTING STATIC REFERENCES (KEEP THESE) =====

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
        // 1. Try existing assigned sprites first (your current system)
        Sprite assigned = GetAssignedSprite(type, tier);
        if (assigned != null)
            return assigned;

        // 2. Fallback to Resources system (new system)
        return LoadFromResources(type.ToString(), tier);
    }

    public static Sprite GetSprite(string pranksterType, int tier)
    {
        // Used by unlock system directly
        return LoadFromResources(pranksterType, tier);
    }

    // ===== EXISTING LOGIC (UNCHANGED BEHAVIOR) =====

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

    // ===== NEW RESOURCE LOADING SYSTEM =====

    private static Sprite LoadFromResources(string pranksterType, int tier)
    {
        string normalized = NormalizePranksterType(pranksterType);
        string resourceName = GetResourceName(normalized, tier);

        Sprite sprite = Resources.Load<Sprite>(BasePath + resourceName);

        if (sprite == null && tier > 0)
        {
            Debug.LogWarning("Missing unlock sprite: " + resourceName + " → falling back to base");
            sprite = Resources.Load<Sprite>(BasePath + normalized);
        }

        if (sprite == null)
        {
            Debug.LogError("Missing base sprite: " + normalized);
        }

        return sprite;
    }

    private static string GetResourceName(string pranksterType, int tier)
    {
        switch (tier)
        {
            case 1: return pranksterType + "Crewleader";
            case 2: return pranksterType + "Expert";
            case 3: return pranksterType + "Master";
            default: return pranksterType;
        }
    }

    private static string NormalizePranksterType(string type)
    {
        switch (type)
        {
            case "BeastMaster": return "Beastmaster";
            default: return type;
        }
    }

    // ===== UI HELPERS =====

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




}