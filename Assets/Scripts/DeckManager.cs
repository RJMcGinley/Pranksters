using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Video;
using System.IO;




public class DeckManager : MonoBehaviour
{
    List<PranksterDeckEntry> deck = new List<PranksterDeckEntry>();
    public TurnManager turnManager;
    List<PrankCard> prankDeck = new List<PrankCard>();
    List<PrankCard> activePranks = new List<PrankCard>();
    List<PranksterDeckEntry> discardPile = new List<PranksterDeckEntry>();
    List<PranksterType> outOfPlayPranksters = new List<PranksterType>();
    public PendingChoiceType pendingChoice = PendingChoiceType.None;
    int lastPrankCompleterIndex = -1;
    int selectedSwapHandIndex = -1;
    bool gameOver = false;
    PrankCard finalCompletedPrank = null;
    public HandDisplay handDisplay;
    public DiscardPileDisplay discardPileDisplay;
    public GameObject prankCardPrefab;
    public Transform activePrankDisplay;
    public float prankCardSpacing = 2.0f;
    public Vector3 prankCardScale = new Vector3(0.28f, 0.28f, 1f);
    public TextMeshProUGUI turnText;

    public GameObject filledMarker1;
    public GameObject filledMarker2;
    public GameObject filledMarker3;
    public TextMeshProUGUI activeFavorPointsText;

    public Sprite thiefIcon;
    public Sprite engineerIcon;
    public Sprite laborerIcon;
    public Sprite scribeIcon;
    public Sprite wizardIcon;
    public Sprite beastmasterIcon;  

    public Image filledMarker1Image;
    public Image filledMarker2Image;
    public Image filledMarker3Image;

    public OpponentDisplayManager opponentDisplayManager;

    public TextMeshProUGUI activeCompletedPranksText;
    public TextMeshProUGUI activeRenownPointsText;
    public TextMeshProUGUI activePlayerLabelText;
    public PrankPreviewPanel prankPreviewPanel;

    public GameObject endGameCanvas;

    [Header("End Game Scoring Panel")]
    public GameObject endGameScoringPanel;

    public GameObject player1Row;
    public GameObject player2Row;
    public GameObject player3Row;
    public GameObject player4Row;

    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player1PrankPointsText;
    public TextMeshProUGUI player1FavorPointsText;
    public TextMeshProUGUI player1TotalPointsText;

    public TextMeshProUGUI player2NameText;
    public TextMeshProUGUI player2PrankPointsText;
    public TextMeshProUGUI player2FavorPointsText;
    public TextMeshProUGUI player2TotalPointsText;

    public TextMeshProUGUI player3NameText;
    public TextMeshProUGUI player3PrankPointsText;
    public TextMeshProUGUI player3FavorPointsText;
    public TextMeshProUGUI player3TotalPointsText;

    public TextMeshProUGUI player4NameText;
    public TextMeshProUGUI player4PrankPointsText;
    public TextMeshProUGUI player4FavorPointsText;
    public TextMeshProUGUI player4TotalPointsText;

    public Image finalPrankImage;
    public GameObject endTurnButton;
    public int hoveredPrankIndex = -1;

    public bool hasTakenActionThisTurn = false;

    public GameObject drawDeckHighlight;
    public GameObject discardPileHighlight;
    public GameObject favorAreaHighlight;
    public GameObject[] prankHighlights;

    public GameObject drawDeckLabel;
    public GameObject discardPileLabel;
    public GameObject favorAreaLabel;

    public bool actionLabelsEnabled = true;
    private int highlightSuppressionCount = 0;
    public TextMeshProUGUI favorPreviewText;
    public GameObject completablePrankHighlightPrefab;

    int selectedSwapPlayerIndex = -1;
    int selectedSwapFavorIndex = -1;

    public OpponentPreviewPanel opponentPreviewPanel;
    public NextPlayerPanelController nextPlayerPanelController;
    public SettingsMenuController settingsMenuController;

    // SwapPrankster temp state
    private List<PranksterType> originalHandSnapshot = null;
    private List<PranksterDeckEntry> tempSwapHand = null;
    private PranksterDeckEntry pendingIncomingPrankster;
    private bool isInSwapHandSelection = false;

    public BotManager botManager;
    public GameObject gameCanvas;
    public bool isRulesPanelOpen = false;

    private PlayerProgressSave player1ProgressSave;
    private Dictionary<PranksterType, int> player1FavorPointsThisGame = new Dictionary<PranksterType, int>();
    private Dictionary<PranksterType, int> player1DiscardCountsThisGame = new Dictionary<PranksterType, int>();
    private Dictionary<PranksterType, int> player1AvailableServiceUsesThisGame = new Dictionary<PranksterType, int>();

    public UnlockRevealPanelController unlockRevealPanelController;
    public EndOfRoundPanelController endOfRoundPanelController;

    private int pendingRoundDealerIndex = -1;
    private int pendingRoundFirstPlayerIndex = -1;
    private bool isEndOfRoundPending = false;

    private GameLocationType pendingRoundLocation = GameLocationType.RebelWorkshop;

    public VideoPlayer winVideoPlayer;
    public GameObject winCutsceneCanvas;
    public RenderTexture winCutsceneRenderTexture; 

    private PranksterType selectedAvailableServiceType;
    private AvailableServicePanelAssignmentController activeServicePanelController;
    private List<int> temporarilyAssignedServiceHandIndexes = new List<int>();
    private AvailableServiceSlotCollider activeAvailableServiceSlotCollider;
    [SerializeField] private AvailableServicesPanelController availableServicesPanelController;
    public TextMeshPro crewCapacityText;
    private bool availableServicesPanelOpen = false;

    private bool selectingInactiveInfluenceService = false;
    private bool viewingInactiveInfluenceServicePanel = false;

    [SerializeField] private LifetimeNotorietyCrewSizeButton lifetimeCrewSizeButton;
    [SerializeField] private SpendInfluenceButton_UseInactiveServices inactiveServicesButton;
    [SerializeField] private LifetimeNotorietyPesterMayorButton pesterMayorButton;
    private Dictionary<PranksterType, bool> serviceAvailabilityThisRound =
        new Dictionary<PranksterType, bool>();

    public PrankCompletionShowcasePanel prankCompletionShowcasePanel;
    public WantedBoardPanelController wantedBoardPanelController;

    [Header("Available Service Instructions")]
    [SerializeField] private AvailableServiceInstructionPanel availableServiceInstructionPanel;

    private bool wantedBoardOpen = false;
    public WantedJailController wantedJailController;

    public bool IsWantedBoardOpen()
    {
        return wantedBoardOpen;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    Player GetCurrentPlayer()
    {
        return turnManager.GetCurrentPlayer();
    }


    void Start()
{
    if (favorPreviewText != null)
        favorPreviewText.gameObject.SetActive(false);

    if (endGameScoringPanel != null)
        endGameScoringPanel.SetActive(false);

    if (endGameCanvas != null)
        endGameCanvas.SetActive(false);
}



    void BuildPranksterDeck()
{
    deck.Clear();

    Debug.Log("BUILD DECK START");

    PranksterType[] pranksters =
    {
        PranksterType.Thief,
        PranksterType.Wizard,
        PranksterType.Engineer,
        PranksterType.BeastMaster,
        PranksterType.Laborer,
        PranksterType.Scribe
    };

    foreach (PranksterType prankster in pranksters)
    {
        List<PranksterDeckEntry> usableUnlockCards = new List<PranksterDeckEntry>();

        for (int tier = 1; tier <= 3; tier++)
        {
            if (SaveSystem.IsPranksterUnlockUsable(prankster.ToString(), tier, PranksterUnlockCategory.PrankCompletion))
            {
                usableUnlockCards.Add(new PranksterDeckEntry
                {
                    pranksterType = prankster,
                    tier = tier,
                    category = PranksterUnlockCategory.PrankCompletion
                });
            }

            if (SaveSystem.IsPranksterUnlockUsable(prankster.ToString(), tier, PranksterUnlockCategory.FavorOffer))
            {
                usableUnlockCards.Add(new PranksterDeckEntry
                {
                    pranksterType = prankster,
                    tier = tier,
                    category = PranksterUnlockCategory.FavorOffer
                });
            }
            //Discard unlocks are currently inactive / legacy.
            // if (SaveSystem.IsPranksterUnlockUsable(prankster.ToString(), tier, PranksterUnlockCategory.Discard))
            // {
            //     usableUnlockCards.Add(new PranksterDeckEntry
            //     {
            //         pranksterType = prankster,
            //         tier = tier,
            //         category = PranksterUnlockCategory.Discard
            //     });
            // }

            if (SaveSystem.IsPranksterUnlockUsable(prankster.ToString(), tier, PranksterUnlockCategory.AvailableService))
            {
                usableUnlockCards.Add(new PranksterDeckEntry
                {
                    pranksterType = prankster,
                    tier = tier,
                    category = PranksterUnlockCategory.AvailableService
                });
            }
        }

        int baseCardCount = 9 - usableUnlockCards.Count;

        if (baseCardCount < 0)
            baseCardCount = 0;

        Debug.Log("BUILD DECK | " + prankster +
                  " | unlocks=" + usableUnlockCards.Count +
                  " | base=" + baseCardCount);

        // Add base cards
        for (int i = 0; i < baseCardCount; i++)
        {
            deck.Add(new PranksterDeckEntry
            {
                pranksterType = prankster,
                tier = 0,
                category = PranksterUnlockCategory.PrankCompletion
            });
        }

        // Add unlocked cards
        for (int i = 0; i < usableUnlockCards.Count; i++)
        {
            PranksterDeckEntry unlock = usableUnlockCards[i];

            deck.Add(new PranksterDeckEntry
            {
                pranksterType = unlock.pranksterType,
                tier = unlock.tier,
                category = unlock.category
            });

            Debug.Log("  + UNLOCK | type=" + unlock.pranksterType +
                      " | tier=" + unlock.tier +
                      " | category=" + unlock.category);
        }
    }

    Debug.Log("BUILD DECK COMPLETE | total cards = " + deck.Count);
}


    void ShufflePranksterDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(0, deck.Count);

            PranksterDeckEntry temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }

        Debug.Log("Deck shuffled");
    }

    void ShufflePrankDeck()
{
    for (int i = 0; i < prankDeck.Count; i++)
    {
        int randomIndex = Random.Range(0, prankDeck.Count);

        PrankCard temp = prankDeck[i];
        prankDeck[i] = prankDeck[randomIndex];
        prankDeck[randomIndex] = temp;
    }

    Debug.Log("Prank deck shuffled");
}

    void DealActivePranks()
{
    activePranks.Clear();

    int prankCount = GetActivePrankCountForCurrentLocation();

    for (int i = 0; i < prankCount; i++)
    {
        if (prankDeck.Count == 0)
        {
            Debug.LogWarning("Prank deck empty.");
            return;
        }

        activePranks.Add(prankDeck[0]);
        prankDeck.RemoveAt(0);
    }
}


    void DrawCard()
{
    if (deck.Count == 0)
    {
        if (discardPile.Count > 0)
        {
            ReshuffleDiscardIntoDeck();
        }
        else
        {
            Debug.Log("Deck and discard pile are both empty.");
            return;
        }
    }

    PranksterDeckEntry drawnCard = deck[0];

    GetCurrentPlayer().hand.Add(drawnCard);
    deck.RemoveAt(0);

    SortCurrentPlayerHand();

    Debug.Log("Drew card: " + drawnCard);
}


    void RefillHandToFour()
{
    while (GetCurrentPlayer().hand.Count < 4)
    {
        int handCountBefore = GetCurrentPlayer().hand.Count;

        DrawCard();

        if (GetCurrentPlayer().hand.Count == handCountBefore)
        {
            Debug.Log("Could not draw more cards. Stopping refill.");
            break;
        }
    }
}


    public int CalculateFavorPoints(PranksterType pranksterType)
{
    int total = 0;

    foreach (PrankCard prank in activePranks)
    {
        int prankCount = 0;

        foreach (PranksterType requiredPrankster in prank.requiredPranksters)
        {
            if (requiredPrankster == pranksterType)
            {
                prankCount++;
                total++;
            }
        }
    }

    return total;
}


    IEnumerator OfferFavor(int handIndex)
{
    Player player = GetCurrentPlayer();

    if (handIndex < 0 || handIndex >= player.hand.Count)
    {
        Debug.LogWarning("Invalid hand index");
        yield break;
    }

    PranksterDeckEntry offeredCard = player.hand[handIndex];

    player.hand.RemoveAt(handIndex);
    player.favorArea.Add(offeredCard);

    int baseFavor = CalculateFavorPoints(offeredCard.pranksterType);
    int favorGained = CalculateTotalFavorForCard(offeredCard);
    int bonusFavor = favorGained - baseFavor;

    player.favorPoints += favorGained;

    if (turnManager.currentPlayerIndex == 0)
    {
        if (!player1FavorPointsThisGame.ContainsKey(offeredCard.pranksterType))
            player1FavorPointsThisGame[offeredCard.pranksterType] = 0;

        player1FavorPointsThisGame[offeredCard.pranksterType] += favorGained;
    }

    UpdateActiveFavorDisplay();
    RefreshAllDisplays();

    if (favorPreviewText != null)
        favorPreviewText.gameObject.SetActive(false);

    Debug.Log("Offered as favor: " + offeredCard.pranksterType +
              " | tier=" + offeredCard.tier +
              " | category=" + offeredCard.category);

    Debug.Log("Base favor: " + baseFavor +
              " | Bonus: " + bonusFavor +
              " | Total gained: " + favorGained);

    Debug.Log("Total favor points: " + player.favorPoints);

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayFavorReward();

    yield return new WaitForSeconds(0.15f);

    if (!player.isBot && AudioManager.Instance != null && Random.value < 0.6f)
        AudioManager.Instance.PlayFavorVoiceLine();

    yield return new WaitForSeconds(0.15f);

    // yield return StartCoroutine(RefillHandToFourOneCardAtATime(0.3f));

    RefreshAllDisplays();

    if (GetCurrentPlayer().isBot)
    {
        Debug.Log("BOT offer favor sequence finished");
        yield break;
    }

    FinishActionAndWaitForEndTurn();
}


    public bool CanCompletePrank(int prankIndex)
    {
        Player player = GetCurrentPlayer();

        if (prankIndex < 0 || prankIndex >= activePranks.Count)
        {
            return false;
        }

        PrankCard selectedPrank = activePranks[prankIndex];

        List<PranksterType> tempHand = ConvertHandToTypes(player.hand);

        foreach (PranksterType requiredPrankster in selectedPrank.requiredPranksters)
        {
            if (tempHand.Contains(requiredPrankster))
            {
                tempHand.Remove(requiredPrankster);
            }
            else
            {
                return false;
            }
        }

        return true;
    }


    bool AttemptCompletePrank(int prankIndex)
{
    if (prankIndex < 0 || prankIndex >= activePranks.Count)
    {
        Debug.Log("Invalid prank index.");
        return false;
    }

    if (!CanCompletePrank(prankIndex))
    {
        Debug.Log("Cannot complete prank.");
        return false;
    }

    CompletePrank(prankIndex);
    return true;
}

    


    void CompletePrank(int prankIndex)
{
    Player player = GetCurrentPlayer();

    PrankCard completedPrank = activePranks[prankIndex];
    finalCompletedPrank = completedPrank;
    lastPrankCompleterIndex = turnManager.currentPlayerIndex;

    int bonusRenown = 0;
    List<PranksterDeckEntry> usedCards = new List<PranksterDeckEntry>();

    Debug.Log("COMPLETE PRANK START | prank=" + completedPrank.title +
              " | baseRenown=" + completedPrank.renownPoints);

    foreach (PranksterType required in completedPrank.requiredPranksters)
    {
        int index = -1;

        for (int i = 0; i < player.hand.Count; i++)
        {
            if (player.hand[i].pranksterType == required)
            {
                index = i;
                break;
            }
        }

        if (index >= 0)
        {
            PranksterDeckEntry card = player.hand[index];

            Debug.Log("PRANK USE | required=" + required +
                      " | using=" + card.pranksterType +
                      " | tier=" + card.tier);

            usedCards.Add(card);
            player.hand.RemoveAt(index);
            discardPile.Add(card);
        }
        else
        {
            Debug.LogError("Missing required card: " + required);
        }
    }

    player.completedPranks.Add(completedPrank);

    // Add base prank value
    player.renownPoints += completedPrank.renownPoints;

    Debug.Log("RENOWN AFTER BASE | totalRenown=" + player.renownPoints);

    // Add upgrade bonuses
    for (int i = 0; i < usedCards.Count; i++)
    {
        int cardBonus = PranksterUnlockRules.GetRenownBonus(usedCards[i]);

        Debug.Log("RENOWN BONUS | card=" + usedCards[i].pranksterType +
                  " | tier=" + usedCards[i].tier +
                  " | bonus=" + cardBonus);

        bonusRenown += cardBonus;
    }

    player.renownPoints += bonusRenown;

    Debug.Log("COMPLETE PRANK END | prank=" + completedPrank.title +
              " | totalBonusRenown=" + bonusRenown +
              " | finalRenown=" + player.renownPoints);

    activePranks.RemoveAt(prankIndex);
    ShowActivePrankCards();

    float showcaseDuration = 1.75f;

    if (AudioManager.Instance != null)
        showcaseDuration = AudioManager.Instance.PlayPrankCompletionSound(completedPrank.title);

    if (wantedJailController != null)
        wantedJailController.HideJailDisplay();

    if (wantedBoardPanelController != null &&
        wantedBoardPanelController.wantedDisplayPanelController != null)
    {
        wantedBoardPanelController.wantedDisplayPanelController.Hide();
    }

    if (prankCompletionShowcasePanel != null)
        prankCompletionShowcasePanel.Show(completedPrank.cardSprite, showcaseDuration);

    if (HasReachedMayorBreakingPoint())
    {
        if (GetCurrentPlayer().isBot && botManager != null)
            botManager.NotifyBotActionHandledTurnFlow();

        StartCoroutine(TriggerEndGameAfterShowcase(showcaseDuration));
        return;
    }

    if (activePranks.Count == 1)
    {
        isEndOfRoundPending = true;
        Debug.Log("END OF ROUND FLAG SET TRUE");

        StartCoroutine(OpenWantedBoardThenContinue(completedPrank, showcaseDuration));
        return;
    }

    // Continue normal flow if round is not ending
    StartCoroutine(OpenWantedBoardThenContinue(completedPrank, showcaseDuration));
}


    bool HasPlayerCompletedFourPranks()
{
    return GetCurrentPlayer().completedPranks.Count >= 4;
}


    void DrawFromDiscard()
{
    Player player = GetCurrentPlayer();

    if (discardPile.Count == 0)
    {
        Debug.Log("Discard empty");
        return;
    }

    PranksterDeckEntry card = discardPile[discardPile.Count - 1];

    discardPile.RemoveAt(discardPile.Count - 1);
    player.hand.Add(card);

    SortCurrentPlayerHand();

    Debug.Log("Drew from discard pile: " + card.pranksterType + " | tier=" + card.tier);
    RefreshAllDisplays();
}


    void DiscardCardFromHand(int handIndex)
{
    Player player = GetCurrentPlayer();

    if (handIndex < 0 || handIndex >= player.hand.Count)
    {
        Debug.Log("Invalid hand index");
        return;
    }

    PranksterDeckEntry card = player.hand[handIndex];
    PranksterType cardType = card.pranksterType;

    if (turnManager.currentPlayerIndex == 0)
    {
        if (!player1DiscardCountsThisGame.ContainsKey(cardType))
            player1DiscardCountsThisGame[cardType] = 0;

        player1DiscardCountsThisGame[cardType]++;
    }

    int discardFavorBonus = 0;
    int discardRenownBonus = 0;

    if (card.category == PranksterUnlockCategory.Discard)
    {
        discardFavorBonus = PranksterUnlockRules.GetDiscardFavorBonus(card);
        discardRenownBonus = PranksterUnlockRules.GetDiscardRenownBonus(card);
    }

    if (discardFavorBonus > 0)
        player.favorPoints += discardFavorBonus;

    if (discardRenownBonus > 0)
        player.renownPoints += discardRenownBonus;

    player.hand.RemoveAt(handIndex);
    discardPile.Add(card);

    discardPileDisplay.UpdateTopDiscardCard();

    Debug.Log("Discarded: " + cardType +
              " | tier=" + card.tier +
              " | category=" + card.category);

    Debug.Log("Discard bonuses | favor=" + discardFavorBonus +
              " | renown=" + discardRenownBonus);

    Debug.Log("Progression tracking | discard count recorded for " + cardType +
              " | no favor progression added from discard bonuses");

    Debug.Log("Player totals after discard | favor=" + player.favorPoints +
              " | renown=" + player.renownPoints);

    RefreshAllDisplays();
}


    void ShowDiscardPile()
{
    Debug.Log("Discard pile:");

    foreach (PranksterDeckEntry card in discardPile)
    {
        Debug.Log(card.pranksterType + " | tier=" + card.tier);
    }
}

    void DealStartingHands()
{
    for (int playerIndex = 0; playerIndex < turnManager.players.Count; playerIndex++)
    {
        Player player = turnManager.players[playerIndex];

        for (int i = 0; i < 4; i++)
        {
            if (deck.Count == 0)
            {
                Debug.LogWarning("Prankster deck ran out while dealing starting hands.");
                return;
            }

            player.hand.Add(deck[0]);
            deck.RemoveAt(0);
        }

        SortHand(player);
    }
}


    void ShowAllPlayerHands()
{
    for (int i = 0; i < turnManager.players.Count; i++)
    {
        Debug.Log("Player " + (i + 1) + " hand:");

        foreach (PranksterDeckEntry card in turnManager.players[i].hand)
        {     
            Debug.Log(card.pranksterType + " (Tier " + card.tier + ")");
        }
    }
}

    void StartPlayerTurn()
{
    hasTakenActionThisTurn = false;

    hoveredPrankIndex = -1;
    highlightSuppressionCount = 0;

    if (prankPreviewPanel != null)
        prankPreviewPanel.Hide();

    if (opponentPreviewPanel != null)
    {
        opponentPreviewPanel.UnlockSwap();
        opponentPreviewPanel.Hide();
    }

    SetAllPrankHighlightsVisible(true);

    if (endTurnButton != null)
        endTurnButton.SetActive(false);

    Debug.Log("==================================================");
    Debug.Log("PLAYER " + (turnManager.currentPlayerIndex + 1) + " TURN");
    Debug.Log("==================================================");

    pendingChoice = PendingChoiceType.ChooseAction;
    Debug.Log("pendingChoice set to ChooseAction from StartPlayerTurn");

    RefreshAllDisplays();

    highlightSuppressionCount = 0;

    ShowActivePrankCards();
    RefreshAllHighlights();

    if (turnText != null)
    {
        turnText.text = "";
        turnText.gameObject.SetActive(false);
    }

    if (AudioManager.Instance != null)
    {
        Player currentPlayer = GetCurrentPlayer();

        AudioManager.Instance.PlayPlayerTurnVoice(
            currentPlayer.playerName,
            turnManager.currentPlayerIndex,
            currentPlayer.isBot
        );
    }

    Debug.Log("Player " + (turnManager.currentPlayerIndex + 1) + "'s turn.");
    ShowCurrentPlayerHand();
    ShowTopDiscardCard();
    ShowAllFavorAreas();

    Debug.Log("Can click draw pile: " + CanClickDrawPile());
    Debug.Log("Can click discard pile: " + CanClickDiscardPile());
    Debug.Log("Turn state reset to: " + pendingChoice);

    StartCoroutine(RefreshHighlightsNextFrame());

    IEnumerator RefreshHighlightsNextFrame()
    {
        yield return null;
        RefreshAllHighlights();
    }

    // ===== BOT TURN TRIGGER =====
if (GetCurrentPlayer().isBot)
{
    Debug.Log("BOT TURN DETECTED");

    pendingChoice = PendingChoiceType.None;
    highlightSuppressionCount = 1;

    if (prankPreviewPanel != null)
        prankPreviewPanel.Hide();

    if (opponentPreviewPanel != null)
    {
        opponentPreviewPanel.UnlockSwap();
        opponentPreviewPanel.Hide();
    }

    RefreshAllHighlights();

    if (botManager != null)
    {
        botManager.StartBotTurn();
    }
    else
    {
        Debug.LogWarning("BotManager not assigned in DeckManager!");
    }

    return;
}
}

void FinishActionAndWaitForEndTurn()
{
    hasTakenActionThisTurn = true;
    pendingChoice = PendingChoiceType.None;

    RefreshAllDisplays();
    ShowActivePrankCards();
    RefreshAllHighlights();

    if (endTurnButton != null)
        endTurnButton.SetActive(true);

    Debug.Log("Action complete. Waiting for End Turn button.");
}


void EndPlayerTurn()
{
    turnManager.NextPlayer();

    if (opponentDisplayManager != null)
        opponentDisplayManager.RefreshDisplays();

    StartPlayerTurn();
}

void StartDrawFromDeckTurn()
{
    if (!CanClickDrawPile())
    {
        Debug.Log("Draw pile is not a valid choice right now. pendingChoice = " + pendingChoice +
          ", deck.Count = " + deck.Count +
          ", discardPile.Count = " + discardPile.Count);
        return;
    }

    DrawCard();

    if (handDisplay != null)
        handDisplay.ShowCurrentPlayerHand();

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayDrawCardAction();

    if (pendingRoundLocation == GameLocationType.ForestClearing)
    {
        StartCoroutine(ForestClearingSecondDraw());
        return;
    }

    FinishDrawFromDeckTurn();
}

 
void ResolveDiscardChoice(int discardHandIndex)
{
    if (pendingChoice != PendingChoiceType.ChooseDiscardFromHand)
        return;

    Debug.Log("Discard choice selected: hand index " + discardHandIndex);

    DiscardCardFromHand(discardHandIndex);

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayDiscardCard();

    handDisplay.ShowCurrentPlayerHand();
    discardPileDisplay.UpdateTopDiscardCard();
    RefreshCrewCapacityDisplay();

    ContinueDiscardingUntilHandAtMax();
}

void Update()
{
    if (Input.GetKeyDown(KeyCode.X))
    {
        Debug.Log("X pressed. Current pendingChoice = " + pendingChoice);
    }

    if (gameOver)
        return;

    // ==============================
    // GLOBAL CANCEL FOR SWAP FLOW
    // ==============================
    if ((pendingChoice == PendingChoiceType.ChooseSwapOpponent ||
         pendingChoice == PendingChoiceType.ChooseSwapTarget ||
         pendingChoice == PendingChoiceType.ChooseSwapHandCard) &&
        Input.GetKeyDown(KeyCode.X))
    {
        CancelSwapPreview();
        return;
    }

    if (pendingChoice == PendingChoiceType.ChooseDiscardFromHand)
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { ResolveDiscardChoice(0); return; }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { ResolveDiscardChoice(1); return; }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { ResolveDiscardChoice(2); return; }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { ResolveDiscardChoice(3); return; }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { ResolveDiscardChoice(4); return; }
    }

    if (pendingChoice == PendingChoiceType.ChooseDiscardAfterDrawFromDiscard)
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { ResolveDiscardAfterDrawFromDiscard(0); return; }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { ResolveDiscardAfterDrawFromDiscard(1); return; }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { ResolveDiscardAfterDrawFromDiscard(2); return; }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { ResolveDiscardAfterDrawFromDiscard(3); return; }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { ResolveDiscardAfterDrawFromDiscard(4); return; }
    }

    if (pendingChoice == PendingChoiceType.ChooseFavorCard)
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { ResolveFavorChoice(0); return; }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { ResolveFavorChoice(1); return; }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { ResolveFavorChoice(2); return; }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { ResolveFavorChoice(3); return; }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { ResolveFavorChoice(4); return; }
    }

    if (pendingChoice == PendingChoiceType.ChoosePrankToComplete)
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { ResolvePrankChoice(0); return; }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { ResolvePrankChoice(1); return; }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { ResolvePrankChoice(2); return; }
    }

    if (pendingChoice == PendingChoiceType.ChooseSwapOpponent)
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { ResolveSwapOpponentChoice(0); return; }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { ResolveSwapOpponentChoice(1); return; }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { ResolveSwapOpponentChoice(2); return; }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { ResolveSwapOpponentChoice(3); return; }
    }

    if (pendingChoice == PendingChoiceType.ChooseSwapTarget)
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { ResolveSwapTargetChoice(0); return; }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { ResolveSwapTargetChoice(1); return; }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { ResolveSwapTargetChoice(2); return; }
    }

    if (pendingChoice == PendingChoiceType.ChooseSwapHandCard)
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { ResolveSwapHandChoice(0); return; }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { ResolveSwapHandChoice(1); return; }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { ResolveSwapHandChoice(2); return; }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { ResolveSwapHandChoice(3); return; }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { ResolveSwapHandChoice(4); return; }
    }

    if (pendingChoice == PendingChoiceType.ChooseAvailableServiceCard)
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { ResolveAvailableServiceCardChoice(0); return; }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { ResolveAvailableServiceCardChoice(1); return; }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { ResolveAvailableServiceCardChoice(2); return; }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { ResolveAvailableServiceCardChoice(3); return; }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { ResolveAvailableServiceCardChoice(4); return; }
    }

    if (pendingChoice == PendingChoiceType.ChooseAction && Input.GetKeyDown(KeyCode.P))
    {
        LogSeparator("PLAYER ACTION: Draw from prankster deck");
        StartDrawFromDeckTurn();
    }

    if (pendingChoice == PendingChoiceType.ChooseAction && Input.GetKeyDown(KeyCode.D))
    {
        LogSeparator("PLAYER ACTION: Draw from discard pile");
        StartDrawFromDiscardTurn();
    }

    if (pendingChoice == PendingChoiceType.ChooseAction && Input.GetKeyDown(KeyCode.O))
    {
        LogSeparator("PLAYER ACTION: Offer favor");
        StartOfferFavorTurn();
    }

    if (pendingChoice == PendingChoiceType.ChooseAction && Input.GetKeyDown(KeyCode.C))
    {
        LogSeparator("PLAYER ACTION: Complete prank");
        StartCompletePrankTurn();
    }

    if (pendingChoice == PendingChoiceType.ChooseAction && Input.GetKeyDown(KeyCode.S))
    {
        LogSeparator("PLAYER ACTION: Swap favor");
        StartSwapFavorTurn();
    }

    if (Input.GetKeyDown(KeyCode.G))
    {
        PrintGameState();
    }

    if (Input.GetKeyDown(KeyCode.B))
    {
        Debug.Log("B pressed. Current pendingChoice = " + pendingChoice);

        if (pendingChoice == PendingChoiceType.ChooseAction)
        {
            StartAvailableServiceTurn(PranksterType.BeastMaster);
        }
        else
        {
            return;
        }
    }
}


void ShowCurrentPlayerHand()
{
    Player player = GetCurrentPlayer();

    string handText = "";

    for (int i = 0; i < player.hand.Count; i++)
    {
        handText += "[" + (i + 1) + "] " + player.hand[i];

        if (i < player.hand.Count - 1)
        {
            handText += ", ";
        }
    }

    Debug.Log("Player " + (turnManager.currentPlayerIndex + 1) + " hand: " + handText);
}


void PrintGameState()
{
    Debug.Log("===== GAME STATE =====");

    // Players
    for (int i = 0; i < turnManager.players.Count; i++)
    {
        Player p = turnManager.players[i];

        string hand = "";
        for (int j = 0; j < p.hand.Count; j++)
        {
            hand += p.hand[j];
            if (j < p.hand.Count - 1) hand += ", ";
        }

        string favor = "";
        for (int j = 0; j < p.favorArea.Count; j++)
        {
            favor += p.favorArea[j];
            if (j < p.favorArea.Count - 1) favor += ", ";
        }

        Debug.Log(
            "Player " + (i + 1) +
            " | Hand: [" + hand + "]" +
            " | Crew: " + p.hand.Count + "/" + p.maxHandSize +
            " | Favor: [" + favor + "]" +
            " | Favor Points: " + p.favorPoints
        );
    }

    // Discard pile
    string discard = "";
    for (int i = 0; i < discardPile.Count; i++)
    {
        discard += discardPile[i];
        if (i < discardPile.Count - 1) discard += ", ";
    }

    Debug.Log("Discard Pile: [" + discard + "]");

    // Deck sizes
    Debug.Log("Prankster Deck: " + deck.Count + " cards");
    Debug.Log("Prank Deck: " + prankDeck.Count + " cards");

    // Active pranks
    Debug.Log("Active Pranks:");
    for (int i = 0; i < activePranks.Count; i++)
    {
        PrankCard prank = activePranks[i];

        string req = "";
        for (int j = 0; j < prank.requiredPranksters.Count; j++)
        {
            req += prank.requiredPranksters[j];
            if (j < prank.requiredPranksters.Count - 1) req += ", ";
        }

        Debug.Log(
            "Prank " + (i + 1) + ": " +
            prank.title +
            " | Requires: " + req
        );
    }

    Debug.Log("======================");
}

bool CanClickDrawPile()
{
    return pendingChoice == PendingChoiceType.ChooseAction &&
    (deck.Count > 0 || discardPile.Count > 0);
}

bool CanClickDiscardPile()
{
    return pendingChoice == PendingChoiceType.ChooseAction && discardPile.Count > 0;
}

void LogSeparator(string title = "")
{
    Debug.Log("--------------------------------------------------");
    
    if (title != "")
    Debug.Log(title);
}

void StartDrawFromDiscardTurn()
{
    if (!CanClickDiscardPile())
    {
        Debug.Log("Discard pile is not a valid choice right now.");
        return;
    }

    DrawFromDiscard();

    RefreshCrewCapacityDisplay();

    handDisplay.ShowCurrentPlayerHand();
    discardPileDisplay.UpdateTopDiscardCard();

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayDrawCardAction();

    // Only discard if over max hand size.
    if (GetCurrentPlayer().hand.Count > GetCurrentPlayer().maxHandSize)
    {
        pendingChoice = PendingChoiceType.ChooseDiscardAfterDrawFromDiscard;

        RefreshAllHighlights();
        RefreshHandVisuals();

        if (AudioManager.Instance != null && Random.value < 0.6f)
            AudioManager.Instance.PlayHmmDecisions();

        LogSeparator("CHOOSE DISCARD");

        Debug.Log("Choose a card to discard.");
        ShowCurrentPlayerHand();
    }
    else
    {
        RefreshHandVisuals();
        FinishActionAndWaitForEndTurn();
    }
}

void ResolveDiscardAfterDrawFromDiscard(int discardHandIndex)
{
    if (pendingChoice != PendingChoiceType.ChooseDiscardAfterDrawFromDiscard)
        return;

    Debug.Log("Discard choice selected: hand index " + discardHandIndex);

    DiscardCardFromHand(discardHandIndex);

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayDiscardCard();

    handDisplay.ShowCurrentPlayerHand();
    discardPileDisplay.UpdateTopDiscardCard();
    RefreshCrewCapacityDisplay();

    ContinueDiscardingUntilHandAtMax();
}

void StartOfferFavorTurn()
{
    if (!CanStartOfferFavor())
    {
        Debug.Log("You cannot offer favor right now.");
        return;
    }

    pendingChoice = PendingChoiceType.ChooseFavorCard;
    RefreshActionHighlights();

    LogSeparator("CHOOSE FAVOR CARD");

    Debug.Log("Choose a card to offer as favor. Press 1, 2, 3, 4.");
    ShowCurrentPlayerHand();
}

void ResolveFavorChoice(int handIndex)
{
    if (pendingChoice != PendingChoiceType.ChooseFavorCard)
        return;

    Debug.Log("Favor choice selected: hand index " + handIndex);

    if (favorPreviewText != null)
        favorPreviewText.gameObject.SetActive(false);

    pendingChoice = PendingChoiceType.None;
    StartCoroutine(OfferFavor(handIndex));
}

bool CanStartOfferFavor()
{
    return pendingChoice == PendingChoiceType.ChooseAction
        && GetCurrentPlayer().favorArea.Count < 3
        && GetCurrentPlayer().hand.Count > 0;
}

void ResetRound()
{
    LogSeparator("ROUND RESET");

    int dealerIndex = DetermineDealerIndex();
    int firstPlayerIndex = (dealerIndex + 1) % turnManager.players.Count;

    turnManager.currentPlayerIndex = firstPlayerIndex;
    selectedSwapHandIndex = -1;
    pendingChoice = PendingChoiceType.None;

    Debug.Log("New dealer: Player " + (dealerIndex + 1));
    Debug.Log("First player this round: Player " + (firstPlayerIndex + 1));

    // Return all player hand cards to prankster deck
    for (int i = 0; i < turnManager.players.Count; i++)
    {
        Player player = turnManager.players[i];

        foreach (PranksterDeckEntry entry in player.hand)
        {
            deck.Add(new PranksterDeckEntry
            {
                pranksterType = entry.pranksterType,
                tier = entry.tier,
                category = entry.category
            });
        }
        player.hand.Clear();

        foreach (PranksterDeckEntry entry in player.favorArea)
        {
            deck.Add(new PranksterDeckEntry
            {
                pranksterType = entry.pranksterType,
                tier = entry.tier,
                category = entry.category
            });
        }
        player.favorArea.Clear();
    }

    // Return discard pile to prankster deck
    foreach (PranksterDeckEntry entry in discardPile)
    {
        deck.Add(new PranksterDeckEntry
        {
            pranksterType = entry.pranksterType,
            tier = entry.tier,
            category = entry.category
        });
    }
    discardPile.Clear();

    ReturnRetainedServiceCardsToDeck();

    // Shuffle prankster deck
    ShufflePranksterDeck();

    // Apply location effects before dealing new hands
    ApplyCurrentLocationEffects();

    // Deal fresh hands
    // Deal fresh hands
    DealStartingHands();

    // Deal 4 new active pranks
    DealActivePranks();
    ShowActivePrankCards();

    CacheServiceAvailabilityForRound();
    RefreshAvailableServiceSlotAvailability();

    Debug.Log("New round started.");

    UpdateActiveFavorDisplay();
    RefreshAllDisplays();
}

int DetermineDealerIndex()
{
    int bestIndex = 0;

    for (int i = 1; i < turnManager.players.Count; i++)
    {
        Player current = turnManager.players[i];
        Player best = turnManager.players[bestIndex];

        if (current.completedPranks.Count > best.completedPranks.Count)
        {
            bestIndex = i;
        }
        else if (current.completedPranks.Count == best.completedPranks.Count)
        {
            if (current.favorPoints > best.favorPoints)
            {
                bestIndex = i;
            }
        }
    }

    // Check if all players tied
    bool tie = true;
    Player reference = turnManager.players[0];

    for (int i = 1; i < turnManager.players.Count; i++)
    {
        Player p = turnManager.players[i];

        if (p.completedPranks.Count != reference.completedPranks.Count ||
            p.favorPoints != reference.favorPoints)
        {
            tie = false;
            break;
        }
    }

    if (tie)
    {
        return lastPrankCompleterIndex;
    }

    return bestIndex;
}

void StartCompletePrankTurn()
{
    if (pendingChoice != PendingChoiceType.ChooseAction)
    {
        Debug.Log("You cannot complete a prank right now.");
        return;
    }

    if (!HasAnyCompletablePrank())
    {
        Debug.Log("No active pranks can be completed right now.");
        return;
    }

    pendingChoice = PendingChoiceType.ChoosePrankToComplete;

    LogSeparator("CHOOSE PRANK");

    Debug.Log("Choose a prank to complete. Press 1, 2, or 3.");
    ShowCompletablePranks();
}

void ResolvePrankChoice(int prankIndex)
{
    if (!CanChoosePrankToComplete(prankIndex))
    {
        Debug.Log("That prank is not a valid completion choice right now. Choose again.");
        ShowCompletablePranks();
        return;
    }

    Debug.Log("Prank choice selected: index " + prankIndex);

    pendingChoice = PendingChoiceType.None;
    AttemptCompletePrank(prankIndex);
}

bool CanChoosePrankToComplete(int prankIndex)
{
    return pendingChoice == PendingChoiceType.ChoosePrankToComplete
        && prankIndex >= 0
        && prankIndex < activePranks.Count
        && CanCompletePrank(prankIndex);
}

bool HasAnyCompletablePrank()
{
    for (int i = 0; i < activePranks.Count; i++)
    {
        if (CanCompletePrank(i))
            return true;
    }

    return false;
}

void ShowTopDiscardCard()
{
    if (discardPile.Count == 0)
    {
        Debug.Log("Top of Discard Pile: [empty]");
        return;
    }

    PranksterDeckEntry topCard = discardPile[discardPile.Count - 1];

    Debug.Log("Top of Discard Pile: " + topCard.pranksterType + " | tier=" + topCard.tier);
}

void ShowAllFavorAreas()
{
    Debug.Log("Favor Areas:");

    for (int i = 0; i < turnManager.players.Count; i++)
    {
        Player player = turnManager.players[i];

        string favorText = "";

        for (int j = 0; j < player.favorArea.Count; j++)
        {
            favorText += "[" + (j + 1) + "] " + player.favorArea[j];

            if (j < player.favorArea.Count - 1)
            {
                favorText += ", ";
            }
        }

        if (favorText == "")
        {
            favorText = "[empty]";
        }

        Debug.Log("Player " + (i + 1) + " favor: " + favorText);
    }
}

void ShowCompletablePranks()
{
    Debug.Log("Prank Choices:");

    for (int i = 0; i < activePranks.Count; i++)
    {
        PrankCard prank = activePranks[i];
        string status = CanCompletePrank(i) ? "COMPLETABLE" : "NOT COMPLETABLE";

        Debug.Log("[" + (i + 1) + "] " + prank.title + " | " + status);
    }
}

void SortCurrentPlayerHand()
{
    GetCurrentPlayer().hand.Sort((a, b) => a.ToString().CompareTo(b.ToString()));
}

void SortHand(Player player)
{
    player.hand.Sort((a, b) => a.ToString().CompareTo(b.ToString()));
}

bool CanStartSwapFavor()
{
    if (pendingChoice != PendingChoiceType.ChooseAction)
        return false;

    if (GetCurrentPlayer().hand.Count == 0)
        return false;

    for (int i = 0; i < turnManager.players.Count; i++)
    {
        if (i == turnManager.currentPlayerIndex)
            continue;

        if (turnManager.players[i].favorArea.Count > 0)
            return true;
    }

    return false;
}

public void StartSwapFavorTurn()
{
    if (!CanStartSwapFavor())
    {
        Debug.Log("You cannot swap favor right now. pendingChoice = " + pendingChoice);
        return;
    }

    selectedSwapHandIndex = -1;
    selectedSwapPlayerIndex = -1;
    selectedSwapFavorIndex = -1;

    pendingChoice = PendingChoiceType.ChooseSwapOpponent;
    Debug.Log("pendingChoice set to ChooseSwapOpponent from StartSwapFavorTurn");

    LogSeparator("CHOOSE PLAYER TO SWAP WITH");

    Debug.Log("Choose a player to swap with. Press the matching player number.");
    Debug.Log("Press X to cancel.");

    for (int i = 0; i < turnManager.players.Count; i++)
    {
        if (i == turnManager.currentPlayerIndex)
            continue;

        Player otherPlayer = turnManager.players[i];

        if (otherPlayer.favorArea.Count > 0)
        {
            Debug.Log("[" + (i + 1) + "] Player " + (i + 1) + " (" + otherPlayer.favorArea.Count + " favor card(s))");
        }
    }

    ShowAllFavorAreas();
}

void ResolveSwapHandChoice(int handIndex)
{
    Player currentPlayer = GetCurrentPlayer();

    if (pendingChoice != PendingChoiceType.ChooseSwapHandCard)
        return;

    if (selectedSwapPlayerIndex < 0 || selectedSwapPlayerIndex >= turnManager.players.Count)
    {
        Debug.Log("No valid opponent is selected for this swap.");
        pendingChoice = PendingChoiceType.ChooseSwapOpponent;
        return;
    }

    Player targetPlayer = turnManager.players[selectedSwapPlayerIndex];

    if (selectedSwapFavorIndex < 0 || selectedSwapFavorIndex >= targetPlayer.favorArea.Count)
    {
        Debug.Log("No valid favor slot is selected for this swap.");
        pendingChoice = PendingChoiceType.ChooseSwapTarget;
        ShowSwapTargetChoices();
        return;
    }

    if (originalHandSnapshot == null || originalHandSnapshot.Count == 0)
    {
        Debug.Log("Original hand snapshot is missing. Canceling swap.");
        CancelSwapPreview();
        return;
    }

    if (handIndex < 0 || handIndex >= originalHandSnapshot.Count)
    {
        Debug.Log("That hand card is not a valid swap choice. Choose one of your original hand cards.");
        ShowCurrentPlayerHand();
        return;
    }

    selectedSwapHandIndex = handIndex;

    ExchangeFavorCards(selectedSwapPlayerIndex, selectedSwapFavorIndex);
    
    if (AudioManager.Instance != null && Random.value < 0.6f)
        AudioManager.Instance.PlaySwapCompleteVoiceLine();

    // Explicitly clear preview panel lock now that swap is committed
    if (opponentPreviewPanel != null)
    {
        opponentPreviewPanel.UnlockSwap();
        opponentPreviewPanel.Hide();
    }

    isInSwapHandSelection = false;
    originalHandSnapshot = null;
    tempSwapHand = null;
    pendingIncomingPrankster = null;

    hasTakenActionThisTurn = true;
    pendingChoice = PendingChoiceType.None;

    if (endTurnButton != null)
        endTurnButton.SetActive(true);

    RefreshAllDisplays();
    RefreshAllHighlights();
    ShowCurrentPlayerHand();
    ShowAllFavorAreas();
}

void ShowSwappableFavorChoices()
{
    Debug.Log("Swappable Favor Choices:");

    int displayIndex = 1;

    for (int i = 0; i < turnManager.players.Count; i++)
    {
        if (i == turnManager.currentPlayerIndex)
            continue;

        Player player = turnManager.players[i];

        for (int j = 0; j < player.favorArea.Count; j++)
        {
            Debug.Log("[" + displayIndex + "] Player " + (i + 1) + " favor[" + (j + 1) + "]: " + player.favorArea[j]);
            displayIndex++;
        }
    }
}

public void ResolveSwapTargetChoice(int favorSlotIndex)
{
    if (pendingChoice != PendingChoiceType.ChooseSwapTarget)
        return;

    if (selectedSwapPlayerIndex < 0 || selectedSwapPlayerIndex >= turnManager.players.Count)
    {
        Debug.Log("No valid opponent is selected for this swap.");
        pendingChoice = PendingChoiceType.ChooseSwapOpponent;
        return;
    }

    Player targetPlayer = turnManager.players[selectedSwapPlayerIndex];

    if (favorSlotIndex < 0 || favorSlotIndex >= targetPlayer.favorArea.Count)
    {
        Debug.Log("That favor slot is not a valid swap target. Choose again.");
        ShowSwapTargetChoices();
        return;
    }

    selectedSwapFavorIndex = favorSlotIndex;
    selectedSwapHandIndex = -1;

    if (opponentPreviewPanel != null)
        opponentPreviewPanel.HideSwapTargetHighlights();

    Player currentPlayer = GetCurrentPlayer();

    originalHandSnapshot = ConvertHandToTypes(currentPlayer.hand);
    pendingIncomingPrankster = targetPlayer.favorArea[selectedSwapFavorIndex];

    tempSwapHand = new List<PranksterDeckEntry>(currentPlayer.hand);
    tempSwapHand.Add(new PranksterDeckEntry
    {
        pranksterType = pendingIncomingPrankster.pranksterType,
        tier = pendingIncomingPrankster.tier,
        category = pendingIncomingPrankster.category
    });

    isInSwapHandSelection = true;

    if (opponentPreviewPanel != null)
        opponentPreviewPanel.Hide();

    pendingChoice = PendingChoiceType.ChooseSwapHandCard;

    LogSeparator("CHOOSE HAND CARD TO SWAP");

    Debug.Log("You selected Player " + (selectedSwapPlayerIndex + 1) +
              " favor slot " + (selectedSwapFavorIndex + 1) +
              " (" + targetPlayer.favorArea[selectedSwapFavorIndex] + ").");

    Debug.Log("Choose a card from your hand to swap.");
    Debug.Log("Press X to cancel.");

    RefreshAllDisplays();
    RefreshAllHighlights();
    ShowCurrentPlayerHand();
    ShowAllFavorAreas();
}

void ExchangeFavorCards(int targetPlayerIndex, int targetFavorIndex)
{
    Player currentPlayer = GetCurrentPlayer();

    if (targetPlayerIndex < 0 || targetPlayerIndex >= turnManager.players.Count)
    {
        Debug.Log("Target player choice is invalid.");
        return;
    }

    Player targetPlayer = turnManager.players[targetPlayerIndex];

    if (selectedSwapHandIndex < 0 || selectedSwapHandIndex >= currentPlayer.hand.Count)
    {
        Debug.Log("Stored swap hand choice is invalid.");
        return;
    }

    if (targetFavorIndex < 0 || targetFavorIndex >= targetPlayer.favorArea.Count)
    {
        Debug.Log("Target favor choice is invalid.");
        return;
    }

    PranksterDeckEntry handCard = currentPlayer.hand[selectedSwapHandIndex];
    PranksterDeckEntry favorCard = targetPlayer.favorArea[targetFavorIndex];

    currentPlayer.hand[selectedSwapHandIndex] = new PranksterDeckEntry
    {
        pranksterType = favorCard.pranksterType,
        tier = favorCard.tier,
        category = favorCard.category
    };

    targetPlayer.favorArea[targetFavorIndex] = new PranksterDeckEntry
    {
        pranksterType = handCard.pranksterType,
        tier = handCard.tier,
        category = handCard.category
    };

    currentPlayer.hand.Sort((a, b) => a.pranksterType.ToString().CompareTo(b.pranksterType.ToString()));

    Debug.Log("Swapped " + handCard.pranksterType +
              " (tier " + handCard.tier + ", category " + handCard.category + ")" +
              " from hand with " + favorCard.pranksterType +
              " (tier " + favorCard.tier + ", category " + favorCard.category + ")" +
              " from Player " + (targetPlayerIndex + 1) +
              "'s favor slot " + (targetFavorIndex + 1) + ".");

    selectedSwapHandIndex = -1;
    selectedSwapPlayerIndex = -1;
    selectedSwapFavorIndex = -1;

    RefreshAllDisplays();
    ShowCurrentPlayerHand();
    ShowAllFavorAreas();

    FinishActionAndWaitForEndTurn();
}

void TriggerEndGameScoring()
{
    Debug.Log("TriggerEndGameScoring START");
    gameOver = true;

    LogSeparator("GAME OVER TRIGGERED");

    Player endgamePlayer = turnManager.players[turnManager.currentPlayerIndex];
    string endgamePlayerName = endgamePlayer.playerName;

    if (string.IsNullOrEmpty(endgamePlayerName))
        endgamePlayerName = "Player " + (turnManager.currentPlayerIndex + 1);

    Debug.Log(endgamePlayerName + " triggered endgame.");

    if (finalCompletedPrank != null)
        Debug.Log("Final completed prank: " + finalCompletedPrank.title);
    else
        Debug.LogWarning("finalCompletedPrank is NULL");

    Debug.Log("Calculating final scores...");

    CalculateFinalScores();
    Debug.Log("CalculateFinalScores COMPLETE");

    ApplyPlayer1MatchResultsToSave();
    Debug.Log("PLAYER 1 PROGRESS AUTOSAVED");

    int combinedMischiefScore = 0;

    for (int i = 0; i < turnManager.players.Count; i++)
    {
        combinedMischiefScore += turnManager.players[i].renownPoints;
    }

    PlayerProgressSave saveData = SaveSystem.Load();
    Debug.Log("TWIN MAYOR UNLOCKED = " + saveData.hasUnlockedTwinMayor);

    if (combinedMischiefScore >= 180)
    {
        StartCoroutine(PlayWinCutsceneThenShowResults("WinScene.mp4"));
        return;
    }

    if (combinedMischiefScore >= 150 && saveData != null && !saveData.hasUnlockedTwinMayor)
    {
        saveData.hasUnlockedTwinMayor = true;
        SaveSystem.Save(saveData);

        StartCoroutine(PlayWinCutsceneThenShowResults("FalseWin.mp4"));
        return;
    }

    ShowFinalResultsUI();

    Debug.Log("TriggerEndGameScoring END");
}

void ShowFinalResultsUI()
{
    if (endGameCanvas != null)
        endGameCanvas.SetActive(true);

    if (endGameScoringPanel != null)
        endGameScoringPanel.SetActive(true);

    StartCoroutine(AnimateEndGamePanel());

    if (finalCompletedPrank != null && finalPrankImage != null)
    {
        finalPrankImage.sprite = finalCompletedPrank.cardSprite;
    }
    else
    {
        Debug.LogWarning("Final prank image not assigned or finalCompletedPrank is NULL");
    }

    ShowGameOverPanel();

    DeclareWinnerByScore();
    Debug.Log("DeclareWinnerByScore COMPLETE");

    if (gameCanvas != null)
        gameCanvas.SetActive(false);

    if (endTurnButton != null)
        endTurnButton.SetActive(false);

    Debug.Log("ShowFinalResultsUI END");
}

void CalculateFinalScores()
{
    for (int i = 0; i < turnManager.players.Count; i++)
    {
        Player player = turnManager.players[i];

        player.finalScore = player.renownPoints + player.favorPoints;

        string displayName = player.playerName;

        if (string.IsNullOrEmpty(displayName))
            displayName = "Player " + (i + 1);

        Debug.Log(displayName + " Final Score:");
       
        Debug.Log("  TOTAL: " + player.finalScore);
    }
}

void DeclareWinnerByScore()
{
    List<Player> contenders = new List<Player>(turnManager.players);

    int bestFinalScore = int.MinValue;
    for (int i = 0; i < contenders.Count; i++)
    {
        if (contenders[i].finalScore > bestFinalScore)
            bestFinalScore = contenders[i].finalScore;
    }

    contenders.RemoveAll(p => p.finalScore < bestFinalScore);

    if (contenders.Count > 1)
    {
        int bestCompletedPranks = int.MinValue;
        for (int i = 0; i < contenders.Count; i++)
        {
            if (contenders[i].completedPranks.Count > bestCompletedPranks)
                bestCompletedPranks = contenders[i].completedPranks.Count;
        }

        contenders.RemoveAll(p => p.completedPranks.Count < bestCompletedPranks);
    }

    if (contenders.Count > 1)
    {
        int bestFavorPoints = int.MinValue;
        for (int i = 0; i < contenders.Count; i++)
        {
            if (contenders[i].favorPoints > bestFavorPoints)
                bestFavorPoints = contenders[i].favorPoints;
        }

        contenders.RemoveAll(p => p.favorPoints < bestFavorPoints);
    }

    if (contenders.Count > 1)
    {
        Player endgamePlayer = turnManager.players[turnManager.currentPlayerIndex];

        if (contenders.Contains(endgamePlayer))
        {
            contenders.Clear();
            contenders.Add(endgamePlayer);
        }
    }

    if (contenders.Count == 1)
    {
        Player winner = contenders[0];
        int winnerIndex = turnManager.players.IndexOf(winner);

        string winnerName = winner.playerName;
        if (string.IsNullOrEmpty(winnerName))
            winnerName = "Player " + (winnerIndex + 1);

        Debug.Log("GAME OVER: " + winnerName + " wins with " + winner.finalScore + " points!");
    }
    else
    {
        string sharedWinners = "";

        for (int i = 0; i < contenders.Count; i++)
        {
            int playerIndex = turnManager.players.IndexOf(contenders[i]);
            Player p = contenders[i];
            string displayName = p.playerName;

            if (string.IsNullOrEmpty(displayName))
                displayName = "Player " + (playerIndex + 1);

            sharedWinners += displayName;

            if (i < contenders.Count - 1)
                sharedWinners += ", ";
        }

        Debug.Log("GAME OVER: Shared victory between " + sharedWinners + " with " + bestFinalScore + " points!");
    }
}

void ShowGameOverScreen()
{
    Debug.Log("Showing game over screen");
}



public void ShowActivePrankCards()
{
    if (activePrankDisplay == null)
    {
        Debug.LogWarning("Active Prank Display is not assigned.");
        return;
    }

    if (prankCardPrefab == null)
    {
        Debug.LogWarning("Prank Card Prefab is not assigned.");
        return;
    }

    // Clear old prank cards
    for (int i = activePrankDisplay.childCount - 1; i >= 0; i--)
    {
        Destroy(activePrankDisplay.GetChild(i).gameObject);
    }

    float startX = -((activePranks.Count - 1) * prankCardSpacing) / 2f;

    for (int i = 0; i < activePranks.Count; i++)
    {
        GameObject prankObject = Instantiate(prankCardPrefab, activePrankDisplay);

        prankObject.transform.localPosition = new Vector3(startX + (i * prankCardSpacing), 0f, 0f);
        prankObject.transform.localRotation = Quaternion.identity;
        prankObject.transform.localScale = prankCardScale;

        Transform cardArtTransform = prankObject.transform.Find("CardArt");

        if (cardArtTransform != null)
        {
            SpriteRenderer artRenderer = cardArtTransform.GetComponent<SpriteRenderer>();

            if (artRenderer != null && activePranks[i].cardSprite != null)
            {
                artRenderer.sprite = activePranks[i].cardSprite;
            }
        }

        PrankHoverPreview hoverPreview = prankObject.GetComponent<PrankHoverPreview>();

        if (hoverPreview != null)
        {
            hoverPreview.previewSprite = activePranks[i].cardSprite;
            hoverPreview.previewPanel = prankPreviewPanel;
            hoverPreview.deckManager = this;
            hoverPreview.nextPlayerPanelController = nextPlayerPanelController;
            hoverPreview.prankIndex = i;
        }

        BoxCollider2D collider = prankObject.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = prankObject.AddComponent<BoxCollider2D>();
        }

        PrankCardClick click = prankObject.GetComponent<PrankCardClick>();
        if (click == null)
        {
            click = prankObject.AddComponent<PrankCardClick>();
        }

        click.deckManager = this;
        click.prankIndex = i;

        Debug.Log("Prank " + i + " = " + activePranks[i].title + " | CanCompletePrank = " + CanCompletePrank(i));

        if (pendingChoice == PendingChoiceType.ChooseAction &&
        highlightSuppressionCount == 0 &&
        CanCompletePrank(i) &&
        completablePrankHighlightPrefab != null)
        {
            GameObject highlight = Instantiate(completablePrankHighlightPrefab, prankObject.transform);

            highlight.transform.localPosition = new Vector3(0f, 0f, -0.1f);
            highlight.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            highlight.transform.localScale = new Vector3(4.6f, 2.3f, 1f);

            ParticleSystemRenderer[] renderers = highlight.GetComponentsInChildren<ParticleSystemRenderer>(true);
            foreach (ParticleSystemRenderer r in renderers)
            {
                r.sortingLayerName = "Default";
                r.sortingOrder = 100;
            }
        }

        prankObject.name = "ActivePrank_" + activePranks[i].title;
    }
}

public int GetDeckCount()
{
    return deck.Count;
}

public int GetDiscardCount()
{
    return discardPile.Count;
}

public PranksterDeckEntry GetTopDiscardCard()
{
    if (discardPile.Count == 0)
        return null;

    return discardPile[discardPile.Count - 1];
}

IEnumerator ShowTurnTextTemporarily()
{
    turnText.gameObject.SetActive(true);

    yield return new WaitForSeconds(3f);

    turnText.gameObject.SetActive(false);
}

void UpdateActiveFavorDisplay()
{
    Player currentPlayer = turnManager.players[0];

    UpdateFavorSlot(filledMarker1Image, currentPlayer, 0);
    UpdateFavorSlot(filledMarker2Image, currentPlayer, 1);
    UpdateFavorSlot(filledMarker3Image, currentPlayer, 2);

    UpdateCurrentPlayerStatsDisplay();
}


Sprite GetFavorIcon(PranksterType type)
{
    switch (type)
    {
        case PranksterType.Thief: return thiefIcon;
        case PranksterType.Engineer: return engineerIcon;
        case PranksterType.Laborer: return laborerIcon;
        case PranksterType.Scribe: return scribeIcon;
        case PranksterType.Wizard: return wizardIcon;
        case PranksterType.BeastMaster: return beastmasterIcon;
    }

    return null;
}

void UpdateFavorSlot(Image slotImage, Player player, int index)
{
    if (slotImage == null || player == null)
    {
        Debug.Log("UpdateFavorSlot: slotImage or player is null");
        return;
    }

    if (index < player.favorArea.Count)
    {
        Sprite icon = GetFavorIcon(player.favorArea[index].pranksterType);

        Debug.Log("Updating slot " + index + " with " + player.favorArea[index].pranksterType + " | tier=" + player.favorArea[index].tier);
        Debug.Log("Icon found: " + (icon != null ? icon.name : "NULL"));

        slotImage.gameObject.SetActive(true);
        slotImage.sprite = icon;
    }
    else
    {
        slotImage.gameObject.SetActive(false);
    }
}

public void RefreshAllDisplays()
{
    if (handDisplay != null)
        handDisplay.ShowCurrentPlayerHand();

    ShowActivePrankCards();

    UpdateActiveFavorDisplay();
    UpdateCrewCapacityDisplay();

    if (discardPileDisplay != null)
        discardPileDisplay.UpdateTopDiscardCard();

    if (opponentDisplayManager != null)
        opponentDisplayManager.RefreshDisplays();

    RefreshActionHighlights();

    if (lifetimeCrewSizeButton != null)
        lifetimeCrewSizeButton.Refresh();

    if (inactiveServicesButton != null)
        inactiveServicesButton.Refresh();   
}

void UpdateCurrentPlayerStatsDisplay()
{
    Player currentPlayer = turnManager.players[0];
    int currentIndex = 0;

    if (activePlayerLabelText != null)
    {
        string displayName = currentPlayer.playerName;

        if (string.IsNullOrEmpty(displayName))
            displayName = "Player " + (currentIndex + 1);

        activePlayerLabelText.text = displayName;
    }

    if (activeCompletedPranksText != null)
        activeCompletedPranksText.text = currentPlayer.completedPranks.Count.ToString();

    if (activeRenownPointsText != null)
        activeRenownPointsText.text = currentPlayer.renownPoints.ToString();

    if (activeFavorPointsText != null)
        activeFavorPointsText.text = currentPlayer.favorPoints.ToString();

    if (pesterMayorButton != null)
        pesterMayorButton.Refresh();

    if (crewCapacityText != null)
    {
        crewCapacityText.text =
            "Crew: " +
            currentPlayer.hand.Count +
            "/" +
            currentPlayer.maxHandSize;

        if (currentPlayer.hand.Count > currentPlayer.maxHandSize)
        {
            crewCapacityText.text += "\nDismiss 1";
        }
    }
}

void ReshuffleDiscardIntoDeck()
{
    if (discardPile.Count == 0)
    {
        Debug.Log("No discard pile to reshuffle.");
        return;
    }

    Debug.Log("Reshuffling discard pile into deck.");

    foreach (PranksterDeckEntry card in discardPile)
    {
        deck.Add(new PranksterDeckEntry
        {
            pranksterType = card.pranksterType,
            tier = card.tier,
            category = card.category
        });
    }

    discardPile.Clear();
    ShufflePranksterDeck();

    if (discardPileDisplay != null)
        discardPileDisplay.UpdateTopDiscardCard();
}

void PopulateFinalScoreRow(
    TextMeshProUGUI nameText,
    TextMeshProUGUI prankPointsText,
    TextMeshProUGUI favorPointsText,
    TextMeshProUGUI totalPointsText)
{
    int finalScore = GetCombinedMischiefScore();

    if (nameText != null)
        nameText.text = "Final Score";

    if (prankPointsText != null)
        prankPointsText.text = finalScore.ToString();

    if (favorPointsText != null)
        favorPointsText.text = "";

    if (totalPointsText != null)
        totalPointsText.text = "";
}

void ShowGameOverPanel()
{
    if (endGameScoringPanel == null)
    {
        Debug.LogWarning("EndGameScoringPanel is not assigned.");
        return;
    }

    endGameScoringPanel.SetActive(true);

    // Create sortable list
    List<Player> sortedPlayers = new List<Player>(turnManager.players);

    // Highest score first
    sortedPlayers.Sort((a, b) => b.finalScore.CompareTo(a.finalScore));

    int playerCount = sortedPlayers.Count;

    if (player1Row != null) player1Row.SetActive(playerCount >= 1);
    if (player2Row != null) player2Row.SetActive(playerCount >= 2);

    // Rows 3 and 4 are now permanent summary rows.
    if (player3Row != null) player3Row.SetActive(true);
    if (player4Row != null) player4Row.SetActive(true);

    PopulateScoreRow(sortedPlayers, 0, player1NameText, player1PrankPointsText, player1FavorPointsText, player1TotalPointsText);
    PopulateScoreRow(sortedPlayers, 1, player2NameText, player2PrankPointsText, player2FavorPointsText, player2TotalPointsText);

    PopulateFinalScoreRow(player3NameText, player3PrankPointsText, player3FavorPointsText, player3TotalPointsText);

    PopulateLifetimeNotorietyRow(player4NameText, player4PrankPointsText, player4FavorPointsText, player4TotalPointsText);
}

void PopulateScoreRow(
    List<Player> sortedPlayers,
    int playerIndex,
    TextMeshProUGUI nameText,
    TextMeshProUGUI prankPointsText,
    TextMeshProUGUI favorPointsText,
    TextMeshProUGUI totalPointsText)
{
    if (playerIndex < 0 || playerIndex >= sortedPlayers.Count)
        return;

    Player player = sortedPlayers[playerIndex];

    int prankPoints = player.renownPoints;
    int favorPoints = player.favorPoints;
    int totalPoints = prankPoints + favorPoints;

    if (nameText != null)
    {
        string displayName = player.playerName;

        if (string.IsNullOrEmpty(displayName))
            displayName = "Player " + (turnManager.players.IndexOf(player) + 1);

        nameText.text = displayName;
    }

    if (prankPointsText != null)
        prankPointsText.text = prankPoints.ToString();

    if (favorPointsText != null)
        favorPointsText.text = favorPoints.ToString();

    if (totalPointsText != null)
        totalPointsText.text = totalPoints.ToString();
}

void PopulateLifetimeNotorietyRow(
    TextMeshProUGUI nameText,
    TextMeshProUGUI prankPointsText,
    TextMeshProUGUI favorPointsText,
    TextMeshProUGUI totalPointsText)
{
    Player player = turnManager.players[0];

    int lifetimeNotorietyEarned =
        player.renownPoints + player.favorPoints;

    if (nameText != null)
        nameText.text = "Lifetime Notoriety Earned";

    if (prankPointsText != null)
        prankPointsText.text = lifetimeNotorietyEarned.ToString();

    if (favorPointsText != null)
        favorPointsText.text = "";

    if (totalPointsText != null)
        totalPointsText.text = "";
}



int GetCombinedMischiefScore()
{
    int total = 0;

    for (int i = 0; i < turnManager.players.Count; i++)
    {
        total += turnManager.players[i].renownPoints;
    }

    return total;
}

public void BeginNewGame()
{
    Debug.Log("BeginNewGame START");

    if (nextPlayerPanelController != null)
    {
        nextPlayerPanelController.HideBotMessage();
        nextPlayerPanelController.HidePanelImmediate();
    }

    StopAllCoroutines();

    if (favorPreviewText != null)
        favorPreviewText.gameObject.SetActive(false);

    if (prankCompletionShowcasePanel != null)
        prankCompletionShowcasePanel.Hide();

    if (turnManager == null)
    {
        Debug.LogError("DeckManager: turnManager is null.");
        return;
    }

    if (turnManager.players == null || turnManager.players.Count == 0)
    {
        Debug.LogError("DeckManager: no players were initialized before BeginNewGame().");
        return;
    }

    // Force every new game to begin with Player 1
    turnManager.currentPlayerIndex = 0;

    Debug.Log("BeginNewGame | currentPlayerIndex reset to Player 1");

    Debug.Log("BeginNewGame | clearing runtime data");

    // Clear all runtime data
    deck.Clear();
    prankDeck.Clear();
    activePranks.Clear();
    discardPile.Clear();
    outOfPlayPranksters.Clear();

    Debug.Log("BeginNewGame | resetting state");

    // Reset state
    pendingChoice = PendingChoiceType.None;
    availableServicesPanelOpen = false;
    temporarilyAssignedServiceHandIndexes.Clear();
    selectedAvailableServiceType = default;
    activeAvailableServiceSlotCollider = null;
    lastPrankCompleterIndex = -1;
    selectedSwapHandIndex = -1;
    gameOver = false;
    finalCompletedPrank = null;
    hoveredPrankIndex = -1;
    hasTakenActionThisTurn = false;
    isEndOfRoundPending = false;
    pendingRoundDealerIndex = -1;
    pendingRoundFirstPlayerIndex = -1;
    highlightSuppressionCount = 0;
    wantedBoardOpen = false;

    if (wantedJailController != null)
    {
        wantedJailController.ClearJailDisplay();
    }

    if (wantedBoardPanelController != null)
    {
        wantedBoardPanelController.ResetWantedBoardState();
    }

    pendingRoundLocation = GameLocationType.RebelWorkshop;

    GameBackgroundManager backgroundManager =
        FindFirstObjectByType<GameBackgroundManager>(FindObjectsInactive.Include);

    if (backgroundManager != null)
    {
        Debug.Log("BeginNewGame | setting background to " + pendingRoundLocation);
        backgroundManager.SetLocation(GameLocationType.RebelWorkshop);
    }
    else
    {
        Debug.LogWarning("BeginNewGame | GameBackgroundManager not found.");
    }

    // Reset players
    for (int i = 0; i < turnManager.players.Count; i++)
    {
        Player player = turnManager.players[i];
        player.hand.Clear();
        player.favorArea.Clear();
        player.completedPranks.Clear();
        player.favorPoints = 0;
        player.renownPoints = 0;
        player.finalScore = 0;

        player.maxHandSize = 4;
        player.lifetimeNotorietyMaxHandSizeBonus = 0;
        player.lifetimeNotorietyCrewUpgradeUsedThisGame = false;

        player.retainedServices.Clear();
        player.activeScoringServiceTypes.Clear();
        player.activeOngoingServiceTypes.Clear();
    }

    ApplyCurrentLocationEffects();

    ResetPlayer1FavorTrackingForNewGame();
    ResetPlayer1DiscardTrackingForNewGame();

    Debug.Log("BeginNewGame | building and shuffling decks");

    // Build and shuffle decks
    BuildPranksterDeck();
    ShufflePranksterDeck();

    prankDeck = PrankDatabase.CreatePrankDeck();
    ShufflePrankDeck();

    Debug.Log("Prankster Deck created with " + deck.Count + " cards");
    Debug.Log("Prank deck size: " + prankDeck.Count);

    Debug.Log("BeginNewGame | about to start BeginNewGameSequence coroutine");

    StartCoroutine(BeginNewGameSequence());
}

public void AdvanceToNextPlayerTurn()
{
    Debug.Log("AdvanceToNextPlayerTurn START");

    turnManager.NextPlayer();

    Debug.Log("Current player after NextPlayer = " + (turnManager.currentPlayerIndex + 1) +
              " | isBot = " + turnManager.GetCurrentPlayer().isBot);

    if (opponentDisplayManager != null)
        opponentDisplayManager.RefreshDisplays();

    StartPlayerTurn();
}

public void RefreshAllHighlights()
{
    Debug.Log("RefreshAllHighlights | suppression=" + highlightSuppressionCount + " | pendingChoice=" + pendingChoice);

    if (drawDeckHighlight != null)
        drawDeckHighlight.SetActive(false);

    if (discardPileHighlight != null)
        discardPileHighlight.SetActive(false);

    if (favorAreaHighlight != null)
        favorAreaHighlight.SetActive(false);

    for (int i = 0; i < prankHighlights.Length; i++)
    {
        if (prankHighlights[i] != null)
            prankHighlights[i].SetActive(false);
    }

    if (drawDeckLabel != null)
        drawDeckLabel.SetActive(false);

    if (discardPileLabel != null)
        discardPileLabel.SetActive(false);

    if (favorAreaLabel != null)
        favorAreaLabel.SetActive(false);

    SetAllPrankHighlightsVisible(false);
    HideOpponentPanelHighlights();

    if (isRulesPanelOpen)
    {
        Debug.Log("RefreshAllHighlights blocked because rules panel is active.");
        return;
    }

    if (nextPlayerPanelController != null && nextPlayerPanelController.IsPanelBlockingInteraction())
    {
        Debug.Log("RefreshAllHighlights blocked because next player panel is active.");
        return;
    }

    if (settingsMenuController != null && settingsMenuController.IsPanelBlockingInteraction())
    {
        Debug.Log("RefreshAllHighlights blocked because settings panel is active.");
        return;
    }

    if (availableServicesPanelOpen)
    {
        Debug.Log("RefreshAllHighlights blocked because Available Services panel is open.");
        return;
    }

    if (highlightSuppressionCount > 0)
        return;

    RefreshActionHighlights();
    RefreshActionLabels();
}

void RefreshActionHighlights()
{
    if (drawDeckHighlight != null)
        SetActiveAndRestart(drawDeckHighlight, pendingChoice == PendingChoiceType.ChooseAction && CanClickDrawPile());

    if (discardPileHighlight != null)
        SetActiveAndRestart(discardPileHighlight, pendingChoice == PendingChoiceType.ChooseAction && CanClickDiscardPile());

    if (favorAreaHighlight != null)
        SetActiveAndRestart(favorAreaHighlight, pendingChoice == PendingChoiceType.ChooseAction && CanStartOfferFavor());

    for (int i = 0; i < activePrankDisplay.childCount; i++)
    {
        var prank = activePrankDisplay.GetChild(i).GetComponent<PrankHoverPreview>();

        if (prank != null)
        {
            bool shouldGlow =
                pendingChoice == PendingChoiceType.ChooseAction &&
                i < activePranks.Count &&
                CanCompletePrank(i);

            prank.SetGlow(shouldGlow);
        }
    }

    if (opponentDisplayManager != null)
        opponentDisplayManager.RefreshSwapHighlights();
}

void SetActiveAndRestart(GameObject go, bool active)
{
    if (go == null) return;

    if (active)
    {
        go.SetActive(true);

        var ps = go.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Clear();
            ps.Play();
        }
    }
    else
    {
        go.SetActive(false);
    }
}

void RefreshActionLabels()
{
    bool showDrawDeckLabel =
        actionLabelsEnabled &&
        pendingChoice == PendingChoiceType.ChooseAction &&
        CanClickDrawPile();

    bool showDiscardPileLabel =
        actionLabelsEnabled &&
        pendingChoice == PendingChoiceType.ChooseAction &&
        CanClickDiscardPile();

    bool showFavorAreaLabel =
        actionLabelsEnabled &&
        pendingChoice == PendingChoiceType.ChooseAction &&
        CanStartOfferFavor();

    if (drawDeckLabel != null)
        drawDeckLabel.SetActive(showDrawDeckLabel);

    if (discardPileLabel != null)
        discardPileLabel.SetActive(showDiscardPileLabel);

    if (favorAreaLabel != null)
        favorAreaLabel.SetActive(showFavorAreaLabel);
}

void HideAllActionLabels()
{
    if (drawDeckLabel != null)
        drawDeckLabel.SetActive(false);

    if (discardPileLabel != null)
        discardPileLabel.SetActive(false);

    if (favorAreaLabel != null)
        favorAreaLabel.SetActive(false);
}

public void PushHighlightSuppression()
{
    highlightSuppressionCount++;
    RefreshAllHighlights();
}

public void PopHighlightSuppression()
{
    highlightSuppressionCount = Mathf.Max(0, highlightSuppressionCount - 1);

    Debug.Log("PopHighlightSuppression -> count = " + highlightSuppressionCount);

    RefreshAllHighlights();
}

public void OnDrawDeckClicked()
{
    Debug.Log("OnDrawDeckClicked called");

    if (pendingChoice != PendingChoiceType.ChooseAction)
    {
        Debug.Log("Draw deck click ignored: not in ChooseAction state.");
        return;
    }

    if (!CanClickDrawPile())
    {
        Debug.Log("Draw deck click ignored: draw pile not currently valid.");
        return;
    }

    LogSeparator("PLAYER ACTION: Draw from prankster deck");
    StartDrawFromDeckTurn();
}

public void OnDiscardPileClicked()
{
    Debug.Log("OnDiscardPileClicked called");

    if (pendingChoice != PendingChoiceType.ChooseAction)
    {
        Debug.Log("Discard click ignored: not in ChooseAction state.");
        return;
    }

    if (!CanClickDiscardPile())
    {
        Debug.Log("Discard click ignored: not valid right now.");
        return;
    }

    LogSeparator("PLAYER ACTION: Draw from discard pile");
    StartDrawFromDiscardTurn();
}

public bool CanHoverDrawDeck()
{
    return pendingChoice == PendingChoiceType.ChooseAction && CanClickDrawPile();
}

public bool CanHoverDiscardPile()
{
    return pendingChoice == PendingChoiceType.ChooseAction && CanClickDiscardPile();
}

public void OnHandCardClicked(int index)
{
    Debug.Log("Hand card clicked: " + index);

    if (availableServicesPanelOpen && pendingChoice != PendingChoiceType.ChooseAvailableServiceCard)
    {
        Debug.Log("Hand card click blocked while Available Services panel is open.");

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayNotAnOption();

        return;
    }

    if (pendingChoice == PendingChoiceType.ChooseFavorCard)
    {
        ResolveFavorChoice(index);
        return;
    }

    if (pendingChoice == PendingChoiceType.ChooseDiscardFromHand)
    {
        ResolveDiscardChoice(index);
        return;
    }

    if (pendingChoice == PendingChoiceType.ChooseDiscardAfterDrawFromDiscard)
    {
        ResolveDiscardAfterDrawFromDiscard(index);
        return;
    }

    if (pendingChoice == PendingChoiceType.ChooseSwapHandCard)
    {
        ResolveSwapHandChoice(index);
        return;
    }

    if (pendingChoice == PendingChoiceType.ChooseAvailableServiceCard)
    {
        ResolveAvailableServiceCardChoice(index);
        return;
    }

    Debug.Log("Hand click ignored: not in valid state.");
}

public bool IsInDiscardSelection()
{
    return pendingChoice == PendingChoiceType.ChooseDiscardFromHand
        || pendingChoice == PendingChoiceType.ChooseDiscardAfterDrawFromDiscard;
}

public void RefreshHandVisuals()
{
    if (handDisplay == null || handDisplay.currentPlayerHandArea == null)
        return;

    Debug.Log("RefreshHandVisuals CALLED");

    foreach (Transform child in handDisplay.currentPlayerHandArea)
    {
        HandCardClick click = child.GetComponent<HandCardClick>();
        if (click != null)
            click.RefreshVisualState();
    }
}

public bool CanHoverFavorArea()
{
    return pendingChoice == PendingChoiceType.ChooseAction && CanStartOfferFavor();
}

public bool IsChoosingFavor()
{
    return pendingChoice == PendingChoiceType.ChooseFavorCard;
}

public void OnFavorAreaClicked(int favorSlotIndex)
{
    if (pendingChoice == PendingChoiceType.ChooseWizardFavorReturn)
    {
        ReturnFavorCardToHand(favorSlotIndex);
        return;
    }

    if (pendingChoice == PendingChoiceType.ChooseFavorCard)
    {
        CancelFavorChoice();
        return;
    }

    if (pendingChoice != PendingChoiceType.ChooseAction)
        return;

    if (!CanStartOfferFavor())
        return;

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayFavorClick();

    StartOfferFavorTurn();
}

public void OnPrankCardClicked(int prankIndex)
{
    Debug.Log("Prank card clicked: " + prankIndex);

    if (pendingChoice == PendingChoiceType.ChooseEngineerPrankReplacement)
    {
        ReplaceActivePrankAndMoveOldToBottom(prankIndex);
        return;
    }

    if (pendingChoice != PendingChoiceType.ChooseAction &&
        pendingChoice != PendingChoiceType.ChoosePrankToComplete)
    {
        Debug.Log("Prank click ignored because pendingChoice is: " + pendingChoice);
        return;
    }

    if (!CanCompletePrank(prankIndex))
    {
        Debug.Log("Clicked prank is not completable.");
        return;
    }

    if (prankPreviewPanel != null)
        prankPreviewPanel.Hide();

    pendingChoice = PendingChoiceType.None;
    highlightSuppressionCount = 1;
    RefreshAllHighlights();

    bool completed = AttemptCompletePrank(prankIndex);

    if (!completed)
    {
        Debug.Log("Prank completion failed. Returning to ChooseAction.");
        highlightSuppressionCount = 0;
        pendingChoice = PendingChoiceType.ChooseAction;
        Debug.Log("pendingChoice set to ChooseAction from OnPrankCardClicked fail branch");
        RefreshAllHighlights();
        return;
    }
}

public int GetNextAvailableFavorIndex()
{
    Player player = GetCurrentPlayer();
    return player.favorArea.Count;
}

public Vector3 GetFavorWellPosition(int index)
{
    if (index == 0 && filledMarker1 != null)
        return filledMarker1.transform.position;

    if (index == 1 && filledMarker2 != null)
        return filledMarker2.transform.position;

    if (index == 2 && filledMarker3 != null)
        return filledMarker3.transform.position;

    return Vector3.zero;
}

void CancelFavorChoice()
{
    if (favorPreviewText != null)
        favorPreviewText.gameObject.SetActive(false);

    pendingChoice = PendingChoiceType.ChooseAction;
    Debug.Log("pendingChoice set to ChooseAction from CancelFavorChoice");

    RefreshAllDisplays();
    RefreshAllHighlights();

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayCancelAction();

    Debug.Log("Favor choice cancelled.");
}

public void SetAllPrankHighlightsVisible(bool isVisible)
{
    if (activePrankDisplay == null)
        return;

    for (int i = 0; i < activePrankDisplay.childCount; i++)
    {
        Transform prank = activePrankDisplay.GetChild(i);
        Transform highlight = prank.Find("FX_CardBrushLine_G(Clone)");

        if (highlight != null)
            highlight.gameObject.SetActive(isVisible);
    }
}

public bool IsPrankPreviewOpen()
{
    return prankPreviewPanel != null && prankPreviewPanel.IsVisible();
}

IEnumerator RefillHandToFourOneCardAtATime(float delayBetweenCards = 0.3f)
{
    while (GetCurrentPlayer().hand.Count < 4)
    {
        int handCountBefore = GetCurrentPlayer().hand.Count;

        DrawCard();

        if (GetCurrentPlayer().hand.Count == handCountBefore)
        {
            Debug.Log("Could not draw more cards. Stopping refill.");
            yield break;
        }

        RefreshAllDisplays();

        if (handDisplay != null)
            handDisplay.ShowCurrentPlayerHand();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDrawCardAction();

        yield return new WaitForSeconds(delayBetweenCards);
    }
}

IEnumerator RefillHandToMaxOneCardAtATime(float delayBetweenCards = 0.3f)
{
    while (GetCurrentPlayer().hand.Count < GetCurrentPlayer().maxHandSize)
    {
        int handCountBefore = GetCurrentPlayer().hand.Count;

        DrawCard();

        if (GetCurrentPlayer().hand.Count == handCountBefore)
        {
            Debug.Log("Could not draw more cards. Stopping refill.");
            yield break;
        }

        RefreshAllDisplays();

        if (handDisplay != null)
            handDisplay.ShowCurrentPlayerHand();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDrawCardAction();

        yield return new WaitForSeconds(delayBetweenCards);
    }
}

[SerializeField] private int startingHandSize = 3;

IEnumerator DealStartingHandsOneCardAtATime(float delayBetweenCards = 0.2f)
{
    for (int cardNumber = 0; cardNumber < startingHandSize; cardNumber++)
    {
        for (int playerIndex = 0; playerIndex < turnManager.players.Count; playerIndex++)
        {
            Player player = turnManager.players[playerIndex];

            if (deck.Count == 0)
            {
                Debug.LogWarning("Prankster deck ran out while dealing starting hands.");
                yield break;
            }

            player.hand.Add(deck[0]);
            deck.RemoveAt(0);

            SortHand(player);

            RefreshAllDisplays();

            if (turnManager.currentPlayerIndex == playerIndex && handDisplay != null)
                handDisplay.ShowCurrentPlayerHand();

            if (playerIndex == turnManager.currentPlayerIndex && AudioManager.Instance != null)
                AudioManager.Instance.PlayDrawCardAction();

            yield return new WaitForSeconds(delayBetweenCards);
        }
    }
}

IEnumerator ResetRoundSequence()
{
    LogSeparator("ROUND RESET");

    int dealerIndex = pendingRoundDealerIndex >= 0
        ? pendingRoundDealerIndex
        : DetermineDealerIndex();

    int firstPlayerIndex = pendingRoundFirstPlayerIndex >= 0
        ? pendingRoundFirstPlayerIndex
        : (dealerIndex + 1) % turnManager.players.Count;

    pendingRoundDealerIndex = -1;
    pendingRoundFirstPlayerIndex = -1;

    turnManager.currentPlayerIndex = firstPlayerIndex;
    selectedSwapHandIndex = -1;
    pendingChoice = PendingChoiceType.None;

    Player firstPlayer = turnManager.players[firstPlayerIndex];

    if (firstPlayer != null && firstPlayer.isBot && nextPlayerPanelController != null)
    {
        string firstPlayerName = firstPlayer.playerName;

        if (string.IsNullOrWhiteSpace(firstPlayerName))
            firstPlayerName = "Player " + (firstPlayerIndex + 1);

        Debug.Log("ROUND RESET: First player is bot. Showing bot panel before dealing hands.");

        nextPlayerPanelController.ShowBotTurnHeader(firstPlayerName);

        yield return null;
    }

    Debug.Log("New dealer: " + turnManager.players[dealerIndex].playerName);
    Debug.Log("First player this round: " + turnManager.players[firstPlayerIndex].playerName);

    for (int i = 0; i < turnManager.players.Count; i++)
    {
        Player player = turnManager.players[i];

        foreach (PranksterDeckEntry entry in player.hand)
        {
            deck.Add(new PranksterDeckEntry
            {
                pranksterType = entry.pranksterType,
                tier = entry.tier,
                category = entry.category
            });
        }
        player.hand.Clear();

        foreach (PranksterDeckEntry entry in player.favorArea)
        {
            deck.Add(new PranksterDeckEntry
            {
                pranksterType = entry.pranksterType,
                tier = entry.tier,
                category = entry.category
            });
        }
        player.favorArea.Clear();
    }

    foreach (PranksterDeckEntry entry in discardPile)
    {
        deck.Add(new PranksterDeckEntry
        {
            pranksterType = entry.pranksterType,
            tier = entry.tier,
            category = entry.category
        });
    }
    discardPile.Clear();

    ReturnRetainedServiceCardsToDeck();

    ShufflePranksterDeck();

    GameBackgroundManager backgroundManager = FindFirstObjectByType<GameBackgroundManager>();

    if (backgroundManager != null)
    {
        backgroundManager.SetLocation(pendingRoundLocation);
        Debug.Log("ROUND RESET APPLY LOCATION | " + pendingRoundLocation);
    }
    else
    {
        Debug.LogWarning("GameBackgroundManager not found during round reset.");
    }

    ApplyCurrentLocationEffects();

    yield return StartCoroutine(DealStartingHandsOneCardAtATime(0.2f));

    DealActivePranks();
    ShowActivePrankCards();
    CacheServiceAvailabilityForRound();
    RefreshAvailableServiceSlotAvailability();

    if (wantedBoardPanelController != null &&
        wantedBoardPanelController.wantedDisplayPanelController != null)
    {
        wantedBoardPanelController.wantedDisplayPanelController.RefreshFromWantedCells(
            wantedBoardPanelController.cells);

        wantedBoardPanelController.wantedDisplayPanelController.Show();
    }

    Debug.Log("New round started.");

    UpdateActiveFavorDisplay();
    RefreshAllDisplays();
    StartPlayerTurn();
}

IEnumerator BeginNewGameSequence()
{
    Debug.Log("BeginNewGameSequence START");

    if (gameCanvas != null)
    {
        gameCanvas.SetActive(true);
        Debug.Log("BeginNewGameSequence | gameCanvas forced ON");
    }

    yield return StartCoroutine(DealStartingHandsOneCardAtATime(0.2f));

    Debug.Log("BeginNewGameSequence | hands dealt");

    Debug.Log("Cards left in deck: " + deck.Count);

    DealActivePranks();
    ShowActivePrankCards();
    CacheServiceAvailabilityForRound();
    RefreshAvailableServiceSlotAvailability();

    Debug.Log("Prank deck size: " + prankDeck.Count);
    Debug.Log("Active pranks: " + activePranks.Count);

    if (endGameScoringPanel != null)
        endGameScoringPanel.SetActive(false);

    if (endGameCanvas != null)
        endGameCanvas.SetActive(false);

    UpdateActiveFavorDisplay();

    if (handDisplay != null)
        handDisplay.ShowCurrentPlayerHand();

    if (opponentDisplayManager != null)
        opponentDisplayManager.RefreshDisplays();

    Debug.Log("BeginNewGameSequence | about to call StartPlayerTurn");
    StartPlayerTurn();
}

public bool ShouldHighlightOpponentPanel(int representedPlayerIndex)
{
    if (turnManager == null || turnManager.players == null || turnManager.players.Count == 0)
        return false;

    if (turnManager.currentPlayerIndex < 0 || turnManager.currentPlayerIndex >= turnManager.players.Count)
        return false;

    if (representedPlayerIndex < 0 || representedPlayerIndex >= turnManager.players.Count)
        return false;

    if (highlightSuppressionCount > 0)
    {
        Debug.Log("FAIL: highlightSuppressionCount > 0");
        return false;
    }

    if (GetCurrentPlayer().isBot)
    {
        Debug.Log("FAIL: current player is bot");
        return false;
    }

    if (pendingChoice != PendingChoiceType.ChooseAction)
    {
        return false;
    }

    if (representedPlayerIndex < 0 || representedPlayerIndex >= turnManager.players.Count)
    {
        Debug.Log("FAIL: invalid representedPlayerIndex");
        return false;
    }

    if (representedPlayerIndex == turnManager.currentPlayerIndex)
    {
        Debug.Log("FAIL: panel represents current player");
        return false;
    }

    if (opponentPreviewPanel != null && opponentPreviewPanel.IsLockedForSwap())
    {
        Debug.Log("FAIL: opponent preview is locked for swap");
        return false;
    }

    bool canSwap = CanSwapWithOpponent(representedPlayerIndex);
    Debug.Log("CanSwapWithOpponent(" + representedPlayerIndex + ") = " + canSwap);

    if (!canSwap)
    {
        Debug.Log("FAIL: CanSwapWithOpponent returned false");
        return false;
    }

    Debug.Log("SUCCESS: highlight opponent panel");
    return true;
}

public bool CanSwapWithOpponent(int opponentPlayerIndex)
{
    // MUST be in action selection state
    if (pendingChoice != PendingChoiceType.ChooseAction)
        return false;

    if (GetCurrentPlayer().hand.Count == 0)
        return false;

    if (opponentPlayerIndex < 0 || opponentPlayerIndex >= turnManager.players.Count)
        return false;

    if (opponentPlayerIndex == turnManager.currentPlayerIndex)
        return false;

    if (turnManager.players[opponentPlayerIndex].favorArea.Count == 0)
        return false;

    return true;
}

public void ResolveSwapOpponentChoice(int opponentPlayerIndex)
{
    if (pendingChoice != PendingChoiceType.ChooseSwapOpponent)
        return;

    if (opponentPlayerIndex < 0 || opponentPlayerIndex >= turnManager.players.Count)
    {
        Debug.Log("That player is not a valid choice. Choose again.");
        return;
    }

    if (opponentPlayerIndex == turnManager.currentPlayerIndex)
    {
        Debug.Log("You cannot swap with yourself. Choose again.");
        return;
    }

    Player targetPlayer = turnManager.players[opponentPlayerIndex];

    if (targetPlayer.favorArea.Count == 0)
    {
        Debug.Log("That player has no favor cards to swap with. Choose again.");
        return;
    }

    selectedSwapPlayerIndex = opponentPlayerIndex;
    selectedSwapFavorIndex = -1;
    selectedSwapHandIndex = -1;

    pendingChoice = PendingChoiceType.ChooseSwapTarget;

    LogSeparator("CHOOSE FAVOR SLOT");

    Debug.Log("Player " + (opponentPlayerIndex + 1) + " selected.");
    Debug.Log("Choose a favor slot from Player " + (opponentPlayerIndex + 1) + ".");

    string validSlotText = "";

    for (int i = 0; i < targetPlayer.favorArea.Count; i++)
    {
        validSlotText += (i + 1);

        if (i < targetPlayer.favorArea.Count - 1)
            validSlotText += ", ";
    }

    Debug.Log("Valid slot choices: " + validSlotText + ".");
    Debug.Log("Press X to cancel.");

    ShowSwapTargetChoices();
}

void ShowSwapTargetChoices()
{
    if (selectedSwapPlayerIndex < 0 || selectedSwapPlayerIndex >= turnManager.players.Count)
    {
        Debug.Log("No valid swap player is currently selected.");
        return;
    }

    Player targetPlayer = turnManager.players[selectedSwapPlayerIndex];

    Debug.Log("Available favor slots for Player " + (selectedSwapPlayerIndex + 1) + ":");

    bool foundAny = false;

    for (int i = 0; i < 3; i++)
    {
        if (i < targetPlayer.favorArea.Count)
        {
            Debug.Log("[" + (i + 1) + "] Favor Slot " + (i + 1) + ": " + targetPlayer.favorArea[i]);
            foundAny = true;
        }
    }

    if (!foundAny)
    {
        Debug.Log("That player has no valid favor slots.");
    }

    if (opponentPreviewPanel != null)
        opponentPreviewPanel.ShowSwapTargetHighlights(targetPlayer.favorArea.Count);
}

public void CancelSwapPreview()
{
    selectedSwapHandIndex = -1;
    selectedSwapPlayerIndex = -1;
    selectedSwapFavorIndex = -1;

    isInSwapHandSelection = false;
    originalHandSnapshot = null;
    tempSwapHand = null;
    pendingIncomingPrankster = default;

    pendingChoice = PendingChoiceType.ChooseAction;

    LogSeparator("SWAP CANCELED");
    Debug.Log("Swap canceled. Returning to action selection.");

    if (opponentPreviewPanel != null)
    {
        opponentPreviewPanel.UnlockSwap();
        opponentPreviewPanel.Hide();
    }

    highlightSuppressionCount = 0;

    RefreshAllDisplays();
    ShowActivePrankCards();
    RefreshAllHighlights();
    ShowCurrentPlayerHand();
    ShowAllFavorAreas();
}

public bool IsInSwapHandSelection()
{
    return isInSwapHandSelection;
}

public List<PranksterDeckEntry> GetTempSwapHand()
{
    return tempSwapHand;
}

public bool IsSwapFlowActive()
{
    return pendingChoice == PendingChoiceType.ChooseSwapOpponent ||
           pendingChoice == PendingChoiceType.ChooseSwapTarget ||
           pendingChoice == PendingChoiceType.ChooseSwapHandCard;
}

// ===== BOT ACCESS METHODS =====

public Player BotGetCurrentPlayer()
{
    return GetCurrentPlayer();
}

public List<Player> BotGetAllPlayers()
{
    return turnManager.players;
}

public int BotGetCurrentPlayerIndex()
{
    return turnManager.currentPlayerIndex;
}

public List<PrankCard> BotGetActivePranks()
{
    return activePranks;
}

public List<PranksterType> BotGetDiscardPile()
{
    List<PranksterType> result = new List<PranksterType>();

    foreach (PranksterDeckEntry entry in discardPile)
    {
        result.Add(entry.pranksterType);
    }

    return result;
}

public int BotCalculateFavorPoints(PranksterType pranksterType)
{
    return CalculateFavorPoints(pranksterType);
}

public bool BotCanCompletePrank(int prankIndex)
{
    return CanCompletePrank(prankIndex);
}

public void BotCompletePrank(int prankIndex)
{
    CompletePrank(prankIndex);
}

public void BotDrawFromDeck()
{
    int drawAmount = GetDrawFromDeckAmount();

    for (int i = 0; i < drawAmount; i++)
    {
        DrawCard();
    }

    RefreshAllDisplays();
}

public void BotDrawFromDiscard()
{
    DrawFromDiscard();
    RefreshAllDisplays();
}

public void BotDiscardCardFromHand(int handIndex)
{
    DiscardCardFromHand(handIndex);
    RefreshAllDisplays();
}

public bool BotCanOfferFavor()
{
    return GetCurrentPlayer().favorArea.Count < 3 && GetCurrentPlayer().hand.Count > 0;
}

public void BotStartOfferFavor(int handIndex)
{
    StartCoroutine(OfferFavor(handIndex));
}

public void BotFinishActionAndWaitForEndTurn()
{
    FinishActionAndWaitForEndTurn();
}

public void BotEndPlayerTurn()
{
    int nextPlayerIndex = (turnManager.currentPlayerIndex + 1) % turnManager.players.Count;
    Player nextPlayer = turnManager.players[nextPlayerIndex];

    if (nextPlayer != null && !nextPlayer.isBot)
    {
        if (nextPlayerPanelController != null)
        {
            string nextPlayerName = nextPlayer.playerName;

            if (string.IsNullOrWhiteSpace(nextPlayerName))
                nextPlayerName = "Player " + (nextPlayerIndex + 1);

            nextPlayerPanelController.ShowNextPlayerPanel(nextPlayerName);
            return;
        }
    }

    EndPlayerTurn();
}

public void BotRefreshAllDisplays()
{
    RefreshAllDisplays();
    ShowActivePrankCards();
    RefreshAllHighlights();
}

public void ShowBotActionMessage(string message)
{
    if (turnText != null)
    {
        turnText.text = message;
        StartCoroutine(ShowTurnTextTemporarily());
    }

    Debug.Log(message);
}

public void BotOfferFavor(int handIndex)
{
    StartCoroutine(OfferFavor(handIndex));
}

public bool BotSwapWithOpponentFavor(int opponentIndex, int opponentFavorIndex, int handIndexToGive)
{
    Player currentPlayer = GetCurrentPlayer();

    if (opponentIndex < 0 || opponentIndex >= turnManager.players.Count)
        return false;

    if (opponentIndex == turnManager.currentPlayerIndex)
        return false;

    Player opponent = turnManager.players[opponentIndex];

    if (opponentFavorIndex < 0 || opponentFavorIndex >= opponent.favorArea.Count)
        return false;

    if (handIndexToGive < 0 || handIndexToGive >= currentPlayer.hand.Count)
        return false;

    PranksterDeckEntry gainedCard = opponent.favorArea[opponentFavorIndex];
    PranksterDeckEntry givenCard = currentPlayer.hand[handIndexToGive];

    opponent.favorArea[opponentFavorIndex] = new PranksterDeckEntry
    {
        pranksterType = givenCard.pranksterType,
        tier = givenCard.tier,
        category = givenCard.category
    };

    currentPlayer.hand[handIndexToGive] = new PranksterDeckEntry
    {
        pranksterType = gainedCard.pranksterType,
        tier = gainedCard.tier,
        category = gainedCard.category
    };

    SortCurrentPlayerHand();
    RefreshAllDisplays();

    Debug.Log("BOT SWAP: Gained " + gainedCard.pranksterType +
              " (tier " + gainedCard.tier + ", category " + gainedCard.category + ")" +
              " and gave " + givenCard.pranksterType +
              " (tier " + givenCard.tier + ", category " + givenCard.category + ")");

    return true;
}

IEnumerator AnimateEndGamePanel()
{
    if (endGameScoringPanel == null)
        yield break;

    Transform panelTransform = endGameScoringPanel.transform;
    panelTransform.localScale = new Vector3(0.6f, 0.6f, 1f);

    float t = 0f;
    float duration = 0.5f;

    while (t < duration)
    {
        t += Time.deltaTime;
        float progress = Mathf.Clamp01(t / duration);
        float scale = Mathf.Lerp(0.85f, 1f, progress);
        panelTransform.localScale = new Vector3(scale, scale, 1f);
        yield return null;
    }

    panelTransform.localScale = Vector3.one;
}

public void ShowBotTurnOverlay(string message)
{
    if (nextPlayerPanelController != null)
        nextPlayerPanelController.ShowBotMessage(message);
}

public void HideBotTurnOverlay()
{
    if (nextPlayerPanelController != null)
        nextPlayerPanelController.HideBotMessage();
}

public void OnRulesPanelOpened()
{
    isRulesPanelOpen = true;

    if (prankPreviewPanel != null)
        prankPreviewPanel.Hide();

    if (opponentPreviewPanel != null)
    {
        opponentPreviewPanel.UnlockSwap();
        opponentPreviewPanel.Hide();
    }

    SetAllPrankHighlightsVisible(false);
    HideOpponentPanelHighlights();
    RefreshAllHighlights(); // ✅ correct call
}

public void OnRulesPanelClosed()
{
    isRulesPanelOpen = false;

    RefreshAllHighlights();
}

void HideOpponentPanelHighlights()
{
    if (opponentDisplayManager == null)
        return;

    if (opponentDisplayManager.topLeftPanel != null)
        opponentDisplayManager.topLeftPanel.SetSwapHighlightVisible(false);

    if (opponentDisplayManager.topCenterPanel != null)
        opponentDisplayManager.topCenterPanel.SetSwapHighlightVisible(false);

    if (opponentDisplayManager.topRightPanel != null)
        opponentDisplayManager.topRightPanel.SetSwapHighlightVisible(false);
}

public bool IsRulesPanelOpen()
{
    return isRulesPanelOpen;
}

public bool IsInteractionBlocked()
{
    if (availableServicesPanelOpen)
        return true;

    if (isRulesPanelOpen)
        return true;

    if (IsSwapFlowActive())
        return true;

    if (nextPlayerPanelController != null && nextPlayerPanelController.IsPanelBlockingInteraction())
        return true;

    if (settingsMenuController != null && settingsMenuController.IsPanelBlockingInteraction())
        return true;

    return false;
}

void ResetPlayer1FavorTrackingForNewGame()
{
    player1FavorPointsThisGame.Clear();

    foreach (PranksterType type in System.Enum.GetValues(typeof(PranksterType)))
    {
        player1FavorPointsThisGame[type] = 0;
    }
}

void ApplyPlayer1MatchResultsToSave()
{
    Debug.Log("APPLY PLAYER 1 MATCH RESULTS START");

    if (player1ProgressSave == null)
    {
        Debug.Log("player1ProgressSave was null, loading save now");
        player1ProgressSave = SaveSystem.Load();
    }

    if (turnManager == null || turnManager.players == null || turnManager.players.Count == 0)
    {
        Debug.LogWarning("ApplyPlayer1MatchResultsToSave aborted: players not ready.");
        return;
    }

    Player player1 = turnManager.players[0];
    bool player1Won = DidPlayer1Win();
    int playerCount = turnManager.players.Count;

    Debug.Log("PLAYER 1 FINAL SCORE = " + player1.finalScore);
    Debug.Log("PLAYER COUNT = " + playerCount);
    Debug.Log("PLAYER 1 WON = " + player1Won);

    // Lifetime accumulated final score
    player1ProgressSave.lifetimeFinalScorePoints += player1.finalScore;
    Debug.Log("LIFETIME FINAL SCORE UPDATED TO = " + player1ProgressSave.lifetimeFinalScorePoints);

    // Highest single game score uses combined mischief from all players
    int combinedMischiefScore = 0;

    for (int i = 0; i < turnManager.players.Count; i++)
    {
        combinedMischiefScore += turnManager.players[i].renownPoints;
    }

    if (combinedMischiefScore > player1ProgressSave.highestSingleGameScore)
    {
        player1ProgressSave.highestSingleGameScore = combinedMischiefScore;
        Debug.Log("NEW HIGHEST SINGLE GAME MISCHIEF SCORE = " + player1ProgressSave.highestSingleGameScore);
    }
    else
    {
        Debug.Log("HIGHEST SINGLE GAME MISCHIEF SCORE REMAINS = " + player1ProgressSave.highestSingleGameScore);
    }

    // Win/loss by player count
    if (playerCount == 2)
    {
        if (player1Won)
            player1ProgressSave.wins2P++;
        else
            player1ProgressSave.losses2P++;

        Debug.Log("UPDATED 2P RECORD = " + player1ProgressSave.wins2P + "-" + player1ProgressSave.losses2P);
    }
    else if (playerCount == 3)
    {
        if (player1Won)
            player1ProgressSave.wins3P++;
        else
            player1ProgressSave.losses3P++;

        Debug.Log("UPDATED 3P RECORD = " + player1ProgressSave.wins3P + "-" + player1ProgressSave.losses3P);
    }
    else if (playerCount == 4)
    {
        if (player1Won)
            player1ProgressSave.wins4P++;
        else
            player1ProgressSave.losses4P++;

        Debug.Log("UPDATED 4P RECORD = " + player1ProgressSave.wins4P + "-" + player1ProgressSave.losses4P);
    }

    // Individual prank completions
    Debug.Log("PLAYER 1 COMPLETED PRANK COUNT THIS GAME = " + player1.completedPranks.Count);

    for (int i = 0; i < player1.completedPranks.Count; i++)
    {
        PrankCard prank = player1.completedPranks[i];
        Debug.Log("ADDING PRANK COMPLETION TO SAVE: " + prank.title);
        AddPrankCompletionToSave(prank.title);
    }

    // Favor points gained by prankster type
    foreach (var kvp in player1FavorPointsThisGame)
    {
        Debug.Log("ADDING FAVOR TO SAVE: " + kvp.Key + " = " + kvp.Value);
        AddFavorPointsToSave(kvp.Key, kvp.Value);
    }

    // Discard counts by prankster type
    foreach (var kvp in player1DiscardCountsThisGame)
    {
        Debug.Log("ADDING DISCARD COUNT TO SAVE: " + kvp.Key + " = " + kvp.Value);
        AddDiscardCountToSave(kvp.Key, kvp.Value);
    }

    // Available Service use counts by prankster type
    foreach (var kvp in player1AvailableServiceUsesThisGame)
    {
        Debug.Log("ADDING AVAILABLE SERVICE USE COUNT TO SAVE: " + kvp.Key + " = " + kvp.Value);
        AddAvailableServiceUseCountToSave(kvp.Key, kvp.Value);
    }

    Debug.Log("ABOUT TO EVALUATE UNLOCKS");
    List<PranksterUnlockEntry> newUnlocks = SaveSystem.EvaluateAndAwardUnlocksFromSavedProgress(player1ProgressSave);
    List<PranksterUnlockEntry> newFavorUnlocks = SaveSystem.EvaluateFavorUnlocks(player1ProgressSave);
    // Discard Unlocks are currently inactive.
    // List<PranksterUnlockEntry> newDiscardUnlocks = SaveSystem.EvaluateDiscardUnlocks(player1ProgressSave);
    List<PranksterUnlockEntry> newAvailableServiceUnlocks = SaveSystem.EvaluateAvailableServiceUnlocks(player1ProgressSave);
    Debug.Log("UNLOCK EVALUATION FINISHED");

    if (newUnlocks != null)
    {
        Debug.Log("NEW UNLOCK COUNT = " + newUnlocks.Count);

        for (int i = 0; i < newUnlocks.Count; i++)
        {
            Debug.Log("NEW UNLOCK RETURNED: " + newUnlocks[i].pranksterType +
                      " tier " + newUnlocks[i].tier +
                      " | order = " + newUnlocks[i].unlockOrder);
        }
    }
    else
    {
        Debug.LogWarning("UNLOCK EVALUATION RETURNED NULL");
    }

    List<PranksterUnlockEntry> sessionUnlocks = SaveSystem.GetSessionNewUnlocks();

    if (sessionUnlocks != null)
    {
        Debug.Log("SESSION NEW UNLOCK COUNT = " + sessionUnlocks.Count);

        for (int i = 0; i < sessionUnlocks.Count; i++)
        {
            Debug.Log("SESSION UNLOCK: " + sessionUnlocks[i].pranksterType +
                      " tier " + sessionUnlocks[i].tier +
                      " | order = " + sessionUnlocks[i].unlockOrder);
        }
    }
    else
    {
        Debug.LogWarning("SESSION NEW UNLOCKS LIST IS NULL");
    }

    Debug.Log("PRE-SAVE EARNED UNLOCK SNAPSHOT:");

    for (int i = 0; i < player1ProgressSave.pranksterUnlocks.Count; i++)
    {
        PranksterUnlockEntry unlock = player1ProgressSave.pranksterUnlocks[i];

        if (unlock != null && unlock.earned)
        {
            Debug.Log("PRE-SAVE UNLOCK: " + unlock.pranksterType +
                      " tier " + unlock.tier +
                      " | order = " + unlock.unlockOrder);
        }
    }

    Debug.Log("ABOUT TO SAVE PLAYER 1 PROGRESS");
    SaveSystem.Save(player1ProgressSave);
    Debug.Log("PLAYER 1 PROGRESS SAVED");

    Debug.Log("APPLY PLAYER 1 MATCH RESULTS END");
}

bool DidPlayer1Win()
{
    return HasReachedMayorBreakingPoint();
}

void AddPrankCompletionToSave(string prankTitle)
{
    for (int i = 0; i < player1ProgressSave.prankCompletions.Count; i++)
    {
        if (player1ProgressSave.prankCompletions[i].prankTitle == prankTitle)
        {
            player1ProgressSave.prankCompletions[i].timesCompleted++;
            return;
        }
    }

    player1ProgressSave.prankCompletions.Add(new PrankCompletionEntry
    {
        prankTitle = prankTitle,
        timesCompleted = 1
    });
}

void AddFavorPointsToSave(PranksterType pranksterType, int amount)
{
    string typeName = pranksterType.ToString();

    for (int i = 0; i < player1ProgressSave.favorPointsByType.Count; i++)
    {
        if (player1ProgressSave.favorPointsByType[i].pranksterType == typeName)
        {
            player1ProgressSave.favorPointsByType[i].totalFavorPointsGained += amount;
            return;
        }
    }

    player1ProgressSave.favorPointsByType.Add(new FavorPointsEntry
    {
        pranksterType = typeName,
        totalFavorPointsGained = amount
    });
}

[ContextMenu("Print Player 1 Save Data")]
public void PrintPlayer1SaveData()
{
    PlayerProgressSave data = SaveSystem.Load();

    Debug.Log("===== PLAYER 1 SAVE DATA =====");
    Debug.Log("2P W/L: " + data.wins2P + "/" + data.losses2P);
    Debug.Log("3P W/L: " + data.wins3P + "/" + data.losses3P);
    Debug.Log("4P W/L: " + data.wins4P + "/" + data.losses4P);
    Debug.Log("Lifetime Final Score Points: " + data.lifetimeFinalScorePoints);
    Debug.Log("Highest Single Game Score: " + data.highestSingleGameScore);

    Debug.Log("---- PRANK COMPLETIONS ----");
    for (int i = 0; i < data.prankCompletions.Count; i++)
    {
        Debug.Log(data.prankCompletions[i].prankTitle + ": " + data.prankCompletions[i].timesCompleted);
    }

    Debug.Log("---- FAVOR POINTS BY TYPE ----");
    for (int i = 0; i < data.favorPointsByType.Count; i++)
    {
        Debug.Log(data.favorPointsByType[i].pranksterType + ": " + data.favorPointsByType[i].totalFavorPointsGained);
    }

    Debug.Log("---- DISCARD COUNTS BY TYPE ----");
    for (int i = 0; i < data.discardCountsByType.Count; i++)
    {
        Debug.Log(data.discardCountsByType[i].pranksterType + ": " + data.discardCountsByType[i].totalDiscards);
    }
}

void ResetPlayer1DiscardTrackingForNewGame()
{
    player1DiscardCountsThisGame.Clear();
    player1AvailableServiceUsesThisGame.Clear();

    foreach (PranksterType type in System.Enum.GetValues(typeof(PranksterType)))
    {
        player1DiscardCountsThisGame[type] = 0;
        player1AvailableServiceUsesThisGame[type] = 0;
    }
}

void AddDiscardCountToSave(PranksterType pranksterType, int amount)
{
    string typeName = pranksterType.ToString();

    for (int i = 0; i < player1ProgressSave.discardCountsByType.Count; i++)
    {
        if (player1ProgressSave.discardCountsByType[i].pranksterType == typeName)
        {
            player1ProgressSave.discardCountsByType[i].totalDiscards += amount;
            return;
        }
    }

    player1ProgressSave.discardCountsByType.Add(new DiscardCountEntry
    {
        pranksterType = typeName,
        totalDiscards = amount
    });
}

List<PranksterType> ConvertHandToTypes(List<PranksterDeckEntry> handEntries)
{
    List<PranksterType> result = new List<PranksterType>();

    if (handEntries == null)
        return result;

    for (int i = 0; i < handEntries.Count; i++)
    {
        result.Add(handEntries[i].pranksterType);
    }

    return result;
}

public int CalculateTotalFavorForCard(PranksterDeckEntry card)
{
    if (card == null)
        return 0;

    int baseFavor = CalculateFavorPoints(card.pranksterType);
    int bonusFavor = 0;

    if (card.category == PranksterUnlockCategory.FavorOffer)
        bonusFavor = PranksterUnlockRules.GetFavorBonusForTier(card.tier);

    return baseFavor + bonusFavor;
}

void ShowEndOfRoundPanelBeforeReset()
{
    PrepareNextRoundInfo();

    pendingChoice = PendingChoiceType.None;
    highlightSuppressionCount = 1;

    if (endTurnButton != null)
        endTurnButton.SetActive(false);

    if (prankPreviewPanel != null)
        prankPreviewPanel.Hide();

    if (opponentPreviewPanel != null)
    {
        opponentPreviewPanel.UnlockSwap();
        opponentPreviewPanel.Hide();
    }

    if (nextPlayerPanelController != null)
        nextPlayerPanelController.HideBotMessage();

    string dealerName = turnManager.players[pendingRoundDealerIndex].playerName;
    string firstPlayerName = turnManager.players[pendingRoundFirstPlayerIndex].playerName;

    if (string.IsNullOrWhiteSpace(dealerName))
        dealerName = "Player " + (pendingRoundDealerIndex + 1);

    if (string.IsNullOrWhiteSpace(firstPlayerName))
        firstPlayerName = "Player " + (pendingRoundFirstPlayerIndex + 1);

    int influenceWinnerIndex = DetermineInfluenceWinnerIndex();
    Player influenceWinner = turnManager.players[influenceWinnerIndex];

    string influenceWinnerName = influenceWinner.playerName;

    if (string.IsNullOrWhiteSpace(influenceWinnerName))
        influenceWinnerName = "Player " + (influenceWinnerIndex + 1);

    bool playerChoosesLocation = influenceWinnerIndex == 0;

    string influenceWinnerText;

    if (playerChoosesLocation)
    {
        influenceWinnerText =
            influenceWinnerName + " has the most influence and will choose" + "\nthe location for the upcoming round.";
    }
    else
    {
        GameLocationType botLocation = GetRandomBotLocation();

        pendingRoundLocation = botLocation;

        influenceWinnerText =
            influenceWinnerName + " has the most influence.\n" +
            influenceWinnerName + " chooses " + GetLocationDisplayName(botLocation) + " for the upcoming round.";

        GameBackgroundManager backgroundManager = FindFirstObjectByType<GameBackgroundManager>();

        if (backgroundManager != null)
            backgroundManager.SetLocation(botLocation);
        else
            Debug.LogWarning("GameBackgroundManager not found. Bot location could not be applied.");
    }

    if (wantedJailController != null)
        wantedJailController.HideJailDisplay();

    if (wantedBoardPanelController != null &&
        wantedBoardPanelController.wantedDisplayPanelController != null)
    {
        wantedBoardPanelController.wantedDisplayPanelController.Hide();
    }

    if (endOfRoundPanelController != null)
    {
        endOfRoundPanelController.Show(
            dealerName,
            firstPlayerName,
            influenceWinnerText,
            playerChoosesLocation,
            () =>
            {
                Debug.Log("Start Next Round callback invoked.");

                if (wantedJailController != null)
                    wantedJailController.ShowJailDisplay();

                StartCoroutine(ResetRoundSequence());
            },
            OnChooseNextLocationFromEndOfRound
        );
    }
    else
    {
        Debug.LogWarning("EndOfRoundPanelController is not assigned. Resetting round immediately.");

        if (wantedJailController != null)
            wantedJailController.ShowJailDisplay();

        StartCoroutine(ResetRoundSequence());
    }
}

void PrepareNextRoundInfo()
{
    pendingRoundDealerIndex = DetermineDealerIndex();
    pendingRoundFirstPlayerIndex = (pendingRoundDealerIndex + 1) % turnManager.players.Count;
}

public bool TryShowEndOfRoundPanelIfPending()
{
    Debug.Log("TryShowEndOfRoundPanelIfPending CALLED | isEndOfRoundPending = " + isEndOfRoundPending);

    if (!isEndOfRoundPending)
    {
        Debug.Log("END OF ROUND FLAG FALSE → skipping panel");
        return false;
    }

    Debug.Log("END OF ROUND FLAG TRUE → showing panel");

    isEndOfRoundPending = false;
    ShowEndOfRoundPanelBeforeReset();
    return true;
}

public PranksterDeckEntry GetFavorCardAtIndex(int index)
{
    Player player = GetCurrentPlayer();

    if (player == null)
        return null;

    if (player.favorArea == null)
        return null;

    if (index < 0 || index >= player.favorArea.Count)
        return null;

    return player.favorArea[index];
}

IEnumerator PlayWinCutsceneThenShowResults(string videoFileName)
{
    Debug.Log("PLAYING WIN CUTSCENE: " + videoFileName);

    AudioListener.pause = true;

    bool finished = false;
    bool videoError = false;

    if (winVideoPlayer != null)
    {
        if (winCutsceneRenderTexture != null)
        {
            RenderTexture.active = winCutsceneRenderTexture;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = null;
        }

        string videoPath = Path.Combine(Application.streamingAssetsPath, videoFileName);

        winVideoPlayer.source = VideoSource.Url;
        winVideoPlayer.url = videoPath;

        Debug.Log("WIN VIDEO PATH: " + videoPath);

        winVideoPlayer.Stop();
        winVideoPlayer.time = 0;

        void OnPrepared(VideoPlayer vp)
        {
            Debug.Log("WIN VIDEO PREPARED");

            if (winCutsceneCanvas != null)
                winCutsceneCanvas.SetActive(true);

            vp.Play();
        }

        void OnFinished(VideoPlayer vp)
        {
            Debug.Log("WIN VIDEO FINISHED");
            finished = true;
        }

        void OnError(VideoPlayer vp, string message)
        {
            Debug.LogError("WIN VIDEO ERROR: " + message);
            videoError = true;
            finished = true;
        }

        winVideoPlayer.prepareCompleted += OnPrepared;
        winVideoPlayer.loopPointReached += OnFinished;
        winVideoPlayer.errorReceived += OnError;

        yield return new WaitForSecondsRealtime(0.75f);

        winVideoPlayer.Prepare();

        float timeout = 30f;
        float timer = 0f;

        while (!finished && !videoError && timer < timeout)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        if (timer >= timeout)
            Debug.LogWarning("WIN VIDEO TIMEOUT - showing results");

        winVideoPlayer.prepareCompleted -= OnPrepared;
        winVideoPlayer.loopPointReached -= OnFinished;
        winVideoPlayer.errorReceived -= OnError;
    }

    if (winCutsceneCanvas != null)
        winCutsceneCanvas.SetActive(false);

    AudioListener.pause = false;

    ShowFinalResultsUI();

    Debug.Log("Win cutscene finished, final results shown.");
}

public bool IsChoosingAvailableService()
{
    return pendingChoice == PendingChoiceType.ChooseAvailableServiceCard;
}

public void ResolveAvailableServiceCardChoice(int handIndex)
{
    Player player = GetCurrentPlayer();

    if (player == null)
        return;

    if (handIndex < 0 || handIndex >= player.hand.Count)
    {
        Debug.LogWarning("Invalid Available Service hand index: " + handIndex);
        return;
    }

    if (temporarilyAssignedServiceHandIndexes.Contains(handIndex))
    {
        Debug.Log("This hand card is already temporarily assigned to Available Services: " + handIndex);
        return;
    }

    PranksterDeckEntry card = player.hand[handIndex];

    if (card.pranksterType != selectedAvailableServiceType)
    {
        Debug.Log("Cannot assign " + card.pranksterType +
                  " to " + selectedAvailableServiceType +
                  " services. Card type must match selected service type.");
        return;
    }

    Sprite cardArt = PranksterSpriteDatabase.GetSprite(
        card.pranksterType,
        card.tier,
        card.category
    );

    // Add this BEFORE refreshing hand display.
    temporarilyAssignedServiceHandIndexes.Add(handIndex);

    bool assignedVisual = false;

    if (activeServicePanelController != null)
    {
        assignedVisual =
            activeServicePanelController.AssignCardToFirstAvailableSlot(cardArt, card);
    }

    if (!assignedVisual)
    {
        temporarilyAssignedServiceHandIndexes.Remove(handIndex);
        Debug.Log("Available Service assignment failed.");
        return;
    }

    activeServicePanelController.SetRetainServicesAvailable(
        temporarilyAssignedServiceHandIndexes.Count > 0
    );

    handDisplay.ShowCurrentPlayerHand();
    UpdateCrewCapacityDisplay();

    Debug.Log("Available Service temp assignment complete | handIndex=" + handIndex +
              " | tempCount=" + temporarilyAssignedServiceHandIndexes.Count);
}

public void StartAvailableServiceTurn(PranksterType serviceType)
{
    if (pendingChoice != PendingChoiceType.ChooseAction)
    {
        Debug.Log("Cannot start Available Service right now.");
        return;
    }

    selectedAvailableServiceType = serviceType;

    activeServicePanelController = null;

    if (availableServicesPanelController != null)
    {
        activeServicePanelController =
            availableServicesPanelController.GetPanelAssignmentController(serviceType);
    }

    if (activeServicePanelController == null)
    {
        Debug.LogWarning("No AvailableServicePanelAssignmentController found for: " + serviceType);
        return;
    }

    Player player = GetCurrentPlayer();

    AvailableServicesRetainedServiceGroup retainedGroup = null;

    foreach (AvailableServicesRetainedServiceGroup group in player.retainedServices)
    {
        if (group.serviceType == serviceType)
        {
            retainedGroup = group;
            break;
        }
    }

    if (retainedGroup != null)
    {
        activeServicePanelController.DisplayRetainedCards(retainedGroup.assignedCards);
    }
    else
    {
        activeServicePanelController.ClearAllAssignments();
    }

    activeServicePanelController.SetRetainServicesAvailable(false);

    pendingChoice = PendingChoiceType.ChooseAvailableServiceCard;

    Debug.Log("Starting Available Service selection for: " + serviceType);

    RefreshAllHighlights();
    ShowCurrentPlayerHand();
}

public bool IsHandCardTemporarilyAssignedToService(int handIndex)
{
    return temporarilyAssignedServiceHandIndexes.Contains(handIndex);
}

public void CancelAvailableServicesSelection()
{
    Debug.Log("Canceling Available Services selection. Current pendingChoice = " + pendingChoice);

    bool wasChoosingAvailableService =
        pendingChoice == PendingChoiceType.ChooseAvailableServiceCard;

    if (wasChoosingAvailableService)
        pendingChoice = PendingChoiceType.ChooseAction;

    temporarilyAssignedServiceHandIndexes.Clear();

    if (activeServicePanelController != null)
    {
        activeServicePanelController.ClearAllAssignments();
        activeServicePanelController.SetRetainServicesAvailable(false);
    }

    if (activeAvailableServiceSlotCollider != null)
    {
        if (!selectingInactiveInfluenceService && !viewingInactiveInfluenceServicePanel)
            activeAvailableServiceSlotCollider.SetAvailable(true);

        activeAvailableServiceSlotCollider = null;
        activeServicePanelController = null;
    }

    if (selectingInactiveInfluenceService || viewingInactiveInfluenceServicePanel)
    {
        CancelInactiveInfluenceServiceSelection();
    }

    handDisplay.ShowCurrentPlayerHand();

    RefreshAllHighlights();

    Debug.Log("Available Services selection canceled. New pendingChoice = " + pendingChoice);
}

public void SetActiveAvailableServiceSlotCollider(AvailableServiceSlotCollider slotCollider)
{
    activeAvailableServiceSlotCollider = slotCollider;
}

public void CommitRetainedServices()
{
    Debug.Log("CommitRetainedServices called.");

    if (activeServicePanelController == null)
    {
        Debug.LogWarning("Cannot commit retained services. Active service panel controller is missing.");
        return;
    }

    Player player = GetCurrentPlayer();

    if (temporarilyAssignedServiceHandIndexes.Count == 0)
    {
        Debug.Log("Cannot retain services. No new service cards were added this turn.");
        return;
    }

    List<PranksterDeckEntry> newlyAssignedCards = new List<PranksterDeckEntry>();

    foreach (int handIndex in temporarilyAssignedServiceHandIndexes)
    {
        if (handIndex >= 0 && handIndex < player.hand.Count)
        {
            PranksterDeckEntry card = player.hand[handIndex];

            newlyAssignedCards.Add(new PranksterDeckEntry
            {
                pranksterType = card.pranksterType,
                tier = card.tier,
                category = card.category
            });
        }
    }

    if (newlyAssignedCards.Count == 0)
    {
        Debug.LogWarning("No valid newly assigned service cards found in hand.");
        return;
    }

    AvailableServicesRetainedServiceGroup group = null;

    foreach (AvailableServicesRetainedServiceGroup existingGroup in player.retainedServices)
    {
        if (existingGroup.serviceType == selectedAvailableServiceType)
        {
            group = existingGroup;
            break;
        }
    }

    if (group == null)
    {
        group = new AvailableServicesRetainedServiceGroup();
        group.serviceType = selectedAvailableServiceType;
        player.retainedServices.Add(group);
    }

    foreach (PranksterDeckEntry card in newlyAssignedCards)
    {
        group.assignedCards.Add(card);
    }

    temporarilyAssignedServiceHandIndexes.Sort();
    temporarilyAssignedServiceHandIndexes.Reverse();

    foreach (int handIndex in temporarilyAssignedServiceHandIndexes)
    {
        if (handIndex >= 0 && handIndex < player.hand.Count)
        {
            player.hand.RemoveAt(handIndex);
        }
    }

    temporarilyAssignedServiceHandIndexes.Clear();

    activeServicePanelController.ClearAllAssignments();
    activeServicePanelController.SetRetainServicesAvailable(false);

    if (activeAvailableServiceSlotCollider != null)
    {
        activeAvailableServiceSlotCollider.SetAvailable(true);
        activeAvailableServiceSlotCollider = null;
    }

    activeServicePanelController = null;

    if (availableServicesPanelController != null)
    {
        availableServicesPanelController.CloseAllServicePanels();
    }

    Debug.Log("Retained " + newlyAssignedCards.Count +
              " new service card(s) for " + selectedAvailableServiceType +
              " on player " + turnManager.currentPlayerIndex +
              ". Total retained now: " + group.assignedCards.Count);

    FinishActionAndWaitForEndTurn();
}

public void ShowAvailableServiceStateOnly(PranksterType serviceType)
{
    selectedAvailableServiceType = serviceType;

    Player player = GetCurrentPlayer();

    AvailableServicesRetainedServiceGroup retainedGroup = null;

    foreach (AvailableServicesRetainedServiceGroup group in player.retainedServices)
    {
        if (group.serviceType == serviceType)
        {
            retainedGroup = group;
            break;
        }
    }

    activeServicePanelController = null;

    if (availableServicesPanelController != null)
    {
        activeServicePanelController =
            availableServicesPanelController.GetPanelAssignmentController(serviceType);
    }

    if (activeServicePanelController == null)
    {
        Debug.LogWarning("No AvailableServicePanelAssignmentController found for: " + serviceType);
        return;
    }

    if (retainedGroup != null)
    {
        activeServicePanelController.DisplayRetainedCards(retainedGroup.assignedCards);
    }
    else
    {
        activeServicePanelController.ClearAllAssignments();
    }

    activeServicePanelController.SetRetainServicesAvailable(false);

    Debug.Log("Showing Available Service state only for: " + serviceType);
}

public bool CanStartAvailableServiceAction()
{
    return pendingChoice == PendingChoiceType.ChooseAction;
}

void UpdateCrewCapacityDisplay()
{
    if (crewCapacityText == null)
        return;

    if (turnManager == null || turnManager.players == null || turnManager.players.Count == 0)
        return;

    Player player = turnManager.players[0];

    crewCapacityText.text =
        "Crew: " +
        player.hand.Count +
        "/" +
        player.maxHandSize;

    if (player.hand.Count > player.maxHandSize)
    {
        int dismissCount = player.hand.Count - player.maxHandSize;
        crewCapacityText.text += " Dismiss " + dismissCount;
    }
}

public void RefreshCrewCapacityDisplay()
{
    UpdateCrewCapacityDisplay();
}

IEnumerator FinishCompletePrankSequence()
{
    Debug.Log("FinishCompletePrankSequence START | isBot = " + GetCurrentPlayer().isBot);
    if (prankPreviewPanel != null)
        prankPreviewPanel.Hide();


    // yield return StartCoroutine(RefillHandToMaxOneCardAtATime(0.3f));

    RefreshAllDisplays();

    if (GetCurrentPlayer().isBot)
    {
        Debug.Log("BOT complete prank sequence finished");

        if (TryShowEndOfRoundPanelIfPending())
        {
            Debug.Log("BOT END OF ROUND PANEL SHOWN. Stopping bot turn flow.");

            if (nextPlayerPanelController != null)
                nextPlayerPanelController.HideBotMessage();

            if (botManager != null)
                botManager.NotifyBotActionHandledTurnFlow();

            yield break;
        }

        ShowActivePrankCards();
        RefreshAllHighlights();

        if (botManager != null)
            botManager.ShowPendingCompletedPrankMessageAndWaitForReady();

        yield break;
    }
    else
    {
        Debug.Log("HUMAN complete prank sequence waiting for End Turn");

        highlightSuppressionCount = 0;

        FinishActionAndWaitForEndTurn();
    }

    Debug.Log("FinishCompletePrankSequence END | isEndOfRoundPending = " + isEndOfRoundPending);
}

void ReturnRetainedServiceCardsToDeck()
{
    int returnedCount = 0;

    if (turnManager == null || turnManager.players == null)
        return;

    for (int i = 0; i < turnManager.players.Count; i++)
    {
        Player player = turnManager.players[i];

        if (player == null || player.retainedServices == null)
            continue;

        foreach (AvailableServicesRetainedServiceGroup group in player.retainedServices)
        {
            if (group == null || group.assignedCards == null)
                continue;

            foreach (PranksterDeckEntry entry in group.assignedCards)
            {
                if (entry == null)
                    continue;

                deck.Add(new PranksterDeckEntry
                {
                    pranksterType = entry.pranksterType,
                    tier = entry.tier,
                    category = entry.category
                });

                returnedCount++;
            }
        }

        player.retainedServices.Clear();
    }

    AvailableServicePanelAssignmentController[] panelControllers =
        FindObjectsByType<AvailableServicePanelAssignmentController>(FindObjectsSortMode.None);

    foreach (AvailableServicePanelAssignmentController controller in panelControllers)
    {
        if (controller != null)
            controller.ClearAllAssignments();
    }

    Debug.Log("Returned " + returnedCount + " retained Available Services card(s) to the prankster deck.");
}

public bool IsServiceTypeAvailableThisRound(PranksterType type)
{
    if (serviceAvailabilityThisRound.TryGetValue(type, out bool available))
        return available;

    Debug.LogWarning("Service availability was not cached for: " + type);
    return false;
}

void RefreshAvailableServiceSlotAvailability()
{
    AvailableServiceSlotCollider[] serviceSlots =
        FindObjectsByType<AvailableServiceSlotCollider>(FindObjectsSortMode.None);

    foreach (AvailableServiceSlotCollider slot in serviceSlots)
    {
        if (slot != null)
            slot.RefreshRoundAvailability();
    }

    Debug.Log("Refreshed Available Services slot availability for the round.");
}

public void ActivateAvailableServiceScoringAction()
{
    Player player = GetCurrentPlayer();

    if (player == null)
        return;

    PranksterType jailbreakType = selectedAvailableServiceType;

    if (player.activeScoringServiceTypes.Contains(jailbreakType))
    {
        Debug.Log("Jailbreak already used for: " + jailbreakType);
        return;
    }

    if (wantedBoardPanelController == null)
    {
        Debug.LogWarning("Cannot activate Jailbreak. WantedBoardPanelController is missing.");
        return;
    }

    if (wantedBoardPanelController.wantedJailController == null)
    {
        Debug.LogWarning("Cannot activate Jailbreak. WantedJailController is missing.");
        return;
    }

    if (!wantedBoardPanelController.wantedJailController.HasJailedRecruitOfType(jailbreakType))
    {
        Debug.LogWarning("Cannot activate Jailbreak. No jailed recruit of type: " + jailbreakType);
        return;
    }

    if (!wantedBoardPanelController.HasOrthogonallyConnectedGroupOfThree(jailbreakType))
    {
        Debug.LogWarning("Cannot activate Jailbreak. Need 3 orthogonally connected wanted posters of type: " + jailbreakType);
        return;
    }

    AvailableServicesRetainedServiceGroup groupToConsume = null;

    foreach (AvailableServicesRetainedServiceGroup group in player.retainedServices)
    {
        if (group.serviceType == jailbreakType)
        {
            groupToConsume = group;
            break;
        }
    }

    if (groupToConsume == null || groupToConsume.assignedCards == null)
    {
        Debug.LogWarning("Cannot activate Jailbreak. No retained card group found for: " + jailbreakType);
        return;
    }

    int cardsToSpend = GetScoringAvailableServiceCost();

    if (groupToConsume.assignedCards.Count < cardsToSpend)
    {
        Debug.LogWarning("Cannot activate Jailbreak. Need " + cardsToSpend +
                         " retained cards for: " +
                         jailbreakType +
                         " | current count=" + groupToConsume.assignedCards.Count);
        return;
    }

    for (int i = 0; i < cardsToSpend; i++)
    {
        PranksterDeckEntry card = groupToConsume.assignedCards[0];

        discardPile.Add(new PranksterDeckEntry
        {
            pranksterType = card.pranksterType,
            tier = card.tier,
            category = card.category
        });

        TrackPlayer1AvailableServiceUse(card);
        ApplyAvailableServiceCardBonus(card, player);

        groupToConsume.assignedCards.RemoveAt(0);
    }

    int remainingCount = groupToConsume.assignedCards.Count;

    if (remainingCount == 0)
        player.retainedServices.Remove(groupToConsume);

    bool jailbreakSucceeded =
        wantedBoardPanelController.wantedJailController.RemoveOneJailedRecruitOfType(jailbreakType);

    if (!jailbreakSucceeded)
    {
        Debug.LogWarning("Jailbreak failed after validation for: " + jailbreakType);
        return;
    }

    player.activeScoringServiceTypes.Add(jailbreakType);

    activeServicePanelController = null;

    if (availableServicesPanelController != null)
    {
        activeServicePanelController =
            availableServicesPanelController.GetPanelAssignmentController(jailbreakType);
    }

    if (activeServicePanelController != null)
    {
        if (remainingCount > 0)
        {
            activeServicePanelController.DisplayRetainedCards(groupToConsume.assignedCards);
        }
        else
        {
            activeServicePanelController.ClearAllAssignments();
        }

        activeServicePanelController.SetRetainServicesAvailable(false);
        activeServicePanelController.SetJailbreakUsedVisible(true);
    }
    else
    {
        Debug.LogWarning("Could not refresh service panel after Jailbreak for: " + jailbreakType);
    }

    if (availableServicesPanelController != null)
        availableServicesPanelController.CloseAllServicePanels();

    UpdateActiveFavorDisplay();
    RefreshAllDisplays();
    RefreshAllHighlights();

    Debug.Log("Activated Jailbreak for " + jailbreakType +
              " on Player " + (turnManager.currentPlayerIndex + 1) +
              ". Moved " + cardsToSpend +
              " retained service card(s) to discard pile. Remaining retained cards: " +
              remainingCount);

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayAvailableServiceScoringAction();

    FinishActionAndWaitForEndTurn();
}

int CountCompletedPrankIcons(Player player, PranksterType type)
{
    int count = 0;

    if (player == null || player.completedPranks == null)
        return count;

    foreach (PrankCard prank in player.completedPranks)
    {
        if (prank == null || prank.requiredPranksters == null)
            continue;

        foreach (PranksterType requiredType in prank.requiredPranksters)
        {
            if (requiredType == type)
                count++;
        }
    }

    return count;
}

public bool HasCurrentPlayerUsedScoringService(PranksterType serviceType)
{
    Player player = GetCurrentPlayer();

    if (player == null || player.activeScoringServiceTypes == null)
        return false;

    return player.activeScoringServiceTypes.Contains(serviceType);
}

public void SetAvailableServicesPanelOpen(bool open)
{
    availableServicesPanelOpen = open;
}

public bool IsAvailableServicesPanelOpen()
{
    return availableServicesPanelOpen;
}

public void ActivateLaborerImmediateAction(bool ignoreServiceRequirements = false)
{
    if (selectedAvailableServiceType != PranksterType.Laborer)
    {
        Debug.LogWarning("Cannot activate Laborer Immediate Action because selected service is: " + selectedAvailableServiceType);
        return;
    }

    StartCoroutine(ActivateLaborerImmediateActionSequence(ignoreServiceRequirements));
}

private IEnumerator ActivateLaborerImmediateActionSequence(bool ignoreServiceRequirements)
{
    Player player = GetCurrentPlayer();

    if (ignoreServiceRequirements)
    {
        if (!TrySpendInactiveInfluenceServiceCost())
            yield break;
    }

    if (!ignoreServiceRequirements)
    {
        AvailableServicesRetainedServiceGroup groupToConsume = null;

        foreach (AvailableServicesRetainedServiceGroup group in player.retainedServices)
        {
            if (group.serviceType == PranksterType.Laborer)
            {
                groupToConsume = group;
                break;
            }
        }

        int cardsToSpend = GetImmediateAvailableServiceCost();

        if (groupToConsume == null ||
            groupToConsume.assignedCards == null ||
            groupToConsume.assignedCards.Count < cardsToSpend)
        {
            Debug.LogWarning("Cannot activate Laborer Immediate Action. Need " +
                            cardsToSpend +
                            " retained Laborer cards.");

            yield break;
        }

        for (int i = 0; i < cardsToSpend; i++)
        {
            PranksterDeckEntry card = groupToConsume.assignedCards[0];

            discardPile.Add(new PranksterDeckEntry
            {
                pranksterType = card.pranksterType,
                tier = card.tier,
                category = card.category
            });

            TrackPlayer1AvailableServiceUse(card);
            ApplyAvailableServiceCardBonus(card, player);

            groupToConsume.assignedCards.RemoveAt(0);
        }

        if (groupToConsume.assignedCards.Count == 0)
            player.retainedServices.Remove(groupToConsume);
    }

    activeServicePanelController = null;

    if (availableServicesPanelController != null)
        availableServicesPanelController.CloseAllServicePanels();

    RefreshAllDisplays();

    yield return StartCoroutine(RefillHandToMaxOneCardAtATime(0.3f));

    RefreshAllDisplays();

    FinishActionAndWaitForEndTurn();
}

public void ActivateWizardImmediateAction(bool ignoreServiceRequirements = false)
{
    if (selectedAvailableServiceType != PranksterType.Wizard)
    {
        Debug.LogWarning("Cannot activate Wizard Immediate Action because selected service is: " + selectedAvailableServiceType);
        return;
    }

    StartCoroutine(ActivateWizardImmediateActionSequence(ignoreServiceRequirements));
}

private IEnumerator ActivateWizardImmediateActionSequence(bool ignoreServiceRequirements)
{
    Player player = GetCurrentPlayer();

    if (ignoreServiceRequirements)
    {
        if (!TrySpendInactiveInfluenceServiceCost())
            yield break;
    }

    if (player.favorArea == null || player.favorArea.Count == 0)
    {
        Debug.LogWarning("Cannot activate Wizard Immediate Action. No favor cards to return.");
        yield break;
    }

    if (!ignoreServiceRequirements)
    {
        AvailableServicesRetainedServiceGroup groupToConsume = null;

        foreach (AvailableServicesRetainedServiceGroup group in player.retainedServices)
        {
            if (group.serviceType == PranksterType.Wizard)
            {
                groupToConsume = group;
                break;
            }
        }

        int cardsToSpend = GetImmediateAvailableServiceCost();

        if (groupToConsume == null ||
            groupToConsume.assignedCards == null ||
            groupToConsume.assignedCards.Count < cardsToSpend)
        {
            Debug.LogWarning("Cannot activate Wizard Immediate Action. Need " +
                            cardsToSpend +
                            " retained Wizard cards.");

            yield break;
        }

        for (int i = 0; i < cardsToSpend; i++)
        {
            PranksterDeckEntry card = groupToConsume.assignedCards[0];

            discardPile.Add(new PranksterDeckEntry
            {
                pranksterType = card.pranksterType,
                tier = card.tier,
                category = card.category
            });

            TrackPlayer1AvailableServiceUse(card);
            ApplyAvailableServiceCardBonus(card, player);

            groupToConsume.assignedCards.RemoveAt(0);
        }

        if (groupToConsume.assignedCards.Count == 0)
            player.retainedServices.Remove(groupToConsume);
    }

    foreach (PranksterDeckEntry card in player.favorArea)
    {
        player.hand.Add(new PranksterDeckEntry
        {
            pranksterType = card.pranksterType,
            tier = card.tier,
            category = card.category
        });
    }

    int returnedCount = player.favorArea.Count;
    player.favorArea.Clear();

    activeServicePanelController = null;

    if (availableServicesPanelController != null)
        availableServicesPanelController.CloseAllServicePanels();

    SortCurrentPlayerHand();

    pendingChoice = PendingChoiceType.None;

    RefreshAllDisplays();
    RefreshAllHighlights();
    RefreshCrewCapacityDisplay();

    Debug.Log("Wizard returned all favor cards to hand. Count = " + returnedCount);

    ContinueDiscardingUntilHandAtMax();
}

private void ReturnFavorCardToHand(int favorSlotIndex)
{
    Player player = GetCurrentPlayer();

    if (favorSlotIndex < 0 || favorSlotIndex >= player.favorArea.Count)
    {
        Debug.LogWarning("Invalid favor slot index: " + favorSlotIndex);
        return;
    }

    PranksterDeckEntry card = player.favorArea[favorSlotIndex];

    player.favorArea.RemoveAt(favorSlotIndex);

    player.hand.Add(new PranksterDeckEntry
    {
        pranksterType = card.pranksterType,
        tier = card.tier,
        category = card.category
    });

    SortCurrentPlayerHand();

    pendingChoice = PendingChoiceType.None;

    RefreshAllDisplays();

    FinishActionAndWaitForEndTurn();

    Debug.Log("Returned favor card to hand: " + card.pranksterType);
}

public void ActivateEngineerImmediateAction(bool ignoreServiceRequirements = false)
{
    if (selectedAvailableServiceType != PranksterType.Engineer)
    {
        Debug.LogWarning("Cannot activate Engineer Immediate Action because selected service is: " + selectedAvailableServiceType);
        return;
    }

    StartCoroutine(ActivateEngineerImmediateActionSequence(ignoreServiceRequirements));
}

private IEnumerator ActivateEngineerImmediateActionSequence(bool ignoreServiceRequirements)
{
    Player player = GetCurrentPlayer();

    if (ignoreServiceRequirements)
    {
        if (!TrySpendInactiveInfluenceServiceCost())
            yield break;
    }

    if (!ignoreServiceRequirements)
    {
        AvailableServicesRetainedServiceGroup groupToConsume = null;

        foreach (AvailableServicesRetainedServiceGroup group in player.retainedServices)
        {
            if (group.serviceType == PranksterType.Engineer)
            {
                groupToConsume = group;
                break;
            }
        }

        int cardsToSpend = GetImmediateAvailableServiceCost();

        if (groupToConsume == null ||
            groupToConsume.assignedCards == null ||
            groupToConsume.assignedCards.Count < cardsToSpend)
        {
            Debug.LogWarning("Cannot activate Engineer Immediate Action. Need " +
                            cardsToSpend +
                            " retained Engineer cards.");

            yield break;
        }

        for (int i = 0; i < cardsToSpend; i++)
        {
            PranksterDeckEntry card = groupToConsume.assignedCards[0];

            discardPile.Add(new PranksterDeckEntry
            {
                pranksterType = card.pranksterType,
                tier = card.tier,
                category = card.category
            });

            TrackPlayer1AvailableServiceUse(card);
            ApplyAvailableServiceCardBonus(card, player);

            groupToConsume.assignedCards.RemoveAt(0);
        }

        if (groupToConsume.assignedCards.Count == 0)
            player.retainedServices.Remove(groupToConsume);
    }

    activeServicePanelController = null;

    if (availableServicesPanelController != null)
        availableServicesPanelController.CloseAllServicePanels();

    RefreshAllDisplays();

    pendingChoice = PendingChoiceType.ChooseEngineerPrankReplacement;
    RefreshAllHighlights();

    availableServiceInstructionPanel.ShowEngineerInstruction("Choose an active prank to replace.");

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayChooseAPrankToGetRidOf();

    Debug.Log("Choose an active prank to replace.");
}

private void ReplaceActivePrankAndMoveOldToBottom(int prankIndex)
{
    if (pendingChoice != PendingChoiceType.ChooseEngineerPrankReplacement)
        return;

    if (prankIndex < 0 || prankIndex >= activePranks.Count)
    {
        Debug.LogWarning("Invalid prank index for Engineer replacement: " + prankIndex);
        return;
    }

    if (prankDeck.Count == 0)
    {
        Debug.LogWarning("Cannot replace prank. Prank deck is empty.");
        return;
    }

    PrankCard oldPrank = activePranks[prankIndex];
    PrankCard newPrank = prankDeck[0];

    prankDeck.RemoveAt(0);
    activePranks[prankIndex] = newPrank;
    prankDeck.Add(oldPrank);

    HideAvailableServiceInstruction();

    pendingChoice = PendingChoiceType.None;

    ShowActivePrankCards();
    RefreshAllDisplays();
    RefreshAllHighlights();

    Debug.Log("Engineer replaced prank: " + oldPrank.title + " with " + newPrank.title);

    FinishActionAndWaitForEndTurn();
}

public bool IsChoosingEngineerPrankReplacement()
{
    return pendingChoice == PendingChoiceType.ChooseEngineerPrankReplacement;
}

public void ActivateThiefImmediateAction(bool ignoreServiceRequirements = false)
{
    if (selectedAvailableServiceType != PranksterType.Thief)
    {
        Debug.LogWarning("Cannot activate Thief Immediate Action because selected service is: " + selectedAvailableServiceType);
        return;
    }

    StartCoroutine(ActivateThiefImmediateActionSequence(ignoreServiceRequirements));
}

private IEnumerator ActivateThiefImmediateActionSequence(bool ignoreServiceRequirements)
{
    Player player = GetCurrentPlayer();

    if (ignoreServiceRequirements)
    {
        if (!TrySpendInactiveInfluenceServiceCost())
            yield break;
    }

    if (!AnyOpponentHasCardsInHand())
    {
        Debug.LogWarning("Cannot activate Thief Immediate Action. No opponents have cards to steal.");

        if (AudioManager.Instance != null)
        {
            if (turnManager.players.Count <= 2)
                AudioManager.Instance.PlayYourOpponentDoesntHaveAnyRecruits();
            else
                AudioManager.Instance.PlayYourOpponentsDontHaveAnyRecruits();
        }

        yield break;
    }

    if (!ignoreServiceRequirements)
    {
        AvailableServicesRetainedServiceGroup groupToConsume = null;

        foreach (AvailableServicesRetainedServiceGroup group in player.retainedServices)
        {
            if (group.serviceType == PranksterType.Thief)
            {
                groupToConsume = group;
                break;
            }
        }

        int cardsToSpend = GetImmediateAvailableServiceCost();

        if (groupToConsume == null ||
            groupToConsume.assignedCards == null ||
            groupToConsume.assignedCards.Count < cardsToSpend)
        {
            Debug.LogWarning("Cannot activate Thief Immediate Action. Need " +
                            cardsToSpend +
                            " retained Thief cards.");

            yield break;
        }

        for (int i = 0; i < cardsToSpend; i++)
        {
            PranksterDeckEntry card = groupToConsume.assignedCards[0];

            discardPile.Add(new PranksterDeckEntry
            {
                pranksterType = card.pranksterType,
                tier = card.tier,
                category = card.category
            });

            TrackPlayer1AvailableServiceUse(card);
            ApplyAvailableServiceCardBonus(card, player);

            groupToConsume.assignedCards.RemoveAt(0);
        }

        if (groupToConsume.assignedCards.Count == 0)
            player.retainedServices.Remove(groupToConsume);
    }

    activeServicePanelController = null;

    if (availableServicesPanelController != null)
        availableServicesPanelController.CloseAllServicePanels();

    ResolveThiefStealFromBarnaby();
}

private void ResolveThiefStealFromBarnaby()
{
    Player currentPlayer = GetCurrentPlayer();

    if (turnManager.players == null || turnManager.players.Count < 2)
    {
        Debug.LogWarning("Cannot resolve Thief action. Barnaby player was not found.");
        return;
    }

    Player barnaby = turnManager.players[1];

    if (barnaby == null || barnaby.hand == null || barnaby.hand.Count == 0)
    {
        Debug.LogWarning("Cannot resolve Thief action. Barnaby has no recruits to steal.");

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayYourOpponentDoesntHaveAnyRecruits();

        return;
    }

    int cardsToSteal = Mathf.Min(2, barnaby.hand.Count);

    for (int i = 0; i < cardsToSteal; i++)
    {
        int randomIndex = Random.Range(0, barnaby.hand.Count);
        PranksterDeckEntry stolenCard = barnaby.hand[randomIndex];

        barnaby.hand.RemoveAt(randomIndex);

        currentPlayer.hand.Add(new PranksterDeckEntry
        {
            pranksterType = stolenCard.pranksterType,
            tier = stolenCard.tier,
            category = stolenCard.category
        });

        Debug.Log("Thief stole from Barnaby: " + stolenCard.pranksterType);
    }

    SortCurrentPlayerHand();

    HideAvailableServiceInstruction();

    pendingChoice = PendingChoiceType.None;

    RefreshAllDisplays();
    RefreshAllHighlights();
    RefreshCrewCapacityDisplay();

    Debug.Log("THIEF HAND SIZE CHECK | hand=" + currentPlayer.hand.Count +
              " | max=" + currentPlayer.maxHandSize);

    ContinueDiscardingUntilHandAtMax();
}

public void ActivateScribeImmediateAction(bool ignoreServiceRequirements = false)
{
    if (selectedAvailableServiceType != PranksterType.Scribe)
    {
        Debug.LogWarning("Cannot activate Scribe Immediate Action because selected service is: " + selectedAvailableServiceType);
        return;
    }

    StartCoroutine(ActivateScribeImmediateActionSequence(ignoreServiceRequirements));
}

private IEnumerator ActivateScribeImmediateActionSequence(bool ignoreServiceRequirements)
{
    Player player = GetCurrentPlayer();

    if (ignoreServiceRequirements)
    {
        if (!TrySpendInactiveInfluenceServiceCost())
            yield break;
    }

    if (!ignoreServiceRequirements)
    {
        AvailableServicesRetainedServiceGroup groupToConsume = null;

        foreach (AvailableServicesRetainedServiceGroup group in player.retainedServices)
        {
            if (group.serviceType == PranksterType.Scribe)
            {
                groupToConsume = group;
                break;
            }
        }

        int cardsToSpend = GetImmediateAvailableServiceCost();

        if (groupToConsume == null ||
            groupToConsume.assignedCards == null ||
            groupToConsume.assignedCards.Count < cardsToSpend)
        {
            Debug.LogWarning("Cannot activate Scribe Immediate Action. Need " +
                            cardsToSpend +
                            " retained Scribe cards.");

            yield break;
        }

        for (int i = 0; i < cardsToSpend; i++)
        {
            PranksterDeckEntry card = groupToConsume.assignedCards[0];

            discardPile.Add(new PranksterDeckEntry
            {
                pranksterType = card.pranksterType,
                tier = card.tier,
                category = card.category
            });

            TrackPlayer1AvailableServiceUse(card);
            ApplyAvailableServiceCardBonus(card, player);

            groupToConsume.assignedCards.RemoveAt(0);
        }

        if (groupToConsume.assignedCards.Count == 0)
            player.retainedServices.Remove(groupToConsume);
    }

    activeServicePanelController = null;

if (availableServicesPanelController != null)
    availableServicesPanelController.CloseAllServicePanels();

pendingChoice = PendingChoiceType.None;

if (availableServiceInstructionPanel != null)
{
    availableServiceInstructionPanel.ShowScribeInstruction(
        "Choose a wanted poster, \nthen choose an empty space."
    );
}

if (wantedBoardPanelController == null)
{
    Debug.LogWarning("Cannot activate Scribe Move Poster service because wantedBoardPanelController is missing.");
    HideAvailableServiceInstruction();
    yield break;
}

if (!wantedBoardPanelController.CanMoveWantedPoster())
{
    Debug.LogWarning("Cannot activate Scribe Move Poster service. Need at least one wanted poster and one empty space.");
    HideAvailableServiceInstruction();
    yield break;
}

wantedBoardPanelController.BeginMovePosterMode(() =>
{
    HideAvailableServiceInstruction();

    RefreshAllDisplays();
    RefreshAllHighlights();

    FinishActionAndWaitForEndTurn();

    Debug.Log("Scribe moved a wanted poster.");
});

RefreshAllDisplays();
RefreshAllHighlights();

Debug.Log("Choose a wanted poster to move.");
}

private void ContinueDiscardingUntilHandAtMax()
{
    Player player = GetCurrentPlayer();

    if (player.hand.Count > player.maxHandSize)
    {
        pendingChoice = PendingChoiceType.ChooseDiscardFromHand;

        RefreshAllHighlights();
        RefreshHandVisuals();
        RefreshCrewCapacityDisplay();

        Debug.Log("Hand still above max size. Choose another card to discard.");
        ShowCurrentPlayerHand();

        return;
    }

    FinishActionAndWaitForEndTurn();
}

public bool IsChoosingBeastmasterDiscardType()
{
    return pendingChoice == PendingChoiceType.ChooseBeastmasterDiscardType;
}

public void ResolveBeastmasterDiscardTypeChoice(PranksterType chosenType)
{
    if (pendingChoice != PendingChoiceType.ChooseBeastmasterDiscardType)
    {
        Debug.Log("ResolveBeastmasterDiscardTypeChoice ignored because pendingChoice is: " + pendingChoice);
        return;
    }

    Player player = GetCurrentPlayer();

    int foundIndex = -1;

    for (int i = discardPile.Count - 1; i >= 0; i--)
    {
        if (discardPile[i].pranksterType == chosenType)
        {
            foundIndex = i;
            break;
        }
    }

    if (foundIndex < 0)
    {
        Debug.Log("No discard pile card found for type: " + chosenType);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayHoundsCouldntTrackThemDown();

        return;
    }

    PranksterDeckEntry recoveredCard = discardPile[foundIndex];

    discardPile.RemoveAt(foundIndex);

    player.hand.Add(new PranksterDeckEntry
    {
        pranksterType = recoveredCard.pranksterType,
        tier = recoveredCard.tier,
        category = recoveredCard.category
    });

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayMenuClick();

    SortCurrentPlayerHand();

    RefreshAvailableServiceSlotAvailability();

    HideAvailableServiceInstruction();

    pendingChoice = PendingChoiceType.None;

    RefreshAllDisplays();
    RefreshAllHighlights();
    RefreshCrewCapacityDisplay();

    Debug.Log("Beastmaster recovered discard card: " + recoveredCard.pranksterType);

    Debug.Log("BEASTMASTER HAND SIZE CHECK | hand=" + player.hand.Count +
            " | max=" + player.maxHandSize);

    ContinueDiscardingUntilHandAtMax();
}

public void ActivateBeastmasterImmediateAction(bool ignoreServiceRequirements = false)
{
    if (selectedAvailableServiceType != PranksterType.BeastMaster)
    {
        Debug.LogWarning("Cannot activate Beastmaster Immediate Action because selected service is: " + selectedAvailableServiceType);
        return;
    }

    StartCoroutine(ActivateBeastmasterImmediateActionSequence(ignoreServiceRequirements));
}

private IEnumerator ActivateBeastmasterImmediateActionSequence(bool ignoreServiceRequirements)
{
    Player player = GetCurrentPlayer();

    if (ignoreServiceRequirements)
    {
        if (!TrySpendInactiveInfluenceServiceCost())
            yield break;
    }

    if (!ignoreServiceRequirements)
    {
        AvailableServicesRetainedServiceGroup groupToConsume = null;

        foreach (AvailableServicesRetainedServiceGroup group in player.retainedServices)
        {
            if (group.serviceType == PranksterType.BeastMaster)
            {
                groupToConsume = group;
                break;
            }
        }

        int cardsToSpend = GetImmediateAvailableServiceCost();

        if (groupToConsume == null ||
            groupToConsume.assignedCards == null ||
            groupToConsume.assignedCards.Count < cardsToSpend)
        {
            Debug.LogWarning("Cannot activate Beastmaster Immediate Action. Need " +
                            cardsToSpend +
                            " retained Beastmaster cards.");

            yield break;
        }

        for (int i = 0; i < cardsToSpend; i++)
        {
            PranksterDeckEntry card = groupToConsume.assignedCards[0];

            discardPile.Add(new PranksterDeckEntry
            {
                pranksterType = card.pranksterType,
                tier = card.tier,
                category = card.category
            });

            TrackPlayer1AvailableServiceUse(card);
            ApplyAvailableServiceCardBonus(card, player);

            groupToConsume.assignedCards.RemoveAt(0);
        }

        if (groupToConsume.assignedCards.Count == 0)
            player.retainedServices.Remove(groupToConsume);
    }

    activeServicePanelController = null;

    if (availableServicesPanelController != null)
        availableServicesPanelController.CloseAllServicePanels();

    pendingChoice = PendingChoiceType.ChooseBeastmasterDiscardType;

    availableServiceInstructionPanel.ShowBeastmasterInstruction(
        "Choose a recruit type to recover\nfrom the discard pile."
    );

    ShowBeastmasterRecruitTypeSelectionGlows();

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayChooseARecruitForTheDogsToTrackDown();

    RefreshAllDisplays();
    RefreshAllHighlights();

    Debug.Log("Choose a recruit type from the Available Services icons.");
}

private void ShowBeastmasterRecruitTypeSelectionGlows()
{
    AvailableServiceSlotCollider[] serviceSlots =
        FindObjectsByType<AvailableServiceSlotCollider>(FindObjectsSortMode.None);

    foreach (AvailableServiceSlotCollider slot in serviceSlots)
    {
        if (slot != null)
            slot.SetAvailable(true);
    }

    Debug.Log("Beastmaster recruit type selection glows enabled.");
}

private bool AnyOpponentHasCardsInHand()
{
    Player currentPlayer = GetCurrentPlayer();

    foreach (Player player in turnManager.players)
    {
        if (player == currentPlayer)
            continue;

        if (player.hand != null && player.hand.Count > 0)
            return true;
    }

    return false;
}

public void HardResetRuntimeStateForMainMenu()
{
    StopAllCoroutines();

    pendingChoice = PendingChoiceType.None;
    hasTakenActionThisTurn = false;
    availableServicesPanelOpen = false;

    temporarilyAssignedServiceHandIndexes.Clear();

    selectedAvailableServiceType = default;
    activeServicePanelController = null;
    activeAvailableServiceSlotCollider = null;

    highlightSuppressionCount = 0;
    hoveredPrankIndex = -1;

    if (availableServicesPanelController != null)
        availableServicesPanelController.CloseAllServicePanels();

    if (endTurnButton != null)
        endTurnButton.SetActive(false);

    if (prankPreviewPanel != null)
        prankPreviewPanel.Hide();

    if (opponentPreviewPanel != null)
    {
        opponentPreviewPanel.UnlockSwap();
        opponentPreviewPanel.Hide();
    }

    RefreshAllHighlights();

    Debug.Log("DeckManager hard runtime state reset for main menu.");
}

void AddAvailableServiceUseCountToSave(PranksterType pranksterType, int amount)
{
    string typeName = pranksterType.ToString();

    for (int i = 0; i < player1ProgressSave.availableServiceUseCountsByType.Count; i++)
    {
        if (player1ProgressSave.availableServiceUseCountsByType[i].pranksterType == typeName)
        {
            player1ProgressSave.availableServiceUseCountsByType[i].totalAvailableServiceUses += amount;
            return;
        }
    }

    player1ProgressSave.availableServiceUseCountsByType.Add(new AvailableServiceUseCountEntry
    {
        pranksterType = typeName,
        totalAvailableServiceUses = amount
    });
}

void TrackPlayer1AvailableServiceUse(PranksterDeckEntry card)
{
    if (card == null)
        return;

    if (turnManager == null || turnManager.currentPlayerIndex != 0)
        return;

    if (!player1AvailableServiceUsesThisGame.ContainsKey(card.pranksterType))
    {
        player1AvailableServiceUsesThisGame[card.pranksterType] = 0;
    }

    player1AvailableServiceUsesThisGame[card.pranksterType]++;

    Debug.Log("TRACKED AVAILABLE SERVICE USE: " +
              card.pranksterType +
              " | total this game = " +
              player1AvailableServiceUsesThisGame[card.pranksterType]);
}

void ApplyAvailableServiceCardBonus(PranksterDeckEntry card, Player player)
{
    if (card == null || player == null)
        return;

    if (card.category != PranksterUnlockCategory.AvailableService)
        return;

    int influenceBonus = 0;
    int renownBonus = 0;

    if (card.tier == 1)
    {
        influenceBonus = 1;
    }
    else if (card.tier == 2)
    {
        influenceBonus = 1;
        renownBonus = 1;
    }
    else if (card.tier == 3)
    {
        influenceBonus = 2;
        renownBonus = 2;
    }

    if (influenceBonus > 0)
    {
        player.favorPoints += influenceBonus;

        if (turnManager != null && turnManager.currentPlayerIndex == 0)
        {
            if (!player1FavorPointsThisGame.ContainsKey(card.pranksterType))
                player1FavorPointsThisGame[card.pranksterType] = 0;

            player1FavorPointsThisGame[card.pranksterType] += influenceBonus;
        }
    }

    if (renownBonus > 0)
    {
        player.renownPoints += renownBonus;
    }

    Debug.Log("AVAILABLE SERVICE CARD BONUS APPLIED | " +
              card.pranksterType +
              " | tier=" + card.tier +
              " | influenceBonus=" + influenceBonus +
              " | renownBonus=" + renownBonus);
}

public void ShowAvailableServiceInstruction(string message)
{
    if (availableServiceInstructionPanel != null)
        availableServiceInstructionPanel.Show(message);
}

public void HideAvailableServiceInstruction()
{
    if (availableServiceInstructionPanel != null)
        availableServiceInstructionPanel.Hide();
}

[SerializeField] private LocationSelectionPanelController locationSelectionPanelController;

private void OnChooseNextLocationFromEndOfRound()
{
    Debug.Log("DeckManager: opening location selection panel.");

    if (wantedJailController != null)
        wantedJailController.HideJailDisplay();

    if (locationSelectionPanelController != null)
    {
        locationSelectionPanelController.Open((selectedLocation) =>
        {
            Debug.Log("DeckManager: location selected = " + selectedLocation);

            pendingRoundLocation = selectedLocation;

            Debug.Log("DeckManager: starting next round.");

            if (wantedJailController != null)
                wantedJailController.ShowJailDisplay();

            StartCoroutine(ResetRoundSequence());
        });
    }
    else
    {
        Debug.LogWarning("DeckManager: locationSelectionPanelController is not assigned.");

        if (wantedJailController != null)
            wantedJailController.ShowJailDisplay();

        StartCoroutine(ResetRoundSequence());
    }
}

GameLocationType GetRandomBotLocation()
{
    GameLocationType[] possibleLocations =
    {
        GameLocationType.SewerHideout,
        GameLocationType.ForestClearing,
        GameLocationType.OutsideTheWalls,
        GameLocationType.Treetop
    };

    int randomIndex = Random.Range(0, possibleLocations.Length);
    return possibleLocations[randomIndex];
}

int DetermineInfluenceWinnerIndex()
{
    int bestIndex = 0;

    for (int i = 1; i < turnManager.players.Count; i++)
    {
        Player current = turnManager.players[i];
        Player best = turnManager.players[bestIndex];

        if (current.favorPoints > best.favorPoints)
        {
            bestIndex = i;
        }
        else if (current.favorPoints == best.favorPoints)
        {
            if (current.renownPoints > best.renownPoints)
            {
                bestIndex = i;
            }
        }
    }

    return bestIndex;
}

string GetLocationDisplayName(GameLocationType locationType)
{
    switch (locationType)
    {
        case GameLocationType.RebelWorkshop:
            return "Rebel Workshop";

        case GameLocationType.SewerHideout:
            return "Sewer Hideout";

        case GameLocationType.ForestClearing:
            return "Forest Clearing";

        case GameLocationType.OutsideTheWalls:
            return "Outside the Walls";

        case GameLocationType.Treetop:
            return "Treetop";

        default:
            return locationType.ToString();
    }
}

int GetDrawFromDeckAmount()
{
    if (pendingRoundLocation == GameLocationType.ForestClearing)
        return 2;

    return 1;
}

IEnumerator DrawCardsOneAtATime(int cardsToDraw, float delayBetweenCards = 0.3f)
{
    for (int i = 0; i < cardsToDraw; i++)
    {
        int handCountBefore = GetCurrentPlayer().hand.Count;

        DrawCard();

        if (GetCurrentPlayer().hand.Count == handCountBefore)
        {
            Debug.Log("Could not draw more cards. Stopping draw sequence.");
            yield break;
        }

        RefreshAllDisplays();

        if (handDisplay != null)
            handDisplay.ShowCurrentPlayerHand();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDrawCardAction();

        yield return new WaitForSeconds(delayBetweenCards);
    }
}

IEnumerator ForestClearingSecondDraw()
{
    yield return new WaitForSeconds(0.3f);

    DrawCard();

    if (handDisplay != null)
        handDisplay.ShowCurrentPlayerHand();

    RefreshHandVisuals();
    RefreshCrewCapacityDisplay();

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayDrawCardAction();

    FinishDrawFromDeckTurn();
}

void FinishDrawFromDeckTurn()
{
    if (GetCurrentPlayer().hand.Count > GetCurrentPlayer().maxHandSize)
    {
        pendingChoice = PendingChoiceType.ChooseDiscardFromHand;

        RefreshAllHighlights();
        RefreshHandVisuals();
        RefreshCrewCapacityDisplay();

        if (AudioManager.Instance != null && Random.value < 0.6f)
            AudioManager.Instance.PlayHmmDecisions();

        LogSeparator("CHOOSE DISCARD");

        Debug.Log("Choose a card to discard.");
        ShowCurrentPlayerHand();
    }
    else
    {
        RefreshHandVisuals();
        FinishActionAndWaitForEndTurn();
    }
}

void ApplyCurrentLocationEffects()
{
    foreach (Player player in turnManager.players)
    {
        player.maxHandSize = 4 + player.lifetimeNotorietyMaxHandSizeBonus;

        if (pendingRoundLocation == GameLocationType.SewerHideout)
            player.maxHandSize += 1;
    }

    RefreshCrewCapacityDisplay();

    if (opponentDisplayManager != null)
        opponentDisplayManager.RefreshDisplays();
}

int GetActivePrankCountForCurrentLocation()
{
    if (pendingRoundLocation == GameLocationType.Treetop)
        return 5;

    return 4;
}

public bool IsAvailableServiceDiscountActive()
{
    return pendingRoundLocation == GameLocationType.OutsideTheWalls;
}

public int GetImmediateAvailableServiceCost()
{
    return IsAvailableServiceDiscountActive() ? 1 : 2;
}

public int GetScoringAvailableServiceCost()
{
    return IsAvailableServiceDiscountActive() ? 2 : 3;
}

public bool CanSafelyReadCurrentPlayer()
{
    if (turnManager == null)
        return false;

    if (turnManager.players == null)
        return false;

    if (turnManager.players.Count == 0)
        return false;

    if (turnManager.currentPlayerIndex < 0 ||
        turnManager.currentPlayerIndex >= turnManager.players.Count)
        return false;

    return true;
}

public void TryPurchaseLifetimeCrewSizeUpgrade()
{
    Player player = GetCurrentPlayer();

    if (hasTakenActionThisTurn)
        return;

    if (player == null)
        return;

    if (player.isBot)
        return;

    if (!SaveSystem.HasLifetimeCrewSizeUnlock())
        return;

    if (player.lifetimeNotorietyCrewUpgradeUsedThisGame)
        return;

    int cost = SaveSystem.GetLifetimeCrewSizeUpgradeCost();

    if (player.favorPoints < cost)
    {
        Debug.Log("Not enough influence to purchase lifetime crew size upgrade.");
        return;
    }

    player.favorPoints -= cost;

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlaySpendInfluence();

    player.lifetimeNotorietyMaxHandSizeBonus = 1;
    player.lifetimeNotorietyCrewUpgradeUsedThisGame = true;

    ApplyCurrentLocationEffects();

    Debug.Log("LIFETIME NOTORIETY ACTION: Spent " + cost + " influence to gain +1 max crew size this game.");

    FinishActionAndWaitForEndTurn();
}

public Player GetCurrentPlayerForUI()
{
    return GetCurrentPlayer();
}

public bool HasCurrentPlayerTakenActionThisTurn()
{
    return hasTakenActionThisTurn;
}

public bool IsSelectingInactiveInfluenceService()
{
    return selectingInactiveInfluenceService;
}

public int GetInactiveInfluenceServiceCost()
{
    return SaveSystem.GetInactiveInfluenceServiceCost();
}

public bool HasInactiveServiceOptionsThisRound()
{
    foreach (PranksterType type in System.Enum.GetValues(typeof(PranksterType)))
    {
        if (!IsServiceTypeAvailableThisRound(type))
            return true;
    }

    return false;
}

public void StartInactiveInfluenceServiceSelection()
{
    Player player = GetCurrentPlayer();

    Debug.Log(
        "START INACTIVE INFLUENCE CHECK | " +
        "playerNull=" + (player == null) +
        " | isBot=" + (player != null && player.isBot) +
        " | hasTakenActionThisTurn=" + hasTakenActionThisTurn +
        " | favor=" + (player != null ? player.favorPoints : -1) +
        " | cost=" + GetInactiveInfluenceServiceCost() +
        " | hasInactiveOptions=" + HasInactiveServiceOptionsThisRound()
    );

    if (player == null)
        return;

    if (player.isBot)
        return;

    if (hasTakenActionThisTurn)
        return;

    int cost = GetInactiveInfluenceServiceCost();

    if (player.favorPoints < cost)
        return;

    if (!HasInactiveServiceOptionsThisRound())
        return;

    selectingInactiveInfluenceService = true;

    Debug.Log("Inactive Influence Service selection started.");

    RefreshAvailableServiceSlotAvailability();
}

public void CancelInactiveInfluenceServiceSelection()
{
    selectingInactiveInfluenceService = false;
    viewingInactiveInfluenceServicePanel = false;

    RefreshAvailableServiceSlotAvailability();

    Debug.Log("Inactive Influence Service selection canceled.");
}

public void SetInactiveServicesButtonVisible(bool visible)
{
    if (inactiveServicesButton != null)
        inactiveServicesButton.SetVisible(visible);
}

public bool IsViewingInactiveInfluenceServicePanel()
{
    return viewingInactiveInfluenceServicePanel;
}

public void MarkInactiveInfluenceServicePanelOpen()
{
    if (!selectingInactiveInfluenceService)
        return;

    viewingInactiveInfluenceServicePanel = true;

    Debug.Log("Viewing inactive influence service panel.");
}

public void SetSelectedInactiveInfluenceServiceType(PranksterType serviceType)
{
    selectedAvailableServiceType = serviceType;
}

private bool TrySpendInactiveInfluenceServiceCost()
{
    Player player = GetCurrentPlayer();

    if (player == null)
        return false;

    int cost = GetInactiveInfluenceServiceCost();

    if (player.favorPoints < cost)
    {
        Debug.LogWarning("Cannot use inactive service action. Need " + cost + " influence.");
        return false;
    }

    player.favorPoints -= cost;
    hasTakenActionThisTurn = true;

    selectingInactiveInfluenceService = false;
    viewingInactiveInfluenceServicePanel = false;

    RefreshAvailableServiceSlotAvailability();
    UpdateActiveFavorDisplay();

    Debug.Log("Spent " + cost + " influence to use inactive service action.");

    return true;
}

private void CacheServiceAvailabilityForRound()
{
    serviceAvailabilityThisRound.Clear();

    foreach (PranksterType type in System.Enum.GetValues(typeof(PranksterType)))
    {
        serviceAvailabilityThisRound[type] = CalculateFavorPoints(type) <= 2;

        Debug.Log(
            "Cached service availability | " +
            type +
            " | availableThisRound=" +
            serviceAvailabilityThisRound[type]
        );
    }
}

public int GetPesterMayorCurrentCost()
{
    Player player = GetCurrentPlayerForUI();

    if (player == null)
        return 0;

    switch (player.pesterMayorUsesThisGame)
    {
        case 0: return 3;
        case 1: return 4;
        case 2: return 5;
        case 3: return 6;
        case 4: return 7;
        case 5: return 8;
        default: return 0;
    }
}

public int GetPesterMayorCurrentMischiefGain()
{
    Player player = GetCurrentPlayerForUI();

    if (player == null)
        return 0;

    switch (player.pesterMayorUsesThisGame)
    {
        case 0: return 1;
        case 1: return 3;
        case 2: return 5;
        case 3: return 7;
        case 4: return 10;
        case 5: return 15;
        default: return 0;
    }
}

public bool CanCurrentPlayerPesterMayor()
{
    Player player = GetCurrentPlayerForUI();

    if (player == null)
        return false;

    if (player.isBot)
        return false;

    if (hasTakenActionThisTurn)
        return false;

    if (!SaveSystem.HasPesterMayorUnlock())
        return false;

    if (player.pesterMayorUsesThisGame >= 6)
        return false;

    int cost = GetPesterMayorCurrentCost();

    return player.favorPoints >= cost;
}

public void TryPesterMayor()
{
    if (!CanCurrentPlayerPesterMayor())
        return;

    Player player = GetCurrentPlayer();

    int cost = GetPesterMayorCurrentCost();
    int mischiefGain = GetPesterMayorCurrentMischiefGain();

    player.favorPoints -= cost;
    player.renownPoints += mischiefGain;

    player.pesterMayorUsesThisGame++;

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayPesterMayorVoice(player.pesterMayorUsesThisGame);

    Debug.Log("PESTER MAYOR: Spent " + cost +
              " influence to gain " + mischiefGain +
              " mischief. Uses this game = " +
              player.pesterMayorUsesThisGame);

    if (HasReachedMayorBreakingPoint())
    {
        TriggerEndGameScoring();
        return;
    }

    FinishActionAndWaitForEndTurn();
}

public bool IsEndOfRoundPendingOrPanelOpen()
{
    if (isEndOfRoundPending)
        return true;

    if (endOfRoundPanelController != null &&
        endOfRoundPanelController.panelRoot != null &&
        endOfRoundPanelController.panelRoot.activeSelf)
    {
        return true;
    }

    return false;
}

IEnumerator TriggerEndGameAfterShowcase(float delay)
{
    yield return new WaitForSeconds(delay);

    TriggerEndGameScoring();
}

IEnumerator OpenWantedBoardThenContinue(PrankCard completedPrank, float showcaseDuration)
{
    yield return new WaitForSeconds(showcaseDuration);

    if (wantedBoardPanelController == null)
    {
        Debug.LogWarning("WantedBoardPanelController is not assigned. Continuing prank flow.");
        yield return StartCoroutine(FinishCompletePrankSequence());
        yield break;
    }

    wantedBoardOpen = true;

    bool placementFinished = false;
    bool lossTriggered = false;
    string lossReason = "";

    if (wantedJailController != null)
        wantedJailController.ShowJailDisplay();

    if (nextPlayerPanelController != null)
        nextPlayerPanelController.HidePanelImmediate();    

    wantedBoardPanelController.OpenForPrank(
        completedPrank,
        () =>
        {
            placementFinished = true;
        },
        (reason) =>
        {
            lossTriggered = true;
            lossReason = reason;
        });

    yield return new WaitUntil(() => placementFinished || lossTriggered);

    wantedBoardOpen = false;

    if (lossTriggered)
    {
        Debug.Log("FORCED LOSS FROM WANTED BOARD | " + lossReason);
        TriggerEndGameScoring();
        yield break;
    }

    yield return StartCoroutine(FinishCompletePrankSequence());
}

bool HasReachedMayorBreakingPoint()
{
    PlayerProgressSave saveData = SaveSystem.Load();

    int threshold = 150;

    if (saveData != null && saveData.hasUnlockedTwinMayor)
        threshold = 200;

    int combinedMischief = GetCombinedMischiefScore();

    Debug.Log("MAYOR BREAKING POINT CHECK | Mischief = " +
              combinedMischief + " / " + threshold);

    return combinedMischief >= threshold;
}
}

