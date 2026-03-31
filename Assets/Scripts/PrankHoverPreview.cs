using UnityEngine;

public class PrankHoverPreview : MonoBehaviour
{
    public Sprite previewSprite;
    public PrankPreviewPanel previewPanel;
    public DeckManager deckManager;
    public int prankIndex;

    private GameObject prankHighlight;

    void Start()
    {
        prankHighlight = transform.Find("FX_CardBrushLine_G(Clone)")?.gameObject;
    }

    void OnMouseEnter()
    {
        if (previewPanel == null)
        {
            Debug.LogWarning("PrankHoverPreview: previewPanel is not assigned on " + gameObject.name);
            return;
        }

        if (previewSprite == null)
        {
            Debug.LogWarning("PrankHoverPreview: previewSprite is not assigned on " + gameObject.name);
            return;
        }

        bool canComplete = deckManager != null && deckManager.CanCompletePrank(prankIndex);

        if (deckManager != null)
        {
            deckManager.hoveredPrankIndex = prankIndex;
            deckManager.SetAllPrankHighlightsVisible(false);
        }

        if (prankHighlight == null)
            prankHighlight = transform.Find("FX_CardBrushLine_G(Clone)")?.gameObject;

        if (prankHighlight != null)
            prankHighlight.SetActive(false);

        previewPanel.ShowFromSource(previewSprite, prankIndex, canComplete);
    }

    void OnMouseExit()
{
    if (deckManager != null && deckManager.hoveredPrankIndex == prankIndex)
        deckManager.hoveredPrankIndex = -1;

    if (previewPanel != null)
        previewPanel.NotifySourceExit(prankIndex);

    if (deckManager != null && (previewPanel == null || !previewPanel.IsVisible()))
        deckManager.SetAllPrankHighlightsVisible(true);
}
}