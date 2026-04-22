using UnityEngine;

public class DiscardPileDisplay : MonoBehaviour
{
    public GameObject topDiscardCard;
    public GameObject layer1;
    public GameObject layer2;
    public GameObject layer3;
    public GameObject emptyDiscardPile;

    public DeckManager deckManager;

    // BASE SPRITES
    public Sprite thiefSprite;
    public Sprite wizardSprite;
    public Sprite engineerSprite;
    public Sprite beastMasterSprite;
    public Sprite laborerSprite;
    public Sprite scribeSprite;

    // CREW LEADER (Tier 1)
    public Sprite thiefCrewLeaderSprite;
    public Sprite wizardCrewLeaderSprite;
    public Sprite engineerCrewLeaderSprite;
    public Sprite beastMasterCrewLeaderSprite;
    public Sprite laborerCrewLeaderSprite;
    public Sprite scribeCrewLeaderSprite;

    // EXPERT (Tier 2)
    public Sprite thiefExpertSprite;
    public Sprite wizardExpertSprite;
    public Sprite engineerExpertSprite;
    public Sprite beastMasterExpertSprite;
    public Sprite laborerExpertSprite;
    public Sprite scribeExpertSprite;

    // MASTER (Tier 3)
    public Sprite thiefMasterSprite;
    public Sprite wizardMasterSprite;
    public Sprite engineerMasterSprite;
    public Sprite beastMasterMasterSprite;
    public Sprite laborerMasterSprite;
    public Sprite scribeMasterSprite;

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
        {
            topDiscardRenderer = cardArtTransform.GetComponent<SpriteRenderer>();
        }

        if (topDiscardRenderer == null)
        {
            Debug.LogError("DiscardPileDisplay: TopDiscardCard/CardArt is missing a SpriteRenderer.");
        }

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

        Debug.Log("DiscardPileDisplay: Top discard card = " + topCard.pranksterType + " | tier = " + topCard.tier);

        topDiscardRenderer.sprite = GetSpriteForPrankster(topCard.pranksterType, topCard.tier);

        Debug.Log("DiscardPileDisplay: Assigned discard sprite = " + topDiscardRenderer.sprite);
    }

    private Sprite GetSpriteForPrankster(PranksterType type, int tier)
    {
        switch (type)
        {
            case PranksterType.Thief:
                switch (tier)
                {
                    case 1: return thiefCrewLeaderSprite != null ? thiefCrewLeaderSprite : thiefSprite;
                    case 2: return thiefExpertSprite != null ? thiefExpertSprite : thiefSprite;
                    case 3: return thiefMasterSprite != null ? thiefMasterSprite : thiefSprite;
                    default: return thiefSprite;
                }

            case PranksterType.Wizard:
                switch (tier)
                {
                    case 1: return wizardCrewLeaderSprite != null ? wizardCrewLeaderSprite : wizardSprite;
                    case 2: return wizardExpertSprite != null ? wizardExpertSprite : wizardSprite;
                    case 3: return wizardMasterSprite != null ? wizardMasterSprite : wizardSprite;
                    default: return wizardSprite;
                }

            case PranksterType.Engineer:
                switch (tier)
                {
                    case 1: return engineerCrewLeaderSprite != null ? engineerCrewLeaderSprite : engineerSprite;
                    case 2: return engineerExpertSprite != null ? engineerExpertSprite : engineerSprite;
                    case 3: return engineerMasterSprite != null ? engineerMasterSprite : engineerSprite;
                    default: return engineerSprite;
                }

            case PranksterType.BeastMaster:
                switch (tier)
                {
                    case 1: return beastMasterCrewLeaderSprite != null ? beastMasterCrewLeaderSprite : beastMasterSprite;
                    case 2: return beastMasterExpertSprite != null ? beastMasterExpertSprite : beastMasterSprite;
                    case 3: return beastMasterMasterSprite != null ? beastMasterMasterSprite : beastMasterSprite;
                    default: return beastMasterSprite;
                }

            case PranksterType.Laborer:
                switch (tier)
                {
                    case 1: return laborerCrewLeaderSprite != null ? laborerCrewLeaderSprite : laborerSprite;
                    case 2: return laborerExpertSprite != null ? laborerExpertSprite : laborerSprite;
                    case 3: return laborerMasterSprite != null ? laborerMasterSprite : laborerSprite;
                    default: return laborerSprite;
                }

            case PranksterType.Scribe:
                switch (tier)
                {
                    case 1: return scribeCrewLeaderSprite != null ? scribeCrewLeaderSprite : scribeSprite;
                    case 2: return scribeExpertSprite != null ? scribeExpertSprite : scribeSprite;
                    case 3: return scribeMasterSprite != null ? scribeMasterSprite : scribeSprite;
                    default: return scribeSprite;
                }

            default:
                return null;
        }
    }
}