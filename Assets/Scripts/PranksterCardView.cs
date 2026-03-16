using UnityEngine;

public class PranksterCardView : MonoBehaviour
{
    public SpriteRenderer characterArtRenderer;

    public void SetArt(Sprite art)
    {
        characterArtRenderer.sprite = art;
    }
}
