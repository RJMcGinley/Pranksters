using UnityEngine;
using UnityEngine.UI;

public class UnlockProgressionPanelPrankView : MonoBehaviour
{
    [Header("References")]
    public Image cardArtImage;
    public Image cardFrame;

    public void SetPrank(PrankCard prank)
    {

        if (prank == null)
        {
            Debug.LogWarning("UnlockProgressionPanelPrankView: prank is null.");
            return;
        }

        if (cardArtImage != null)
        {
            cardArtImage.sprite = prank.cardSprite;
            cardArtImage.enabled = prank.cardSprite != null;
            cardArtImage.color = Color.white;
            cardArtImage.preserveAspect = true;
        }

        if (cardFrame != null)
        {
            cardFrame.enabled = true;
            cardFrame.color = Color.white;
        }

        Debug.Log("UnlockProgressionPanelPrankView displaying prank: " + prank.title);
    }
}