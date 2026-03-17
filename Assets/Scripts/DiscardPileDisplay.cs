using UnityEngine;

public class DiscardPileDisplay : MonoBehaviour
{
    public GameObject topDiscardCard;
    public GameObject layer1;
    public GameObject layer2;
    public GameObject layer3;
    public GameObject emptyDiscardPile;

    public DeckManager deckManager;

    public Sprite thiefSprite;
    public Sprite wizardSprite;
    public Sprite engineerSprite;
    public Sprite beastMasterSprite;
    public Sprite laborerSprite;
    public Sprite scribeSprite;

    private SpriteRenderer topDiscardRenderer;

    void Start()
    {
        Transform cardArtTransform = topDiscardCard.transform.Find("CardArt");

        if (cardArtTransform != null)
        {
            topDiscardRenderer = cardArtTransform.GetComponent<SpriteRenderer>();
        }

        if (topDiscardRenderer == null)
        {
            Debug.LogError("TopDiscardCard/CardArt is missing a SpriteRenderer.");
        }
    }

    void Update()
    {
        int count = deckManager.GetDiscardCount();

        topDiscardCard.SetActive(count > 0);
        layer1.SetActive(count > 10);
        layer2.SetActive(count > 25);
        layer3.SetActive(count > 40);
        emptyDiscardPile.SetActive(count == 0);
    }

    public void UpdateTopDiscardCard()
{
    PranksterType? topCard = deckManager.GetTopDiscardCard();
    Debug.Log("Top discard card: " + topCard);

    if (topDiscardRenderer == null)
    {
        Debug.Log("topDiscardRenderer is NULL");
        return;
    }

    if (topCard == null)
    {
        Debug.Log("topCard is NULL, clearing sprite");
        topDiscardRenderer.sprite = null;
        return;
    }

    switch (topCard.Value)
    {
        case PranksterType.Thief:
            topDiscardRenderer.sprite = thiefSprite;
            break;

        case PranksterType.Wizard:
            topDiscardRenderer.sprite = wizardSprite;
            break;

        case PranksterType.Engineer:
            topDiscardRenderer.sprite = engineerSprite;
            break;

        case PranksterType.BeastMaster:
            topDiscardRenderer.sprite = beastMasterSprite;
            break;

        case PranksterType.Laborer:
            topDiscardRenderer.sprite = laborerSprite;
            break;

        case PranksterType.Scribe:
            topDiscardRenderer.sprite = scribeSprite;
            break;
    }

    Debug.Log("Assigned discard sprite: " + topDiscardRenderer.sprite);
}
}