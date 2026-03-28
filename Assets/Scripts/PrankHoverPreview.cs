using UnityEngine;

public class PrankHoverPreview : MonoBehaviour
{
    public Sprite previewSprite;
    public PrankPreviewPanel previewPanel;
    public DeckManager deckManager;
    public int prankIndex;

    private GameObject previewHighlight;
    private GameObject prankHighlight;

    void Start()
{
    prankHighlight = transform.Find("FX_CardBrushLine_G(Clone)")?.gameObject;

    if (previewPanel != null)
    {
        previewHighlight = previewPanel.transform.Find("PreviewCompletableHighlight")?.gameObject;

        if (previewHighlight != null)
        {
            ParticleSystemRenderer[] renderers = previewHighlight.GetComponentsInChildren<ParticleSystemRenderer>(true);
            foreach (ParticleSystemRenderer r in renderers)
            {
                r.sortingLayerName = "Default";
                r.sortingOrder = 1000;
            }
        }
    }
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

        Debug.Log("HOVER ENTER → PrankIndex: " + prankIndex +
                  " | CanComplete: " + canComplete +
                  " | PreviewHighlight exists: " + (previewHighlight != null));

        if (previewHighlight != null && canComplete)
        {
            Debug.Log("→ Turning ON preview highlight");
            previewHighlight.SetActive(true);
        }
        else if (previewHighlight != null)
        {
            Debug.Log("→ Turning OFF preview highlight");
            previewHighlight.SetActive(false);
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
        Debug.Log("HOVER EXIT → PrankIndex: " + prankIndex);

        if (deckManager != null && deckManager.hoveredPrankIndex == prankIndex)
            deckManager.hoveredPrankIndex = -1;

        if (prankHighlight == null)
            prankHighlight = transform.Find("FX_CardBrushLine_G(Clone)")?.gameObject;

        if (deckManager != null)
            deckManager.SetAllPrankHighlightsVisible(true);

        if (previewHighlight != null)
        {
            Debug.Log("→ Turning OFF preview highlight (exit)");
            previewHighlight.SetActive(false);
        }

        if (previewPanel != null)
            previewPanel.Hide();
    }
}