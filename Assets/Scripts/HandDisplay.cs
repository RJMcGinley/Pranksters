using System.Collections.Generic;
using UnityEngine;

public class HandDisplay : MonoBehaviour
{
    public GameObject pranksterCardPrefab;
    public Transform currentPlayerHandArea;
    public DeckManager deckManager;

    // BASE SPRITES ONLY
    public Sprite beastMasterSprite;
    public Sprite engineerSprite;
    public Sprite laborerSprite;
    public Sprite scribeSprite;
    public Sprite thiefSprite;
    public Sprite wizardSprite;
    [Header("Beastmaster Action Glows")]
    public GameObject immediateActionGlow;
    public GameObject scoringActionGlow;
    public GameObject ongoingActionGlow;

    public void ShowCurrentPlayerHand()
    {
        foreach (Transform child in currentPlayerHandArea)
        {
            Destroy(child.gameObject);
        }

        Player currentPlayer = deckManager.turnManager.GetCurrentPlayer();

        List<PranksterDeckEntry> hand;
        if (deckManager != null && deckManager.IsInSwapHandSelection())
            hand = deckManager.GetTempSwapHand();
        else
            hand = currentPlayer.hand;

        bool choosingFavor = deckManager != null && deckManager.IsChoosingFavor();
        bool choosingSwapHand = deckManager != null && deckManager.IsInSwapHandSelection();

        float spacing = choosingFavor ? 2.5f : 2.1f;
        float yOffset = choosingFavor ? 0.2f : 0f;

        if (choosingSwapHand)
        {
            spacing = 1.9f;
            yOffset = 0f;
        }

        int count = hand.Count;

        Vector3 cardScale = Vector3.one;

        // Dynamic hand resizing for larger hands
        if (count >= 9)
        {
            spacing = 1.25f;
            cardScale = new Vector3(0.62f, 0.62f, 1f);
        }
        if (count == 8)
        {
            spacing = 1.25f;
            cardScale = new Vector3(0.62f, 0.62f, 1f);
        }
        if (count == 7)
        {
            spacing = 1.45f;
            cardScale = new Vector3(0.72f, 0.72f, 1f);
        }
        else if (count == 6)
        {
            spacing = 1.65f;
            cardScale = new Vector3(0.82f, 0.82f, 1f);
        }
        else if (count == 5)
        {
            spacing = 1.9f;
            cardScale = new Vector3(0.92f, 0.92f, 1f);
        }

        float startX = -(count - 1) * spacing / 2f;

        for (int i = 0; i < count; i++)
        {
            if (deckManager != null &&
                deckManager.IsHandCardTemporarilyAssignedToService(i))
            {
                Debug.Log("HAND DISPLAY SKIPPING TEMP SERVICE CARD INDEX: " + i);
                continue;
            }

            float x = startX + i * spacing;
            Sprite art = GetSpriteForEntry(hand[i]);

            Debug.Log("HAND DISPLAY | slot=" + i +
                    " | type=" + hand[i].pranksterType +
                    " | tier=" + hand[i].tier +
                    " | category=" + hand[i].category +
                    " | sprite=" + (art != null ? art.name : "NULL"));

            CreateCard(art, hand[i].tier, new Vector3(x, yOffset, 0), i, cardScale);
        }
    }

    private Sprite GetSpriteForEntry(PranksterDeckEntry entry)
    {
        if (entry == null)
            return null;

        if (entry.tier <= 0)
            return GetBaseSprite(entry.pranksterType);

        if (entry.category == PranksterUnlockCategory.FavorOffer)
            return GetFavorOfferSprite(entry.pranksterType, entry.tier);

        if (entry.category == PranksterUnlockCategory.Discard)
            return GetDiscardSprite(entry.pranksterType, entry.tier);

        if (entry.category == PranksterUnlockCategory.AvailableService)
            return GetDiscardSprite(entry.pranksterType, entry.tier);

        return GetPrankCompletionSprite(entry.pranksterType, entry.tier);
    }

    private Sprite GetBaseSprite(PranksterType type)
    {
        switch (type)
        {
            case PranksterType.BeastMaster: return beastMasterSprite;
            case PranksterType.Engineer: return engineerSprite;
            case PranksterType.Laborer: return laborerSprite;
            case PranksterType.Scribe: return scribeSprite;
            case PranksterType.Thief: return thiefSprite;
            case PranksterType.Wizard: return wizardSprite;
            default: return null;
        }
    }

    private Sprite GetPrankCompletionSprite(PranksterType type, int tier)
    {
        string pranksterName = GetResourcePranksterName(type);
        string suffix = "";

        switch (tier)
        {
            case 1: suffix = "Crewleader"; break;
            case 2: suffix = "Expert"; break;
            case 3: suffix = "Master"; break;
            default: return GetBaseSprite(type);
        }

        Sprite sprite = Resources.Load<Sprite>("UnlockCards/" + pranksterName + suffix);

        if (sprite == null)
        {
            Debug.LogWarning("Missing prank-completion sprite: UnlockCards/" + pranksterName + suffix);
            return GetBaseSprite(type);
        }

        return sprite;
    }

    private Sprite GetFavorOfferSprite(PranksterType type, int tier)
    {
        string pranksterName = GetResourcePranksterName(type);
        string suffix = "";

        switch (tier)
        {
            case 1: suffix = "Courier"; break;
            case 2: suffix = "Operative"; break;
            case 3: suffix = "Kingmaker"; break;
            default: return GetBaseSprite(type);
        }

        Sprite sprite = Resources.Load<Sprite>("UnlockCards/" + pranksterName + suffix);

        if (sprite == null)
        {
            Debug.LogWarning("Missing favor sprite: UnlockCards/" + pranksterName + suffix);
            return GetBaseSprite(type);
        }

        return sprite;
    }

    private Sprite GetDiscardSprite(PranksterType type, int tier)
    {
        string pranksterName = GetResourcePranksterName(type);
        string suffix = "";

        switch (tier)
        {
            case 1: suffix = "Hustler"; break;
            case 2: suffix = "Opportunist"; break;
            case 3: suffix = "Specialist"; break;
            default: return GetBaseSprite(type);
        }

        Sprite sprite = Resources.Load<Sprite>("UnlockCards/" + pranksterName + suffix);

        if (sprite == null)
        {
            Debug.LogWarning("Missing discard sprite: UnlockCards/" + pranksterName + suffix);
            return GetBaseSprite(type);
        }

        return sprite;
    }

    private string GetResourcePranksterName(PranksterType type)
    {
        switch (type)
        {
            case PranksterType.BeastMaster:
                return "Beastmaster";
            default:
                return type.ToString();
        }
    }

    void CreateCard(Sprite art, int tier, Vector3 localPosition, int index, Vector3 cardScale)
    {
        GameObject card = Instantiate(pranksterCardPrefab, currentPlayerHandArea);
        card.transform.localPosition = localPosition;

        card.transform.localScale = cardScale;

        if (deckManager != null && deckManager.IsChoosingFavor())
        {
            card.transform.localScale = new Vector3(
                cardScale.x * 1.15f,
                cardScale.y * 1.15f,
                1f
            );
        }
        else if (deckManager != null && deckManager.IsInSwapHandSelection())
        {
            card.transform.localScale = new Vector3(
                cardScale.x * 0.9f,
                cardScale.y * 0.9f,
                1f
            );
        }

        PranksterCardView cardView = card.GetComponent<PranksterCardView>();
        if (cardView != null)
        {
            cardView.SetArt(art);
            cardView.SetTierIndicator(tier);
        }

        HandCardClick click = card.GetComponent<HandCardClick>();
        if (click != null)
        {
            click.deckManager = deckManager;
            click.cardIndex = index;
            click.SetBaseScale(card.transform.localScale);

            if (deckManager != null &&
                deckManager.IsInDiscardSelection() &&
                cardView != null &&
                cardView.characterArtRenderer != null)
            {
                cardView.characterArtRenderer.color = Color.white;
            }
        }

        HandCardDragReorder drag = card.GetComponent<HandCardDragReorder>();
        if (drag != null)
        {
            drag.deckManager = deckManager;
            drag.cardIndex = index;
        }
    }
}