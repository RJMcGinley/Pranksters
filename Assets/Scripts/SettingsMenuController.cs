using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    public GameObject settingsPanel;
    public DeckManager deckManager;
    public BotManager botManager;

    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public WhoopieToggleButton whoopieToggleButton;
    public GameObject handDisplayObject;
    public WantedJailController wantedJailController;

    public void OpenSettings()
    {
        if (deckManager != null && deckManager.IsWantedBoardOpen())
            return;

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayUIClick();
        }

        if (handDisplayObject != null)
            handDisplayObject.SetActive(false);

        if (wantedJailController != null)
            wantedJailController.HideJailDisplay();

        if (deckManager != null)
            deckManager.RefreshAllHighlights();
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayUIClick();
        }

        if (handDisplayObject != null)
            handDisplayObject.SetActive(true);

        if (wantedJailController != null)
            wantedJailController.ShowJailDisplay();

        if (deckManager != null)
            deckManager.RefreshAllHighlights();
    }

    public bool IsPanelBlockingInteraction()
    {
        return settingsPanel != null && settingsPanel.activeSelf;
    }

    public void QuitToMainMenuCleanup()
    {
        if (botManager != null)
            botManager.StopBotRuntime();

        if (deckManager != null)
            deckManager.HardResetRuntimeStateForMainMenu();

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    void Start()
    {
        LoadPlayerSettings();
    }

    public void LoadPlayerSettings()
    {
        PlayerProgressSave data = SaveSystem.Load();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(data.musicVolume);
            AudioManager.Instance.SetSFXVolume(data.sfxVolume);
            AudioManager.Instance.SetFartSoundsEnabled(data.fartSoundsEnabled);
        }

        if (musicVolumeSlider != null)
            musicVolumeSlider.value = data.musicVolume;

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = data.sfxVolume;

        if (whoopieToggleButton != null && whoopieToggleButton.checkmarkObject != null)
            whoopieToggleButton.checkmarkObject.SetActive(data.fartSoundsEnabled);
    }

    public void SaveMusicVolume(float value)
    {
        PlayerProgressSave data = SaveSystem.Load();
        data.musicVolume = value;
        SaveSystem.Save(data);
    }

    public void SaveSFXVolume(float value)
    {
        PlayerProgressSave data = SaveSystem.Load();
        data.sfxVolume = value;
        SaveSystem.Save(data);
    }

    public void SaveWhoopieSetting()
    {
        if (AudioManager.Instance == null)
            return;

        PlayerProgressSave data = SaveSystem.Load();
        data.fartSoundsEnabled = AudioManager.Instance.fartSoundsEnabled;
        SaveSystem.Save(data);
    }


}