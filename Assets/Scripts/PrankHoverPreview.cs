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

        if (deckManager != null)
        {
            deckManager.hoveredPrankIndex = prankIndex;
            deckManager.SetAllPrankHighlightsVisible(false);
        }

        if (prankHighlight == null)
            prankHighlight = transform.Find("FX_CardBrushLine_G(Clone)")?.gameObject;

        if (prankHighlight != null)
            prankHighlight.SetActive(false);

        previewPanel.Show(previewSprite);
    }

    void OnMouseExit()
    {
        if (deckManager != null && deckManager.hoveredPrankIndex == prankIndex)
            deckManager.hoveredPrankIndex = -1;

        if (prankHighlight == null)
            prankHighlight = transform.Find("FX_CardBrushLine_G(Clone)")?.gameObject;

        if (deckManager != null)
            deckManager.SetAllPrankHighlightsVisible(true);

        if (previewPanel != null)
            previewPanel.Hide();
    }
}