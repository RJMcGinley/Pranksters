using UnityEngine;
using System.Collections.Generic;

public class AvailableServicePanelAssignmentController : MonoBehaviour
{
    [Header("Service Identity")]
    [SerializeField] private PranksterType serviceType;

    [Header("Visual Setup")]
    [SerializeField] private GameObject cardVisualPrefab;

    [Header("Card Slots")]
    [SerializeField] private AvailableServicesServiceCardSlot[] serviceSlots;

    [Header("Action Glows")]
    [SerializeField] private GameObject immediateActionGlow;
    [SerializeField] private GameObject scoringActionGlow;
    [SerializeField] private GameObject ongoingActionGlow;
    [SerializeField] private GameObject retainServicesGlow;

    [Header("Action Colliders")]
    [SerializeField] private AvailableServiceActionCollider immediateActionCollider;
    [SerializeField] private AvailableServiceActionCollider scoringActionCollider;
    [SerializeField] private AvailableServiceActionCollider ongoingActionCollider;
    [SerializeField] private AvailableServiceActionCollider retainServicesCollider;

    [Header("References")]
    [SerializeField] private DeckManager deckManager;

    public PranksterType ServiceType => serviceType;

    public bool AssignCardToFirstAvailableSlot(Sprite cardArt, PranksterDeckEntry card)
    {
        if (serviceSlots == null || serviceSlots.Length == 0)
        {
            Debug.LogWarning("No service slots assigned for: " + serviceType);
            return false;
        }

        foreach (AvailableServicesServiceCardSlot slot in serviceSlots)
        {
            if (slot != null && slot.IsEmpty())
            {
                slot.AssignVisual(cardVisualPrefab, cardArt, card);
                UpdateActionGlowState();

                Debug.Log("Assigned service card visual to first available slot for: " + serviceType);
                return true;
            }
        }

        Debug.Log("No empty service slots available for: " + serviceType);
        return false;
    }

    private int GetAssignedSlotCount()
    {
        int count = 0;

        if (serviceSlots == null)
            return count;

        foreach (AvailableServicesServiceCardSlot slot in serviceSlots)
        {
            if (slot != null && !slot.IsEmpty())
                count++;
        }

        return count;
    }

    public void UpdateActionGlowState()
    {
        int assignedCount = GetAssignedSlotCount();

        bool immediateAvailable = assignedCount >= 2;

        if (immediateActionGlow != null)
            immediateActionGlow.SetActive(immediateAvailable);

        if (immediateActionCollider != null)
            immediateActionCollider.SetAvailable(immediateAvailable);

        bool scoringAlreadyUsed = false;
        bool ongoingAlreadyUsed = false;

        if (deckManager != null)
        {
            scoringAlreadyUsed = deckManager.HasCurrentPlayerUsedScoringService(serviceType);
            ongoingAlreadyUsed = deckManager.HasCurrentPlayerUsedOngoingService(serviceType);
        }

        bool scoringAvailable = assignedCount >= 3 && !scoringAlreadyUsed;
        bool ongoingAvailable = assignedCount >= 4 && !ongoingAlreadyUsed;

        if (scoringActionGlow != null)
            scoringActionGlow.SetActive(scoringAvailable);

        if (scoringActionCollider != null)
            scoringActionCollider.SetAvailable(scoringAvailable);

        if (ongoingActionGlow != null)
            ongoingActionGlow.SetActive(ongoingAvailable);

        if (ongoingActionCollider != null)
            ongoingActionCollider.SetAvailable(ongoingAvailable);

        Debug.Log("Available Service glow update | serviceType=" + serviceType +
                  " | assignedCount=" + assignedCount);
    }

    public void SetRetainServicesAvailable(bool available)
    {
        if (retainServicesCollider != null)
            retainServicesCollider.SetAvailable(available);

        if (retainServicesGlow != null)
            retainServicesGlow.SetActive(available);
    }

    public void ClearAllAssignments()
    {
        if (serviceSlots != null)
        {
            foreach (AvailableServicesServiceCardSlot slot in serviceSlots)
            {
                if (slot != null)
                    slot.ClearAssignment();
            }
        }

        if (immediateActionGlow != null)
            immediateActionGlow.SetActive(false);

        if (scoringActionGlow != null)
            scoringActionGlow.SetActive(false);

        if (ongoingActionGlow != null)
            ongoingActionGlow.SetActive(false);

        SetRetainServicesAvailable(false);

        Debug.Log("Cleared all Available Services assignments for: " + serviceType);
    }

    public List<PranksterDeckEntry> GetAssignedCardsInSlotOrder()
    {
        List<PranksterDeckEntry> assignedCards = new List<PranksterDeckEntry>();

        if (serviceSlots == null)
            return assignedCards;

        foreach (AvailableServicesServiceCardSlot slot in serviceSlots)
        {
            if (slot != null && slot.HasAssignedCard())
                assignedCards.Add(slot.GetAssignedCardCopy());
        }

        return assignedCards;
    }

    public void DisplayRetainedCards(List<PranksterDeckEntry> retainedCards)
    {
        ClearAllAssignments();

        if (retainedCards == null || retainedCards.Count == 0)
        {
            Debug.Log("No retained service cards to display for: " + serviceType);
            return;
        }

        if (serviceSlots == null || serviceSlots.Length == 0)
        {
            Debug.LogWarning("No service slots assigned for: " + serviceType);
            return;
        }

        int maxCards = Mathf.Min(retainedCards.Count, serviceSlots.Length);

        for (int i = 0; i < maxCards; i++)
        {
            if (serviceSlots[i] == null)
                continue;

            PranksterDeckEntry card = retainedCards[i];

            Sprite cardArt = PranksterSpriteDatabase.GetSprite(
                card.pranksterType,
                card.tier,
                card.category
            );

            serviceSlots[i].AssignVisual(cardVisualPrefab, cardArt, card);
        }

        UpdateActionGlowState();

        Debug.Log("Displayed " + maxCards + " retained service card(s) for " + serviceType);
    }
}