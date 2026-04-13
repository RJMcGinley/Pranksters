using UnityEngine;

public class OpponentPanelHoverPreview : MonoBehaviour
{
    public PlayerInfoPanel sourcePanel;
    public OpponentPreviewPanel previewPanel;
    public DeckManager deckManager;

    [Header("Popup Arm")]
    public PopupArm popupArm;

    string GetPath()
    {
        string path = gameObject.name;
        Transform current = transform.parent;

        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }

        return path;
    }

    void OnMouseEnter()
    {
        if (deckManager != null && deckManager.IsInteractionBlocked())
            return;

        Debug.Log("OPPONENT HOVER ENTER on: " + GetPath());

        if (previewPanel != null && sourcePanel != null)
            previewPanel.ShowFromPlayerInfoPanel(sourcePanel);

        if (popupArm != null && (previewPanel == null || !previewPanel.IsLockedForSwap()))
            popupArm.Show();
    }

    void OnMouseExit()
    {
        Debug.Log("OPPONENT HOVER EXIT on: " + GetPath());

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
        if (deckManager != null && deckManager.IsInteractionBlocked())
            return;
    }
}