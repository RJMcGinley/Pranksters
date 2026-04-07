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
    public PranksterType gainedCard;
    public int gainedCardFavorValue;
    public int prankRenown;
    public int giveAwayScore;
}

public class BotManager : MonoBehaviour
{
    public DeckManager deckManager;
    public TurnManager turnManager;

    Player GetCurrentPlayer()
    {
        return turnManager.GetCurrentPlayer();
    }

    public void TakeBotTurn()
{
    if (TryCompletePrank())
    {
        // CompletePrank runs its own DeckManager sequence.
        // Do not end the turn here or it conflicts with FinishCompletePrankSequence.
        return;
    }

    if (TryTakeDiscardForExactProgress(4))
    {
        Debug.Log("BOT: Action complete. Ending turn.");
        deckManager.BotRefreshAllDisplays();
        StartCoroutine(EndTurnAfterDelay(1.2f));
        return;
    }

    if (TrySwapForFavorCardForExactProgress(4))
    {
        Debug.Log("BOT: Action complete. Ending turn.");
        deckManager.BotRefreshAllDisplays();
        StartCoroutine(EndTurnAfterDelay(1.2f));
        return;
    }

    if (TryTakeDiscardForExactProgress(3))
    {
        Debug.Log("BOT: Action complete. Ending turn.");
        deckManager.BotRefreshAllDisplays();
        StartCoroutine(EndTurnAfterDelay(1.2f));
        return;
    }

    if (TrySwapForFavorCardForExactProgress(3))
    {
        Debug.Log("BOT: Action complete. Ending turn.");
        deckManager.BotRefreshAllDisplays();
        StartCoroutine(EndTurnAfterDelay(1.2f));
        return;
    }

    if (!HandContainsFavorValue(0))
    {
        if (TryOfferFavor())
        {
            Debug.Log("BOT: Offered favor.");
            deckManager.BotRefreshAllDisplays();
            StartCoroutine(EndTurnAfterDelay(1.2f));
            return;
        }
    }

    Debug.Log("BOT: No strong visible action. Drawing from deck.");
    DrawFromDeckAndDiscardBestChoice();

    Debug.Log("BOT: Action complete. Ending turn.");
    deckManager.BotRefreshAllDisplays();
    StartCoroutine(EndTurnAfterDelay(1.2f));
}

    bool HandContainsFavorValue(int value)
    {
        Player player = GetCurrentPlayer();

        foreach (PranksterType card in player.hand)
        {
            int favorValue = deckManager.BotCalculateFavorPoints(card);

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

    deckManager.ShowBotActionMessage("Completed:\n" + prankName);

    Debug.Log("BOT: Completing prank at index " + prankIndex);
    deckManager.BotCompletePrank(prankIndex);

    return true;
}

    bool CardCreatesExactProgress(PranksterType candidateCard, int targetProgress, out int bestPrankIndex)
{
    bestPrankIndex = -1;

    Player player = GetCurrentPlayer();
    List<PrankCard> activePranks = deckManager.BotGetActivePranks();

    for (int i = 0; i < activePranks.Count; i++)
    {
        int progressBefore = CountProgressTowardPrank(player.hand, activePranks[i]);

        List<PranksterType> simulatedHand = new List<PranksterType>(player.hand);
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

bool TryTakeDiscardForExactProgress(int targetProgress)
{
    List<PranksterType> discardPile = deckManager.BotGetDiscardPile();

    if (discardPile.Count == 0)
        return false;

    PranksterType topCard = discardPile[discardPile.Count - 1];

    if (!CardCreatesExactProgress(topCard, targetProgress, out int bestPrankIndex))
        return false;

    deckManager.ShowBotActionMessage("Bot took from discard:\n" + topCard);

    Debug.Log("BOT: Taking discard for " + targetProgress + "-of-4 progress");

    deckManager.BotDrawFromDiscard();

    int discardIndex = ChooseDiscardIndexAfterGain(bestPrankIndex);
    deckManager.BotDiscardCardFromHand(discardIndex);

    return true;
}

int ChooseDiscardIndexAfterGain(int protectedPrankIndex)
{
    Player player = GetCurrentPlayer();
    List<PrankCard> activePranks = deckManager.BotGetActivePranks();

    if (protectedPrankIndex < 0 || protectedPrankIndex >= activePranks.Count)
        return ChooseBestDiscardIndexFromCurrentHand();

    PrankCard protectedPrank = activePranks[protectedPrankIndex];

    int bestIndex = -1;
    int bestScore = int.MinValue;

    for (int i = 0; i < player.hand.Count; i++)
    {
        List<PranksterType> handWithoutCard = new List<PranksterType>(player.hand);
        handWithoutCard.RemoveAt(i);

        int protectedProgressAfterDiscard = CountProgressTowardPrank(handWithoutCard, protectedPrank);
        int favorValue = deckManager.BotCalculateFavorPoints(player.hand[i]);

        bool supportsThree = CardSupportsThreeOfFour(i);
        bool supportsPair = CardSupportsAnyPair(i);

        int score = 0;

        // Strongly protect the prank we just improved toward
        if (protectedProgressAfterDiscard < 3)
            score -= 200;

        // General protection rules
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

void DrawFromDeckAndDiscardBestChoice()
{
    deckManager.BotDrawFromDeck();

    int discardIndex = ChooseBestDiscardIndexFromCurrentHand();

    if (discardIndex >= 0)
    {
        Player player = GetCurrentPlayer();
        PranksterType discardedCard = player.hand[discardIndex];

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDrawCardAction();

        deckManager.ShowBotActionMessage("Bot drew a card\nDiscarded " + discardedCard);

        Debug.Log("BOT: Discarding card at hand index " + discardIndex);
        deckManager.BotDiscardCardFromHand(discardIndex);
    }
}

int ChooseLowestFavorValueHandIndex()
{
    return ChooseBestDiscardIndexFromCurrentHand();
}

IEnumerator EndTurnAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);
    deckManager.BotEndPlayerTurn();
}

bool CardSupportsAnyPair(int handIndex)
{
    Player player = GetCurrentPlayer();
    List<PrankCard> activePranks = deckManager.BotGetActivePranks();

    if (handIndex < 0 || handIndex >= player.hand.Count)
        return false;

    PranksterType card = player.hand[handIndex];

    for (int i = 0; i < activePranks.Count; i++)
    {
        PrankCard prank = activePranks[i];

        int progressWithCard = CountProgressTowardPrank(player.hand, prank);

        List<PranksterType> handWithoutCard = new List<PranksterType>(player.hand);
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

    for (int i = 0; i < activePranks.Count; i++)
    {
        PrankCard prank = activePranks[i];

        int progressWithCard = CountProgressTowardPrank(player.hand, prank);

        if (progressWithCard < 3)
            continue;

        List<PranksterType> handWithoutCard = new List<PranksterType>(player.hand);
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
        PranksterType card = player.hand[i];
        int favorValue = deckManager.BotCalculateFavorPoints(card);

        bool supportsThree = CardSupportsThreeOfFour(i);
        bool supportsPair = CardSupportsAnyPair(i);

        int score = 0;

        // Best discard candidates get HIGHER score
        if (!supportsThree && !supportsPair)
            score += 100;   // dead card

        if (!supportsThree && supportsPair)
            score += 40;    // pair-only support, somewhat expendable

        if (supportsThree)
            score -= 100;   // strongly protect 3-of-4 cards

        // Prefer discarding lower favor value
        score += (10 - favorValue);

        // Small random tie breaker
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

    int bestRenown = -1;

    for (int i = 0; i < activePranks.Count; i++)
    {
        int progressBefore = CountProgressTowardPrank(player.hand, activePranks[i]);

        List<PranksterType> simulatedHand = new List<PranksterType>(player.hand);
        simulatedHand[handIndexToReplace] = candidateCard;

        int progressAfter = CountProgressTowardPrank(simulatedHand, activePranks[i]);

        Debug.Log(
            "BOT SWAP CHECK: gain " + candidateCard +
            " replace " + player.hand[handIndexToReplace] +
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

    List<PranksterType> simulatedHand = new List<PranksterType>(player.hand);
    simulatedHand[handIndex] = gainedCard;

    int protectedProgress = CountProgressTowardPrank(simulatedHand, activePranks[protectedPrankIndex]);

    if (protectedProgress != targetProgress)
        return int.MinValue;

    int favorValue = deckManager.BotCalculateFavorPoints(player.hand[handIndex]);
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
            PranksterType gainedCard = opponent.favorArea[favorIndex];
            int gainedFavorValue = deckManager.BotCalculateFavorPoints(gainedCard);

            for (int handIndex = 0; handIndex < currentPlayer.hand.Count; handIndex++)
            {
                if (!SwapCreatesExactProgress(gainedCard, handIndex, targetProgress, out int prankIndex))
                    continue;

                int giveAwayScore = ScoreSwapAwayHandIndex(handIndex, prankIndex, gainedCard, targetProgress);

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

bool TrySwapForFavorCardForExactProgress(int targetProgress)
{
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
    PranksterType gainedCard = players[opponentIndex].favorArea[opponentFavorIndex];

    deckManager.ShowBotActionMessage("Bot swapped for " + gainedCard);

    Debug.Log(
        "BOT: Swapping for " + gainedCard +
        " to create " + targetProgress + "-of-4 progress"
    );

    bool success = deckManager.BotSwapWithOpponentFavor(opponentIndex, opponentFavorIndex, handIndexToGive);

    return success;
}

bool TryOfferFavor()
{
    Player player = GetCurrentPlayer();

    int bestIndex = -1;
    int bestScore = int.MinValue;

    for (int i = 0; i < player.hand.Count; i++)
    {
        PranksterType card = player.hand[i];
        int favorValue = deckManager.BotCalculateFavorPoints(card);

        // Never offer 0-value cards (safety rule)
        if (favorValue == 0)
            continue;

        bool supportsThree = CardSupportsThreeOfFour(i);
        bool supportsPair = CardSupportsAnyPair(i);

        // Never give away important structure
        if (supportsThree || supportsPair)
            continue;

        int score = 0;

        // Prefer giving away lower-value cards
        score += (10 - favorValue);

        // Small randomness to avoid deterministic play
        score += Random.Range(0, 3);

        if (score > bestScore)
        {
            bestScore = score;
            bestIndex = i;
        }
    }

    if (bestIndex == -1)
        return false;

    PranksterType selectedCard = player.hand[bestIndex];

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayFavorClick();

    deckManager.ShowBotActionMessage("Bot offered as favor:\n" + selectedCard);

    Debug.Log("BOT: Offering favor " + selectedCard);

    Debug.Log("BOT: TryOfferFavor selected hand index " + bestIndex + " card " + selectedCard);
    deckManager.BotOfferFavor(bestIndex);

    return true;
}

}

