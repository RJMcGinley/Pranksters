using UnityEngine;

public class DiscardPileDisplay : MonoBehaviour
{
    public GameObject topDiscardCard;
    public GameObject layer1;
    public GameObject layer2;
    public GameObject layer3;
    public GameObject emptyDiscardPile;

    public DeckManager deckManager;

    // BASE SPRITES ONLY
    public Sprite thiefSprite;
    public Sprite wizardSprite;
    public Sprite engineerSprite;
    public Sprite beastMasterSprite;
    public Sprite laborerSprite;
    public Sprite scribeSprite;

    private SpriteRenderer topDiscardRenderer;

    void Start()
    {
        if (topDiscardCard == null)
        {
            Debug.LogError("DiscardPileDisplay: topDiscardCard is not assigned.");
            return;
        }

        Transform cardArtTransform = topDiscardCard.transform.Find("CardArt");

        if (cardArtTransform != null)
            topDiscardRenderer = cardArtTransform.GetComponent<SpriteRenderer>();

        if (topDiscardRenderer == null)
            Debug.LogError("DiscardPileDisplay: TopDiscardCard/CardArt is missing a SpriteRenderer.");

        UpdateTopDiscardCard();
    }

    void Update()
    {
        if (deckManager == null)
            return;

        int count = deckManager.GetDiscardCount();

        if (topDiscardCard != null)
            topDiscardCard.SetActive(count > 0);

        if (layer1 != null)
            layer1.SetActive(count > 10);

        if (layer2 != null)
            layer2.SetActive(count > 25);

        if (layer3 != null)
            layer3.SetActive(count > 40);

        if (emptyDiscardPile != null)
            emptyDiscardPile.SetActive(count == 0);
    }

    public void UpdateTopDiscardCard()
    {
        if (deckManager == null)
        {
            Debug.LogWarning("DiscardPileDisplay: deckManager is NULL");
            return;
        }

        if (topDiscardRenderer == null)
        {
            Debug.LogWarning("DiscardPileDisplay: topDiscardRenderer is NULL");
            return;
        }

        PranksterDeckEntry topCard = deckManager.GetTopDiscardCard();

        if (topCard == null)
        {
            Debug.Log("DiscardPileDisplay: topCard is NULL, clearing sprite");
            topDiscardRenderer.sprite = null;
            return;
        }

        Debug.Log("DiscardPileDisplay: Top discard card = " + topCard.pranksterType +
                  " | tier = " + topCard.tier +
                  " | category = " + topCard.category);

        topDiscardRenderer.sprite = GetSpriteForEntry(topCard);

        Debug.Log("DiscardPileDisplay: Assigned discard sprite = " +
                  (topDiscardRenderer.sprite != null ? topDiscardRenderer.sprite.name : "NULL"));
    }

    private Sprite GetSpriteForEntry(PranksterDeckEntry entry)
    {
        if (entry == null)
            return null;

        if (entry.tier <= 0)
            return GetBaseSprite(entry.pranksterType);

        if (entry.category == PranksterUnlockCategory.FavorOffer)
            return GetFavorOfferSprite(entry.pranksterType, entry.tier);

        if (entry.category == PranksterUnlockCategory.Discard)
            return GetDiscardSprite(entry.pranksterType, entry.tier);

        return GetPrankCompletionSprite(entry.pranksterType, entry.tier);
    }

    private Sprite GetBaseSprite(PranksterType type)
    {
        switch (type)
        {
            case PranksterType.Thief: return thiefSprite;
            case PranksterType.Wizard: return wizardSprite;
            case PranksterType.Engineer: return engineerSprite;
            case PranksterType.BeastMaster: return beastMasterSprite;
            case PranksterType.Laborer: return laborerSprite;
            case PranksterType.Scribe: return scribeSprite;
            default: return null;
        }
    }

    private Sprite GetPrankCompletionSprite(PranksterType type, int tier)
    {
        string pranksterName = GetResourcePranksterName(type);
        string suffix = "";

        switch (tier)
        {
            case 1: suffix = "Crewleader"; break;
            case 2: suffix = "Expert"; break;
            case 3: suffix = "Master"; break;
            default: return GetBaseSprite(type);
        }

        Sprite sprite = Resources.Load<Sprite>("UnlockCards/" + pranksterName + suffix);

        if (sprite == null)
        {
            Debug.LogWarning("Missing prank-completion sprite: UnlockCards/" + pranksterName + suffix);
            return GetBaseSprite(type);
        }

        return sprite;
    }

    private Sprite GetFavorOfferSprite(PranksterType type, int tier)
    {
        string pranksterName = GetResourcePranksterName(type);
        string suffix = "";

        switch (tier)
        {
            case 1: suffix = "Assistant"; break;
            case 2: suffix = "Strategist"; break;
            case 3: suffix = "Advisor"; break;
            default: return GetBaseSprite(type);
        }

        Sprite sprite = Resources.Load<Sprite>("UnlockCards/" + pranksterName + suffix);

        if (sprite == null)
        {
            Debug.LogWarning("Missing favor sprite: UnlockCards/" + pranksterName + suffix);
            return GetBaseSprite(type);
        }

        return sprite;
    }

    private string GetResourcePranksterName(PranksterType type)
    {
        switch (type)
        {
            case PranksterType.BeastMaster:
                return "Beastmaster";
            default:
                return type.ToString();
        }
    }

    private Sprite GetDiscardSprite(PranksterType type, int tier)
    {
        string pranksterName = GetResourcePranksterName(type);
        string suffix = "";

        switch (tier)
        {
            case 1: suffix = "Hustler"; break;
            case 2: suffix = "Opportunist"; break;
            case 3: suffix = "Manipulator"; break;
            default: return GetBaseSprite(type);
        }

        Sprite sprite = Resources.Load<Sprite>("UnlockCards/" + pranksterName + suffix);

        if (sprite == null)
        {
            Debug.LogWarning("Missing discard sprite: UnlockCards/" + pranksterName + suffix);
            return GetBaseSprite(type);
        }

        return sprite;
    }




}