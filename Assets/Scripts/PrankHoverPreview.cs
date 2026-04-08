using UnityEngine;

public class PrankHoverPreview : MonoBehaviour
{
    public Sprite previewSprite;
    public PrankPreviewPanel previewPanel;
    public DeckManager deckManager;
    public NextPlayerPanelController nextPlayerPanelController;
    public int prankIndex;

    private GameObject prankHighlight;

    void Start()
    {
        prankHighlight = transform.Find("FX_CardBrushLine_G(Clone)")?.gameObject;
    }

    public void CacheHighlightReference()
    {
        if (prankHighlight == null)
            prankHighlight = transform.Find("FX_CardBrushLine_G(Clone)")?.gameObject;
    }

    public void SetGlow(bool shouldGlow)
    {
        CacheHighlightReference();

        if (prankHighlight != null)
            prankHighlight.SetActive(shouldGlow);
    }

    bool IsHoverBlocked()
    {
        if (deckManager != null && deckManager.IsSwapFlowActive())
            return true;

        if (nextPlayerPanelController != null && nextPlayerPanelController.IsPanelBlockingInteraction())
            return true;

        return false;
    }

    void OnMouseEnter()
    {
        if (IsHoverBlocked())
            return;

        bool canComplete = deckManager != null && deckManager.CanCompletePrank(prankIndex);

        if (deckManager != null)
        {
            deckManager.hoveredPrankIndex = prankIndex;
            deckManager.SetAllPrankHighlightsVisible(false);
        }

        CacheHighlightReference();

        if (prankHighlight != null)
            prankHighlight.SetActive(false);

        if (previewPanel != null)
            previewPanel.ShowFromSource(previewSprite, prankIndex, canComplete);
    }

    void OnMouseExit()
    {
        if (IsHoverBlocked())
            return;

        if (deckManager != null && deckManager.hoveredPrankIndex == prankIndex)
            deckManager.hoveredPrankIndex = -1;

        if (previewPanel != null)
            previewPanel.NotifySourceExit(prankIndex);

        if (deckManager != null && (previewPanel == null || !previewPanel.IsVisible()))
            deckManager.SetAllPrankHighlightsVisible(true);
    }
}