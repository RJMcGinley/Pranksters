using UnityEngine;

public class AvailableServiceActionCollider : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private AvailableServiceActionCollider retainServicesCollider;
    
    [Header("Action Identity")]
    [SerializeField] private string actionName;

    [Header("Availability")]
    [SerializeField] private bool isAvailable;

    [Header("Hover Description")]
    [SerializeField] private GameObject[] actionTitleTextObjects;
    [SerializeField] private TMPro.TMP_Text descriptionText;
    [SerializeField] [TextArea] private string actionDescription;
    [SerializeField] [TextArea] private string influenceModeActionDescription;
    [SerializeField] private float hoverDelay = 0.5f;
    [SerializeField] private GameObject retainServicesImage;
    [SerializeField] private GameObject retainServicesGlow;

    private bool isHovering;
    private float hoverTimer;

    private void Start()
{
    SetAvailable(isAvailable);
    ResetVisualState();
}

    private void OnMouseDown()
{
    if (deckManager == null)
        return;

    bool canUseAction =
        deckManager.IsChoosingAvailableService() ||
        deckManager.IsViewingInactiveInfluenceServicePanel();

    if (!canUseAction)
    {
        Debug.Log("Available Service action blocked because panel is in view-only mode: " + actionName);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayNotAnOption();

        return;
    }

    if (!isAvailable)
    {
        Debug.Log("Available Service action blocked because action is not available: " + actionName);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayNotAnOption();

        return;
    }

    Debug.Log("Available Service action clicked: " + actionName);

    if (actionName == "Retain Services")
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuClick();

        deckManager.CommitRetainedServices();
    }
    else if (actionName == "Scoring Action")
    {
        deckManager.ActivateAvailableServiceScoringAction();
    }
    else if (actionName == "Laborer Immediate Action")
    {
        deckManager.ActivateLaborerImmediateAction(
            deckManager.IsViewingInactiveInfluenceServicePanel()
        );
    }
    else if (actionName == "Wizard Immediate Action")
    {
        deckManager.ActivateWizardImmediateAction(
            deckManager.IsViewingInactiveInfluenceServicePanel()
        );
    }
    else if (actionName == "Engineer Immediate Action")
    {
        deckManager.ActivateEngineerImmediateAction(
            deckManager.IsViewingInactiveInfluenceServicePanel()
        );
    }
    else if (actionName == "Thief Immediate Action")
    {
        deckManager.ActivateThiefImmediateAction(
            deckManager.IsViewingInactiveInfluenceServicePanel()
        );
    }
    else if (actionName == "Scribe Immediate Action")
    {
        deckManager.ActivateScribeImmediateAction(
            deckManager.IsViewingInactiveInfluenceServicePanel()
        );
    }
    else if (actionName == "Beastmaster Immediate Action")
    {
        deckManager.ActivateBeastmasterImmediateAction(
            deckManager.IsViewingInactiveInfluenceServicePanel()
        );
    }
}

    public void SetAvailable(bool available)
{
    isAvailable = available;
}

    public bool IsAvailable()
{
    return isAvailable;
}

    private void Update()
{
    if (!isHovering)
        return;

    hoverTimer += Time.deltaTime;

    if (hoverTimer >= hoverDelay)
    {
        ShowDescription();
    }
}

private void OnMouseEnter()
{
    Debug.Log("AVAILABLE SERVICE HOVER ENTER: " + gameObject.name + " | " + actionName);

    isHovering = true;
    hoverTimer = 0f;
}

private void OnMouseExit()
{
    isHovering = false;
    hoverTimer = 0f;

    HideDescription();
}

private void ShowDescription()
{
    if (actionTitleTextObjects != null)
    {
        foreach (GameObject obj in actionTitleTextObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    if (retainServicesImage != null)
        retainServicesImage.SetActive(false);

    if (retainServicesGlow != null)
        retainServicesGlow.SetActive(false);

    if (descriptionText != null)
    {
        descriptionText.gameObject.SetActive(true);

        if (deckManager != null &&
            deckManager.IsViewingInactiveInfluenceServicePanel() &&
            !string.IsNullOrWhiteSpace(influenceModeActionDescription))
        {
            descriptionText.text = influenceModeActionDescription;
        }
        else
        {
            descriptionText.text = actionDescription;
        }
    }
}

private void HideDescription()
{
    if (actionTitleTextObjects != null)
    {
        foreach (GameObject obj in actionTitleTextObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }

    if (retainServicesImage != null)
        retainServicesImage.SetActive(true);

    if (retainServicesGlow != null && retainServicesCollider != null)
        retainServicesGlow.SetActive(retainServicesCollider.IsAvailable());

    if (descriptionText != null)
    {
        descriptionText.gameObject.SetActive(false);
    }
}

public void ResetVisualState()
{
    isHovering = false;
    hoverTimer = 0f;

    if (actionTitleTextObjects != null)
    {
        foreach (GameObject obj in actionTitleTextObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }

    if (retainServicesImage != null)
        retainServicesImage.SetActive(true);

    if (retainServicesGlow != null)
    {
        bool retainAvailable =
            retainServicesCollider != null && retainServicesCollider.IsAvailable();

        retainServicesGlow.SetActive(retainAvailable);
    }

    if (descriptionText != null)
    {
        descriptionText.text = "";
        descriptionText.gameObject.SetActive(false);
    }
}

private void OnEnable()
{
    Collider2D col = GetComponent<Collider2D>();

    Debug.Log(
        $"AVAILABLE SERVICE COLLIDER ENABLED | {gameObject.name} | " +
        $"Action={actionName} | " +
        $"ActiveInHierarchy={gameObject.activeInHierarchy} | " +
        $"Layer={LayerMask.LayerToName(gameObject.layer)} | " +
        $"Collider={(col != null ? col.GetType().Name : "NULL")} | " +
        $"ColliderEnabled={(col != null && col.enabled)} | " +
        $"WorldPos={transform.position}"
    );
}


}
