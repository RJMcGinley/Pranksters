using UnityEngine;

public class PreviewFavorSlotClick : MonoBehaviour
{
    public DeckManager deckManager;
    public OpponentPreviewPanel previewPanel;
    public int favorSlotIndex;

    void OnMouseDown()
    {
        Debug.Log("PREVIEW FAVOR SLOT CLICKED: " + favorSlotIndex);

        if (deckManager == null || previewPanel == null)
            return;

        if (!previewPanel.IsLockedForSwap())
            return;

        deckManager.ResolveSwapTargetChoice(favorSlotIndex);
    }
}