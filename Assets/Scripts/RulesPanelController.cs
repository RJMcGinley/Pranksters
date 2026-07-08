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
    public GameObject pageJailbreak;
    public GameObject pageLocations;
    public GameObject handDisplayObject;
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

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUIClick();

        if (handDisplayObject != null)
            handDisplayObject.SetActive(!willOpen);

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

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayUIClick();

        if (deckManager != null)
            deckManager.OnRulesPanelClosed();

        if (handDisplayObject != null)
            handDisplayObject.SetActive(true);
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

    public void RulesShowJailbreakPage()
    {
        ShowOnly(pageJailbreak);
    }

    public void RulesShowLocationsPage()
    {
        ShowOnly(pageLocations);
    }

    private void ShowOnly(GameObject pageToShow)
    {
        if (pageToShow == null)
            return;

        bool isChangingPage = currentPage != null && currentPage != pageToShow;

        SetPageActive(pageObjective, pageToShow);
        SetPageActive(pageIcons, pageToShow);
        SetPageActive(pageControls, pageToShow);
        SetPageActive(pagePlayerActions, pageToShow);
        SetPageActive(pageWantedBoard, pageToShow);
        SetPageActive(pageBarnaby, pageToShow);
        SetPageActive(pageAvailableServices, pageToShow);
        SetPageActive(pageSpendInfluence, pageToShow);
        SetPageActive(pageJailbreak, pageToShow);
        SetPageActive(pageLocations, pageToShow);
        

        currentPage = pageToShow;

        if (isChangingPage && AudioManager.Instance != null)
            AudioManager.Instance.PlayRulesPageTurn();
    }

    private void SetPageActive(GameObject page, GameObject pageToShow)
    {
        if (page == null)
            return;

        page.SetActive(page == pageToShow);
    }
}