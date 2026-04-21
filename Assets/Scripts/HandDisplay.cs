using System.Collections.Generic;
using UnityEngine;

public class HandDisplay : MonoBehaviour
{
    public GameObject pranksterCardPrefab;
    public Transform currentPlayerHandArea;
    public DeckManager deckManager;

    public Sprite beastMasterSprite;
    public Sprite engineerSprite;
    public Sprite laborerSprite;
    public Sprite scribeSprite;
    public Sprite thiefSprite;
    public Sprite wizardSprite;

    public Sprite beastMasterCrewLeaderSprite;
    public Sprite engineerCrewLeaderSprite;
    public Sprite laborerCrewLeaderSprite;
    public Sprite scribeCrewLeaderSprite;
    public Sprite thiefCrewLeaderSprite;
    public Sprite wizardCrewLeaderSprite;

    public Sprite beastMasterExpertSprite;
    public Sprite engineerExpertSprite;
    public Sprite laborerExpertSprite;
    public Sprite scribeExpertSprite;
    public Sprite thiefExpertSprite;
    public Sprite wizardExpertSprite;

    public Sprite beastMasterMasterSprite;
    public Sprite engineerMasterSprite;
    public Sprite laborerMasterSprite;
    public Sprite scribeMasterSprite;
    public Sprite thiefMasterSprite;
    public Sprite wizardMasterSprite;

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
            Sprite art = GetSpriteForPrankster(hand[i].pranksterType, hand[i].tier);
            Debug.Log("HAND DISPLAY | slot=" + i +
                      " | type=" + hand[i].pranksterType +
                      " | tier=" + hand[i].tier);
            CreateCard(art, hand[i].tier, new Vector3(x, yOffset, 0), i);
        }
    }

    Sprite GetSpriteForPrankster(PranksterType type, int tier)
{
    switch (type)
    {
        case PranksterType.BeastMaster:
            switch (tier)
            {
                case 1: return beastMasterCrewLeaderSprite != null ? beastMasterCrewLeaderSprite : beastMasterSprite;
                case 2: return beastMasterExpertSprite != null ? beastMasterExpertSprite : beastMasterSprite;
                case 3: return beastMasterMasterSprite != null ? beastMasterMasterSprite : beastMasterSprite;
                default: return beastMasterSprite;
            }

        case PranksterType.Engineer:
            switch (tier)
            {
                case 1: return engineerCrewLeaderSprite != null ? engineerCrewLeaderSprite : engineerSprite;
                case 2: return engineerExpertSprite != null ? engineerExpertSprite : engineerSprite;
                case 3: return engineerMasterSprite != null ? engineerMasterSprite : engineerSprite;
                default: return engineerSprite;
            }

        case PranksterType.Laborer:
            switch (tier)
            {
                case 1: return laborerCrewLeaderSprite != null ? laborerCrewLeaderSprite : laborerSprite;
                case 2: return laborerExpertSprite != null ? laborerExpertSprite : laborerSprite;
                case 3: return laborerMasterSprite != null ? laborerMasterSprite : laborerSprite;
                default: return laborerSprite;
            }

        case PranksterType.Scribe:
            switch (tier)
            {
                case 1: return scribeCrewLeaderSprite != null ? scribeCrewLeaderSprite : scribeSprite;
                case 2: return scribeExpertSprite != null ? scribeExpertSprite : scribeSprite;
                case 3: return scribeMasterSprite != null ? scribeMasterSprite : scribeSprite;
                default: return scribeSprite;
            }

        case PranksterType.Thief:
            switch (tier)
            {
                case 1: return thiefCrewLeaderSprite != null ? thiefCrewLeaderSprite : thiefSprite;
                case 2: return thiefExpertSprite != null ? thiefExpertSprite : thiefSprite;
                case 3: return thiefMasterSprite != null ? thiefMasterSprite : thiefSprite;
                default: return thiefSprite;
            }

        case PranksterType.Wizard:
            switch (tier)
            {
                case 1: return wizardCrewLeaderSprite != null ? wizardCrewLeaderSprite : wizardSprite;
                case 2: return wizardExpertSprite != null ? wizardExpertSprite : wizardSprite;
                case 3: return wizardMasterSprite != null ? wizardMasterSprite : wizardSprite;
                default: return wizardSprite;
            }

        default:
            return null;
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