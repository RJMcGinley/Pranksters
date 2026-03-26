using UnityEngine;

public class HandCardClick : MonoBehaviour
{
    public DeckManager deckManager;
    public int cardIndex;

    void OnMouseDown()
    {
        if (deckManager == null)
            return;

        deckManager.OnHandCardClicked(cardIndex);
    }
}