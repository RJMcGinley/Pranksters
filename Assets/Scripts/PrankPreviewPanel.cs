using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PrankPreviewPanel : MonoBehaviour
{
    [Header("References")]
    public GameObject rootObject;
    public Image previewImage;
    public DeckManager deckManager;
    public GameObject previewHighlight;
    public PopupArm popupArm;

    [Header("Timing")]
    public float hideDelay = 0.25f;

    private bool isSourceHovered = false;
    private bool isPanelHovered = false;
    private bool isVisible = false;
    private bool currentCanComplete = false;
    private int currentPrankIndex = -1;

    private Coroutine hideCoroutine;

    public int CurrentPrankIndex => currentPrankIndex;
    public bool CurrentCanComplete => currentCanComplete;
    public bool IsPanelHovered => isPanelHovered;
    public bool IsSourceHovered => isSourceHovered;

    private bool IsSamePreview(Sprite sprite, int prankIndex, bool canComplete)
    {
        return isVisible &&
               currentPrankIndex == prankIndex &&
               currentCanComplete == canComplete &&
               previewImage != null &&
               previewImage.sprite == sprite;
    }

    public void ShowFromSource(Sprite sprite, int prankIndex, bool canComplete)
    {
        if (sprite == null)
            return;

        bool samePrankAlreadyShowing = IsSamePreview(sprite, prankIndex, canComplete);

        currentPrankIndex = prankIndex;
        currentCanComplete = canComplete;
        isSourceHovered = true;

        if (previewImage != null)
            previewImage.sprite = sprite;

        CancelPendingHide();

        if (!isVisible)
        {
            if (deckManager != null)
                deckManager.PushHighlightSuppression();

            if (rootObject != null)
                rootObject.SetActive(true);
            else
                gameObject.SetActive(true);

            isVisible = true;

            RefreshCompletableVisuals();
            return;
        }

        if (!samePrankAlreadyShowing)
            RefreshCompletableVisuals();
    }

    public void NotifySourceExit(int prankIndex)
    {
        if (prankIndex != currentPrankIndex)
            return;

        isSourceHovered = false;

        if (!isPanelHovered)
            TryHideOrStayOpen();
    }

    public void NotifyPanelEnter()
    {
        isPanelHovered = true;
        CancelPendingHide();
    }

    public void NotifyPanelExit()
    {
        isPanelHovered = false;

        if (!isSourceHovered)
            TryHideOrStayOpen();
    }

    public void OnPanelClicked()
    {
        if (!isVisible)
            return;

        if (!currentCanComplete)
            return;

        if (deckManager == null)
            return;

        deckManager.OnPrankCardClicked(currentPrankIndex);
    }

    private void TryHideOrStayOpen()
    {
        CancelPendingHide();

        if (!currentCanComplete)
        {
            HideImmediate();
            return;
        }

        hideCoroutine = StartCoroutine(HideIfNoHoverAfterDelay());
    }

    private IEnumerator HideIfNoHoverAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);

        if (!isSourceHovered && !isPanelHovered)
            HideImmediate();

        hideCoroutine = null;
    }

    private void HideImmediate()
    {
        if (!isVisible)
            return;

        if (previewHighlight != null)
            previewHighlight.SetActive(false);

        if (popupArm != null)
            popupArm.Hide();

        if (deckManager != null)
            deckManager.PopHighlightSuppression();

        if (rootObject != null)
            rootObject.SetActive(false);
        else
            gameObject.SetActive(false);

        if (deckManager != null)
            deckManager.SetAllPrankHighlightsVisible(true);

        isVisible = false;
        currentPrankIndex = -1;
        currentCanComplete = false;
        isSourceHovered = false;
        isPanelHovered = false;
    }

    private void RefreshCompletableVisuals()
    {
        if (previewHighlight != null)
            previewHighlight.SetActive(currentCanComplete);

        if (popupArm != null)
        {
            if (currentCanComplete)
                popupArm.ReplayDrop();
            else
                popupArm.Hide();
        }
    }

    private void CancelPendingHide()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
    }

    public void Hide()
    {
        HideImmediate();
    }

    public bool IsVisible()
    {
        return isVisible;
    }
}