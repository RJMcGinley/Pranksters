using UnityEngine;

public class AvailableServicesServiceCardSlot : MonoBehaviour
{
    [Header("Slot Setup")]
    public Transform cardSpawnPoint;

    private GameObject assignedVisual;
    private PranksterDeckEntry assignedCard;

    public bool IsEmpty()
    {
        return assignedVisual == null;
    }

    public void AssignVisual(GameObject visualPrefab, Sprite cardArt, PranksterDeckEntry card)
    {
        if (!IsEmpty())
            return;

        if (visualPrefab == null)
        {
            Debug.LogWarning("ServiceCardSlot missing visual prefab.");
            return;
        }

        if (cardSpawnPoint == null)
        {
            Debug.LogWarning("ServiceCardSlot missing CardSpawnPoint.");
            return;
        }

        assignedCard = new PranksterDeckEntry
        {
            pranksterType = card.pranksterType,
            tier = card.tier,
            category = card.category
        };

        assignedVisual = Instantiate(visualPrefab, cardSpawnPoint);
        assignedVisual.transform.localPosition = Vector3.zero;
        assignedVisual.transform.localRotation = Quaternion.identity;
        assignedVisual.transform.localScale = Vector3.one;

        PranksterCardView cardView = assignedVisual.GetComponent<PranksterCardView>();
        if (cardView != null)
        {
            cardView.SetArt(cardArt);
            return;
        }

        PranksterCardUIView cardUIView = assignedVisual.GetComponent<PranksterCardUIView>();
        if (cardUIView != null)
        {
            cardUIView.SetCharacterArt(cardArt);
            return;
        }

        Debug.LogWarning("Assigned service visual has no PranksterCardView or PranksterCardUIView.");
    }

    public void ClearAssignment()
    {
        if (assignedVisual != null)
        {
            Destroy(assignedVisual);
            assignedVisual = null;
        }

        assignedCard = null;
    }

    public bool HasAssignedCard()
    {
        return assignedCard != null;
    }

    public PranksterDeckEntry GetAssignedCardCopy()
    {
        if (assignedCard == null)
            return null;

        return new PranksterDeckEntry
        {
            pranksterType = assignedCard.pranksterType,
            tier = assignedCard.tier,
            category = assignedCard.category
        };
    }

}