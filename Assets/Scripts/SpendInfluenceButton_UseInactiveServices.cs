using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpendInfluenceButton_UseInactiveServices : MonoBehaviour
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

    public void Refresh()
    {
        if (buttonRoot == null)
            return;

        int cost = 7;

        if (deckManager != null)
            cost = deckManager.GetInactiveInfluenceServiceCost();

        if (costText != null)
            costText.text = "-" + cost;

        bool unlocked = SaveSystem.HasInactiveInfluenceServiceUnlock();

        buttonRoot.SetActive(unlocked);

        bool canUse = false;

        if (unlocked && deckManager != null)
        {
            Player player = deckManager.GetCurrentPlayerForUI();

            if (player != null)
            {
                canUse =
                    deckManager.CanCurrentPlayerUseSpendInfluenceActions() &&
                    player.favorPoints >= cost &&
                    deckManager.HasInactiveServiceOptionsThisRound();
            }
        }

        if (button != null)
            button.interactable = canUse;
    }

    public void OnClick()
    {
        if (deckManager == null)
            return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuClick();

        deckManager.StartInactiveInfluenceServiceSelection();

        Refresh();
    }

    public void SetVisible(bool visible)
    {

        if (buttonRoot != null)
            buttonRoot.SetActive(visible);
    }

    public int GetCurrentCost()
    {
        if (deckManager == null)
            return 5;

        return deckManager.GetInactiveInfluenceServiceCost();
    }
}