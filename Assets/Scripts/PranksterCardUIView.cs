using UnityEngine;
using UnityEngine.UI;

public class PranksterCardUIView : MonoBehaviour
{
    public Image backgroundImage;
    public Image characterArtImage;
    public Image frameImage;

    public void SetCard(PranksterDeckEntry card)
    {
        if (card == null)
        {
            Debug.LogWarning("PranksterCardUIView SetCard received NULL card.");
            return;
        }

        Sprite sprite = PranksterSpriteDatabase.GetSprite(card.pranksterType, card.tier);

        Debug.Log("CARD UI VIEW SET CARD | type=" + card.pranksterType +
                  " | tier=" + card.tier +
                  " | sprite=" + (sprite != null ? sprite.name : "NULL"));

        SetCharacterArt(sprite);
    }

    public void SetCharacterArt(Sprite sprite)
    {
        if (characterArtImage == null)
        {
            Debug.LogError("PranksterCardUIView characterArtImage is NOT wired.");
            return;
        }

        characterArtImage.sprite = sprite;
        characterArtImage.enabled = sprite != null;
        characterArtImage.color = Color.white;
        characterArtImage.preserveAspect = true;
    }
}