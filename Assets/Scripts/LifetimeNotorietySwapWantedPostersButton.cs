using UnityEngine;
using UnityEngine.UI;

public class LifetimeNotorietySwapWantedPostersButton : MonoBehaviour
{
    [SerializeField] private GameObject buttonRoot;
    [SerializeField] private Button button;
    [SerializeField] private DeckManager deckManager;

    private void Start()
    {
        if (buttonRoot != null)
            buttonRoot.SetActive(false);
    }

    public void Refresh()
    {
        if (buttonRoot == null)
            return;

        bool unlocked = SaveSystem.HasSwapWantedPostersUnlock();

        buttonRoot.SetActive(unlocked);

        bool canUse = false;

        if (unlocked && deckManager != null)
            canUse = deckManager.CanCurrentPlayerStartSwapWantedPostersAction();

        if (button != null)
            button.interactable = canUse;
    }

    public void OnClick()
    {
        if (deckManager == null)
            return;

        deckManager.TryStartSwapWantedPostersAction();

        Refresh();
    }

    public string GetCurrentCostText()
    {
        return "2 Cards";
    }
}