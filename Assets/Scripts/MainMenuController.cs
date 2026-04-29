using UnityEngine;
using System.Collections.Generic;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject mainGame;
    public GameObject endGameCanvas;
    public GameObject gameCanvas;
    public GameObject playerCountPanel;
    public MainMenuPlayerSetup playerSetup;
    public UnlockRevealPanelController unlockRevealPanelController;

    [Header("Prank Collection")]
    public GameObject prankCollectionScreen;
    public PrankCollectionUI prankCollectionUI;
    public GameObject mainMenuButtonsRoot;

    [Header("Statistics")]
    [SerializeField] private GameObject statisticsScreen;

    [Header("Shop")]
    [SerializeField] private GameObject shopScreen;



    public void OpenPlaySetup()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuClick();

        Debug.Log("Play button clicked");

        if (prankCollectionScreen != null)
            prankCollectionScreen.SetActive(false);

        if (mainMenuButtonsRoot != null)
            mainMenuButtonsRoot.SetActive(true);

        if (playerCountPanel != null)
            playerCountPanel.SetActive(true);
    }

    public void ClosePlaySetup()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBackClick();

        if (playerCountPanel != null)
            playerCountPanel.SetActive(false);
    }

    public void OpenPrankCollection()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuClick();

        Debug.Log("Opening prank collection");

        if (playerCountPanel != null)
            playerCountPanel.SetActive(false);

        if (mainMenuButtonsRoot != null)
            mainMenuButtonsRoot.SetActive(false);

        if (prankCollectionScreen != null)
            prankCollectionScreen.SetActive(true);

        if (prankCollectionUI != null)
            prankCollectionUI.BuildCollection();
        else
            Debug.LogWarning("MainMenuController: prankCollectionUI is not assigned.");
    }

    public void ClosePrankCollection()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBackClick();

        Debug.Log("Closing prank collection");

        if (prankCollectionScreen != null)
            prankCollectionScreen.SetActive(false);

        if (mainMenuButtonsRoot != null)
            mainMenuButtonsRoot.SetActive(true);
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
        Debug.Log("StartGame START");

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayConfirmClick();

        if (playerSetup == null)
        {
            Debug.LogWarning("MainMenuController: playerSetup is not assigned.");
            return;
        }

        int activePlayerCount = playerSetup.GetActivePlayerCount();
        GameSettings.PlayerCount = activePlayerCount;

        if (prankCollectionScreen != null)
            prankCollectionScreen.SetActive(false);

        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(false);

        if (endGameCanvas != null)
            endGameCanvas.SetActive(false);

        if (mainGame != null)
            mainGame.SetActive(true);

        if (gameCanvas != null)
            gameCanvas.SetActive(true);

        TurnManager turnManager = FindFirstObjectByType<TurnManager>();
        if (turnManager != null)
        {
            turnManager.StartGame(activePlayerCount);
            Debug.Log("Starting game with " + activePlayerCount + " players.");

            int activeSlotIndex = 0;

            for (int i = 0; i < playerSetup.playerSlots.Count; i++)
            {
                if (playerSetup.playerSlots[i].playerType == MenuPlayerType.Closed)
                    continue;

                if (activeSlotIndex >= turnManager.players.Count)
                {
                    Debug.LogWarning("More active menu slots than runtime players.");
                    break;
                }

                bool isBot = playerSetup.playerSlots[i].playerType == MenuPlayerType.AI;
                turnManager.players[activeSlotIndex].isBot = isBot;
                turnManager.players[activeSlotIndex].playerName = playerSetup.playerSlots[i].playerName;

                Debug.Log(
                    "Runtime Player " + (activeSlotIndex + 1) +
                    " mapped from Menu Slot " + (i + 1) +
                    " | Name = " + playerSetup.playerSlots[i].playerName +
                    " | Type = " + playerSetup.playerSlots[i].playerType +
                    " | isBot = " + isBot
                );

                activeSlotIndex++;
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
            Debug.Log("StartGame | about to call DeckManager.BeginNewGame");
            deckManager.BeginNewGame();
            Debug.Log("StartGame | DeckManager.BeginNewGame was called");
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
    Debug.Log("ReturnToMainMenu START");

    SettingsMenuController settings = FindFirstObjectByType<SettingsMenuController>();
    if (settings != null)
        settings.CloseSettings();

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayBackClick();

    Debug.Log("Returning to main menu");

    if (gameCanvas != null)
    {
        gameCanvas.SetActive(false);
        Debug.Log("gameCanvas was set to false");
    }

    if (endGameCanvas != null)
    {
        endGameCanvas.SetActive(false);
        Debug.Log("endGameCanvas was set to false");
    }

    if (mainGame != null)
    {
        mainGame.SetActive(false);
        Debug.Log("mainGame was set to false");
    }

    if (playerCountPanel != null)
        playerCountPanel.SetActive(false);

    if (prankCollectionScreen != null)
        prankCollectionScreen.SetActive(false);

    if (statisticsScreen != null)
        statisticsScreen.SetActive(false);

    if (shopScreen != null)
        shopScreen.SetActive(false);

    if (mainMenuButtonsRoot != null)
    {
        mainMenuButtonsRoot.SetActive(true);
        Debug.Log("mainMenuButtonsRoot was set to true");
    }

    if (mainMenuCanvas != null)
    {
        mainMenuCanvas.SetActive(true);
        Debug.Log("mainMenuCanvas was set to true");
    }


    Debug.Log("ReturnToMainMenu END");
}

    public void QuitGame()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayConfirmClick();

        Debug.Log("Quit Game button clicked.");
        Application.Quit();
    }

    public void OpenStatistics()
{
    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayMenuClick();

    Debug.Log("Opening statistics");

    if (playerCountPanel != null)
        playerCountPanel.SetActive(false);

    if (prankCollectionScreen != null)
        prankCollectionScreen.SetActive(false);

    if (mainMenuButtonsRoot != null)
        mainMenuButtonsRoot.SetActive(false);

    if (statisticsScreen != null)
        statisticsScreen.SetActive(true);
    else
        Debug.LogWarning("MainMenuController: statisticsScreen is not assigned.");
}

public void CloseStatistics()
{
    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayBackClick();

    Debug.Log("Closing statistics");

    if (statisticsScreen != null)
        statisticsScreen.SetActive(false);

    if (mainMenuButtonsRoot != null)
        mainMenuButtonsRoot.SetActive(true);
}

public void ReturnToMainMenuFromEndGame()
{
    List<PranksterUnlockEntry> newUnlocks = SaveSystem.GetSessionNewUnlocks();

    if (newUnlocks != null && newUnlocks.Count > 0)
    {
        if (unlockRevealPanelController == null)
        {
            Debug.LogWarning("unlockRevealPanelController is not assigned. Returning to main menu without unlock reveal.");
            ReturnToMainMenu();
            return;
        }

        if (endGameCanvas != null)
            endGameCanvas.SetActive(false);

        unlockRevealPanelController.ShowUnlocks(newUnlocks, OnUnlockSequenceFinished);
        return;
    }

    ReturnToMainMenu();
}

private void OnUnlockSequenceFinished()
{
    Debug.Log("OnUnlockSequenceFinished START");
    SaveSystem.ClearSessionNewUnlocks();
    Debug.Log("OnUnlockSequenceFinished | session unlocks cleared");
    ReturnToMainMenu();
    Debug.Log("OnUnlockSequenceFinished END");
}

private void ShowUnlockScreen(List<PranksterUnlockEntry> unlocks)
{
    Debug.Log("SHOW UNLOCK SCREEN");

    for (int i = 0; i < unlocks.Count; i++)
    {
        Debug.Log("Unlocked: " + unlocks[i].pranksterType + " tier " + unlocks[i].tier);
    }

    // TEMP: just go back to menu for now
    ReturnToMainMenu();
}

public void OpenShop()
{
    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayMenuClick();

    Debug.Log("Opening shop");

    if (playerCountPanel != null)
        playerCountPanel.SetActive(false);

    if (prankCollectionScreen != null)
        prankCollectionScreen.SetActive(false);

    if (statisticsScreen != null)
        statisticsScreen.SetActive(false);

    if (mainMenuButtonsRoot != null)
        mainMenuButtonsRoot.SetActive(false);

    if (shopScreen != null)
        shopScreen.SetActive(true);
    else
        Debug.LogWarning("MainMenuController: shopScreen is not assigned.");
}

public void CloseShop()
{
    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayBackClick();

    Debug.Log("Closing shop");

    if (shopScreen != null)
        shopScreen.SetActive(false);

    if (mainMenuButtonsRoot != null)
        mainMenuButtonsRoot.SetActive(true);
}

}