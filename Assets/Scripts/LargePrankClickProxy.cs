using UnityEngine;

public class LargePrankClickProxy : MonoBehaviour
{
    public PrankPreviewPanel previewPanel;
    public DeckManager deckManager;
    private Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (previewPanel == null || col == null)
            return;

        // Only enable clicking when preview is visible AND completable
        bool shouldEnable = previewPanel.IsVisible() && previewPanel.CurrentCanComplete;

        col.enabled = shouldEnable;
    }

    void OnMouseDown()
    {
        if (previewPanel == null || deckManager == null)
            return;

        if (!previewPanel.IsVisible())
            return;

        if (!previewPanel.CurrentCanComplete)
            return;

        int prankIndex = previewPanel.CurrentPrankIndex;

        if (prankIndex < 0)
            return;

        Debug.Log("CLICK PROXY TRIGGERED → Completing prank index: " + prankIndex);

        deckManager.OnPrankCardClicked(prankIndex);
    }
}