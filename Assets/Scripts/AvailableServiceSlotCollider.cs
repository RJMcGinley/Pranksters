using UnityEngine;

public class AvailableServiceSlotCollider : MonoBehaviour
{
    [Header("References")]
    public AvailableServicesPanelController servicesPanelController;
    public DeckManager deckManager;

    [Header("Service Identity")]
    public PranksterType serviceType;

    [Header("Visual References")]
    public GameObject glowObject;

    private bool isAvailable = true;

    void Start()
    {
        SetAvailable(isAvailable);
    }

    
void OnMouseDown()
{
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

    public void SetAvailable(bool available)
    {
        isAvailable = available;

        if (glowObject != null)
            glowObject.SetActive(available);
    }
}