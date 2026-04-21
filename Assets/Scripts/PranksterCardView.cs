using UnityEngine;
using TMPro;

public class PranksterCardView : MonoBehaviour
{
    public SpriteRenderer characterArtRenderer;
    public TextMeshPro tierIndicatorText;

    public void SetArt(Sprite art)
    {
        characterArtRenderer.sprite = art;
    }

    public void SetTierIndicator(int tier)
{
    if (tierIndicatorText == null)
        return;

    tierIndicatorText.text = "";
}
}