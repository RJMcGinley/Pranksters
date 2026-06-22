using UnityEngine;
using UnityEngine.UI;

public class WantedJailController : MonoBehaviour
{
    [Header("Jail Slots")]
    public Image[] jailSlotImages;

    [Header("Jailed Recruit Sprites")]
    public Sprite thiefInJailCell;
    public Sprite wizardInJailCell;
    public Sprite engineerInJailCell;
    public Sprite beastmasterInJailCell;
    public Sprite laborerInJailCell;
    public Sprite scribeInJailCell;

    private int jailedRecruitCount = 0;

    private void Awake()
    {
        ClearJailDisplay();
    }

    public void AddJailedRecruit(PranksterType type)
    {
        if (jailedRecruitCount >= jailSlotImages.Length)
        {
            Debug.Log("WANTED JAIL FULL | Cannot add more jailed recruits.");
            return;
        }

        Image slotImage = jailSlotImages[jailedRecruitCount];

        if (slotImage != null)
        {
            slotImage.gameObject.SetActive(true);
            slotImage.sprite = GetJailSpriteForType(type);
            slotImage.enabled = true;
        }

        jailedRecruitCount++;

        Debug.Log("JAIL DISPLAY UPDATED | Total Jailed = " + jailedRecruitCount);

        if (jailedRecruitCount >= 5)
        {
            Debug.Log("WANTED BOARD LOSS | Jail is full.");
        }
    }

    public int GetJailedRecruitCount()
    {
        return jailedRecruitCount;
    }

    public void ClearJailDisplay()
    {
        jailedRecruitCount = 0;

        foreach (Image image in jailSlotImages)
        {
            if (image != null)
            {
                image.sprite = null;
                image.enabled = false;
                image.gameObject.SetActive(false);
            }
        }
    }

    private Sprite GetJailSpriteForType(PranksterType type)
    {
        switch (type)
        {
            case PranksterType.Thief:
                return thiefInJailCell;

            case PranksterType.Wizard:
                return wizardInJailCell;

            case PranksterType.Engineer:
                return engineerInJailCell;

            case PranksterType.BeastMaster:
                return beastmasterInJailCell;

            case PranksterType.Laborer:
                return laborerInJailCell;

            case PranksterType.Scribe:
                return scribeInJailCell;

            default:
                return null;
        }
    }

    [ContextMenu("Test Add Thief")]
    public void TestAddThief()
    {
        AddJailedRecruit(PranksterType.Thief);
    }
}