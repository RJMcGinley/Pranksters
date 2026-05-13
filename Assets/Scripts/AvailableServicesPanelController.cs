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

    private void Start()
    {
        ApplyUnlockVisibility();
        HideAllServicePanels();
    }

    public void OnServiceSelected(PranksterType serviceType)
    {
        Debug.Log("Selected available service: " + serviceType);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuClick();

        HideAllServicePanels();

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
    }

    public void OnCloseServicePanelClicked()
    {
        Debug.Log("Closed available service panel.");

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBackClick();

        DeckManager deckManager = FindFirstObjectByType<DeckManager>();

        if (deckManager != null)
        {
            deckManager.CancelAvailableServicesSelection();
        }

        HideAllServicePanels();
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
    }

    private void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null)
            panel.SetActive(active);
    }

    public void CloseAllServicePanels()
    {
        HideAllServicePanels();
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

    
}