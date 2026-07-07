using UnityEngine;

public class HideoutController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInfoPanel playerInfoPanel;
    [SerializeField] private HideoutCardPreview hideoutCardPreview;

    [Header("Slot Highlights")]
    [SerializeField] private GameObject[] swapHighlights;

    private bool cardPreviewEnabled = true;

    public void SetCardPreviewEnabled(bool isEnabled)
    {
        cardPreviewEnabled = isEnabled;

        if (!cardPreviewEnabled)
            HideCardPreview();
    }

    public PranksterDeckEntry GetCardInSlot(int slotIndex)
    {
        if (playerInfoPanel == null)
            return null;

        return playerInfoPanel.GetFavorCardInSlot(slotIndex);
    }

    public void ShowCardPreview(int slotIndex)
    {
        if (!cardPreviewEnabled)
            return;

        Debug.Log("HIDEOUT CONTROLLER SHOW CARD PREVIEW | slot=" + slotIndex +
                  " | preview=" + (hideoutCardPreview != null ? hideoutCardPreview.name : "NULL"));

        if (hideoutCardPreview == null)
            return;

        PranksterDeckEntry card = GetCardInSlot(slotIndex);

        Debug.Log("HIDEOUT CONTROLLER CARD RESULT | " +
                  (card != null
                      ? "type=" + card.pranksterType + " | tier=" + card.tier + " | category=" + card.category
                      : "NULL"));

        hideoutCardPreview.Show(card);
    }

    public void HideCardPreview()
    {
        if (hideoutCardPreview != null)
            hideoutCardPreview.Hide();
    }

    public void RefreshSlotHighlights()
    {
        bool swapAvailable = false;

        if (playerInfoPanel != null && playerInfoPanel.deckManager != null)
        {
            swapAvailable = playerInfoPanel.deckManager.CanSwapWithOpponent(
                playerInfoPanel.representedPlayerIndex
            );
        }

        for (int i = 0; i < swapHighlights.Length; i++)
        {
            if (swapHighlights[i] == null)
                continue;

            bool hasCard = GetCardInSlot(i) != null;
            swapHighlights[i].SetActive(swapAvailable && hasCard);
        }
    }

    public void StartSwapFromSlot(int slotIndex)
    {
        if (playerInfoPanel == null || playerInfoPanel.deckManager == null)
            return;

        int targetPlayerIndex = playerInfoPanel.representedPlayerIndex;

        playerInfoPanel.deckManager.StartSwapFromHideoutSlot(targetPlayerIndex, slotIndex);
    }

    public void SetAllSlotHighlightsVisible(bool isVisible)
    {
        if (swapHighlights == null)
            return;

        for (int i = 0; i < swapHighlights.Length; i++)
        {
            if (swapHighlights[i] != null)
                swapHighlights[i].SetActive(isVisible);
        }
    }
}