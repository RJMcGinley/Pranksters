using UnityEngine;

public class SettingsMenuController : MonoBehaviour
{
    public GameObject settingsPanel;
    public DeckManager deckManager;

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        if (deckManager != null)
            deckManager.RefreshAllHighlights();
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (deckManager != null)
            deckManager.RefreshAllHighlights();
    }

    public bool IsPanelBlockingInteraction()
    {
        return settingsPanel != null && settingsPanel.activeSelf;
    }
}