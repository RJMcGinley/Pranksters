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

        float spacing = choosingFavor ? 2.4f : 1.9f;
        float yOffset = choosingFavor ? 0.2f : 0f;

        if (choosingSwapHand)
        {
            spacing = 1.7f;
            yOffset = 0f;
        }

        int count = hand.Count;
        float startX = -(count - 1) * spacing / 2f;

        for (int i = 0; i < count; i++)
        {
            float x = startX + i * spacing;
            Sprite art = GetSpriteForEntry(hand[i]);

            Debug.Log("HAND DISPLAY | slot=" + i +
                      " | type=" + hand[i].pranksterType +
                      " | tier=" + hand[i].tier +
                      " | category=" + hand[i].category +
                      " | sprite=" + (art != null ? art.name : "NULL"));

            CreateCard(art, hand[i].tier, new Vector3(x, yOffset, 0), i);
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
            case 1: suffix = "Assistant"; break;
            case 2: suffix = "Strategist"; break;
            case 3: suffix = "Advisor"; break;
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

    void CreateCard(Sprite art, int tier, Vector3 localPosition, int index)
    {
        GameObject card = Instantiate(pranksterCardPrefab, currentPlayerHandArea);
        card.transform.localPosition = localPosition;

        if (deckManager != null && deckManager.IsChoosingFavor())
        {
            card.transform.localScale = new Vector3(1.15f, 1.15f, 1f);
        }
        else if (deckManager != null && deckManager.IsInSwapHandSelection())
        {
            card.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
        }
        else
        {
            card.transform.localScale = Vector3.one;
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
                cardView.characterArtRenderer.color = Color.red;
            }
        }
    }
}