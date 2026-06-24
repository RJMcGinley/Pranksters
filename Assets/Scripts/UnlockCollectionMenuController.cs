using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class UnlockCollectionMenuController : MonoBehaviour
{
    [Header("Row 1 Text")]
    public TMP_Text prankNameText;
    public TMP_Text currentValuePrankCompletion;

    [Header("Row 2 Text")]
    public TMP_Text currentValueInfluenceGained;

    [Header("Row 3 Text")]
    public TMP_Text currentValueAvailableService;

    [Header("Prank Completion Cards")]
    public PranksterCardUIView prankCompletionTier1;
    public PranksterCardUIView prankCompletionTier2;
    public PranksterCardUIView prankCompletionTier3;

    [Header("Favor Offer Cards")]
    public PranksterCardUIView favorTier1;
    public PranksterCardUIView favorTier2;
    public PranksterCardUIView favorTier3;

    [Header("Available Service Cards")]
    public PranksterCardUIView serviceTier1;
    public PranksterCardUIView serviceTier2;
    public PranksterCardUIView serviceTier3;

    private PlayerProgressSave progressData;

    [Header("Prank Card")]
    public UnlockProgressionPanelPrankView prankCardView;

    [Header("Recruit Tabs")]
    public UnlockRecruitTabButton rogueTab;
    public UnlockRecruitTabButton mysticTab;
    public UnlockRecruitTabButton plannerTab;
    public UnlockRecruitTabButton wranglerTab;
    public UnlockRecruitTabButton handsTab;
    public UnlockRecruitTabButton scholarTab;

    private void OnEnable()
    {
        progressData = SaveSystem.Load();
        ShowRecruit(PranksterType.Scribe);
    }

    public void ShowThief()
    {
        ShowRecruit(PranksterType.Thief);
    }

    public void ShowWizard()
    {
        ShowRecruit(PranksterType.Wizard);
    }

    public void ShowEngineer()
    {
        ShowRecruit(PranksterType.Engineer);
    }

    public void ShowBeastMaster()
    {
        ShowRecruit(PranksterType.BeastMaster);
    }

    public void ShowLaborer()
    {
        ShowRecruit(PranksterType.Laborer);
    }

    public void ShowScribe()
    {
        ShowRecruit(PranksterType.Scribe);
    }

    public void ShowRecruitFromTab(PranksterType type)
    {
        ShowRecruit(type);
    }

    private void ShowRecruit(PranksterType type)
    {
        if (progressData == null)
            progressData = SaveSystem.Load();

        if (prankNameText != null)
            prankNameText.text = GetPrankName(type);

        SetCurrentValues(type);
        SetPrankCardImage(type);

        SetCard(prankCompletionTier1, type, 1, PranksterUnlockCategory.PrankCompletion);
        SetCard(prankCompletionTier2, type, 2, PranksterUnlockCategory.PrankCompletion);
        SetCard(prankCompletionTier3, type, 3, PranksterUnlockCategory.PrankCompletion);

        SetCard(favorTier1, type, 1, PranksterUnlockCategory.FavorOffer);
        SetCard(favorTier2, type, 2, PranksterUnlockCategory.FavorOffer);
        SetCard(favorTier3, type, 3, PranksterUnlockCategory.FavorOffer);

        SetCard(serviceTier1, type, 1, PranksterUnlockCategory.AvailableService);
        SetCard(serviceTier2, type, 2, PranksterUnlockCategory.AvailableService);
        SetCard(serviceTier3, type, 3, PranksterUnlockCategory.AvailableService);

        UpdateTabVisuals(type);
    }

    private void SetCurrentValues(PranksterType type)
    {
        if (currentValuePrankCompletion != null)
            currentValuePrankCompletion.text = GetPrankCompletionCurrentValue(type).ToString();

        if (currentValueInfluenceGained != null)
            currentValueInfluenceGained.text = GetInfluenceCurrentValue(type).ToString();

        if (currentValueAvailableService != null)
            currentValueAvailableService.text = GetAvailableServiceCurrentValue(type).ToString();
    }

    private int GetPrankCompletionCurrentValue(PranksterType type)
    {
        if (progressData == null || progressData.prankCompletions == null)
            return 0;

        string prankName = GetPrankName(type);

        for (int i = 0; i < progressData.prankCompletions.Count; i++)
        {
            PrankCompletionEntry entry = progressData.prankCompletions[i];

            if (entry != null && entry.prankTitle == prankName)
                return entry.timesCompleted;
        }

        return 0;
    }

    private int GetInfluenceCurrentValue(PranksterType type)
    {
        if (progressData == null || progressData.favorPointsByType == null)
            return 0;

        string typeName = type.ToString();

        for (int i = 0; i < progressData.favorPointsByType.Count; i++)
        {
            FavorPointsEntry entry = progressData.favorPointsByType[i];

            if (entry != null && entry.pranksterType == typeName)
                return entry.totalFavorPointsGained;
        }

        return 0;
    }

    private int GetAvailableServiceCurrentValue(PranksterType type)
    {
        if (progressData == null || progressData.availableServiceUseCountsByType == null)
            return 0;

        string typeName = type.ToString();

        for (int i = 0; i < progressData.availableServiceUseCountsByType.Count; i++)
        {
            AvailableServiceUseCountEntry entry = progressData.availableServiceUseCountsByType[i];

            if (entry != null && entry.pranksterType == typeName)
                return entry.totalAvailableServiceUses;
        }

        return 0;
    }

    private void SetCard(PranksterCardUIView cardView, PranksterType type, int tier, PranksterUnlockCategory category)
{
    if (cardView == null)
    {
        Debug.LogWarning("UnlockCollectionMenuController: cardView is not assigned for " +
                         type + " tier " + tier + " category " + category);
        return;
    }

    PranksterDeckEntry card = new PranksterDeckEntry
    {
        pranksterType = type,
        tier = tier,
        category = category
    };

    cardView.SetCard(card);

    bool unlocked = IsUnlocked(type, tier, category);
    cardView.SetUnlockedVisual(unlocked);

    UnlockPranksterCardHoverTarget hoverTarget =
        cardView.GetComponent<UnlockPranksterCardHoverTarget>();

    if (hoverTarget != null)
        hoverTarget.SetHoverData(card, unlocked);
}

    private bool IsUnlocked(PranksterType type, int tier, PranksterUnlockCategory category)
    {
        if (progressData == null || progressData.pranksterUnlocks == null)
            return false;

        string typeName = type.ToString();

        for (int i = 0; i < progressData.pranksterUnlocks.Count; i++)
        {
            PranksterUnlockEntry entry = progressData.pranksterUnlocks[i];

            if (entry.pranksterType == typeName &&
                entry.tier == tier &&
                entry.category == category)
            {
                return entry.earned;
            }
        }

        return false;
    }

    private string GetPrankName(PranksterType type)
    {
        switch (type)
        {
            case PranksterType.Thief:
                return "Wooden Nickels";

            case PranksterType.Wizard:
                return "The Angry Moon";

            case PranksterType.Engineer:
                return "Statue Swap";

            case PranksterType.BeastMaster:
                return "Fowl Play Exchange";

            case PranksterType.Laborer:
                return "Privies on the Porch";

            case PranksterType.Scribe:
                return "Municipal Paper Shuffle";

            default:
                return "";
        }
    }

    private void SetPrankCardImage(PranksterType type)
{

    if (prankCardView == null)
        return;

    string prankName = GetPrankName(type);
    PrankCard prank = FindPrankByName(prankName);

    if (prank == null)
    {
        Debug.LogWarning("UnlockCollectionMenuController: Could not find prank named " + prankName);
        return;
    }

    prankCardView.SetPrank(prank);

    UnlockPrankCardHoverTarget hoverTarget =
        prankCardView.GetComponent<UnlockPrankCardHoverTarget>();

    if (hoverTarget != null)
        hoverTarget.SetHoverData(prank);
}

private PrankCard FindPrankByName(string prankName)
{
    List<PrankCard> prankDeck = PrankDatabase.CreatePrankDeck();

    for (int i = 0; i < prankDeck.Count; i++)
    {
        if (prankDeck[i].title == prankName)
            return prankDeck[i];
    }

    return null;
}

private void UpdateTabVisuals(PranksterType selectedType)
{
    if (rogueTab != null)
        rogueTab.SetSelected(selectedType == PranksterType.Thief);

    if (mysticTab != null)
        mysticTab.SetSelected(selectedType == PranksterType.Wizard);

    if (plannerTab != null)
        plannerTab.SetSelected(selectedType == PranksterType.Engineer);

    if (wranglerTab != null)
        wranglerTab.SetSelected(selectedType == PranksterType.BeastMaster);

    if (handsTab != null)
        handsTab.SetSelected(selectedType == PranksterType.Laborer);

    if (scholarTab != null)
        scholarTab.SetSelected(selectedType == PranksterType.Scribe);
}
}