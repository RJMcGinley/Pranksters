using System.Collections;
using UnityEngine;
using TMPro;

public class EndTurnButtonController : MonoBehaviour
{
    public GameObject airReleaseEffect;
    public GameObject nextPlayerPanel;
    public GameObject endTurnButton;
    public GameObject readyButton;
    public TMP_Text turnMessageText;

    public float puffDuration = 0.3f;

    private TurnManager turnManager;
    private DeckManager deckManager;

    void Start()
    {
        turnManager = FindFirstObjectByType<TurnManager>();
        deckManager = FindFirstObjectByType<DeckManager>();
    }

    public void OnEndTurnPressed()
    {
        Debug.Log("END TURN BUTTON CLICKED");
        StartCoroutine(HandleEndTurnPress());
    }

    private IEnumerator HandleEndTurnPress()
{
    if (turnManager == null)
    {
        Debug.LogWarning("TurnManager not found in EndTurnButtonController.");
        yield break;
    }

    if (deckManager == null)
    {
        Debug.LogWarning("DeckManager not found in EndTurnButtonController.");
        yield break;
    }

    if (airReleaseEffect != null)
        airReleaseEffect.SetActive(true);

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayRandomFart();

    yield return new WaitForSeconds(puffDuration);

    if (airReleaseEffect != null)
        airReleaseEffect.SetActive(false);

    int nextPlayerIndex = (turnManager.currentPlayerIndex + 1) % turnManager.players.Count;
    bool nextPlayerIsBot = turnManager.players[nextPlayerIndex].isBot;

    Debug.Log("Next player is Player " + (nextPlayerIndex + 1) + ". isBot = " + nextPlayerIsBot);

    // BOT FLOW
    if (nextPlayerIsBot)
    {
        if (nextPlayerPanel != null)
            nextPlayerPanel.SetActive(false);

        if (readyButton != null)
            readyButton.SetActive(false);

        if (turnMessageText != null)
            turnMessageText.text = "";

        deckManager.AdvanceToNextPlayerTurn();

        if (endTurnButton != null)
            endTurnButton.SetActive(false);

        yield break;
    }

    // HUMAN FLOW
    if (endTurnButton != null)
        endTurnButton.SetActive(false);

    if (turnMessageText != null)
        turnMessageText.text = "Player " + (nextPlayerIndex + 1) + "'s Turn";

    if (readyButton != null)
        readyButton.SetActive(true);

    if (nextPlayerPanel != null)
        nextPlayerPanel.SetActive(true);
}
}