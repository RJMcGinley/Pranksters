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
}

void OnMouseExit()
{
    Debug.Log("CLICK PROXY HOVER EXIT");
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