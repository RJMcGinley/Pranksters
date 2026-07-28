using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WantedJailController : MonoBehaviour
{
    [Header("Jail Slots")]
    public Image[] jailSlotImages;
    public Sprite emptyJailCellSprite;

    [Header("Jailed Recruit Sprites")]
    public Sprite thiefInJailCell;
    public Sprite wizardInJailCell;
    public Sprite engineerInJailCell;
    public Sprite beastmasterInJailCell;
    public Sprite laborerInJailCell;
    public Sprite scribeInJailCell;
    

    private int jailedRecruitCount = 0;

    private List<PranksterType> jailedRecruitTypes = new List<PranksterType>();

    private void Awake()
    {
        ClearJailDisplay();
    }

    public void AddJailedRecruit(PranksterType type)
    {
        if (jailSlotImages == null || jailSlotImages.Length == 0)
        {
            Debug.LogWarning("WANTED JAIL DISPLAY HAS NO SLOTS ASSIGNED.");
        }

        jailedRecruitTypes.Add(type);
        jailedRecruitCount = jailedRecruitTypes.Count;

        RefreshJailDisplay();

        Debug.Log("JAIL DISPLAY UPDATED | Total Jailed = " + jailedRecruitCount);
    }

    public int GetJailedRecruitCount()
    {
        return jailedRecruitCount;
    }

    public void ClearJailDisplay()
    {
        jailedRecruitCount = 0;
        jailedRecruitTypes.Clear();

        foreach (Image image in jailSlotImages)
        {
            if (image != null)
            {
                image.sprite = emptyJailCellSprite;
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

    public void HideJailDisplay()
    {
        foreach (Image image in jailSlotImages)
        {
            if (image != null)
                image.gameObject.SetActive(false);
        }
    }

    public bool HasJailedRecruitOfType(PranksterType type)
    {
        return jailedRecruitTypes.Contains(type);
    }

    public bool RemoveOneJailedRecruitOfType(PranksterType type)
    {
        if (!jailedRecruitTypes.Contains(type))
        {
            Debug.Log("JAILBREAK FAILED | No jailed recruit of type: " + type);
            return false;
        }

        jailedRecruitTypes.Remove(type);
        jailedRecruitCount = jailedRecruitTypes.Count;

        RefreshJailDisplay();

        Debug.Log("JAILBREAK SUCCESS | Freed jailed recruit of type: " + type);

        return true;
    }

    private void RefreshJailDisplay()
    {
        for (int i = 0; i < jailSlotImages.Length; i++)
        {
            Image image = jailSlotImages[i];

            if (image == null)
                continue;

            if (i < jailedRecruitTypes.Count)
            {
                image.sprite = GetJailSpriteForType(jailedRecruitTypes[i]);
                image.enabled = true;
                image.gameObject.SetActive(true);
            }
            else
            {
                image.gameObject.SetActive(false);
            }
        }
    }

    public void ShowJailDisplay()
    {
        RefreshJailDisplay();
    }

}