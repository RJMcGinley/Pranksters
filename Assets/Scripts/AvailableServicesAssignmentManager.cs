using UnityEngine;
using System.Collections.Generic;

public class AvailableServicesAssignmentManager : MonoBehaviour
{
    [Header("Visual Setup")]
    public GameObject cardVisualPrefab;

    [Header("Beastmaster Slots")]
    public AvailableServicesServiceCardSlot[] beastmasterSlots;

    [Header("Beastmaster Action Glows")]
    public GameObject immediateActionGlow;
    public GameObject scoringActionGlow;
    public GameObject ongoingActionGlow;
    public GameObject retainServicesGlow;

    [Header("Beastmaster Action Colliders")]
    public AvailableServiceActionCollider immediateActionCollider;
    public AvailableServiceActionCollider scoringActionCollider;
    public AvailableServiceActionCollider ongoingActionCollider;
    public AvailableServiceActionCollider retainServicesCollider;

    [Header("References")]
    [SerializeField] private DeckManager deckManager;

    public bool AssignCardToFirstAvailableSlot(Sprite cardArt, PranksterDeckEntry card)
    {
        if (beastmasterSlots == null || beastmasterSlots.Length == 0)
        {
            Debug.LogWarning("No Beastmaster service slots assigned.");
            return false;
        }

        foreach (AvailableServicesServiceCardSlot slot in beastmasterSlots)
        {
            if (slot != null && slot.IsEmpty())
            {
                slot.AssignVisual(cardVisualPrefab, cardArt, card);
                // UpdateBeastmasterGlowState();

                Debug.Log("Assigned service card visual to first available Beastmaster slot.");
                return true;
            }
        }

        Debug.Log("No empty Beastmaster service slots available.");
        return false;
    }

    private int GetAssignedSlotCount()
    {
        int count = 0;

        if (beastmasterSlots == null)
            return count;

        foreach (AvailableServicesServiceCardSlot slot in beastmasterSlots)
        {
            if (slot != null && !slot.IsEmpty())
                count++;
        }

        return count;
    }

    private void UpdateBeastmasterGlowState()
    {
        int assignedCount = GetAssignedSlotCount();

        if (immediateActionGlow != null)
            immediateActionGlow.SetActive(assignedCount >= 2);

        if (immediateActionCollider != null)
            immediateActionCollider.SetAvailable(assignedCount >= 2);

        bool scoringAlreadyUsed = false;
        bool ongoingAlreadyUsed = false;

        if (deckManager != null)
        {
            scoringAlreadyUsed = deckManager.HasCurrentPlayerUsedScoringService(PranksterType.BeastMaster);
            ongoingAlreadyUsed = deckManager.HasCurrentPlayerUsedOngoingService(PranksterType.BeastMaster);
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

        Debug.Log("Beastmaster glow update | assignedCount=" + assignedCount);
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
        if (beastmasterSlots != null)
        {
            foreach (AvailableServicesServiceCardSlot slot in beastmasterSlots)
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

        Debug.Log("Cleared all Available Services assignments.");
    }

    public List<PranksterDeckEntry> GetAssignedCardsInSlotOrder()
    {
        List<PranksterDeckEntry> assignedCards = new List<PranksterDeckEntry>();

        if (beastmasterSlots == null)
            return assignedCards;

        foreach (AvailableServicesServiceCardSlot slot in beastmasterSlots)
        {
            if (slot != null && slot.HasAssignedCard())
            {
                assignedCards.Add(slot.GetAssignedCardCopy());
            }
        }

        return assignedCards;
    }

    public void DisplayRetainedCardsForService(PranksterType serviceType, List<PranksterDeckEntry> retainedCards)
    {
        ClearAllAssignments();

        if (retainedCards == null || retainedCards.Count == 0)
        {
            Debug.Log("No retained service cards to display for: " + serviceType);
            return;
        }

        if (serviceType != PranksterType.BeastMaster)
        {
            Debug.Log("Retained service display not yet set up for: " + serviceType);
            return;
        }

        if (beastmasterSlots == null || beastmasterSlots.Length == 0)
        {
            Debug.LogWarning("No Beastmaster service slots assigned.");
            return;
        }

        int maxCards = Mathf.Min(retainedCards.Count, beastmasterSlots.Length);

        for (int i = 0; i < maxCards; i++)
        {
            if (beastmasterSlots[i] == null)
                continue;

            PranksterDeckEntry card = retainedCards[i];

            Sprite cardArt = PranksterSpriteDatabase.GetSprite(
                card.pranksterType,
                card.tier,
                card.category
            );

            beastmasterSlots[i].AssignVisual(cardVisualPrefab, cardArt, card);
        }

        UpdateBeastmasterGlowState();

        Debug.Log("Displayed " + maxCards + " retained service card(s) for " + serviceType);
    }
}