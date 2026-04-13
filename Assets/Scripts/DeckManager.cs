using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;


public class DeckManager : MonoBehaviour
{
    List<PranksterType> deck = new List<PranksterType>();
    public TurnManager turnManager;
    List<PrankCard> prankDeck = new List<PrankCard>();
    List<PrankCard> activePranks = new List<PrankCard>();
    List<PranksterType> discardPile = new List<PranksterType>();
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
    public float prankCardSpacing = 2.8f;
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
    private List<PranksterType> tempSwapHand = null;
    private PranksterType pendingIncomingPrankster;
    private bool isInSwapHandSelection = false;

    public BotManager botManager;
    public GameObject gameCanvas;
    public bool isRulesPanelOpen = false;
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
            for (int i = 0; i < 9; i++)
            {
                deck.Add(prankster);
            }
        }
    }


    void ShufflePranksterDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(0, deck.Count);

            PranksterType temp = deck[i];
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

    for (int i = 0; i < 3; i++)
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

    PranksterType drawnCard = deck[0];

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

    Debug.Log("----- CALCULATE FAVOR POINTS -----");
    Debug.Log("Checking prankster type: " + pranksterType);

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

        Debug.Log(prank.title + " contributes: " + prankCount);
    }

    Debug.Log("TOTAL FAVOR = " + total);
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

    PranksterType offeredCard = player.hand[handIndex];

    player.hand.RemoveAt(handIndex);
    player.favorArea.Add(offeredCard);

    int favorGained = CalculateFavorPoints(offeredCard);
    player.favorPoints += favorGained;

    UpdateActiveFavorDisplay();
    RefreshAllDisplays();

    if (favorPreviewText != null)
        favorPreviewText.gameObject.SetActive(false);

    Debug.Log("Offered as favor: " + offeredCard);
    Debug.Log("Favor gained: " + favorGained);
    Debug.Log("Total favor points: " + player.favorPoints);

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayFavorReward();

    yield return new WaitForSeconds(0.3f);

    yield return StartCoroutine(RefillHandToFourOneCardAtATime(0.3f));

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

        List<PranksterType> tempHand = new List<PranksterType>(player.hand);

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

    foreach (PranksterType required in completedPrank.requiredPranksters)
    {
        int index = player.hand.IndexOf(required);

        if (index >= 0)
        {
            PranksterType card = player.hand[index];
            player.hand.RemoveAt(index);
            discardPile.Add(card);
        }
        else
        {
            Debug.LogError("Missing required card: " + required);
        }
    }

    player.completedPranks.Add(completedPrank);
    player.renownPoints += completedPrank.renownPoints;

    activePranks.RemoveAt(prankIndex);
    ShowActivePrankCards();

    Debug.Log("Completed prank: " + completedPrank.title);

    if (AudioManager.Instance != null)
    AudioManager.Instance.PlayCompletePrank();

    if (HasPlayerCompletedFourPranks())
    {
        if (GetCurrentPlayer().isBot && botManager != null)
            botManager.NotifyBotActionHandledTurnFlow();

        TriggerEndGameScoring();
        return;
    }

    if (activePranks.Count == 0)
    {
        if (GetCurrentPlayer().isBot && botManager != null)
         botManager.NotifyBotActionHandledTurnFlow();

        StartCoroutine(ResetRoundSequence());
        return;
    }

    StartCoroutine(FinishCompletePrankSequence());
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

        PranksterType card = discardPile[discardPile.Count - 1];

        discardPile.RemoveAt(discardPile.Count - 1);
        player.hand.Add(card);

        SortCurrentPlayerHand();

        Debug.Log("Drew from discard pile: " + card);
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

     PranksterType card = player.hand[handIndex];

        player.hand.RemoveAt(handIndex);
        discardPile.Add(card);

        discardPileDisplay.UpdateTopDiscardCard();

        Debug.Log("Discarded: " + card);
        RefreshAllDisplays();
    }


    void ShowDiscardPile()
    {
        Debug.Log("Discard pile:");

        foreach (PranksterType card in discardPile)
        {
            Debug.Log(card);
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

        foreach (PranksterType card in turnManager.players[i].hand)
        {
            Debug.Log(card);
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
        AudioManager.Instance.PlayPlayerTurnVoice(turnManager.currentPlayerIndex);

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

    handDisplay.ShowCurrentPlayerHand();

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayDrawCardAction();

    pendingChoice = PendingChoiceType.ChooseDiscardFromHand;
    RefreshAllHighlights();
    RefreshHandVisuals();

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayHmmDecisions();

    LogSeparator("CHOOSE DISCARD");

    Debug.Log("Choose a card to discard. Press 1, 2, 3, 4, or 5.");
    ShowCurrentPlayerHand();
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

    FinishActionAndWaitForEndTurn();
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

    handDisplay.ShowCurrentPlayerHand();
    discardPileDisplay.UpdateTopDiscardCard();

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayDrawCardAction();

    pendingChoice = PendingChoiceType.ChooseDiscardAfterDrawFromDiscard;
    RefreshAllHighlights();
    RefreshHandVisuals();

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayHmmDecisions();

    LogSeparator("CHOOSE DISCARD");

    Debug.Log("Choose a card to discard. Press 1, 2, 3, 4, or 5.");
    ShowCurrentPlayerHand();
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

    FinishActionAndWaitForEndTurn();
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

    Debug.Log("Choose a card to offer as favor. Press 1, 2, 3, 4, or 5.");
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
    turnManager.currentPlayerIndex = (dealerIndex + 1) % turnManager.players.Count;
    selectedSwapHandIndex = -1;
    pendingChoice = PendingChoiceType.None;

    Debug.Log("New dealer: Player " + (dealerIndex + 1));

    // Return all player hand cards to prankster deck
    for (int i = 0; i < turnManager.players.Count; i++)
    {
        Player player = turnManager.players[i];

        deck.AddRange(player.hand);
        player.hand.Clear();

        deck.AddRange(player.favorArea);
        player.favorArea.Clear();
    }

    // Return discard pile to prankster deck
    deck.AddRange(discardPile);
    discardPile.Clear();

    // Return out-of-play pranksters to prankster deck
    // OLD SYSTEM (out-of-play → deck)
    // deck.AddRange(outOfPlayPranksters);
    // outOfPlayPranksters.Clear();

    // Shuffle prankster deck
    ShufflePranksterDeck();

    // Deal fresh hands
    DealStartingHands();

    // Deal 3 new active pranks
    DealActivePranks();
    ShowActivePrankCards();

    Debug.Log("New round started.");

    UpdateActiveFavorDisplay();
    RefreshAllDisplays();
    Debug.Log("New dealer: Player " + (dealerIndex + 1));
    Debug.Log("First player this round: Player " + (firstPlayerIndex + 1)); 

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
        return (lastPrankCompleterIndex + 1) % turnManager.players.Count;
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

    PranksterType topCard = discardPile[discardPile.Count - 1];
    Debug.Log("Top of Discard Pile: " + topCard);
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

    // Explicitly clear preview panel lock now that swap is committed
    if (opponentPreviewPanel != null)
    {
        opponentPreviewPanel.UnlockSwap();
        opponentPreviewPanel.Hide();
    }

    isInSwapHandSelection = false;
    originalHandSnapshot = null;
    tempSwapHand = null;
    pendingIncomingPrankster = default;

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

    originalHandSnapshot = new List<PranksterType>(currentPlayer.hand);
    pendingIncomingPrankster = targetPlayer.favorArea[selectedSwapFavorIndex];

    tempSwapHand = new List<PranksterType>(originalHandSnapshot);
    tempSwapHand.Add(pendingIncomingPrankster);

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

    PranksterType handCard = currentPlayer.hand[selectedSwapHandIndex];
    PranksterType favorCard = targetPlayer.favorArea[targetFavorIndex];

    currentPlayer.hand[selectedSwapHandIndex] = favorCard;
    targetPlayer.favorArea[targetFavorIndex] = handCard;

    currentPlayer.hand.Sort((a, b) => a.ToString().CompareTo(b.ToString()));

    Debug.Log("Swapped " + handCard +
              " from hand with " + favorCard +
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

    // Show end game canvas and scoring panel
    if (endGameCanvas != null)
        endGameCanvas.SetActive(true);

    if (endGameScoringPanel != null)
        endGameScoringPanel.SetActive(true);

    // Start entry animation after panel is active
    StartCoroutine(AnimateEndGamePanel());

    // Set final prank image if available
    if (finalCompletedPrank != null && finalPrankImage != null)
    {
        finalPrankImage.sprite = finalCompletedPrank.cardSprite;
    }
    else
    {
        Debug.LogWarning("Final prank image not assigned or finalCompletedPrank is NULL");
    }

    // Fill the score table
    ShowGameOverPanel();

    DeclareWinnerByScore();
    Debug.Log("DeclareWinnerByScore COMPLETE");

    // Disable gameplay layer so it cannot block clicks
    if (gameCanvas != null)
        gameCanvas.SetActive(false);

    if (endTurnButton != null)
        endTurnButton.SetActive(false);

    Debug.Log("TriggerEndGameScoring END");
}

void CalculateFinalScores()
{
    if (finalCompletedPrank == null)
    {
        Debug.LogError("Final completed prank is null. Cannot calculate final scores.");
        return;
    }

    Debug.Log("Final completed prank: " + finalCompletedPrank.title);
    Debug.Log("Final prank favor multiplier: " + finalCompletedPrank.favorMultiplier);

    for (int i = 0; i < turnManager.players.Count; i++)
    {
        Player player = turnManager.players[i];

        int prankPoints = player.renownPoints;
        int favorPoints = player.favorPoints;
        int victoryPoints = favorPoints * finalCompletedPrank.favorMultiplier;
        int totalScore = prankPoints + victoryPoints;

        player.finalScore = totalScore;

        string displayName = player.playerName;

        if (string.IsNullOrEmpty(displayName))
            displayName = "Player " + (i + 1);

        Debug.Log(displayName + " Final Score:");
        Debug.Log("  Completed Pranks: " + player.completedPranks.Count);
        Debug.Log("  Prank Points: " + prankPoints);
        Debug.Log("  Favor Points: " + favorPoints);
        Debug.Log("  Victory Points: " + victoryPoints);
        Debug.Log("  TOTAL: " + totalScore);
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

        if (CanCompletePrank(i) && completablePrankHighlightPrefab != null)
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

public PranksterType? GetTopDiscardCard()
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
    Player currentPlayer = GetCurrentPlayer();

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
        Sprite icon = GetFavorIcon(player.favorArea[index]);

        Debug.Log("Updating slot " + index + " with " + player.favorArea[index]);
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

    if (discardPileDisplay != null)
        discardPileDisplay.UpdateTopDiscardCard();

    if (opponentDisplayManager != null)
        opponentDisplayManager.RefreshDisplays();

    RefreshActionHighlights();
}

void UpdateCurrentPlayerStatsDisplay()
{
    Player currentPlayer = GetCurrentPlayer();
    int currentIndex = turnManager.currentPlayerIndex;

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
}

void ReshuffleDiscardIntoDeck()
{
    if (discardPile.Count == 0)
    {
        Debug.Log("No discard pile to reshuffle.");
        return;
    }

    Debug.Log("Reshuffling discard pile into deck.");

    deck.AddRange(discardPile);
    discardPile.Clear();
    ShufflePranksterDeck();

    if (discardPileDisplay != null)
        discardPileDisplay.UpdateTopDiscardCard();
}

void ShowGameOverPanel()
{
    if (endGameScoringPanel == null)
    {
        Debug.LogWarning("EndGameScoringPanel is not assigned.");
        return;
    }

    endGameScoringPanel.SetActive(true);

    int playerCount = turnManager.players.Count;

    if (player1Row != null) player1Row.SetActive(playerCount >= 1);
    if (player2Row != null) player2Row.SetActive(playerCount >= 2);
    if (player3Row != null) player3Row.SetActive(playerCount >= 3);
    if (player4Row != null) player4Row.SetActive(playerCount >= 4);

    PopulateScoreRow(0, player1NameText, player1PrankPointsText, player1FavorPointsText, player1TotalPointsText);
    PopulateScoreRow(1, player2NameText, player2PrankPointsText, player2FavorPointsText, player2TotalPointsText);
    PopulateScoreRow(2, player3NameText, player3PrankPointsText, player3FavorPointsText, player3TotalPointsText);
    PopulateScoreRow(3, player4NameText, player4PrankPointsText, player4FavorPointsText, player4TotalPointsText);
}

void PopulateScoreRow(
    int playerIndex,
    TextMeshProUGUI nameText,
    TextMeshProUGUI prankPointsText,
    TextMeshProUGUI favorPointsText,
    TextMeshProUGUI totalPointsText)
{
    if (playerIndex < 0 || playerIndex >= turnManager.players.Count)
        return;

    Player player = turnManager.players[playerIndex];

    int prankPoints = player.renownPoints;
    int favorPoints = player.favorPoints;
    int totalPoints = player.finalScore;

    if (nameText != null)
    {
        string displayName = player.playerName;

        if (string.IsNullOrEmpty(displayName))
            displayName = "Player " + (playerIndex + 1);

        nameText.text = displayName;
    }

    if (prankPointsText != null)
        prankPointsText.text = prankPoints.ToString();

    if (favorPointsText != null)
        favorPointsText.text = favorPoints.ToString();

    if (totalPointsText != null)
        totalPointsText.text = totalPoints.ToString();
}

public void BeginNewGame()
{
    if (favorPreviewText != null)
        favorPreviewText.gameObject.SetActive(false);

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

    // Clear all runtime data
    deck.Clear();
    prankDeck.Clear();
    activePranks.Clear();
    discardPile.Clear();
    outOfPlayPranksters.Clear();

    // Reset state
    pendingChoice = PendingChoiceType.None;
    lastPrankCompleterIndex = -1;
    selectedSwapHandIndex = -1;
    gameOver = false;
    finalCompletedPrank = null;
    hoveredPrankIndex = -1;
    hasTakenActionThisTurn = false;

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
    }

    // Build and shuffle decks (ONLY ONCE)
    BuildPranksterDeck();
    ShufflePranksterDeck();

    prankDeck = PrankDatabase.CreatePrankDeck();
    ShufflePrankDeck();

    Debug.Log("Prankster Deck created with " + deck.Count + " cards");
    Debug.Log("Prank deck size: " + prankDeck.Count);

    // Start game flow
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

public void OnFavorAreaClicked()
{
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

    bool completed = AttemptCompletePrank(prankIndex);

    if (!completed)
    {
        Debug.Log("Prank completion failed. Returning to ChooseAction.");
        pendingChoice = PendingChoiceType.ChooseAction;
        Debug.Log("pendingChoice set to ChooseAction from OnPrankCardClicked fail branch");
        RefreshAllHighlights();
        return;
    }

    pendingChoice = PendingChoiceType.None;
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

IEnumerator FinishCompletePrankSequence()
{
    Debug.Log("FinishCompletePrankSequence START | isBot = " + GetCurrentPlayer().isBot);

    yield return StartCoroutine(RefillHandToFourOneCardAtATime(0.3f));

    RefreshAllDisplays();

    if (GetCurrentPlayer().isBot)
    {
        Debug.Log("BOT complete prank sequence finished");

        ShowActivePrankCards();
        RefreshAllHighlights();

        yield break;
    }
    else
    {
        Debug.Log("HUMAN complete prank sequence waiting for End Turn");
        FinishActionAndWaitForEndTurn();
    }
}

IEnumerator DealStartingHandsOneCardAtATime(float delayBetweenCards = 0.2f)
{
    for (int cardNumber = 0; cardNumber < 4; cardNumber++)
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

    int dealerIndex = DetermineDealerIndex();
    turnManager.currentPlayerIndex = dealerIndex;
    selectedSwapHandIndex = -1;
    pendingChoice = PendingChoiceType.None;

    Debug.Log("New dealer: Player " + (dealerIndex + 1));

    for (int i = 0; i < turnManager.players.Count; i++)
    {
        Player player = turnManager.players[i];

        deck.AddRange(player.hand);
        player.hand.Clear();

        deck.AddRange(player.favorArea);
        player.favorArea.Clear();
    }

    deck.AddRange(discardPile);
    discardPile.Clear();

    ShufflePranksterDeck();

    yield return StartCoroutine(DealStartingHandsOneCardAtATime(0.2f));

    DealActivePranks();
    ShowActivePrankCards();

    Debug.Log("New round started.");

    UpdateActiveFavorDisplay();
    RefreshAllDisplays();
    StartPlayerTurn();
}

IEnumerator BeginNewGameSequence()
{
    yield return StartCoroutine(DealStartingHandsOneCardAtATime(0.2f));

    Debug.Log("Hands Dealt");
    Debug.Log("Cards left in deck: " + deck.Count);

    DealActivePranks();
    ShowActivePrankCards();

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

    StartPlayerTurn();
}

public bool ShouldHighlightOpponentPanel(int representedPlayerIndex)
{
    Debug.Log("---- ShouldHighlightOpponentPanel START ----");
    Debug.Log("representedPlayerIndex = " + representedPlayerIndex);
    Debug.Log("currentPlayerIndex = " + turnManager.currentPlayerIndex);
    Debug.Log("pendingChoice = " + pendingChoice);
    Debug.Log("highlightSuppressionCount = " + highlightSuppressionCount);

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
        Debug.Log("FAIL: pendingChoice is not ChooseAction");
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

public List<PranksterType> GetTempSwapHand()
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
    return discardPile;
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
    DrawCard();
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

    PranksterType gainedCard = opponent.favorArea[opponentFavorIndex];
    PranksterType givenCard = currentPlayer.hand[handIndexToGive];

    opponent.favorArea[opponentFavorIndex] = givenCard;
    currentPlayer.hand[handIndexToGive] = gainedCard;

    SortCurrentPlayerHand();
    RefreshAllDisplays();

    Debug.Log("BOT SWAP: Gained " + gainedCard + " and gave " + givenCard);

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

}

