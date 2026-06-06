using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LifetimeNotorietyCrewSizeButton : MonoBehaviour
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
        return SaveSystem.GetLifetimeCrewSizeUpgradeCost();
    }

    public void Refresh()
    {
        if (buttonRoot == null)
            return;

        bool unlocked = SaveSystem.HasLifetimeCrewSizeUnlock();
        int cost = GetCurrentCost();

        if (costText != null)
            costText.text = "-" + cost;

        buttonRoot.SetActive(unlocked);

        bool canUse = false;

        if (unlocked && deckManager != null)
        {
            Player player = deckManager.GetCurrentPlayerForUI();

            if (player != null)
            {
                canUse =
                    !player.isBot &&
                    !deckManager.HasCurrentPlayerTakenActionThisTurn() &&
                    !player.lifetimeNotorietyCrewUpgradeUsedThisGame &&
                    player.favorPoints >= cost;
            }
        }

        if (button != null)
            button.interactable = canUse;
    }

    public void OnClick()
    {
        if (deckManager == null)
        {
            Debug.LogWarning("LifetimeNotorietyCrewSizeButton: deckManager is not assigned.");
            return;
        }

        deckManager.TryPurchaseLifetimeCrewSizeUpgrade();

        Refresh();
    }

}