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
    public float botMessageDelay = 1.2f;


    
    public void ShowNextPlayerPanel(string playerName)
{
    Debug.Log("SHOW NEXT PLAYER PANEL");

    if (turnMessageText != null)
    {
        turnMessageText.text = playerName + "'s Turn";
        turnMessageText.gameObject.SetActive(true);
    }

    if (botActionText != null)
        botActionText.gameObject.SetActive(false);

    if (nextPlayerPanel != null)
        nextPlayerPanel.SetActive(true);

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

        if (nextPlayerPanel != null)
            nextPlayerPanel.SetActive(false);

        if (deckManager != null)
            deckManager.AdvanceToNextPlayerTurn();
    }

    public void ShowBotMessage(string message)
{
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
        turnMessageText.gameObject.SetActive(false);

    if (botActionText != null)
        botActionText.gameObject.SetActive(false);
}

    public IEnumerator ShowBotMessageThenHide(string message, float delay = -1f)
    {
        ShowBotMessage(message);

        float actualDelay = delay > 0f ? delay : botMessageDelay;
        yield return new WaitForSeconds(actualDelay);

        HideBotMessage();
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
        ShowNextPlayerPanel("Player " + (nextPlayerIndex + 1));
    }

    public void HidePanelImmediate()
    {
        if (nextPlayerPanel != null)
            nextPlayerPanel.SetActive(false);

        if (turnMessageText != null)
            turnMessageText.text = "";

        if (readyButton != null)
            readyButton.SetActive(true);
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
}