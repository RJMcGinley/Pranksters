using UnityEngine;
using UnityEngine.EventSystems;

public class UnlockPrankCardHoverTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnlockProgressionHoverPreviewController previewController;

    private PrankCard currentPrank;

    public void SetHoverData(PrankCard prank)
    {
        currentPrank = prank;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (previewController != null && currentPrank != null)
            previewController.ShowPrankCard(currentPrank);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (previewController != null)
            previewController.Hide();
    }
}