using UnityEngine;

public class AvailableServicesPanelController : MonoBehaviour
{
    [Header("Testing Overrides")]
    [SerializeField] private bool ignoreServiceUnlocksForTesting = false;

    [Header("Service Panels")]
    [SerializeField] private GameObject beastMasterServicePanel;
    [SerializeField] private GameObject thiefServicePanel;
    [SerializeField] private GameObject wizardServicePanel;
    [SerializeField] private GameObject scribeServicePanel;
    [SerializeField] private GameObject engineerServicePanel;
    [SerializeField] private GameObject laborerServicePanel;

    [Header("Unlock Controlled Objects")]
    [SerializeField] private GameObject scoringActionObject;
    [SerializeField] private GameObject immediateActionObject;
    [SerializeField] private GameObject ongoingActionObject;

    [Header("Service Selection Colliders")]
    [SerializeField] private BoxCollider2D[] serviceSelectionColliders;

    private void Start()
    {
        ApplyUnlockVisibility();
        HideAllServicePanels();
    }

    public void OnServiceSelected(PranksterType serviceType)
    {
        DeckManager deckManager = FindFirstObjectByType<DeckManager>();
        Debug.Log("Inactive influence selected service type set to: " + serviceType);

        if (deckManager != null)
        {
            deckManager.SetInactiveServicesButtonVisible(false);
            deckManager.SetAvailableServicesPanelOpen(true);
            deckManager.MarkInactiveInfluenceServicePanelOpen();
            deckManager.SetSelectedInactiveInfluenceServiceType(serviceType);
            deckManager.SetActivePrankCardCollidersEnabled(false);
            deckManager.RefreshAllHighlights();
        }

        Debug.Log("Selected available service: " + serviceType);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuClick();

        HideAllServicePanels();
        SetServiceSelectionCollidersEnabled(false);

        switch (serviceType)
        {
            case PranksterType.BeastMaster:
                ShowServicePanel(beastMasterServicePanel);
                break;

            case PranksterType.Thief:
                ShowServicePanel(thiefServicePanel);
                break;

            case PranksterType.Wizard:
                ShowServicePanel(wizardServicePanel);
                break;

            case PranksterType.Scribe:
                ShowServicePanel(scribeServicePanel);
                break;

            case PranksterType.Engineer:
                ShowServicePanel(engineerServicePanel);
                break;

            case PranksterType.Laborer:
                ShowServicePanel(laborerServicePanel);
                break;

            default:
                Debug.LogWarning("No service panel exists for: " + serviceType);
                break;
        }

        if (deckManager != null && deckManager.IsViewingInactiveInfluenceServicePanel())
        {
            AvailableServicePanelAssignmentController panelController =
                GetPanelAssignmentController(serviceType);

            if (panelController != null)
                panelController.ShowInactiveInfluenceModeActions();
        }
    }

    public void OnCloseServicePanelClicked()
    {
        Debug.Log("Closed available service panel.");

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBackClick();

        DeckManager deckManager = FindFirstObjectByType<DeckManager>();

        if (deckManager != null)
        {
            deckManager.SetAvailableServicesPanelOpen(false);
            deckManager.SetInactiveServicesButtonVisible(true);
            deckManager.CancelAvailableServicesSelection();
            deckManager.SetActivePrankCardCollidersEnabled(true);
        }

        HideAllServicePanels();
        SetServiceSelectionCollidersEnabled(true);

        if (deckManager != null)
        {
            deckManager.RefreshAllHighlights();
        }
    }

    public void HideAllServicePanels()
    {
        SetPanelActive(beastMasterServicePanel, false);
        SetPanelActive(thiefServicePanel, false);
        SetPanelActive(wizardServicePanel, false);
        SetPanelActive(scribeServicePanel, false);
        SetPanelActive(engineerServicePanel, false);
        SetPanelActive(laborerServicePanel, false);
    }

    private void ShowServicePanel(GameObject panel)
    {
        if (panel == null)
        {
            Debug.LogWarning("Service panel reference is missing.");
            return;
        }

        panel.SetActive(true);

        AvailableServicePanelAssignmentController assignmentController =
            panel.GetComponent<AvailableServicePanelAssignmentController>();

        if (assignmentController != null)
        {
            assignmentController.UpdateActionGlowState();
            assignmentController.ResetAllActionVisualStates();
        }
    }

    private void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null)
            panel.SetActive(active);
    }

    public void CloseAllServicePanels()
    {
        DeckManager deckManager = FindFirstObjectByType<DeckManager>();

        if (deckManager != null)
        {
            deckManager.SetAvailableServicesPanelOpen(false);
            deckManager.SetActivePrankCardCollidersEnabled(true);
        }
        
        HideAllServicePanels();
        SetServiceSelectionCollidersEnabled(true);
    }

    void ApplyUnlockVisibility()
{
    PlayerProgressSave save = SaveSystem.Load();

    bool scoringUnlocked = ignoreServiceUnlocksForTesting || AvailableServicesUnlockRules.CanUseScoring(save);
    bool immediateUnlocked = ignoreServiceUnlocksForTesting || AvailableServicesUnlockRules.CanUseImmediate(save);
    bool ongoingUnlocked = ignoreServiceUnlocksForTesting || AvailableServicesUnlockRules.CanUseOngoing(save);

    if (scoringActionObject != null)
        scoringActionObject.SetActive(scoringUnlocked);

    if (immediateActionObject != null)
        immediateActionObject.SetActive(immediateUnlocked);

    if (ongoingActionObject != null)
        ongoingActionObject.SetActive(ongoingUnlocked);
}

public AvailableServicePanelAssignmentController GetPanelAssignmentController(PranksterType serviceType)
{
    GameObject panel = null;

    switch (serviceType)
    {
        case PranksterType.BeastMaster:
            panel = beastMasterServicePanel;
            break;

        case PranksterType.Thief:
            panel = thiefServicePanel;
            break;

        case PranksterType.Wizard:
            panel = wizardServicePanel;
            break;

        case PranksterType.Scribe:
            panel = scribeServicePanel;
            break;

        case PranksterType.Engineer:
            panel = engineerServicePanel;
            break;

        case PranksterType.Laborer:
            panel = laborerServicePanel;
            break;
    }

    if (panel == null)
        return null;

    return panel.GetComponent<AvailableServicePanelAssignmentController>();
}

private void SetServiceSelectionCollidersEnabled(bool enabled)
{
    if (serviceSelectionColliders == null)
        return;

    foreach (BoxCollider2D collider in serviceSelectionColliders)
    {
        if (collider != null)
            collider.enabled = enabled;
    }
}

public void SetServiceSelectionGlowsVisible(bool visible)
{
    if (serviceSelectionColliders == null)
        return;

    foreach (BoxCollider2D collider in serviceSelectionColliders)
    {
        if (collider == null)
            continue;

        AvailableServiceSlotCollider slot =
            collider.GetComponent<AvailableServiceSlotCollider>();

        if (slot != null)
            slot.SetGlowVisible(visible);
    }
}
    
}