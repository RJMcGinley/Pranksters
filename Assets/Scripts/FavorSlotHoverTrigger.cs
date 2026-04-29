using UnityEngine;

public class FavorSlotHoverTrigger : MonoBehaviour
{
    public int favorSlotIndex = 0;
    public FavorAreaHover favorAreaHover;
    public FavorAreaPreview preview;

    public float hoverDelay = 0.5f;

    private bool isHovering = false;
    private float hoverTimer = 0f;
    private bool previewVisible = false;

    void Start()
    {
        Debug.Log("FAVOR SLOT START | slot=" + favorSlotIndex +
                  " | preview assigned=" + (preview != null));

        if (preview != null)
            preview.Hide();
    }

    void OnMouseEnter()
{
    Debug.Log("FAVOR SLOT HOVER ENTER | slot=" + favorSlotIndex);

    if (favorAreaHover == null || favorAreaHover.deckManager == null)
        return;

    if (favorAreaHover.deckManager.IsInteractionBlocked())
        return;

    if (!favorAreaHover.deckManager.CanHoverFavorArea())
        return;

    isHovering = true;
    hoverTimer = 0f;
    previewVisible = false;

    if (preview != null)
        preview.Hide();

    if (favorAreaHover.helperObject != null)
        favorAreaHover.helperObject.SetActive(true);

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayFavorHover();
}

    void OnMouseExit()
    {
        Debug.Log("FAVOR SLOT HOVER EXIT | slot=" + favorSlotIndex);

        ResetHoverState();
    }

    void Update()
    {
        if (!isHovering)
            return;

        if (favorAreaHover == null || favorAreaHover.deckManager == null)
        {
            ResetHoverState();
            return;
        }

        if (favorAreaHover.deckManager.IsInteractionBlocked())
        {
            Debug.Log("FAVOR SLOT PREVIEW BLOCKED | interaction blocked");
            ResetHoverState();
            return;
        }

        hoverTimer += Time.deltaTime;

        if (previewVisible)
            return;

        if (hoverTimer < hoverDelay)
            return;

        bool success = ShowPreviewForSlot();

        if (success)
            previewVisible = true;
    }

    bool ShowPreviewForSlot()
    {
        if (favorAreaHover == null)
        {
            Debug.Log("PREVIEW CANCELLED | favorAreaHover is NULL");
            return false;
        }

        if (favorAreaHover.deckManager == null)
        {
            Debug.Log("PREVIEW CANCELLED | favorAreaHover.deckManager is NULL");
            return false;
        }

        if (favorAreaHover.deckManager.IsInteractionBlocked())
        {
            Debug.Log("PREVIEW CANCELLED | interaction is blocked");
            if (preview != null)
                preview.Hide();

            return false;
        }

        if (preview == null)
        {
            Debug.Log("PREVIEW CANCELLED | preview is NULL");
            return false;
        }

        PranksterDeckEntry card = favorAreaHover.deckManager.GetFavorCardAtIndex(favorSlotIndex);

        if (card == null)
        {
            Debug.Log("PREVIEW CANCELLED | no card found at favor slot index " + favorSlotIndex);
            preview.Hide();
            return false;
        }

        Debug.Log("PREVIEW CARD FOUND | slot=" + favorSlotIndex +
                  " | type=" + card.pranksterType +
                  " | tier=" + card.tier +
                  " | category=" + card.category);

        preview.Show(card);

        return true;
    }

    void OnMouseDown()
    {
        Debug.Log("FAVOR SLOT CLICK DETECTED");

        if (preview != null)
            preview.Hide();

        if (favorAreaHover == null)
        {
            Debug.Log("CLICK FAILED | favorAreaHover is NULL");
            return;
        }

        if (favorAreaHover.deckManager == null)
        {
            Debug.Log("CLICK FAILED | favorAreaHover.deckManager is NULL");
            return;
        }

        if (favorAreaHover.deckManager.IsInteractionBlocked())
        {
            Debug.Log("CLICK BLOCKED | interaction is blocked");
            return;
        }

        if (favorAreaHover.helperObject != null)
            favorAreaHover.helperObject.SetActive(false);

        Debug.Log("CLICK | calling OnFavorAreaClicked");
        favorAreaHover.deckManager.OnFavorAreaClicked();
    }

    private void ResetHoverState()
{
    isHovering = false;
    hoverTimer = 0f;
    previewVisible = false;

    if (preview != null)
        preview.Hide();

    if (favorAreaHover != null && favorAreaHover.helperObject != null)
        favorAreaHover.helperObject.SetActive(false);
}
}