using UnityEngine;
using TMPro;

public class HandCardClick : MonoBehaviour
{
    public DeckManager deckManager;
    public int cardIndex;

    private SpriteRenderer targetRenderer;
    private Vector3 originalScale;

    private readonly Color normalColor = Color.white;
    private readonly Color discardSelectableColor = Color.white;
    private readonly Color hoverColor = Color.white;

    void Awake()
    {
        PranksterCardView cardView = GetComponent<PranksterCardView>();
        if (cardView != null)
            targetRenderer = cardView.characterArtRenderer;

        originalScale = transform.localScale;
    }

    void OnMouseEnter()
    {
        if (deckManager == null)
            return;

        if (!deckManager.IsInDiscardSelection() &&
            !deckManager.IsChoosingFavor() &&
            !deckManager.IsInSwapHandSelection())
            return;

        transform.localScale = originalScale * 1.1f;

        if (targetRenderer != null)
            targetRenderer.color = hoverColor;

        if (deckManager.IsChoosingFavor())
        {
            Player player = deckManager.turnManager.GetCurrentPlayer();

            if (cardIndex < 0 || cardIndex >= player.hand.Count)
                return;

            PranksterDeckEntry card = player.hand[cardIndex];

            int favorValue = deckManager.CalculateTotalFavorForCard(card);

            if (deckManager.favorPreviewText != null)
            {
                int index = deckManager.GetNextAvailableFavorIndex();
                Vector3 pos = deckManager.GetFavorWellPosition(index);

                deckManager.favorPreviewText.text = favorValue.ToString();
                deckManager.favorPreviewText.transform.position = pos;
                deckManager.favorPreviewText.gameObject.SetActive(true);
            }
        }
    }

    void OnMouseExit()
    {
        transform.localScale = originalScale;

        if (targetRenderer != null)
        {
            if (deckManager != null && deckManager.IsInDiscardSelection())
                targetRenderer.color = discardSelectableColor;
            else
                targetRenderer.color = normalColor;
        }

        if (deckManager != null && deckManager.favorPreviewText != null)
        {
            deckManager.favorPreviewText.gameObject.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        if (deckManager == null)
            return;

        if (!deckManager.IsInDiscardSelection() &&
            !deckManager.IsChoosingFavor() &&
            !deckManager.IsInSwapHandSelection())
            return;

        deckManager.OnHandCardClicked(cardIndex);
    }

    public void RefreshVisualState()
    {
        Debug.Log("RefreshVisualState called for card index " + cardIndex);

        if (targetRenderer == null)
            return;

        if (deckManager != null && deckManager.IsInDiscardSelection())
            targetRenderer.color = new Color(50f, 50f, 50f);
        else
            targetRenderer.color = Color.white;
    }

    public void SetBaseScale(Vector3 scale)
    {
        originalScale = scale;
        transform.localScale = scale;
    }
}