using UnityEngine;

public class OpponentDisplayManager : MonoBehaviour
{
    public TurnManager turnManager;

    [Header("Panels")]
    public PlayerInfoPanel topLeftPanel;
    public PlayerInfoPanel topCenterPanel;
    public PlayerInfoPanel topRightPanel;
    public DeckManager deckManager;

    public void RefreshDisplays()
{
    if (turnManager == null)
    {
        Debug.LogWarning("OpponentDisplayManager: TurnManager not assigned.");
        return;
    }

    if (turnManager.players == null || turnManager.players.Count == 0)
    {
        Debug.LogWarning("OpponentDisplayManager: No players found.");
        return;
    }

    if (topLeftPanel != null) topLeftPanel.SetVisible(false);
    if (topCenterPanel != null) topCenterPanel.SetVisible(false);
    if (topRightPanel != null) topRightPanel.SetVisible(false);

    int barnabyIndex = -1;

    for (int i = 0; i < turnManager.players.Count; i++)
    {
        if (turnManager.players[i].isBot)
        {
            barnabyIndex = i;
            break;
        }
    }

    if (barnabyIndex == -1)
    {
        Debug.LogWarning("OpponentDisplayManager: No bot player found to display as Barnaby.");
        return;
    }

    if (topCenterPanel != null)
    {
        topCenterPanel.RefreshPlayer(barnabyIndex);
    }
}

    public void RefreshSwapHighlights()
{
    if (deckManager == null)
    {
        Debug.LogWarning("OpponentDisplayManager: DeckManager not assigned.");
        return;
    }

    if (topLeftPanel != null)
    {
        bool shouldHighlight = deckManager.ShouldHighlightOpponentPanel(topLeftPanel.representedPlayerIndex);
        topLeftPanel.SetSwapHighlightVisible(shouldHighlight);
    }

    if (topCenterPanel != null)
    {
        bool shouldHighlight = deckManager.ShouldHighlightOpponentPanel(topCenterPanel.representedPlayerIndex);
        topCenterPanel.SetSwapHighlightVisible(shouldHighlight);
    }

    if (topRightPanel != null)
    {
        bool shouldHighlight = deckManager.ShouldHighlightOpponentPanel(topRightPanel.representedPlayerIndex);
        topRightPanel.SetSwapHighlightVisible(shouldHighlight);
    }
}

}