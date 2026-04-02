using UnityEngine;

public class OpponentPanelClickProxy : MonoBehaviour
{
    public PlayerInfoPanel sourcePanel;
    public OpponentPreviewPanel previewPanel;
    public DeckManager deckManager;

    [Header("Popup Arm")]
    public PopupArm popupArm;

    void OnMouseEnter()
    {
        Debug.Log("CLICK PROXY HOVER ENTER");

        if (previewPanel != null && sourcePanel != null)
            previewPanel.ShowFromPlayerInfoPanel(sourcePanel);

        if (popupArm != null && (previewPanel == null || !previewPanel.IsLockedForSwap()))
            popupArm.Show();
    }

    void OnMouseExit()
    {
        Debug.Log("CLICK PROXY HOVER EXIT");

        if (previewPanel != null && previewPanel.IsLockedForSwap())
            return;

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

    void OnMouseDown()
    {
        Debug.Log("CLICK PROXY HIT");

        if (sourcePanel == null || previewPanel == null || deckManager == null)
            return;

        int opponentIndex = sourcePanel.representedPlayerIndex;

        if (!deckManager.CanSwapWithOpponent(opponentIndex))
            return;

        Debug.Log("VALID CLICK → LOCK PREVIEW");

        previewPanel.LockForSwap();
        previewPanel.ShowFromPlayerInfoPanel(sourcePanel);

        if (popupArm != null)
            popupArm.Hide();

        // This is the important change:
        deckManager.RefreshAllDisplays();
        deckManager.RefreshAllHighlights();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDiscardPileHover();
    }
}