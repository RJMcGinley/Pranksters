using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInfoPanel : MonoBehaviour
{
    public GameObject swapAvailableHighlight;

    [Header("Managers")]
    public TurnManager turnManager;
    public DeckManager deckManager;

    [Header("Panel Root")]
    public GameObject rootObject;

    [Header("Text")]
    public TextMeshProUGUI playerLabelText;
    public TextMeshProUGUI pranksCompletedText;
    public TextMeshProUGUI renownPointsText;
    public TextMeshProUGUI favorPointsText;

    [Header("Favor Slot Images")]
    public Image favorSlot1Image;
    public Image favorSlot2Image;
    public Image favorSlot3Image;

    [Header("Prankster Icon Sprites")]
    public Sprite thiefIcon;
    public Sprite engineerIcon;
    public Sprite laborerIcon;
    public Sprite scribeIcon;
    public Sprite wizardIcon;
    public Sprite beastmasterIcon;

    // ✅ NEW: stores which player this panel represents
    public int representedPlayerIndex = -1;

    public void SetVisible(bool isVisible)
    {
        if (rootObject != null)
            rootObject.SetActive(isVisible);
        else
            gameObject.SetActive(isVisible);
    }

    public void RefreshPlayer(int playerIndex)
    {
        if (turnManager == null)
        {
            Debug.LogWarning(name + ": TurnManager is not assigned.");
            SetVisible(false);
            return;
        }

        if (playerIndex < 0 || playerIndex >= turnManager.players.Count)
        {
            Debug.LogWarning(name + ": Invalid player index " + playerIndex);
            SetVisible(false);
            return;
        }

        Debug.Log("RefreshPlayer called for player index: " + playerIndex);

        // ✅ NEW: store the player index for later use (swap logic, preview, etc.)
        representedPlayerIndex = playerIndex;

        Player player = turnManager.players[playerIndex];

        SetVisible(true);

        if (playerLabelText != null)
        {
            string displayName = player.playerName;

            if (string.IsNullOrEmpty(displayName))
                displayName = "Player " + (playerIndex + 1);

            playerLabelText.text = displayName;
        }

        if (pranksCompletedText != null)
            pranksCompletedText.text = player.completedPranks.Count.ToString();

        if (renownPointsText != null)
            renownPointsText.text = player.renownPoints.ToString();

        if (favorPointsText != null)
            favorPointsText.text = player.favorPoints.ToString();

        UpdateFavorSlot(favorSlot1Image, player, 0);
        UpdateFavorSlot(favorSlot2Image, player, 1);
        UpdateFavorSlot(favorSlot3Image, player, 2);

        Debug.Log("swapAvailableHighlight is null? " + (swapAvailableHighlight == null));
        Debug.Log("deckManager is null? " + (deckManager == null));

        if (swapAvailableHighlight != null && deckManager != null)
        {
            bool shouldHighlight = deckManager.ShouldHighlightOpponentPanel(playerIndex);

            Debug.Log("Panel " + playerIndex + " highlight = " + shouldHighlight);

            swapAvailableHighlight.SetActive(shouldHighlight);
        }
    }

    void UpdateFavorSlot(Image slotImage, Player player, int index)
{
    if (slotImage == null || player == null)
        return;

    if (index < player.favorArea.Count)
    {
        Sprite icon = GetFavorIcon(player.favorArea[index].pranksterType);
        slotImage.gameObject.SetActive(true);
        slotImage.sprite = icon;
    }
    else
    {
        slotImage.gameObject.SetActive(false);
    }
}

    Sprite GetFavorIcon(PranksterType type)
    {
        switch (type)
        {
            case PranksterType.Thief: return thiefIcon;
            case PranksterType.Engineer: return engineerIcon;
            case PranksterType.Laborer: return laborerIcon;
            case PranksterType.Scribe: return scribeIcon;
            case PranksterType.Wizard: return wizardIcon;
            case PranksterType.BeastMaster: return beastmasterIcon;
            default: return null;
        }
    }

    public void SetSwapHighlightVisible(bool isVisible)
    {
        if (swapAvailableHighlight != null)
         swapAvailableHighlight.SetActive(isVisible);
    }


}