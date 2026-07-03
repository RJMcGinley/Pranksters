using UnityEngine;

public class RulesPanelController : MonoBehaviour
{
    [Header("Main")]
    public GameObject rulesPanel;
    public DeckManager deckManager;

    [Header("Rules Pages")]
    public GameObject pageObjective;
    public GameObject pageIcons;
    public GameObject pageControls;
    public GameObject pagePlayerActions;
    public GameObject pageWantedBoard;
    public GameObject pageBarnaby;
    public GameObject pageAvailableServices;
    public GameObject pageSpendInfluence;
    public RulesPanelActionButtonsPageShowHideActions actionButtonsPageController;

    private GameObject currentPage;

    public void ToggleRules()
    {
        if (deckManager != null && deckManager.IsWantedBoardOpen())
            return;

        if (rulesPanel == null)
            return;

        bool willOpen = !rulesPanel.activeSelf;
        rulesPanel.SetActive(willOpen);

        if (willOpen)
        {
            RulesShowObjectivePage();

            if (deckManager != null)
                deckManager.OnRulesPanelOpened();
        }
        else
        {
            if (deckManager != null)
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

    public void RulesShowObjectivePage()
    {
        ShowOnly(pageObjective);
    }

    public void RulesShowIconsPage()
    {
        ShowOnly(pageIcons);
    }

    public void RulesShowControlsPage()
    {
        ShowOnly(pageControls);
    }

    public void RulesShowPlayerActionsPage()
    {
        ShowOnly(pagePlayerActions);
    }

    public void RulesShowWantedBoardPage()
    {
        ShowOnly(pageWantedBoard);
    }

    public void RulesShowBarnabyPage()
    {
        ShowOnly(pageBarnaby);
    }

    public void RulesShowAvailableServicesPage()
    {
        ShowOnly(pageAvailableServices);
    }

    public void RulesShowSpendInfluencePage()
    {
        ShowOnly(pageSpendInfluence);

        if (actionButtonsPageController != null)
            actionButtonsPageController.Refresh();
    }

    private void ShowOnly(GameObject pageToShow)
    {
        if (pageToShow == null)
            return;

        SetPageActive(pageObjective, pageToShow);
        SetPageActive(pageIcons, pageToShow);
        SetPageActive(pageControls, pageToShow);
        SetPageActive(pagePlayerActions, pageToShow);
        SetPageActive(pageWantedBoard, pageToShow);
        SetPageActive(pageBarnaby, pageToShow);
        SetPageActive(pageAvailableServices, pageToShow);
        SetPageActive(pageSpendInfluence, pageToShow);

        currentPage = pageToShow;
    }

    private void SetPageActive(GameObject page, GameObject pageToShow)
    {
        if (page == null)
            return;

        page.SetActive(page == pageToShow);
    }
}