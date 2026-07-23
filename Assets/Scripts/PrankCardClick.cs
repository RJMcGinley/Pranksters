using UnityEngine;

public class PrankCardClick : MonoBehaviour
{
    public DeckManager deckManager;
    public int prankIndex;

    void OnMouseDown()
    {
        if (TutorialController.Instance != null &&
            TutorialController.Instance.IsTutorialOpen)
        {
            return;
        }

        if (deckManager == null)
            return;

        if (!deckManager.IsChoosingEngineerPrankReplacement() &&
            !deckManager.CanCompletePrank(prankIndex))
        {
            return;
        }

        deckManager.OnPrankCardClicked(prankIndex);
    }
}