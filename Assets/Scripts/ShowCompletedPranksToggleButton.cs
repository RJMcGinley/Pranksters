using UnityEngine;

public class ShowCompletedPranksToggleButton : MonoBehaviour
{
    public GameObject checkmarkObject;
    public DeckManager deckManager;

    void Start()
    {
        if (deckManager != null && checkmarkObject != null)
            checkmarkObject.SetActive(deckManager.IsPrankCompletionShowcaseEnabled());
    }

    public void ToggleShowCompletedPranks()
    {
        if (deckManager == null)
            return;

        bool newState = !deckManager.IsPrankCompletionShowcaseEnabled();

        deckManager.SetPrankCompletionShowcaseEnabled(newState);

        if (checkmarkObject != null)
            checkmarkObject.SetActive(newState);
    }
}