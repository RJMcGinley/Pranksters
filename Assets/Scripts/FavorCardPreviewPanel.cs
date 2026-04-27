using UnityEngine;

public class FavorCardPreviewPanel : MonoBehaviour
{
    public GameObject panelRoot;
    public PranksterCardView cardView;

    void Start()
    {
        Hide();
    }

    public void Show(PranksterDeckEntry card, Vector3 worldPosition)
    {
        if (card == null)
        {
            Hide();
            return;
        }

        // Use the position passed in (DO NOT override it)
        transform.position = worldPosition;

        if (panelRoot != null)
            panelRoot.SetActive(true);

        var sprite = PranksterSpriteDatabase.GetSprite(card.pranksterType, card.tier);

        if (cardView != null)
        {
            cardView.SetArt(sprite);
            cardView.SetTierIndicator(card.tier);
        }
    }

    public void Hide()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }
}