using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PrankCollectionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject prankCardPrefab;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Card Layout")]
    [SerializeField] private Vector2 cardRootSize = new Vector2(600f, 400f);
    [SerializeField] private Vector2 cardArtAnchoredPosition = new Vector2(0f, 0f);
    [SerializeField] private Vector2 cardArtSize = new Vector2(500f, 330f);
    [SerializeField] private Vector2 cardFrameAnchoredPosition = new Vector2(0f, 0f);
    [SerializeField] private Vector2 cardFrameSize = new Vector2(558f, 396f);
    [SerializeField] private Vector2 countAnchoredPosition = new Vector2(0f, -235f);
    [SerializeField] private Vector2 countSize = new Vector2(200f, 20f);
    [SerializeField] private float countFontSize = 90f;

    [Header("Art Backdrop")]
    [SerializeField] private Sprite artBackdropSprite;
    [SerializeField] private Vector2 artBackdropAnchoredPosition = new Vector2(0f, 0f);
    [SerializeField] private Vector2 artBackdropSize = new Vector2(500f, 330f);

    [Header("Materials")]
    [SerializeField] private Material grayscaleMaterial;

    [Header("Frame Sprites")]
    [SerializeField] private Sprite bwFrameSprite;
    [SerializeField] private Sprite baseFrameSprite;
    [SerializeField] private Sprite silverFrameSprite;
    [SerializeField] private Sprite goldFrameSprite;

    private const string CardArtChildName = "CardArt";
    private const string CardFrameChildName = "CardFrame";
    private const string CountTextChildName = "CountText";
    private const string CardArtBackdropChildName = "CardArtBackdrop";

    public void BuildCollection()
    {
        if (!ValidateReferences())
            return;

        ClearExistingCards();

        List<PrankCard> prankDeck = PrankDatabase.CreatePrankDeck();
        PlayerProgressSave saveData = SaveSystem.Load();
        Dictionary<string, int> completionLookup = BuildCompletionLookup(saveData);

        Debug.Log("PrankCollectionUI | prank count = " + prankDeck.Count);

        for (int i = 0; i < prankDeck.Count; i++)
        {
            PrankCard prank = prankDeck[i];
            GameObject card = Instantiate(prankCardPrefab, contentParent);
            card.SetActive(true);

            ConfigureCardRoot(card);

            Transform artTransform = card.transform.Find(CardArtChildName);
            Transform frameTransform = card.transform.Find(CardFrameChildName);
            Transform countTransform = card.transform.Find(CountTextChildName);

            if (artTransform == null)
            {
                Debug.LogError("PrankCollectionUI: Missing child '" + CardArtChildName + "' on spawned card.");
                Destroy(card);
                continue;
            }

            if (frameTransform == null)
            {
                Debug.LogError("PrankCollectionUI: Missing child '" + CardFrameChildName + "' on spawned card.");
                Destroy(card);
                continue;
            }

            if (countTransform == null)
            {
                Debug.LogError("PrankCollectionUI: Missing child '" + CountTextChildName + "' on spawned card.");
                Destroy(card);
                continue;
            }

            Image artImage = artTransform.GetComponent<Image>();
            Image frameImage = frameTransform.GetComponent<Image>();
            TMP_Text countText = countTransform.GetComponent<TMP_Text>();

            if (artImage == null)
            {
                Debug.LogError("PrankCollectionUI: '" + CardArtChildName + "' is missing an Image component.");
                Destroy(card);
                continue;
            }

            if (frameImage == null)
            {
                Debug.LogError("PrankCollectionUI: '" + CardFrameChildName + "' is missing an Image component.");
                Destroy(card);
                continue;
            }

            if (countText == null)
            {
                Debug.LogError("PrankCollectionUI: '" + CountTextChildName + "' is missing a TMP_Text component.");
                Destroy(card);
                continue;
            }

            ConfigureArtBackdrop(card.transform, artTransform);
            ConfigureCardArt(artTransform, artImage, prank, completionLookup);
            ConfigureCardFrame(frameTransform, frameImage, completionLookup, prank.title);
            ConfigureCountText(countTransform, countText, completionLookup, prank.title);

            Debug.Log("Spawned prank card: " + prank.title + " | sprite = " + prank.cardSprite);
        }

        Canvas.ForceUpdateCanvases();

        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }

    private bool ValidateReferences()
    {
        if (contentParent == null)
        {
            Debug.LogError("PrankCollectionUI: Content Parent is not assigned.");
            return false;
        }

        if (prankCardPrefab == null)
        {
            Debug.LogError("PrankCollectionUI: Prank Card Prefab is not assigned.");
            return false;
        }

        return true;
    }

    private void ClearExistingCards()
    {
        for (int i = contentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(contentParent.GetChild(i).gameObject);
        }
    }

    private Dictionary<string, int> BuildCompletionLookup(PlayerProgressSave saveData)
    {
        Dictionary<string, int> lookup = new Dictionary<string, int>();

        if (saveData == null || saveData.prankCompletions == null)
            return lookup;

        for (int i = 0; i < saveData.prankCompletions.Count; i++)
        {
            PrankCompletionEntry entry = saveData.prankCompletions[i];

            if (entry == null || string.IsNullOrWhiteSpace(entry.prankTitle))
                continue;

            lookup[entry.prankTitle] = entry.timesCompleted;
        }

        return lookup;
    }

    private void ConfigureCardRoot(GameObject card)
    {
        RectTransform cardRect = card.GetComponent<RectTransform>();
        if (cardRect != null)
            cardRect.sizeDelta = cardRootSize;
    }

    private void ConfigureArtBackdrop(Transform cardRoot, Transform artTransform)
    {
        if (artBackdropSprite == null)
            return;

        Transform existingBackdrop = cardRoot.Find(CardArtBackdropChildName);
        Image backdropImage;
        RectTransform backdropRect;

        if (existingBackdrop == null)
        {
            GameObject backdropObject = new GameObject(
                CardArtBackdropChildName,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image));

            backdropObject.transform.SetParent(cardRoot, false);

            backdropRect = backdropObject.GetComponent<RectTransform>();
            backdropImage = backdropObject.GetComponent<Image>();

            backdropRect.anchorMin = new Vector2(0.5f, 0.5f);
            backdropRect.anchorMax = new Vector2(0.5f, 0.5f);
            backdropRect.pivot = new Vector2(0.5f, 0.5f);
        }
        else
        {
            backdropRect = existingBackdrop.GetComponent<RectTransform>();
            backdropImage = existingBackdrop.GetComponent<Image>();
        }

        backdropRect.anchoredPosition = artBackdropAnchoredPosition;
        backdropRect.sizeDelta = artBackdropSize;

        backdropImage.enabled = true;
        backdropImage.sprite = artBackdropSprite;
        backdropImage.color = Color.white;
        backdropImage.material = null;
        backdropImage.raycastTarget = false;
        backdropImage.type = Image.Type.Simple;
        backdropImage.preserveAspect = false;

        // Put backdrop directly behind CardArt.
        int artSiblingIndex = artTransform.GetSiblingIndex();
        backdropRect.SetSiblingIndex(artSiblingIndex);
    }

    private void ConfigureCardArt(
        Transform artTransform,
        Image artImage,
        PrankCard prank,
        Dictionary<string, int> completionLookup)
    {
        RectTransform artRect = artTransform.GetComponent<RectTransform>();
        if (artRect != null)
        {
            artRect.anchoredPosition = cardArtAnchoredPosition;
            artRect.sizeDelta = cardArtSize;
        }

        artImage.enabled = true;
        artImage.sprite = prank.cardSprite;
        artImage.color = Color.white;

        int completionCount = GetCompletionCount(completionLookup, prank.title);

        if (completionCount == 0)
            artImage.material = grayscaleMaterial;
        else
            artImage.material = null;
    }

    private void ConfigureCardFrame(
        Transform frameTransform,
        Image frameImage,
        Dictionary<string, int> completionLookup,
        string prankTitle)
    {
        RectTransform frameRect = frameTransform.GetComponent<RectTransform>();
        if (frameRect != null)
        {
            frameRect.anchoredPosition = cardFrameAnchoredPosition;
            frameRect.sizeDelta = cardFrameSize;
        }

        frameImage.enabled = true;
        frameImage.material = null;
        frameImage.color = Color.white;

        int completionCount = GetCompletionCount(completionLookup, prankTitle);

        if (completionCount == 0)
        {
            frameImage.sprite = bwFrameSprite;
        }
        else if (completionCount <= 4)
        {
            frameImage.sprite = baseFrameSprite;
        }
        else if (completionCount <= 9)
        {
            frameImage.sprite = silverFrameSprite;
        }
        else
        {
            frameImage.sprite = goldFrameSprite;
        }
    }

    private void ConfigureCountText(
        Transform countTransform,
        TMP_Text countText,
        Dictionary<string, int> completionLookup,
        string prankTitle)
    {
        RectTransform countRect = countTransform.GetComponent<RectTransform>();
        if (countRect != null)
        {
            countRect.anchoredPosition = countAnchoredPosition;
            countRect.sizeDelta = countSize;
        }

        countText.enabled = true;
        countText.fontSize = countFontSize;
        countText.alignment = TextAlignmentOptions.Center;

        int completionCount = GetCompletionCount(completionLookup, prankTitle);
        countText.text = completionCount.ToString();
    }

    private int GetCompletionCount(Dictionary<string, int> completionLookup, string prankTitle)
    {
        if (!string.IsNullOrWhiteSpace(prankTitle) &&
            completionLookup.TryGetValue(prankTitle, out int savedCount))
        {
            return savedCount;
        }

        return 0;
    }
}