using UnityEngine;

public class HideoutCardPreview : MonoBehaviour
{
    [SerializeField] private Transform cardSpawnPoint;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Vector3 cardScale = Vector3.one;

    private GameObject spawnedCard;

    public void Show(PranksterDeckEntry card)
    {
        if (card == null)
        {
            Hide();
            return;
        }

        if (spawnedCard != null)
            Destroy(spawnedCard);

        spawnedCard = Instantiate(cardPrefab, cardSpawnPoint);
        spawnedCard.transform.localPosition = Vector3.zero;
        spawnedCard.transform.localRotation = Quaternion.identity;
        spawnedCard.transform.localScale = cardScale;

        Sprite sprite = PranksterSpriteDatabase.GetSprite(card.pranksterType, card.tier, card.category);

        PranksterCardView view = spawnedCard.GetComponent<PranksterCardView>();
        if (view != null)
        {
            view.SetArt(sprite);
            view.SetTierIndicator(card.tier);
        }

        SpriteRenderer[] renderers = spawnedCard.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.sortingOrder += 1000;
        }
    }

    public void Hide()
    {
        if (spawnedCard != null)
        {
            Destroy(spawnedCard);
            spawnedCard = null;
        }
    }
}