using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LifetimeNotorietyPesterMayorButton : MonoBehaviour
{
    [SerializeField] private GameObject buttonRoot;
    [SerializeField] private Button button;
    [SerializeField] private DeckManager deckManager;

    [SerializeField] private TMP_Text influenceCostText;
    [SerializeField] private TMP_Text mischiefGainedText;

    private void Start()
    {
        if (buttonRoot != null)
            buttonRoot.SetActive(false);
    }

    public int GetCurrentCost()
    {
        if (deckManager == null)
            return 0;

        return deckManager.GetPesterMayorCurrentCost();
    }

    public int GetCurrentMischiefGain()
    {
        if (deckManager == null)
            return 0;

        return deckManager.GetPesterMayorCurrentMischiefGain();
    }

    public void Refresh()
    {
        if (buttonRoot == null)
            return;

        bool unlocked = SaveSystem.HasPesterMayorUnlock();

        buttonRoot.SetActive(unlocked);

        int cost = GetCurrentCost();
        int gain = GetCurrentMischiefGain();

        if (influenceCostText != null)
            influenceCostText.text = "-" + cost;

        if (mischiefGainedText != null)
            mischiefGainedText.text = "+" + gain;

        bool canUse = false;

        if (unlocked && deckManager != null)
        {
            Player player = deckManager.GetCurrentPlayerForUI();

            if (player != null)
            {
                canUse =
                    !player.isBot &&
                    !deckManager.HasCurrentPlayerTakenActionThisTurn() &&
                    deckManager.CanCurrentPlayerPesterMayor();
            }
        }

        if (button != null)
            button.interactable = canUse;
    }

    public void OnClick()
    {
        if (deckManager == null)
        {
            Debug.LogWarning("LifetimeNotorietyPesterMayorButton: deckManager is not assigned.");
            return;
        }

        deckManager.TryPesterMayor();

        Refresh();
    }
}