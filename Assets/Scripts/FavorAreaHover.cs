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
        isShowing = false;

        if (helperObject != null)
            helperObject.SetActive(false);
    }

    void Update()
    {
        if (isShowing && (deckManager == null || !deckManager.CanHoverFavorArea()))
        {
            isShowing = false;

            if (helperObject != null)
                helperObject.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        if (deckManager == null)
            return;

        if (helperObject != null)
            helperObject.SetActive(false);

        isShowing = false;

        deckManager.OnFavorAreaClicked();
    }


}