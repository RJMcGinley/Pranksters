using System.Collections;
using UnityEngine;
using TMPro;

public class EndTurnButtonController : MonoBehaviour
{
    public GameObject airReleaseEffect;
    public GameObject nextPlayerPanel;
    public GameObject endTurnButton;
    public TMP_Text turnMessageText;

    public float puffDuration = 0.3f;

    private TurnManager turnManager;

    void Start()
    {
        turnManager = FindFirstObjectByType<TurnManager>();
    }

    public void OnEndTurnPressed()
    {
        Debug.Log("END TURN BUTTON CLICKED");
        StartCoroutine(HandleEndTurnPress());
    }

    private IEnumerator HandleEndTurnPress()
    {
        if (airReleaseEffect != null)
            airReleaseEffect.SetActive(true);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayRandomFart();

        yield return new WaitForSeconds(puffDuration);

        if (airReleaseEffect != null)
            airReleaseEffect.SetActive(false);

        if (endTurnButton != null)
            endTurnButton.SetActive(false);

        if (turnMessageText != null && turnManager != null && turnManager.players != null && turnManager.players.Count > 0)
        {
            int nextPlayerIndex = (turnManager.currentPlayerIndex + 1) % turnManager.players.Count;
            int playerNumber = nextPlayerIndex + 1;
            turnMessageText.text = "Player " + playerNumber + "'s Turn";
        }

        if (nextPlayerPanel != null)
            nextPlayerPanel.SetActive(true);
    }
}