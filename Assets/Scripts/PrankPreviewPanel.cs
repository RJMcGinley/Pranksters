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

    public void ShowFromSource(Sprite sprite, int prankIndex, bool canComplete)
    {
        if (sprite == null)
            return;

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
        }

        RefreshCompletableVisuals();
    }

    public void NotifySourceExit(int prankIndex)
{
    if (prankIndex != currentPrankIndex)
        return;

    Debug.Log("SOURCE EXIT for prank " + prankIndex);

    isSourceHovered = false;

    if (!isPanelHovered)
        TryHideOrStayOpen();
}

public void NotifyPanelEnter()
{
    Debug.Log("PANEL ENTER");
    isPanelHovered = true;
    CancelPendingHide();
}

public void NotifyPanelExit()
{
    Debug.Log("PANEL EXIT");
    isPanelHovered = false;

    if (!isSourceHovered)
        TryHideOrStayOpen();
}

private IEnumerator HideIfNoHoverAfterDelay()
{
    Debug.Log("HIDE TIMER STARTED");

    yield return new WaitForSeconds(hideDelay);

    Debug.Log("HIDE TIMER CHECK | sourceHovered=" + isSourceHovered + " | panelHovered=" + isPanelHovered);

    if (!isSourceHovered && !isPanelHovered)
    {
        Debug.Log("HIDING PREVIEW PANEL");
        HideImmediate();
    }

    hideCoroutine = null;
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
        hideCoroutine = StartCoroutine(HideIfNoHoverAfterDelay());
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