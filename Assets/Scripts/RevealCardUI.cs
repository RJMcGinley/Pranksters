using UnityEngine;
using UnityEngine.UI;

public class RevealCardUI : MonoBehaviour
{
    public Image cardArtImage;

    public void SetSprite(Sprite sprite)
    {
        if (cardArtImage != null)
        {
            cardArtImage.sprite = sprite;
            cardArtImage.enabled = sprite != null;
        }
    }
}