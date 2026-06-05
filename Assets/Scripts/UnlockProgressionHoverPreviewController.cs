using UnityEngine;

public class UnlockProgressionHoverPreviewController : MonoBehaviour
{
    [Header("Panel")]
    public GameObject previewPanel;

    [Header("Preview Objects")]
    public GameObject largePranksterCardPreviewRoot;
    public GameObject largePrankCardPreviewRoot;

    [Header("Large Prankster Card")]
    public PranksterCardUIView largePranksterCardView;

    public UnlockProgressionPanelPrankView largePrankCardView;
    

    public void ShowPranksterCard(PranksterDeckEntry card, bool unlocked)
    {
        if (previewPanel != null)
            previewPanel.SetActive(true);

        if (largePranksterCardPreviewRoot != null)
            largePranksterCardPreviewRoot.SetActive(true);

        if (largePrankCardPreviewRoot != null)
            largePrankCardPreviewRoot.SetActive(false);

        if (largePranksterCardView != null)
        {
            largePranksterCardView.SetCard(card);
            largePranksterCardView.SetUnlockedVisual(unlocked);
        }
    }

    public void Hide()
    {
        if (previewPanel != null)
            previewPanel.SetActive(false);
    }

    public void ShowPrankCard(PrankCard prank)
{
    if (previewPanel != null)
        previewPanel.SetActive(true);

    if (largePranksterCardPreviewRoot != null)
        largePranksterCardPreviewRoot.SetActive(false);

    if (largePrankCardPreviewRoot != null)
        largePrankCardPreviewRoot.SetActive(true);

    if (largePrankCardView != null)
        largePrankCardView.SetPrank(prank);
}
}