using UnityEngine;

public class DiscardPileHover : MonoBehaviour
{
    public DeckManager deckManager;
    public PopupArm popupArm;

    void OnMouseEnter()
    {
        if (deckManager == null || !deckManager.CanHoverDiscardPile())
            return;

        Debug.Log("DISCARD HOVER ENTER");

        if (popupArm != null)
            popupArm.Show();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDiscardPileHover();
    }

    void OnMouseExit()
    {
        Debug.Log("DISCARD HOVER EXIT");

        if (popupArm != null)
            popupArm.Hide();
    }

    void OnMouseDown()
    {
        Debug.Log("DISCARD CLICKED");

        if (deckManager != null)
            deckManager.OnDiscardPileClicked();
    }
}