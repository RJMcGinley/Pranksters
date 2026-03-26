using TMPro;
using UnityEngine;

public class NextPlayerPanelController : MonoBehaviour
{
    public GameObject nextPlayerPanel;
    public GameObject endTurnButton;
    public TextMeshProUGUI turnMessageText;
    public DeckManager deckManager;

    public void ShowNextPlayerPanel(string playerName)
    {
        Debug.Log("SHOW NEXT PLAYER PANEL");

        if (turnMessageText != null)
            turnMessageText.text = playerName + "'s Turn";

        if (nextPlayerPanel != null)
            nextPlayerPanel.SetActive(true);

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
}