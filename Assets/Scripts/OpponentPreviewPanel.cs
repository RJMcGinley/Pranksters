using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OpponentPreviewPanel : MonoBehaviour
{
    [Header("Root")]
    public GameObject rootObject;

    [Header("Swap Preview Highlight")]
    public GameObject previewPanelHighlight;

    [Header("Text")]
    public TextMeshProUGUI opponentLabelText;
    public TextMeshProUGUI pranksCompletedText;
    public TextMeshProUGUI renownPointsText;
    public TextMeshProUGUI favorPointsText;

    [Header("Favor Slots")]
    public Image favorSlot1Image;
    public Image favorSlot2Image;
    public Image favorSlot3Image;

    [Header("Prankster Icon Sprites")]
    public Sprite thiefIcon;
    public Sprite engineerIcon;
    public Sprite laborerIcon;
    public Sprite scribeIcon;
    public Sprite wizardIcon;
    public Sprite beastmasterIcon;

    public DeckManager deckManager;

    bool isLockedForSwap = false;

    [Header("Popup Arm")]
    public PopupArm popupArm;

    public void ShowFromPlayerInfoPanel(PlayerInfoPanel sourcePanel)
    {
        if (previewPanelHighlight != null && !isLockedForSwap)
            previewPanelHighlight.SetActive(false);

        if (sourcePanel == null)
            return;

        Debug.Log("ShowFromPlayerInfoPanel called");

        if (opponentLabelText != null && sourcePanel.playerLabelText != null)
            opponentLabelText.text = sourcePanel.playerLabelText.text;

        if (pranksCompletedText != null && sourcePanel.pranksCompletedText != null)
            pranksCompletedText.text = sourcePanel.pranksCompletedText.text;

        if (renownPointsText != null && sourcePanel.renownPointsText != null)
            renownPointsText.text = sourcePanel.renownPointsText.text;

        if (favorPointsText != null && sourcePanel.favorPointsText != null)
            favorPointsText.text = sourcePanel.favorPointsText.text;

        CopyFavorSlot(sourcePanel.favorSlot1Image, favorSlot1Image);
        CopyFavorSlot(sourcePanel.favorSlot2Image, favorSlot2Image);
        CopyFavorSlot(sourcePanel.favorSlot3Image, favorSlot3Image);

        if (deckManager != null)
        {
            deckManager.PushHighlightSuppression();
            deckManager.SetAllPrankHighlightsVisible(false);
        }
        else
        {
            Debug.Log("DeckManager is NULL in OpponentPreviewPanel");
        }

        if (rootObject != null)
            rootObject.SetActive(true);
        else
            gameObject.SetActive(true);

        if (popupArm != null)
        {
            int opponentIndex = sourcePanel.representedPlayerIndex;

            Debug.Log("representedPlayerIndex = " + opponentIndex);

            if (deckManager != null)
            {
                bool canSwap = deckManager.CanSwapWithOpponent(opponentIndex);
                Debug.Log("CanSwapWithOpponent = " + canSwap);

                if (canSwap && !isLockedForSwap)
                {
                    Debug.Log("VALID SWAP → ReplayDrop + Play Sound");

                    popupArm.gameObject.SetActive(true);
                    popupArm.ReplayDrop();

                    if (AudioManager.Instance != null)
                    {
                        Debug.Log("AudioManager FOUND → Playing discard hover sound");
                        AudioManager.Instance.PlayDiscardPileHover();
                    }
                    else
                    {
                        Debug.Log("AudioManager INSTANCE is NULL");
                    }
                }
                else
                {
                    Debug.Log("SWAP NOT VALID OR LOCKED → Hiding popup arm");
                    popupArm.Hide();
                }
            }
        }
        else
        {
            Debug.Log("PopupArm is NULL");
        }
    }

    public void Hide()
    {
        if (popupArm != null)
            popupArm.gameObject.SetActive(false);

        if (previewPanelHighlight != null)
            previewPanelHighlight.SetActive(false);

        isLockedForSwap = false;

        if (deckManager != null)
            deckManager.PopHighlightSuppression();

        if (rootObject != null)
            rootObject.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    public bool IsVisible()
    {
        if (rootObject != null)
            return rootObject.activeSelf;

        return gameObject.activeSelf;
    }

    void CopyFavorSlot(Image source, Image target)
    {
        if (target == null)
            return;

        if (source != null && source.gameObject.activeSelf && source.sprite != null)
        {
            target.gameObject.SetActive(true);
            target.sprite = source.sprite;
        }
        else
        {
            target.gameObject.SetActive(false);
        }
    }

    public void ShowPreviewPanelHighlight()
    {
        if (previewPanelHighlight != null)
            previewPanelHighlight.SetActive(true);
    }

    public void HidePreviewPanelHighlight()
    {
        if (previewPanelHighlight != null)
            previewPanelHighlight.SetActive(false);
    }

    public void LockForSwap()
    {
        isLockedForSwap = true;

        if (previewPanelHighlight != null)
            previewPanelHighlight.SetActive(true);

        if (popupArm != null)
            popupArm.Hide();
    }

    public void UnlockSwap()
    {
        isLockedForSwap = false;

        if (previewPanelHighlight != null)
            previewPanelHighlight.SetActive(false);
    }

    public bool IsLockedForSwap()
    {
        return isLockedForSwap;
    }
}