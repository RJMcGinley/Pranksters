using UnityEngine;

public class PranksterSpriteDatabase : MonoBehaviour
{
    public static Sprite thief;
    public static Sprite wizard;
    public static Sprite engineer;
    public static Sprite beastMaster;
    public static Sprite laborer;
    public static Sprite scribe;

    public static Sprite GetSprite(PranksterType type)
    {
        switch (type)
        {
            case PranksterType.Thief: return thief;
            case PranksterType.Wizard: return wizard;
            case PranksterType.Engineer: return engineer;
            case PranksterType.BeastMaster: return beastMaster;
            case PranksterType.Laborer: return laborer;
            case PranksterType.Scribe: return scribe;
        }

        return null;
    }
}