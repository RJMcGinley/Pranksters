using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LifetimeNotorietyDistractBarnabyButton : MonoBehaviour
{
    [SerializeField] private GameObject buttonRoot;
    [SerializeField] private Button button;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private TMP_Text costText;

    private void Start()
    {
        if (buttonRoot != null)
            buttonRoot.SetActive(false);
    }

    public int GetCurrentCost()
    {
        if (deckManager == null)
            return 0;

        return deckManager.GetDistractBarnabyCurrentCost();
    }

    public void Refresh()
    {
        if (buttonRoot == null)
            return;

        bool unlocked = SaveSystem.HasDistractBarnabyUnlock();

        buttonRoot.SetActive(unlocked);

        int cost = GetCurrentCost();

        if (costText != null)
            costText.text = "-" + cost;

        bool canUse = false;

        if (unlocked && deckManager != null)
            canUse = deckManager.CanCurrentPlayerDistractBarnaby();

        if (button != null)
            button.interactable = canUse;
    }

    public void OnClick()
    {
        if (deckManager == null)
            return;

        deckManager.TryDistractBarnaby();

        Refresh();
    }
}