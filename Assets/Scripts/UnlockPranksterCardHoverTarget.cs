using UnityEngine;
using UnityEngine.EventSystems;

public class UnlockPranksterCardHoverTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnlockProgressionHoverPreviewController previewController;

    private PranksterDeckEntry currentCard;
    private bool currentUnlocked;

    public void SetHoverData(PranksterDeckEntry card, bool unlocked)
    {
        currentCard = card;
        currentUnlocked = unlocked;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (previewController != null && currentCard != null)
            previewController.ShowPranksterCard(currentCard, currentUnlocked);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (previewController != null)
            previewController.Hide();
    }
}