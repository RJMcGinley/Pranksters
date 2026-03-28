using UnityEngine;

public class PrankCardClick : MonoBehaviour
{
    public DeckManager deckManager;
    public int prankIndex;

    void OnMouseDown()
    {
        if (deckManager == null)
            return;

        if (!deckManager.CanCompletePrank(prankIndex))
            return;

        deckManager.OnPrankCardClicked(prankIndex);
    }
}