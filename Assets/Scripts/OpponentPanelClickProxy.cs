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

        if (deckManager == null)
            return;

        // During swap-hand selection, do not show opponent hover preview.
        // The player should only be choosing a hand card or clicking the panel to cancel.
        if (deckManager.pendingChoice == PendingChoiceType.ChooseSwapHandCard)
            return;

        if (previewPanel != null && sourcePanel != null)
            previewPanel.ShowFromPlayerInfoPanel(sourcePanel);

        if (popupArm != null && (previewPanel == null || !previewPanel.IsLockedForSwap()))
            popupArm.Show();
    }

    void OnMouseExit()
    {
        Debug.Log("CLICK PROXY HOVER EXIT");

        if (deckManager == null)
            return;

        // During swap-hand selection, hover preview should stay disabled,
        // so there is nothing to hide here.
        if (deckManager.pendingChoice == PendingChoiceType.ChooseSwapHandCard)
            return;

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
        Debug.Log("CLICK PROXY MOUSEDOWN");

        if (sourcePanel == null || previewPanel == null || deckManager == null)
            return;

        int opponentIndex = sourcePanel.representedPlayerIndex;

        Debug.Log("CLICK PROXY HIT");
        Debug.Log("CLICK PROXY HIT | pendingChoice = " + deckManager.pendingChoice +
                  " | swapFlow = " + deckManager.IsSwapFlowActive() +
                  " | canSwap = " + deckManager.CanSwapWithOpponent(opponentIndex));

        // Cancel swap by clicking opponent panel during any swap state
        if (deckManager.IsSwapFlowActive())
        {
            Debug.Log("CANCEL SWAP via panel click");

            deckManager.CancelSwapPreview();
            return;
        }

        // Start swap flow
        if (!deckManager.CanSwapWithOpponent(opponentIndex))
            return;

        deckManager.StartSwapFavorTurn();

        previewPanel.LockForSwap();
        previewPanel.ShowFromPlayerInfoPanel(sourcePanel);

        deckManager.ResolveSwapOpponentChoice(opponentIndex);

        if (popupArm != null)
            popupArm.Hide();

        deckManager.RefreshAllDisplays();
        deckManager.RefreshAllHighlights();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDiscardPileHover();
    }

    void OnMouseUp()
    {
        Debug.Log("CLICK PROXY MOUSEUP");
    }
}