using UnityEngine;

public class DrawDeckHover : MonoBehaviour
{
    public PopupArm popupArm;
    public DeckManager deckManager;

    void OnMouseEnter()
    {
        if (deckManager != null && deckManager.IsPrankPreviewOpen())
            return;

        if (deckManager == null || !deckManager.CanHoverDrawDeck())
            return;

        if (popupArm != null)
            popupArm.Show();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDrawDeckHover();
    }

    void OnMouseExit()
    {
        if (popupArm != null)
            popupArm.Hide();
    }

    void OnMouseDown()
    {
        Debug.Log("DRAW DECK CLICKED");

        if (deckManager != null)
            deckManager.OnDrawDeckClicked();
    }
}