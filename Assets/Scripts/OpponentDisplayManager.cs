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

        int playerCount = turnManager.players.Count;
        int current = turnManager.currentPlayerIndex;

        if (topLeftPanel != null) topLeftPanel.SetVisible(false);
        if (topCenterPanel != null) topCenterPanel.SetVisible(false);
        if (topRightPanel != null) topRightPanel.SetVisible(false);

        if (playerCount == 2)
        {
            // Only top center = the other player
            int opponent = (current + 1) % playerCount;

            if (topCenterPanel != null)
                topCenterPanel.RefreshPlayer(opponent);
        }
        else if (playerCount == 3)
        {
            // Top left = next player, Top right = following player
            int leftOpponent = (current + 1) % playerCount;
            int rightOpponent = (current + 2) % playerCount;

            if (topLeftPanel != null)
                topLeftPanel.RefreshPlayer(leftOpponent);

            if (topRightPanel != null)
                topRightPanel.RefreshPlayer(rightOpponent);
        }
        else if (playerCount >= 4)
        {
            // Top left = next, Top center = after that, Top right = after that
            int leftOpponent = (current + 1) % playerCount;
            int centerOpponent = (current + 2) % playerCount;
            int rightOpponent = (current + 3) % playerCount;

            if (topLeftPanel != null)
                topLeftPanel.RefreshPlayer(leftOpponent);

            if (topCenterPanel != null)
                topCenterPanel.RefreshPlayer(centerOpponent);

            if (topRightPanel != null)
                topRightPanel.RefreshPlayer(rightOpponent);
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