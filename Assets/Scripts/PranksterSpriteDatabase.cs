using UnityEngine;

public class PranksterSpriteDatabase : MonoBehaviour
{
    // Base
    public static Sprite thief;
    public static Sprite wizard;
    public static Sprite engineer;
    public static Sprite beastMaster;
    public static Sprite laborer;
    public static Sprite scribe;

    // Crew Leader
    public static Sprite thiefCrewLeader;
    public static Sprite wizardCrewLeader;
    public static Sprite engineerCrewLeader;
    public static Sprite beastMasterCrewLeader;
    public static Sprite laborerCrewLeader;
    public static Sprite scribeCrewLeader;

    // Expert
    public static Sprite thiefExpert;
    public static Sprite wizardExpert;
    public static Sprite engineerExpert;
    public static Sprite beastMasterExpert;
    public static Sprite laborerExpert;
    public static Sprite scribeExpert;

    // Master
    public static Sprite thiefMaster;
    public static Sprite wizardMaster;
    public static Sprite engineerMaster;
    public static Sprite beastMasterMaster;
    public static Sprite laborerMaster;
    public static Sprite scribeMaster;

    public static Sprite GetSprite(PranksterType type)
    {
        return GetSprite(type, 0);
    }

    public static Sprite GetSprite(PranksterType type, int tier)
    {
        switch (type)
        {
            case PranksterType.Thief:
                switch (tier)
                {
                    case 1: return thiefCrewLeader != null ? thiefCrewLeader : thief;
                    case 2: return thiefExpert != null ? thiefExpert : thief;
                    case 3: return thiefMaster != null ? thiefMaster : thief;
                    default: return thief;
                }

            case PranksterType.Wizard:
                switch (tier)
                {
                    case 1: return wizardCrewLeader != null ? wizardCrewLeader : wizard;
                    case 2: return wizardExpert != null ? wizardExpert : wizard;
                    case 3: return wizardMaster != null ? wizardMaster : wizard;
                    default: return wizard;
                }

            case PranksterType.Engineer:
                switch (tier)
                {
                    case 1: return engineerCrewLeader != null ? engineerCrewLeader : engineer;
                    case 2: return engineerExpert != null ? engineerExpert : engineer;
                    case 3: return engineerMaster != null ? engineerMaster : engineer;
                    default: return engineer;
                }

            case PranksterType.BeastMaster:
                switch (tier)
                {
                    case 1: return beastMasterCrewLeader != null ? beastMasterCrewLeader : beastMaster;
                    case 2: return beastMasterExpert != null ? beastMasterExpert : beastMaster;
                    case 3: return beastMasterMaster != null ? beastMasterMaster : beastMaster;
                    default: return beastMaster;
                }

            case PranksterType.Laborer:
                switch (tier)
                {
                    case 1: return laborerCrewLeader != null ? laborerCrewLeader : laborer;
                    case 2: return laborerExpert != null ? laborerExpert : laborer;
                    case 3: return laborerMaster != null ? laborerMaster : laborer;
                    default: return laborer;
                }

            case PranksterType.Scribe:
                switch (tier)
                {
                    case 1: return scribeCrewLeader != null ? scribeCrewLeader : scribe;
                    case 2: return scribeExpert != null ? scribeExpert : scribe;
                    case 3: return scribeMaster != null ? scribeMaster : scribe;
                    default: return scribe;
                }
        }

        return null;
    }
}