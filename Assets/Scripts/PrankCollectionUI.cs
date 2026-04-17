using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PrankCollectionUI : MonoBehaviour
{
    public Transform contentParent;
    public GameObject prankCardPrefab;
    public ScrollRect scrollRect;

    void Start()
    {
        BuildCollection();
    }

    void BuildCollection()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        List<PrankCard> prankDeck = PrankDatabase.CreatePrankDeck();

        Debug.Log("PrankCollectionUI | prank count = " + prankDeck.Count);

        for (int i = 0; i < prankDeck.Count; i++)
        {
            GameObject card = Instantiate(prankCardPrefab, contentParent);

            Transform artTransform = card.transform.Find("CardArt");
            Transform countTransform = card.transform.Find("CountText");

            if (artTransform == null)
            {
                Debug.LogError("Missing child: CardArt");
                continue;
            }

            if (countTransform == null)
            {
                Debug.LogError("Missing child: CountText");
                continue;
            }

            Image art = artTransform.GetComponent<Image>();
            TMP_Text countText = countTransform.GetComponent<TMP_Text>();

            if (art == null)
            {
                Debug.LogError("CardArt missing Image component.");
                continue;
            }

            if (countText == null)
            {
                Debug.LogError("CountText missing TMP_Text component.");
                continue;
            }

            art.enabled = true;
            countText.enabled = true;

            art.sprite = prankDeck[i].cardSprite;
            countText.text = "0";

            Debug.Log("Spawned prank card: " + prankDeck[i].title + " | sprite = " + prankDeck[i].cardSprite);
        }

        Canvas.ForceUpdateCanvases();

        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }
}