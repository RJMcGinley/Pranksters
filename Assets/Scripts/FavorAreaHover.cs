using UnityEngine;

public class FavorAreaHover : MonoBehaviour
{
    public DeckManager deckManager;
    public GameObject helperObject;
    public SendRecruitToBarnabyHelper popupHelper;

    private bool isShowing = false;

    void Start()
    {
        isShowing = false;

        if (helperObject != null)
            helperObject.SetActive(false);

        if (popupHelper == null && helperObject != null)
            popupHelper = helperObject.GetComponent<SendRecruitToBarnabyHelper>();
    }

    void OnMouseEnter()
    {
        if (deckManager != null && deckManager.IsInteractionBlocked())
            return;

        if (deckManager != null && deckManager.IsPrankPreviewOpen())
            return;

        if (deckManager == null || !deckManager.CanHoverFavorArea() || isShowing)
            return;

        isShowing = true;

        if (helperObject != null)
            helperObject.SetActive(true);

        if (popupHelper != null)
            popupHelper.Show();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayFavorHover();
    }

    void OnMouseExit()
    {
        HideHelper();
    }

    void Update()
    {
        if (isShowing &&
            (deckManager == null || deckManager.IsInteractionBlocked() || !deckManager.CanHoverFavorArea()))
        {
            HideHelper();
        }
    }

    void OnMouseDown()
    {
        if (deckManager == null)
            return;

        if (deckManager.IsInteractionBlocked())
            return;

        HideHelper();

        deckManager.OnFavorAreaClicked(-1);
    }

    private void HideHelper()
{
    isShowing = false;

    if (popupHelper != null)
        popupHelper.Hide();

    // Do NOT turn helperObject off here.
    // It has to stay active so SendRecruitToBarnabyHelper.Update()
    // can animate it back to hiddenX.
}
}