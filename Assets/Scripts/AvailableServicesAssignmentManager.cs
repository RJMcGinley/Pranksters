using UnityEngine;

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

    [Header("Beastmaster Action Colliders")]
    public AvailableServiceActionCollider immediateActionCollider;
    public AvailableServiceActionCollider scoringActionCollider;
    public AvailableServiceActionCollider ongoingActionCollider;

    public bool AssignCardToFirstAvailableSlot(Sprite cardArt)
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
                slot.AssignVisual(cardVisualPrefab, cardArt);
                UpdateBeastmasterGlowState();

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

    if (scoringActionGlow != null)
        scoringActionGlow.SetActive(assignedCount >= 3);

    if (scoringActionCollider != null)
        scoringActionCollider.SetAvailable(assignedCount >= 3);

    if (ongoingActionGlow != null)
        ongoingActionGlow.SetActive(assignedCount >= 4);

    if (ongoingActionCollider != null)
        ongoingActionCollider.SetAvailable(assignedCount >= 4);

    Debug.Log("Beastmaster glow update | assignedCount=" + assignedCount);
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

    Debug.Log("Cleared all Available Services assignments.");
}   
}