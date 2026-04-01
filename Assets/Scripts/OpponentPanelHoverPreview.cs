using UnityEngine;

public class OpponentPanelHoverPreview : MonoBehaviour
{
    public PlayerInfoPanel sourcePanel;
    public OpponentPreviewPanel previewPanel;
    public DeckManager deckManager;

    void OnMouseEnter()
    {
        Debug.Log("OPPONENT HOVER ENTER");

        if (previewPanel != null && sourcePanel != null)
            previewPanel.ShowFromPlayerInfoPanel(sourcePanel);
    }

    void OnMouseExit()
    {
        Debug.Log("OPPONENT HOVER EXIT");

        if (previewPanel != null)
            previewPanel.Hide();

        if (deckManager != null && (previewPanel == null || !previewPanel.IsVisible()))
        {
            deckManager.SetAllPrankHighlightsVisible(true);
            deckManager.RefreshAllHighlights();
        }
    }
}