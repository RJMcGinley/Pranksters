using UnityEngine;

public class AvailableServicesServiceCardSlot : MonoBehaviour
{
    [Header("Slot Setup")]
    public Transform cardSpawnPoint;

    private GameObject assignedVisual;

    public bool IsEmpty()
    {
        return assignedVisual == null;
    }

    public void AssignVisual(GameObject visualPrefab, Sprite cardArt)
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
    }
}