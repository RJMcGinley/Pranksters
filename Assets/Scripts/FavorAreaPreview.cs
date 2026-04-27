using UnityEngine;

public class FavorAreaPreview : MonoBehaviour
{
    public GameObject previewPanel;
    public Transform cardSpawnPoint;
    public GameObject cardPrefab;

    private GameObject spawnedCard;

    void Start()
    {
        Hide();
    }

    public void Show(PranksterDeckEntry card)
    {
        if (card == null)
        {
            Hide();
            return;
        }

        if (previewPanel != null)
            previewPanel.SetActive(true);

        if (spawnedCard != null)
            Destroy(spawnedCard);

        spawnedCard = Instantiate(cardPrefab);
        spawnedCard.transform.SetParent(cardSpawnPoint, false);

        RectTransform rt = spawnedCard.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(220f, 320f);
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;
        }
        else
        {
            spawnedCard.transform.localPosition = Vector3.zero;
            spawnedCard.transform.localScale = Vector3.one;
        }

        var sprite = PranksterSpriteDatabase.GetSprite(card.pranksterType, card.tier, card.category);
        Debug.Log("PREVIEW SPRITE RESULT | type=" + card.pranksterType +
          " | tier=" + card.tier +
          " | category=" + card.category +
          " | sprite=" + (sprite != null ? sprite.name : "NULL"));

        var view = spawnedCard.GetComponent<PranksterCardUIView>();

        Debug.Log("PREVIEW VIEW RESULT | view=" + (view != null ? "FOUND" : "NULL"));

        if (view != null)
            view.SetCharacterArt(sprite);
    }

    public void Hide()
    {
        if (spawnedCard != null)
        {
            Destroy(spawnedCard);
            spawnedCard = null;
        }

        if (previewPanel != null)
            previewPanel.SetActive(false);
    }
}