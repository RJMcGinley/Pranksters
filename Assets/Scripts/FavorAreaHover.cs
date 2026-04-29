using UnityEngine;

public class FavorAreaHover : MonoBehaviour
{
    public DeckManager deckManager;
    public GameObject helperObject;

    private bool isShowing = false;

    void Start()
    {
        isShowing = false;

        if (helperObject != null)
            helperObject.SetActive(false);
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

        deckManager.OnFavorAreaClicked();
    }

    private void HideHelper()
    {
        isShowing = false;

        if (helperObject != null)
            helperObject.SetActive(false);
    }
}