using TMPro;
using UnityEngine;
using System.Collections;

public class NextPlayerPanelController : MonoBehaviour
{
    public GameObject nextPlayerPanel;
    public GameObject endTurnButton;
    public TextMeshProUGUI turnMessageText;
    public TextMeshProUGUI botActionText;
    public DeckManager deckManager;
    public GameObject readyButton;
    public float botMessageDelay = 3f;
    [SerializeField] private SpendInfluenceButton_UseInactiveServices inactiveServiceButton;
    public TextMeshProUGUI crewSizeText;

    
    public void ShowNextPlayerPanel(string playerName)
{
    Debug.Log("SHOW NEXT PLAYER PANEL");

    if (turnMessageText != null)
    {
        turnMessageText.text = playerName + "'s Turn";
        turnMessageText.gameObject.SetActive(true);
    }

    if (nextPlayerPanel != null)
        nextPlayerPanel.SetActive(true);

    UpdateCrewSizeText();

    if (readyButton != null)
        readyButton.SetActive(true);

    if (endTurnButton != null)
        endTurnButton.SetActive(false);
}

    public void HideNextPlayerPanel()
{
    Debug.Log("HIDE NEXT PLAYER PANEL");

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayReadyButton();

    if (crewSizeText != null)
        crewSizeText.gameObject.SetActive(false);

    if (nextPlayerPanel != null)
        nextPlayerPanel.SetActive(false);
}

    public void ShowBotMessage(string message)
{
    if (deckManager != null && deckManager.IsEndOfRoundPendingOrPanelOpen())
    {
        Debug.Log("ShowBotMessage blocked because end-of-round is pending or open.");
        return;
    }

    if (nextPlayerPanel != null)
        nextPlayerPanel.SetActive(true);

    if (turnMessageText != null)
        turnMessageText.gameObject.SetActive(true);

    if (botActionText != null)
    {
        botActionText.text = message;
        botActionText.gameObject.SetActive(true);
    }

    if (readyButton != null)
        readyButton.SetActive(false);

    if (endTurnButton != null)
        endTurnButton.SetActive(false);
}

    public void HideBotMessage()
{
    if (nextPlayerPanel != null)
        nextPlayerPanel.SetActive(false);

    if (turnMessageText != null)
    {
        turnMessageText.text = "";
        turnMessageText.gameObject.SetActive(false);
    }

    if (botActionText != null)
    {
        botActionText.text = "";
        botActionText.gameObject.SetActive(false);
    }

    if (readyButton != null)
        readyButton.SetActive(true);

    if (crewSizeText != null)
        crewSizeText.gameObject.SetActive(false);
}

    

    public void OnEndTurnPressed()
    {
        Debug.Log("END TURN BUTTON CLICKED");
        StartCoroutine(HandleEndTurnPress());
    }

    IEnumerator HandleEndTurnPress()
    {
        if (deckManager == null || deckManager.turnManager == null)
        {
            Debug.LogWarning("DeckManager or TurnManager missing in NextPlayerPanelController.");
            yield break;
        }

        int currentPlayerIndex = deckManager.turnManager.currentPlayerIndex;
        int nextPlayerIndex = (currentPlayerIndex + 1) % deckManager.turnManager.players.Count;

        bool nextPlayerIsBot = deckManager.turnManager.players[nextPlayerIndex].isBot;

        // If the next player is a bot, skip the old ready-panel flow entirely.
        if (nextPlayerIsBot)
        {
            Debug.Log("Next player is a bot. Skipping I'm Ready panel.");

            if (nextPlayerPanel != null)
                nextPlayerPanel.SetActive(false);

            if (readyButton != null)
                readyButton.SetActive(false);

            if (endTurnButton != null)
                endTurnButton.SetActive(false);

            yield return null;
            deckManager.AdvanceToNextPlayerTurn();
            yield break;
        }

        // Human pass-and-play flow
        string nextPlayerName = deckManager.turnManager.players[nextPlayerIndex].playerName;

        if (string.IsNullOrWhiteSpace(nextPlayerName))
            nextPlayerName = "Player " + (nextPlayerIndex + 1);

        ShowNextPlayerPanel(nextPlayerName);    
    }

    public void HidePanelImmediate()
    {
        if (nextPlayerPanel != null)
            nextPlayerPanel.SetActive(false);

        if (turnMessageText != null)
        {
            turnMessageText.text = "";
            turnMessageText.gameObject.SetActive(false);
        }

        if (botActionText != null)
        {
            botActionText.text = "";
            botActionText.gameObject.SetActive(false);
        }

        if (readyButton != null)
            readyButton.SetActive(true);

        if (crewSizeText != null)
            crewSizeText.gameObject.SetActive(false);
    }

    public void ShowBotTurnHeader(string playerName)
{
    if (nextPlayerPanel != null)
        nextPlayerPanel.SetActive(true);

    if (turnMessageText != null)
    {
        turnMessageText.text = playerName + "'s Turn";
        turnMessageText.gameObject.SetActive(true);
    }

    if (botActionText != null)
        botActionText.gameObject.SetActive(false);

    if (readyButton != null)
        readyButton.SetActive(false);

    if (endTurnButton != null)
        endTurnButton.SetActive(false);
}

public bool IsPanelBlockingInteraction()
{
    return nextPlayerPanel != null && nextPlayerPanel.activeSelf;
}

private void RestoreGameplayButtonsAfterPanel()
{
    if (inactiveServiceButton != null)
        inactiveServiceButton.Refresh();
}

void UpdateCrewSizeText()
{
    if (crewSizeText == null)
        return;

    if (deckManager == null || deckManager.turnManager == null)
        return;

    if (deckManager.turnManager.players.Count < 2)
        return;

    Player barnaby = deckManager.turnManager.players[1];

    crewSizeText.text =
        "Crew: " +
        barnaby.hand.Count +
        "/" +
        barnaby.maxHandSize;

    crewSizeText.gameObject.SetActive(true);
}

}