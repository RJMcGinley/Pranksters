using UnityEngine;
using UnityEngine.UI;

public class AvailableServiceSlotCollider : MonoBehaviour
{
    [Header("References")]
    public AvailableServicesPanelController servicesPanelController;
    public DeckManager deckManager;

    [Header("Service Identity")]
    public PranksterType serviceType;

    [Header("Visual References")]
    public GameObject glowObject;

    [SerializeField] private Image iconImage;
    [SerializeField] private Material grayscaleMaterial;

    private Material originalMaterial;

    private bool isAvailable = true;

    void Start()
    {
        if (deckManager == null)
            deckManager = FindFirstObjectByType<DeckManager>();

        if (iconImage != null)
            originalMaterial = iconImage.material;

        RefreshRoundAvailability();
    }

    void OnMouseDown()
    {
        if (deckManager != null && deckManager.IsChoosingBeastmasterDiscardType())
        {
            deckManager.ResolveBeastmasterDiscardTypeChoice(serviceType);
            return;
        }

        if (!isAvailable)
            return;

        if (deckManager != null && deckManager.IsInteractionBlocked())
            return;

        if (deckManager != null && deckManager.IsPrankPreviewOpen())
            return;

        if (servicesPanelController == null)
        {
            Debug.LogWarning("AvailableServiceSlotCollider has no servicesPanelController assigned.");
            return;
        }

        servicesPanelController.OnServiceSelected(serviceType);

        if (deckManager != null && deckManager.CanStartAvailableServiceAction())
        {
            deckManager.StartAvailableServiceTurn(serviceType);
            deckManager.SetActiveAvailableServiceSlotCollider(this);
            SetAvailable(false);
        }
        else if (deckManager != null)
        {
            deckManager.ShowAvailableServiceStateOnly(serviceType);
        }
    }

    public void RefreshRoundAvailability()
    {
        if (deckManager == null)
        {
            SetAvailable(false);
            Debug.LogWarning("Cannot refresh service availability. DeckManager is missing.");
            return;
        }

        bool availableThisRound =
            deckManager.IsServiceTypeAvailableThisRound(serviceType);

        SetAvailable(availableThisRound);

        Debug.Log("Service availability refreshed: " + serviceType +
                  " | availableThisRound=" + availableThisRound);
    }

    public void SetAvailable(bool available)
    {
        isAvailable = available;

        if (glowObject != null)
            glowObject.SetActive(available);

        if (iconImage != null)
        {
            iconImage.material = originalMaterial;
            iconImage.color = available 
                ? Color.white 
                : new Color(0.45f, 0.45f, 0.45f, 1f);
        }
    }
}