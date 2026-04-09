using UnityEngine;

public class RulesPanelController : MonoBehaviour
{
    public GameObject rulesPanel;
    public DeckManager deckManager;

    public void ToggleRules()
    {
        if (rulesPanel == null)
            return;

        bool willOpen = !rulesPanel.activeSelf;
        rulesPanel.SetActive(willOpen);

        if (deckManager != null)
        {
            if (willOpen)
                deckManager.OnRulesPanelOpened();
            else
                deckManager.OnRulesPanelClosed();
        }
    }

    public void CloseRules()
    {
        if (rulesPanel != null)
            rulesPanel.SetActive(false);

        if (deckManager != null)
            deckManager.OnRulesPanelClosed();
    }
}