using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject mainGame;
    public GameObject endGameCanvas;
    public GameObject playerCountPanel;

    public void OpenPlaySetup()
    {
        Debug.Log("Play button clicked");

        if (playerCountPanel != null)
            playerCountPanel.SetActive(true);
    }

    public void ClosePlaySetup()
    {
        if (playerCountPanel != null)
            playerCountPanel.SetActive(false);
    }

    public void SetTwoPlayers()
    {
        GameSettings.PlayerCount = 2;
        Debug.Log("Player count set to 2");
    }

    public void SetThreePlayers()
    {
        GameSettings.PlayerCount = 3;
        Debug.Log("Player count set to 3");
    }

    public void SetFourPlayers()
    {
        GameSettings.PlayerCount = 4;
        Debug.Log("Player count set to 4");
    }

    public void StartGame()
{
    if (mainMenuCanvas != null)
        mainMenuCanvas.SetActive(false);

    if (endGameCanvas != null)
        endGameCanvas.SetActive(false);

    if (mainGame != null)
        mainGame.SetActive(true);

    GameObject gameCanvas = GameObject.Find("GameCanvas");
    if (gameCanvas != null)
        gameCanvas.SetActive(true);

    TurnManager turnManager = FindFirstObjectByType<TurnManager>();
    if (turnManager != null)
    {
        turnManager.StartGame(GameSettings.PlayerCount);
        Debug.Log("Starting game with " + GameSettings.PlayerCount + " players.");

        // Player 1 = human, all others = bots
        for (int i = 0; i < turnManager.players.Count; i++)
        {
            turnManager.players[i].isBot = (i != 0);
            Debug.Log("Player " + (i + 1) + " isBot = " + turnManager.players[i].isBot);
        }
    }
    else
    {
        Debug.LogWarning("TurnManager not found.");
        return;
    }

    DeckManager deckManager = FindFirstObjectByType<DeckManager>();
    if (deckManager != null)
    {
        deckManager.BeginNewGame();
    }
    else
    {
        Debug.LogWarning("DeckManager not found.");
    }

    if (playerCountPanel != null)
        playerCountPanel.SetActive(false);
}

    public void ReturnToMainMenu()
    {
        Debug.Log("Returning to main menu");

        if (endGameCanvas != null)
            endGameCanvas.SetActive(false);

        if (mainGame != null)
            mainGame.SetActive(false);

        if (playerCountPanel != null)
            playerCountPanel.SetActive(false);

        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(true);
    }
}