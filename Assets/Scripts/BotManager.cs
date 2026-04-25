using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

class BotSwapCandidate
{
    public int opponentIndex;
    public int opponentFavorIndex;
    public int handIndexToGive;
    public int prankIndex;

    public PranksterDeckEntry gainedCard;   // ✅ full card, not just type
    public int gainedCardFavorValue;

    public int prankRenown;
    public int giveAwayScore;
}

public class BotManager : MonoBehaviour
{
    public DeckManager deckManager;
    public TurnManager turnManager;
    public NextPlayerPanelController nextPlayerPanelController;
    private bool botActionHandledTurnFlow = false;

    Player GetCurrentPlayer()
    {
        return turnManager.GetCurrentPlayer();
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

    PranksterType GetHandTypeAt(Player player, int handIndex)
    {
    if (player == null || handIndex < 0 || handIndex >= player.hand.Count)
        return PranksterType.Thief;

    return player.hand[handIndex].pranksterType;
    }

    public void TakeBotTurn()
    {
    string actionMessage = TakeBotTurnAndReturnMessage();

    if (!string.IsNullOrEmpty(actionMessage))
        deckManager.ShowBotTurnOverlay(actionMessage);

    // CompletePrank runs its own DeckManager coroutine flow.
    // In that case, do not force another end-turn here.
    if (actionMessage != null && actionMessage.StartsWith("Completed prank:"))
        return;

    deckManager.BotRefreshAllDisplays();
    StartCoroutine(EndTurnAfterDelay(1.2f));
    }

    string TakeBotTurnAndReturnMessage()
    {
    botActionHandledTurnFlow = false;

    // ===== COMPLETE PRANK =====
    int prankIndex = FindBestCompletablePrankIndex();
    if (prankIndex != -1)
    {
        List<PrankCard> activePranks = deckManager.BotGetActivePranks();
        string prankName = activePranks[prankIndex].title;

        Debug.Log("BOT: Completing prank at index " + prankIndex);
        deckManager.BotCompletePrank(prankIndex);

        return "Completed prank:\n" + prankName;
    }

    // ===== DISCARD FOR 4/4 =====
    if (TryTakeDiscardForExactProgress(4, out string discardMessage4))
        return discardMessage4;

    if (TrySwapForFavorCardForExactProgress(4, out string swapMessage4))
        return swapMessage4;

    // ===== DISCARD FOR 3/4 =====
    if (TryTakeDiscardForExactProgress(3, out string discardMessage3))
        return discardMessage3;

    if (TrySwapForFavorCardForExactProgress(3, out string swapMessage3))
        return swapMessage3;

    // ===== OFFER FAVOR =====
    if (!HandContainsFavorValue(0))
    {
        if (TryOfferFavor(out string favorMessage))
            return favorMessage;
    }

    // ===== FALLBACK: DRAW =====
    Debug.Log("BOT: No strong visible action. Drawing from deck.");

    if (DrawFromDeckAndDiscardBestChoice(out string drawMessage))
        return drawMessage;

    return "Drew from deck";
    }

    bool HandContainsFavorValue(int value)
{
    Player player = GetCurrentPlayer();

    foreach (PranksterDeckEntry card in player.hand)
    {
        int favorValue = deckManager.CalculateTotalFavorForCard(card);

        if (favorValue == value)
            return true;
    }

    return false;
}                              

    int CountProgressTowardPrank(List<PranksterType> hand, PrankCard prank)
    {
        List<PranksterType> tempHand = new List<PranksterType>(hand);
        int matched = 0;

        foreach (PranksterType requiredCard in prank.requiredPranksters)
        {
            if (tempHand.Contains(requiredCard))
            {
                tempHand.Remove(requiredCard);
                matched++;
            }
        }

        return matched;
    }

    int FindBestCompletablePrankIndex()
    {
        List<PrankCard> activePranks = deckManager.BotGetActivePranks();
        List<int> completableIndexes = new List<int>();

        for (int i = 0; i < activePranks.Count; i++)
        {
            if (deckManager.BotCanCompletePrank(i))
                completableIndexes.Add(i);
        }

        if (completableIndexes.Count == 0)
            return -1;

        int bestIndex = completableIndexes[0];
        int bestRenown = activePranks[bestIndex].renownPoints;

        for (int i = 1; i < completableIndexes.Count; i++)
        {
            int testIndex = completableIndexes[i];
            int testRenown = activePranks[testIndex].renownPoints;

            if (testRenown > bestRenown)
            {
                bestIndex = testIndex;
                bestRenown = testRenown;
            }
        }

        return bestIndex;
    }

    bool TryCompletePrank()
{
    int prankIndex = FindBestCompletablePrankIndex();

    if (prankIndex == -1)
        return false;

    List<PrankCard> activePranks = deckManager.BotGetActivePranks();
    string prankName = activePranks[prankIndex].title;

    Debug.Log("BOT: Completing prank at index " + prankIndex);
    deckManager.BotCompletePrank(prankIndex);

    return true;
}

    bool CardCreatesExactProgress(PranksterType candidateCard, int targetProgress, out int bestPrankIndex)
{
    bestPrankIndex = -1;

    Player player = GetCurrentPlayer();
    List<PrankCard> activePranks = deckManager.BotGetActivePranks();
    List<PranksterType> currentHandTypes = ConvertHandToTypes(player.hand);

    for (int i = 0; i < activePranks.Count; i++)
    {
        int progressBefore = CountProgressTowardPrank(currentHandTypes, activePranks[i]);

        List<PranksterType> simulatedHand = new List<PranksterType>(currentHandTypes);
        simulatedHand.Add(candidateCard);

        int progressAfter = CountProgressTowardPrank(simulatedHand, activePranks[i]);

        Debug.Log(
            "BOT CHECK: " + candidateCard +
            " | Prank: " + activePranks[i].title +
            " | Before: " + progressBefore +
            " | After: " + progressAfter
        );

        bool cardActuallyImprovedProgress =
            progressBefore < targetProgress &&
            progressAfter == targetProgress;

        if (cardActuallyImprovedProgress)
        {
            bestPrankIndex = i;
            return true;
        }
    }

    return false;
}   

    bool TryTakeDiscardForExactProgress(int targetProgress, out string actionMessage)
{   
    actionMessage = "";

    List<PranksterType> discardPile = deckManager.BotGetDiscardPile();

    if (discardPile.Count == 0)
        return false;

    PranksterType topCard = discardPile[discardPile.Count - 1];
    Player player = GetCurrentPlayer();

    if (!CardCreatesExactProgress(topCard, targetProgress, out int bestPrankIndex))
        return false;

    List<PranksterType> simulatedHand = ConvertHandToTypes(player.hand);
    simulatedHand.Add(topCard);

    int bestDiscardIndex = -1;
    int bestScore = int.MinValue;

    for (int i = 0; i < simulatedHand.Count; i++)
    {
        if (i == simulatedHand.Count - 1)
            continue;

        List<PranksterType> handAfterDiscard = new List<PranksterType>(simulatedHand);
        handAfterDiscard.RemoveAt(i);

        int progressAfterDiscard = CountProgressTowardPrank(handAfterDiscard, deckManager.BotGetActivePranks()[bestPrankIndex]);

        if (progressAfterDiscard < targetProgress)
            continue;

        // ✅ FIX: use full card instead of type-only
        PranksterDeckEntry card = player.hand[i];
        int favorValue = deckManager.CalculateTotalFavorForCard(card);

        bool supportsThree = false;
        bool supportsPair = false;

        if (i < player.hand.Count)
        {
            supportsThree = CardSupportsThreeOfFour(i);
            supportsPair = CardSupportsAnyPair(i);
        }

        int score = 0;

        if (!supportsThree && !supportsPair)
            score += 100;

        if (!supportsThree && supportsPair)
            score += 40;

        if (supportsThree)
            score -= 100;

        score += (10 - favorValue);
        score += Random.Range(0, 3);

        if (score > bestScore)
        {
            bestScore = score;
            bestDiscardIndex = i;
        }
    }

    if (bestDiscardIndex == -1)
    {
        Debug.Log("BOT: Skipping discard-pile take because no valid discard keeps the gained card useful.");
        return false;
    }

    Debug.Log("BOT: Taking discard for " + targetProgress + "-of-4 progress");

    deckManager.BotDrawFromDiscard();

    if (bestDiscardIndex < 0 || bestDiscardIndex >= player.hand.Count)
        return false;

    PranksterDeckEntry discardedCard = player.hand[bestDiscardIndex];
    string discardedCardName = GetBotCardDisplayName(discardedCard);

    deckManager.BotDiscardCardFromHand(bestDiscardIndex);

    actionMessage =
        "Took " + topCard + " from discard " +
        "\nDiscarded: " + discardedCardName;

    return true;
}

    int ChooseDiscardIndexAfterGain(int protectedPrankIndex, int targetProgress)
{
    Player player = GetCurrentPlayer();
    List<PrankCard> activePranks = deckManager.BotGetActivePranks();

    if (player.hand.Count == 0)
        return -1;

    if (protectedPrankIndex < 0 || protectedPrankIndex >= activePranks.Count)
        return ChooseBestDiscardIndexFromCurrentHand();

    PrankCard protectedPrank = activePranks[protectedPrankIndex];
    List<PranksterType> currentHandTypes = ConvertHandToTypes(player.hand);

    int bestIndex = -1;
    int bestScore = int.MinValue;

    for (int i = 0; i < player.hand.Count; i++)
    {
        List<PranksterType> handWithoutCard = new List<PranksterType>(currentHandTypes);
        handWithoutCard.RemoveAt(i);

        int protectedProgressAfterDiscard = CountProgressTowardPrank(handWithoutCard, protectedPrank);

        if (protectedProgressAfterDiscard < targetProgress)
            continue;

        int favorValue = deckManager.CalculateTotalFavorForCard(player.hand[i]);

        bool supportsThree = CardSupportsThreeOfFour(i);
        bool supportsPair = CardSupportsAnyPair(i);

        int score = 0;

        if (!supportsThree && !supportsPair)
            score += 100;

        if (!supportsThree && supportsPair)
            score += 40;

        if (supportsThree)
            score -= 100;

        score += (10 - favorValue);
        score += Random.Range(0, 3);

        if (score > bestScore)
        {
            bestScore = score;
            bestIndex = i;
        }
    }

    return bestIndex;
}

    bool DrawFromDeckAndDiscardBestChoice(out string actionMessage)
{
    actionMessage = "";

    deckManager.BotDrawFromDeck();

    int discardIndex = ChooseBestDiscardIndexFromCurrentHand();

    if (discardIndex < 0)
        return false;

    Player player = GetCurrentPlayer();

    if (discardIndex >= player.hand.Count)
        return false;

    PranksterDeckEntry discardedCard = player.hand[discardIndex];
        string discardedCardName = GetBotCardDisplayName(discardedCard);

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayDrawCardAction();

    Debug.Log("BOT: Discarding card at hand index " + discardIndex);

    deckManager.BotDiscardCardFromHand(discardIndex);

    actionMessage =
        "Drew from deck\n" +
        "Discarded: " + discardedCardName;

    return true;
}

    int ChooseLowestFavorValueHandIndex()
{
    return ChooseBestDiscardIndexFromCurrentHand();
}

    IEnumerator EndTurnAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);

    deckManager.HideBotTurnOverlay();
    deckManager.BotEndPlayerTurn();
}

    bool CardSupportsAnyPair(int handIndex)
{
    Player player = GetCurrentPlayer();
    List<PrankCard> activePranks = deckManager.BotGetActivePranks();

    if (handIndex < 0 || handIndex >= player.hand.Count)
        return false;

    List<PranksterType> currentHandTypes = ConvertHandToTypes(player.hand);

    for (int i = 0; i < activePranks.Count; i++)
    {
        PrankCard prank = activePranks[i];

        int progressWithCard = CountProgressTowardPrank(currentHandTypes, prank);

        List<PranksterType> handWithoutCard = new List<PranksterType>(currentHandTypes);
        handWithoutCard.RemoveAt(handIndex);

        int progressWithoutCard = CountProgressTowardPrank(handWithoutCard, prank);

        bool cardMattersToPairOrBetter =
            progressWithCard >= 2 &&
            progressWithoutCard < progressWithCard;

        if (cardMattersToPairOrBetter)
            return true;
    }

    return false;
}

    bool CardSupportsThreeOfFour(int handIndex)
{
    Player player = GetCurrentPlayer();
    List<PrankCard> activePranks = deckManager.BotGetActivePranks();

    if (handIndex < 0 || handIndex >= player.hand.Count)
        return false;

    List<PranksterType> currentHandTypes = ConvertHandToTypes(player.hand);

    for (int i = 0; i < activePranks.Count; i++)
    {
        PrankCard prank = activePranks[i];

        int progressWithCard = CountProgressTowardPrank(currentHandTypes, prank);

        if (progressWithCard < 3)
            continue;

        List<PranksterType> handWithoutCard = new List<PranksterType>(currentHandTypes);
        handWithoutCard.RemoveAt(handIndex);

        int progressWithoutCard = CountProgressTowardPrank(handWithoutCard, prank);

        if (progressWithoutCard < 3)
            return true;
    }

    return false;
}

    int ChooseBestDiscardIndexFromCurrentHand()
{
    Player player = GetCurrentPlayer();

    if (player.hand.Count == 0)
        return -1;

    int bestIndex = -1;
    int bestScore = int.MinValue;

    for (int i = 0; i < player.hand.Count; i++)
    {
        PranksterDeckEntry card = player.hand[i];
        int favorValue = deckManager.CalculateTotalFavorForCard(card);

        bool supportsThree = CardSupportsThreeOfFour(i);
        bool supportsPair = CardSupportsAnyPair(i);
        bool hasSameTypeBaseAlternative = HasSameTypeBaseAlternative(player, i);

        int score = 0;

        // Protect upgraded cards when a base card of the same prankster type is available.
        if (card.tier > 0 && hasSameTypeBaseAlternative && !IsDiscardRewardCard(card))
        {
            score -= 500;
        }

        // Prefer discarding base cards when an upgraded same-type card exists.
        if (card.tier == 0)
        {
            for (int j = 0; j < player.hand.Count; j++)
            {
                if (j == i)
                    continue;

                PranksterDeckEntry other = player.hand[j];

                if (other.pranksterType == card.pranksterType &&
                    other.tier > 0 &&
                    !IsDiscardRewardCard(other))
                {
                    score += 500;
                    break;
                }
            }
        }

        // Protect favor bonus cards over base cards when possible.
        if (IsFavorRewardCard(card) && hasSameTypeBaseAlternative)
        {
            score -= 400;
        }

        // Strongly prefer discard reward cards only if they are not protecting 3-of-4 progress.
        if (IsDiscardRewardCard(card) && !supportsThree)
        {
            score += GetDiscardRewardScore(card);
        }

        if (!supportsThree && !supportsPair)
            score += 100;

        if (!supportsThree && supportsPair)
            score += 40;

        if (supportsThree)
            score -= 100;

        score += (10 - favorValue);
        score += Random.Range(0, 3);

        if (score > bestScore)
        {
            bestScore = score;
            bestIndex = i;
        }
    }

    return bestIndex;
}

    bool SwapCreatesExactProgress(
    PranksterType candidateCard,
    int handIndexToReplace,
    int targetProgress,
    out int bestPrankIndex)
    {
    bestPrankIndex = -1;

    Player player = GetCurrentPlayer();
    List<PrankCard> activePranks = deckManager.BotGetActivePranks();

    if (handIndexToReplace < 0 || handIndexToReplace >= player.hand.Count)
        return false;

    List<PranksterType> currentHandTypes = ConvertHandToTypes(player.hand);
    int bestRenown = -1;

    for (int i = 0; i < activePranks.Count; i++)
    {
        int progressBefore = CountProgressTowardPrank(currentHandTypes, activePranks[i]);

        List<PranksterType> simulatedHand = new List<PranksterType>(currentHandTypes);
        simulatedHand[handIndexToReplace] = candidateCard;

        int progressAfter = CountProgressTowardPrank(simulatedHand, activePranks[i]);

        Debug.Log(
            "BOT SWAP CHECK: gain " + candidateCard +
            " replace " + GetHandTypeAt(player, handIndexToReplace) +
            " | Prank: " + activePranks[i].title +
            " | Before: " + progressBefore +
            " | After: " + progressAfter
        );

        bool swapActuallyImprovedProgress =
            progressBefore < targetProgress &&
            progressAfter == targetProgress;

        if (swapActuallyImprovedProgress)
        {
            int renown = activePranks[i].renownPoints;

            if (renown > bestRenown)
            {
                bestRenown = renown;
                bestPrankIndex = i;
            }
        }
    }

    return bestPrankIndex != -1;
    }

    int ScoreSwapAwayHandIndex(int handIndex, int protectedPrankIndex, PranksterType gainedCard, int targetProgress)
{
    Player player = GetCurrentPlayer();
    List<PrankCard> activePranks = deckManager.BotGetActivePranks();

    if (handIndex < 0 || handIndex >= player.hand.Count)
        return int.MinValue;

    if (protectedPrankIndex < 0 || protectedPrankIndex >= activePranks.Count)
        return int.MinValue;

    List<PranksterType> simulatedHand = ConvertHandToTypes(player.hand);
    simulatedHand[handIndex] = gainedCard;

    int protectedProgress = CountProgressTowardPrank(simulatedHand, activePranks[protectedPrankIndex]);

    if (protectedProgress != targetProgress)
        return int.MinValue;

    int favorValue = deckManager.CalculateTotalFavorForCard(player.hand[handIndex]);

    bool supportsThree = CardSupportsThreeOfFour(handIndex);
    bool supportsPair = CardSupportsAnyPair(handIndex);

    int score = 0;

    if (!supportsThree && !supportsPair)
        score += 100;

    if (!supportsThree && supportsPair)
        score += 40;

    if (supportsThree)
        score -= 100;

    score += (10 - favorValue);
    score += Random.Range(0, 3);

    return score;
}

   bool FindBestSwapCandidateForExactProgress(
    int targetProgress,
    out int bestOpponentIndex,
    out int bestOpponentFavorIndex,
    out int bestHandIndexToGive,
    out int bestPrankIndex)
{
    bestOpponentIndex = -1;
    bestOpponentFavorIndex = -1;
    bestHandIndexToGive = -1;
    bestPrankIndex = -1;

    Player currentPlayer = GetCurrentPlayer();
    List<Player> players = deckManager.BotGetAllPlayers();
    List<PrankCard> activePranks = deckManager.BotGetActivePranks();

    BotSwapCandidate bestCandidate = null;

    for (int opponentIndex = 0; opponentIndex < players.Count; opponentIndex++)
    {
        if (opponentIndex == deckManager.BotGetCurrentPlayerIndex())
            continue;

        Player opponent = players[opponentIndex];

        for (int favorIndex = 0; favorIndex < opponent.favorArea.Count; favorIndex++)
        {
            PranksterDeckEntry gainedCard = opponent.favorArea[favorIndex];
            int gainedFavorValue = deckManager.CalculateTotalFavorForCard(gainedCard);

            for (int handIndex = 0; handIndex < currentPlayer.hand.Count; handIndex++)
            {
                if (!SwapCreatesExactProgress(gainedCard.pranksterType, handIndex, targetProgress, out int prankIndex))
                    continue;

                int giveAwayScore = ScoreSwapAwayHandIndex(handIndex, prankIndex, gainedCard.pranksterType, targetProgress);

                if (giveAwayScore == int.MinValue)
                    continue;

                BotSwapCandidate candidate = new BotSwapCandidate
                {
                    opponentIndex = opponentIndex,
                    opponentFavorIndex = favorIndex,
                    handIndexToGive = handIndex,
                    prankIndex = prankIndex,
                    gainedCard = gainedCard,
                    gainedCardFavorValue = gainedFavorValue,
                    prankRenown = activePranks[prankIndex].renownPoints,
                    giveAwayScore = giveAwayScore
                };

                if (bestCandidate == null)
                {
                    bestCandidate = candidate;
                    continue;
                }

                if (candidate.gainedCardFavorValue > bestCandidate.gainedCardFavorValue)
                {
                    bestCandidate = candidate;
                    continue;
                }

                if (candidate.gainedCardFavorValue == bestCandidate.gainedCardFavorValue &&
                    candidate.prankRenown > bestCandidate.prankRenown)
                {
                    bestCandidate = candidate;
                    continue;
                }

                if (candidate.gainedCardFavorValue == bestCandidate.gainedCardFavorValue &&
                    candidate.prankRenown == bestCandidate.prankRenown &&
                    candidate.giveAwayScore > bestCandidate.giveAwayScore)
                {
                    bestCandidate = candidate;
                    continue;
                }
            }
        }
    }

    if (bestCandidate == null)
        return false;

    bestOpponentIndex = bestCandidate.opponentIndex;
    bestOpponentFavorIndex = bestCandidate.opponentFavorIndex;
    bestHandIndexToGive = bestCandidate.handIndexToGive;
    bestPrankIndex = bestCandidate.prankIndex;

    return true;
}

bool TrySwapForFavorCardForExactProgress(int targetProgress, out string actionMessage)
{
    actionMessage = "";

    if (!FindBestSwapCandidateForExactProgress(
        targetProgress,
        out int opponentIndex,
        out int opponentFavorIndex,
        out int handIndexToGive,
        out int prankIndex))
    {
        return false;
    }

    List<Player> players = deckManager.BotGetAllPlayers();
    Player currentPlayer = GetCurrentPlayer();

    if (handIndexToGive < 0 || handIndexToGive >= currentPlayer.hand.Count)
        return false;

    PranksterDeckEntry givenCard = currentPlayer.hand[handIndexToGive];
    PranksterDeckEntry gainedCard = players[opponentIndex].favorArea[opponentFavorIndex];

    Debug.Log(
        "BOT: Swapping " + givenCard.pranksterType +
        " (tier " + givenCard.tier + ", category " + givenCard.category + ")" +
        " for " + gainedCard.pranksterType +
        " (tier " + gainedCard.tier + ", category " + gainedCard.category + ")" +
        " to create " + targetProgress + "-of-4 progress"
    );

    bool success = deckManager.BotSwapWithOpponentFavor(opponentIndex, opponentFavorIndex, handIndexToGive);

    if (!success)
        return false;

    actionMessage =
        "Swapped for: " + GetBotCardDisplayName(gainedCard) +
        "\nGave away: " + GetBotCardDisplayName(givenCard);

    return true;
}

bool TryOfferFavor(out string actionMessage)
{
    actionMessage = "";

    Player player = GetCurrentPlayer();

    if (player.favorArea.Count >= 3)
        return false;

    int bestIndex = -1;
    int bestScore = int.MinValue;

    int remainingPranks = GetRemainingPranks();
    int bestProgress = GetBestPrankProgress();
    bool canImprove = CanReachTwoOfFourFromVisibleSources();

    bool lateRoundFarFromGoal =
        remainingPranks == 1 &&
        bestProgress <= 1 &&
        !canImprove;

    bool lateRoundStall =
        remainingPranks == 1 &&
        bestProgress >= 3 &&
        !canImprove;

    for (int i = 0; i < player.hand.Count; i++)
    {
        PranksterDeckEntry card = player.hand[i];
        int favorValue = deckManager.CalculateTotalFavorForCard(card);

        if (favorValue == 0)
            continue;

        bool supportsThree = CardSupportsThreeOfFour(i);
        bool supportsPair = CardSupportsAnyPair(i);

        if (!lateRoundFarFromGoal && !lateRoundStall)
        {
            if (supportsThree || supportsPair)
                continue;
        }
        else if (lateRoundStall)
        {
            if ((supportsThree || supportsPair) && favorValue < 4)
                continue;
        }

        int score = 0;

        // Absolute priority: offer cards designed to give extra favor.
        score += GetFavorRewardScore(card);

        if (!IsFavorRewardCard(card))
        {
            score += favorValue * 10;

            if (favorValue >= 4)
                score += 40;
            else if (favorValue == 3)
                score += 20;
        }

        score += Random.Range(0, 3);

        if (score > bestScore)
        {
            bestScore = score;
            bestIndex = i;
        }
    }

    if (bestIndex == -1)
        return false;

    PranksterDeckEntry bestCard = player.hand[bestIndex];
    int bestFavorValue = deckManager.CalculateTotalFavorForCard(bestCard);

    if (remainingPranks == 1)
    {
        bool allowLateRoundFavor =
            (lateRoundFarFromGoal && bestFavorValue >= 3) ||
            (lateRoundStall && bestFavorValue >= 4);

        if (!allowLateRoundFavor)
        {
            Debug.Log("BOT: Skipping favor (late round prefers progress)");
            return false;
        }

        Debug.Log("BOT: Late round favor allowed");
    }
    else
    {
        float decisionRoll = Random.value;
        float threshold = 0.5f;

        if (bestFavorValue >= 4)
            threshold = 0.8f;
        else if (bestFavorValue == 3)
            threshold = 0.65f;

        if (decisionRoll > threshold)
        {
            Debug.Log("BOT: Skipping offer favor due to randomness");
            return false;
        }
    }

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayFavorClick();

    Debug.Log("BOT: Offering favor " + bestCard.pranksterType +
              " | tier=" + bestCard.tier +
              " | category=" + bestCard.category +
              " | favorValue=" + bestFavorValue);

    Debug.Log("BOT: TryOfferFavor selected hand index " + bestIndex +
              " card " + bestCard.pranksterType +
              " | tier=" + bestCard.tier +
              " | category=" + bestCard.category);

    deckManager.BotOfferFavor(bestIndex);

    actionMessage =
        "Offered as favor:\n" +
        GetBotCardDisplayName(bestCard);

    return true;
}

public void StartBotTurn()
{
    StartCoroutine(BotTurnSequence());
}

IEnumerator BotTurnSequence()
{
    string playerName = turnManager.players[turnManager.currentPlayerIndex].playerName;

    if (string.IsNullOrEmpty(playerName))
        playerName = "Player " + (turnManager.currentPlayerIndex + 1);

    if (nextPlayerPanelController != null)
        nextPlayerPanelController.ShowBotTurnHeader(playerName);

    yield return new WaitForSeconds(0.8f);

    string actionMessage = TakeBotTurnAndReturnMessage();

    if (nextPlayerPanelController != null)
        nextPlayerPanelController.ShowBotMessage(actionMessage);

    yield return new WaitForSeconds(2f);

    if (nextPlayerPanelController != null)
        nextPlayerPanelController.HideBotMessage();

    if (botActionHandledTurnFlow)
    {
        Debug.Log("Bot action already handled turn flow. Skipping BotEndPlayerTurn.");
        yield break;
    }

    if (deckManager != null && deckManager.IsGameOver())
    {
        Debug.Log("Game is over. Skipping BotEndPlayerTurn.");
        yield break;
    }

    deckManager.BotEndPlayerTurn();
}

public void NotifyBotActionHandledTurnFlow()
{
    botActionHandledTurnFlow = true;
}

int GetRemainingPranks()
{
    return deckManager.BotGetActivePranks().Count;
}

int GetBestPrankProgress()
{
    int bestProgress = 0;

    List<PrankCard> pranks = deckManager.BotGetActivePranks();
    Player currentPlayer = GetCurrentPlayer();
    List<PranksterType> currentHandTypes = ConvertHandToTypes(currentPlayer.hand);

    for (int i = 0; i < pranks.Count; i++)
    {
        int progress = CountProgressTowardPrank(currentHandTypes, pranks[i]);

        if (progress > bestProgress)
            bestProgress = progress;
    }

    return bestProgress;
}

bool CanReachTwoOfFourFromVisibleSources()
{
    List<PrankCard> pranks = deckManager.BotGetActivePranks();
    List<PranksterType> discard = deckManager.BotGetDiscardPile();
    List<Player> players = deckManager.BotGetAllPlayers();
    Player currentPlayer = GetCurrentPlayer();
    List<PranksterType> currentHandTypes = ConvertHandToTypes(currentPlayer.hand);

    for (int i = 0; i < pranks.Count; i++)
    {
        int currentProgress = CountProgressTowardPrank(currentHandTypes, pranks[i]);

        if (currentProgress != 1)
            continue;

        if (discard.Count > 0)
        {
            PranksterType topCard = discard[discard.Count - 1];

            List<PranksterType> simulatedHand = new List<PranksterType>(currentHandTypes);
            simulatedHand.Add(topCard);

            int newProgress = CountProgressTowardPrank(simulatedHand, pranks[i]);

            if (newProgress >= 2)
                return true;
        }

        for (int p = 0; p < players.Count; p++)
        {
            if (p == turnManager.currentPlayerIndex)
                continue;

            foreach (PranksterDeckEntry favorCard in players[p].favorArea)
            {
                List<PranksterType> simulatedHand = new List<PranksterType>(currentHandTypes);
                simulatedHand.Add(favorCard.pranksterType);

                int newProgress = CountProgressTowardPrank(simulatedHand, pranks[i]);

                if (newProgress >= 2)
                    return true;
            }
        }
    }

    return false;
}

List<int> GetProtectedHandIndexesForPrank(List<PranksterType> hand, PrankCard prank)
{
    List<int> protectedIndexes = new List<int>();
    List<PranksterType> remainingNeeded = new List<PranksterType>(prank.requiredPranksters);

    for (int i = 0; i < hand.Count; i++)
    {
        if (remainingNeeded.Contains(hand[i]))
        {
            protectedIndexes.Add(i);
            remainingNeeded.Remove(hand[i]);
        }

        if (remainingNeeded.Count == 0)
            break;
    }

    return protectedIndexes;
}

bool IsDiscardRewardCard(PranksterDeckEntry card)
{
    if (card == null)
        return false;

    string categoryName = card.category.ToString();

    return categoryName == "Hustler" ||
           categoryName == "Plotter" ||
           categoryName == "Schemer";
}

bool IsFavorRewardCard(PranksterDeckEntry card)
{
    if (card == null)
        return false;

    string categoryName = card.category.ToString();

    return categoryName == "Assistant" ||
           categoryName == "Strategist" ||
           categoryName == "Advisor";
}

int GetDiscardRewardScore(PranksterDeckEntry card)
{
    if (card == null)
        return 0;

    string categoryName = card.category.ToString();

    if (categoryName == "Hustler")
        return 10000; // +1 renown

    if (categoryName == "Plotter")
        return 20000; // +1 favor, +1 renown

    if (categoryName == "Schemer")
        return 40000; // +2 favor, +2 renown

    return 0;
}

int GetFavorRewardScore(PranksterDeckEntry card)
{
    if (card == null)
        return 0;

    if (!IsFavorRewardCard(card))
        return 0;

    int totalFavorValue = deckManager.CalculateTotalFavorForCard(card);

    return 10000 + (totalFavorValue * 1000);
}

bool HasSameTypeBaseAlternative(Player player, int handIndex)
{
    if (player == null || handIndex < 0 || handIndex >= player.hand.Count)
        return false;

    PranksterDeckEntry card = player.hand[handIndex];

    for (int i = 0; i < player.hand.Count; i++)
    {
        if (i == handIndex)
            continue;

        PranksterDeckEntry other = player.hand[i];

        if (other.pranksterType == card.pranksterType &&
            other.tier == 0)
        {
            return true;
        }
    }

    return false;
}

string GetBotCardDisplayName(PranksterDeckEntry card)
{
    if (card == null)
        return "Unknown";

    string typeName = card.pranksterType.ToString();

    if (typeName == "BeastMaster")
        typeName = "Beastmaster";

    if (card.tier <= 0)
        return typeName;

    string upgradeName = "";

    string categoryName = card.category.ToString();

    if (categoryName == "PrankCompletion")
    {
        upgradeName = PranksterSpriteDatabase.GetTierTitle(card.tier);
    }
    else if (categoryName == "FavorOffer" ||
             categoryName == "Assistant" ||
             categoryName == "Strategist" ||
             categoryName == "Advisor")
    {
        upgradeName = PranksterSpriteDatabase.GetFavorTierTitle(card.tier);
    }
    else if (categoryName == "Discard" ||
             categoryName == "Hustler" ||
             categoryName == "Opportunist" ||
             categoryName == "Manipulator" ||
             categoryName == "Plotter" ||
             categoryName == "Schemer")
    {
        upgradeName = PranksterSpriteDatabase.GetDiscardTierTitle(card.tier);
    }
    else
    {
        upgradeName = PranksterSpriteDatabase.GetTierTitle(card.tier);
    }

    if (string.IsNullOrWhiteSpace(upgradeName) || upgradeName == "Base")
        return typeName;

    return typeName + " " + upgradeName;
}

}