using UnityEngine;

public class OpponentPanelHoverPreview : MonoBehaviour
{
    public PlayerInfoPanel sourcePanel;
    public OpponentPreviewPanel previewPanel;
    public DeckManager deckManager;

    [Header("Popup Arm")]
    public PopupArm popupArm;

    void OnMouseEnter()
    {
        Debug.Log("OPPONENT HOVER ENTER");

        if (previewPanel != null && sourcePanel != null)
            previewPanel.ShowFromPlayerInfoPanel(sourcePanel);

        if (popupArm != null)
            popupArm.Show();
    }

    void OnMouseExit()
    {
        Debug.Log("OPPONENT HOVER EXIT");

        if (previewPanel != null)
            previewPanel.Hide();

        if (popupArm != null)
            popupArm.Hide();

        if (deckManager != null && (previewPanel == null || !previewPanel.IsVisible()))
        {
            deckManager.SetAllPrankHighlightsVisible(true);
            deckManager.RefreshAllHighlights();
        }
    }
}