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

    public void ShowCurrentPlayerHand()
    {
        foreach (Transform child in currentPlayerHandArea)
        {
            Destroy(child.gameObject);
        }

        Player currentPlayer = deckManager.turnManager.GetCurrentPlayer();
        List<PranksterType> hand = currentPlayer.hand;

        bool choosingFavor = deckManager != null && deckManager.IsChoosingFavor();

        float spacing = choosingFavor ? 2.4f : 1.9f;
        float yOffset = choosingFavor ? 0.2f : 0f;

        int count = hand.Count;
        float startX = -(count - 1) * spacing / 2f;

        for (int i = 0; i < count; i++)
        {
            float x = startX + i * spacing;
            Sprite art = GetSpriteForPrankster(hand[i]);
            CreateCard(art, new Vector3(x, yOffset, 0), i);
        }
    }

    Sprite GetSpriteForPrankster(PranksterType type)
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

    void CreateCard(Sprite art, Vector3 localPosition, int index)
{
    GameObject card = Instantiate(pranksterCardPrefab, currentPlayerHandArea);
    card.transform.localPosition = localPosition;

    if (deckManager != null && deckManager.IsChoosingFavor())
    {
        card.transform.localScale = new Vector3(1.15f, 1.15f, 1f);
    }
    else
    {
        card.transform.localScale = Vector3.one;
    }

    PranksterCardView cardView = card.GetComponent<PranksterCardView>();
    if (cardView != null)
        cardView.SetArt(art);

    HandCardClick click = card.GetComponent<HandCardClick>();
    if (click != null)
    {
        click.deckManager = deckManager;
        click.cardIndex = index;
        click.SetBaseScale(card.transform.localScale);

        if (deckManager.IsInDiscardSelection() && cardView != null && cardView.characterArtRenderer != null)
        {
            cardView.characterArtRenderer.color = Color.red;
        }
}
}
}