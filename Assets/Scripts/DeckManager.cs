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
    PendingChoiceType pendingChoice = PendingChoiceType.None;
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

    public AudioSource audioSource;
    public AudioClip player1TurnClip;
    public AudioClip player2TurnClip;
    public AudioClip hmmmDecisionsClip;

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


    Player GetCurrentPlayer()
    {
        return turnManager.GetCurrentPlayer();
    }


    void Start()
    {
        BuildPranksterDeck();
        ShufflePranksterDeck();

        Debug.Log("Prankster Deck created with " + deck.Count + " cards");

        DealStartingHands();

        Debug.Log("Hands Dealt");
        Debug.Log("Cards left in deck: " + deck.Count);

        prankDeck = PrankDatabase.CreatePrankDeck();
        
        ShufflePrankDeck();
        DealActivePranks();
        ShowActivePrankCards();
        // ShowActivePranks(); may need for debugging later
        Debug.Log("Prank deck size: " + prankDeck.Count);
        Debug.Log("Active pranks: " + activePranks.Count);
    
        UpdateActiveFavorDisplay();
        StartPlayerTurn();
        handDisplay.ShowCurrentPlayerHand();
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
        Debug.Log("Deck empty");
        return;
    }

    PranksterType drawnCard = deck[0];

    GetCurrentPlayer().hand.Add(drawnCard);
    deck.RemoveAt(0);

    SortCurrentPlayerHand();

    Debug.Log("Drew card: " + drawnCard);
}


    void RefillHandToFour()
{
        while (GetCurrentPlayer().hand.Count < 4 && deck.Count > 0)
        {
            DrawCard();
        }
}


    int CalculateFavorPoints(PranksterType pranksterType)
{
        int total = 0;

        foreach (PrankCard prank in activePranks)
        {
            foreach (PranksterType requiredPrankster in prank.requiredPranksters)
            {
                if (requiredPrankster == pranksterType)
                {
                    total++;
                }
            }
        }

        return total;
}


    void OfferFavor(int handIndex)
    {
        Player player = GetCurrentPlayer();

        if (handIndex < 0 || handIndex >= player.hand.Count)
        {
            Debug.LogWarning("Invalid hand index");
            return;
        }

        PranksterType offeredCard = player.hand[handIndex];

        player.hand.RemoveAt(handIndex);
        player.favorArea.Add(offeredCard);

        int favorGained = CalculateFavorPoints(offeredCard);
        player.favorPoints += favorGained;

        UpdateActiveFavorDisplay();

        RefillHandToFour();

        Debug.Log("Offered as favor: " + offeredCard);
        Debug.Log("Favor gained: " + favorGained);
        Debug.Log("Total favor points: " + player.favorPoints);

        EndPlayerTurn();
    }


    bool CanCompletePrank(int prankIndex)
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


    void AttemptCompletePrank(int prankIndex)
{
    if (prankIndex < 0 || prankIndex >= activePranks.Count)
    {
        Debug.Log("Invalid prank index.");
        return;
    }

    if (!CanCompletePrank(prankIndex))
    {
        Debug.Log("Cannot complete prank.");
        return;
    }

    CompletePrank(prankIndex);
}

    void CompletePrank(int prankIndex)
{
    Player player = GetCurrentPlayer();

    PrankCard completedPrank = activePranks[prankIndex];

    lastPrankCompleterIndex = turnManager.currentPlayerIndex;

    foreach (PranksterType required in completedPrank.requiredPranksters)
    {
        int index = player.hand.IndexOf(required);

        if (index >= 0)
        {
            PranksterType card = player.hand[index];
            player.hand.RemoveAt(index);
            outOfPlayPranksters.Add(card);
        }
    }

    player.completedPranks.Add(completedPrank);
    activePranks.RemoveAt(prankIndex);
    ShowActivePrankCards();

    Debug.Log("Completed prank: " + completedPrank.title);

    if (HasPlayerCompletedThreePranks())
    {
        finalCompletedPrank = completedPrank;
        TriggerEndGameScoring();
        return;
    }

    if (activePranks.Count == 0)
    {
        ResetRound();
        StartPlayerTurn();
        return;
    }

    RefillHandToFour();
    EndPlayerTurn();
}


    bool HasPlayerCompletedThreePranks()
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
    Debug.Log("==================================================");
    Debug.Log("PLAYER " + (turnManager.currentPlayerIndex + 1) + " TURN");
    Debug.Log("==================================================");

    pendingChoice = PendingChoiceType.ChooseAction;

    handDisplay.ShowCurrentPlayerHand();
    UpdateActiveFavorDisplay();

    if (turnText != null)
    {
        turnText.text = "PLAYER " + (turnManager.currentPlayerIndex + 1) + "'S TURN";
        StartCoroutine(ShowTurnTextTemporarily());
    }

    if (audioSource != null)
    {
        if (turnManager.currentPlayerIndex == 0 && player1TurnClip != null)
         audioSource.PlayOneShot(player1TurnClip);

        if (turnManager.currentPlayerIndex == 1 && player2TurnClip != null)
         audioSource.PlayOneShot(player2TurnClip);
    }

    Debug.Log("Player " + (turnManager.currentPlayerIndex + 1) + "'s turn.");
    ShowCurrentPlayerHand();
    ShowTopDiscardCard();
    ShowAllFavorAreas();
    ShowActivePranks();

    Debug.Log("Can click draw pile: " + CanClickDrawPile());
    Debug.Log("Can click discard pile: " + CanClickDiscardPile());
    Debug.Log("Turn state reset to: " + pendingChoice);
}

void EndPlayerTurn()
{
    turnManager.NextPlayer();
    StartPlayerTurn();
}

void StartDrawFromDeckTurn()
{
    if (!CanClickDrawPile())
    {
        Debug.Log("Draw pile is not a valid choice right now. pendingChoice = " + pendingChoice + ", deck.Count = " + deck.Count);
        return;
    }

    DrawCard();

    handDisplay.ShowCurrentPlayerHand();

    if (audioSource != null && hmmmDecisionsClip != null)
    {
        audioSource.PlayOneShot(hmmmDecisionsClip);
    }

    pendingChoice = PendingChoiceType.ChooseDiscardFromHand;

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
    handDisplay.ShowCurrentPlayerHand();

    pendingChoice = PendingChoiceType.None;
    EndPlayerTurn();
}

void Update()
{
    if (gameOver)
        return;

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

    if (pendingChoice == PendingChoiceType.ChooseSwapHandCard)
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { ResolveSwapHandChoice(0); return; }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { ResolveSwapHandChoice(1); return; }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { ResolveSwapHandChoice(2); return; }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { ResolveSwapHandChoice(3); return; }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { ResolveSwapHandChoice(4); return; }
    }

    if (pendingChoice == PendingChoiceType.ChooseSwapTarget)
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { ResolveSwapTargetChoice(1); return; }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { ResolveSwapTargetChoice(2); return; }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { ResolveSwapTargetChoice(3); return; }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { ResolveSwapTargetChoice(4); return; }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { ResolveSwapTargetChoice(5); return; }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { ResolveSwapTargetChoice(6); return; }
        if (Input.GetKeyDown(KeyCode.Alpha7)) { ResolveSwapTargetChoice(7); return; }
        if (Input.GetKeyDown(KeyCode.Alpha8)) { ResolveSwapTargetChoice(8); return; }
        if (Input.GetKeyDown(KeyCode.Alpha9)) { ResolveSwapTargetChoice(9); return; }
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

void ShowActivePranks()
{
    Debug.Log("Active Pranks:");

    for (int i = 0; i < activePranks.Count; i++)
    {
        PrankCard prank = activePranks[i];

        string requirementsText = "";

        for (int j = 0; j < prank.requiredPranksters.Count; j++)
        {
            requirementsText += prank.requiredPranksters[j];

            if (j < prank.requiredPranksters.Count - 1)
            {
                requirementsText += ", ";
            }
        }

        Debug.Log(
            "Prank " + (i + 1) + ": " +
            prank.title +
            " | Requires: " + requirementsText +
            " | Renown: " + prank.renownPoints +
            " | Favor Multiplier: " + prank.favorMultiplier
        );
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
        return pendingChoice == PendingChoiceType.ChooseAction && deck.Count > 0;
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

    if (audioSource != null && hmmmDecisionsClip != null)
    {
        audioSource.PlayOneShot(hmmmDecisionsClip);
    }

    pendingChoice = PendingChoiceType.ChooseDiscardAfterDrawFromDiscard;

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
    handDisplay.ShowCurrentPlayerHand();
    discardPileDisplay.UpdateTopDiscardCard();

    pendingChoice = PendingChoiceType.None;
    EndPlayerTurn();
}

void StartOfferFavorTurn()
{
    if (!CanStartOfferFavor())
    {
        Debug.Log("You cannot offer favor right now.");
        return;
    }

    pendingChoice = PendingChoiceType.ChooseFavorCard;

    LogSeparator("CHOOSE FAVOR CARD");

    Debug.Log("Choose a card to offer as favor. Press 1, 2, 3, 4, or 5.");
    ShowCurrentPlayerHand();
}

void ResolveFavorChoice(int handIndex)
{
    if (pendingChoice != PendingChoiceType.ChooseFavorCard)
        return;

    Debug.Log("Favor choice selected: hand index " + handIndex);

    pendingChoice = PendingChoiceType.None;
    OfferFavor(handIndex);
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
    turnManager.currentPlayerIndex = dealerIndex;
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
    deck.AddRange(outOfPlayPranksters);
    outOfPlayPranksters.Clear();

    // Shuffle prankster deck
    ShufflePranksterDeck();

    // Deal fresh hands
    DealStartingHands();

    // Deal 3 new active pranks
    DealActivePranks();
    ShowActivePrankCards();

    Debug.Log("New round started.");

    UpdateActiveFavorDisplay();
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

void StartSwapFavorTurn()
{
    if (!CanStartSwapFavor())
    {
        Debug.Log("You cannot swap favor right now.");
        return;
    }

    pendingChoice = PendingChoiceType.ChooseSwapHandCard;

    LogSeparator("CHOOSE HAND CARD TO SWAP");

    Debug.Log("Choose a card from your hand to swap. Press 1, 2, 3, 4, or 5.");
    ShowCurrentPlayerHand();
}

void ResolveSwapHandChoice(int handIndex)
{
    Player player = GetCurrentPlayer();

    if (pendingChoice != PendingChoiceType.ChooseSwapHandCard)
        return;

    if (handIndex < 0 || handIndex >= player.hand.Count)
    {
        Debug.Log("That hand card is not a valid swap choice. Choose again.");
        ShowCurrentPlayerHand();
        return;
    }

    selectedSwapHandIndex = handIndex;
    pendingChoice = PendingChoiceType.ChooseSwapTarget;

    LogSeparator("CHOOSE FAVOR TARGET");

    Debug.Log("Choose a favor card to take.");
    ShowSwappableFavorChoices();
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

void ResolveSwapTargetChoice(int choiceNumber)
{
    if (pendingChoice != PendingChoiceType.ChooseSwapTarget)
        return;

    if (selectedSwapHandIndex == -1)
    {
        Debug.Log("No hand card selected for swap.");
        return;
    }

    int runningIndex = 1;

    for (int i = 0; i < turnManager.players.Count; i++)
    {
        if (i == turnManager.currentPlayerIndex)
            continue;

        Player otherPlayer = turnManager.players[i];

        for (int j = 0; j < otherPlayer.favorArea.Count; j++)
        {
            if (runningIndex == choiceNumber)
            {
                ExchangeFavorCards(i, j);
                return;
            }

            runningIndex++;
        }
    }

    Debug.Log("That favor card is not a valid swap target. Choose again.");
    ShowSwappableFavorChoices();
}

void ExchangeFavorCards(int targetPlayerIndex, int targetFavorIndex)
{
    Player currentPlayer = GetCurrentPlayer();
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

    Debug.Log("Swapped " + handCard + " from hand with " + favorCard + " from Player " + (targetPlayerIndex + 1) + "'s favor area.");

    selectedSwapHandIndex = -1;
    pendingChoice = PendingChoiceType.None;
    EndPlayerTurn();
}

void TriggerEndGameScoring()
{
    gameOver = true;

    LogSeparator("GAME OVER TRIGGERED");
    Debug.Log("Player " + (turnManager.currentPlayerIndex + 1) + " completed their 3rd prank.");
    Debug.Log("Final completed prank: " + finalCompletedPrank.title);
    Debug.Log("Calculating final scores...");

    CalculateFinalScores();
    DeclareWinnerByScore();
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

        int prankPoints = 0;
        for (int j = 0; j < player.completedPranks.Count; j++)
        {
            prankPoints += player.completedPranks[j].renownPoints;
        }

        int favorPoints = player.favorPoints;
        int victoryPoints = favorPoints * finalCompletedPrank.favorMultiplier;
        int totalScore = prankPoints + victoryPoints;

        player.finalScore = totalScore;

        Debug.Log("Player " + (i + 1) + " Final Score:");
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

    // 1. Highest final score
    int bestFinalScore = int.MinValue;
    for (int i = 0; i < contenders.Count; i++)
    {
        if (contenders[i].finalScore > bestFinalScore)
        {
            bestFinalScore = contenders[i].finalScore;
        }
    }

    contenders.RemoveAll(p => p.finalScore < bestFinalScore);

    // 2. Most completed pranks
    if (contenders.Count > 1)
    {
        int bestCompletedPranks = int.MinValue;
        for (int i = 0; i < contenders.Count; i++)
        {
            if (contenders[i].completedPranks.Count > bestCompletedPranks)
            {
                bestCompletedPranks = contenders[i].completedPranks.Count;
            }
        }

        contenders.RemoveAll(p => p.completedPranks.Count < bestCompletedPranks);
    }

    // 3. Highest favor points
    if (contenders.Count > 1)
    {
        int bestFavorPoints = int.MinValue;
        for (int i = 0; i < contenders.Count; i++)
        {
            if (contenders[i].favorPoints > bestFavorPoints)
            {
                bestFavorPoints = contenders[i].favorPoints;
            }
        }

        contenders.RemoveAll(p => p.favorPoints < bestFavorPoints);
    }

    // 4. Player who triggered endgame
    if (contenders.Count > 1)
    {
        Player endgamePlayer = turnManager.players[turnManager.currentPlayerIndex];

        if (contenders.Contains(endgamePlayer))
        {
            contenders.Clear();
            contenders.Add(endgamePlayer);
        }
    }

    // 5. Shared victory
    if (contenders.Count == 1)
    {
        int winnerIndex = turnManager.players.IndexOf(contenders[0]);

        Debug.Log("GAME OVER: Player " + (winnerIndex + 1) + " wins with " + contenders[0].finalScore + " points!");
    }
    else
    {
        string sharedWinners = "";

        for (int i = 0; i < contenders.Count; i++)
        {
            int playerIndex = turnManager.players.IndexOf(contenders[i]);
            sharedWinners += "Player " + (playerIndex + 1);

            if (i < contenders.Count - 1)
            {
                sharedWinners += ", ";
            }
        }

        Debug.Log("GAME OVER: Shared victory between " + sharedWinners + " with " + bestFinalScore + " points!");
    }
}

void ShowActivePrankCards()
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

    // Clear old prank card visuals first
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

        // Find the CardArt child
        Transform cardArtTransform = prankObject.transform.Find("CardArt");

        if (cardArtTransform != null)
        {
            SpriteRenderer artRenderer = cardArtTransform.GetComponent<SpriteRenderer>();

            if (artRenderer != null)
            {
                Debug.Log("Showing prank: " + activePranks[i].title);

                if (activePranks[i].cardSprite != null)
                {
                    Debug.Log("Assigned sprite: " + activePranks[i].cardSprite.name);
                    artRenderer.sprite = activePranks[i].cardSprite;
                }
                else
                {
                    Debug.LogWarning("No sprite assigned for prank: " + activePranks[i].title);
                }
            }
            else
            {
                Debug.LogWarning("CardArt has no SpriteRenderer on prank: " + activePranks[i].title);
            }
        }
        else
        {
            Debug.LogWarning("CardArt child not found on prank prefab.");
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

    if (activeFavorPointsText != null)
        activeFavorPointsText.text = currentPlayer.favorPoints.ToString();
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

}

