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

        isHovering = true;
        hoverTimer = 0f;
        previewVisible = false;

        if (preview != null)
        {
            Debug.Log("HOVER ENTER | hiding preview before timer starts");
            preview.Hide();
        }
        else
        {
            Debug.Log("HOVER ENTER CANCEL WARNING | preview is NULL");
        }
    }

    void OnMouseExit()
    {
        Debug.Log("FAVOR SLOT HOVER EXIT | slot=" + favorSlotIndex +
                  " | timer=" + hoverTimer.ToString("0.00") +
                  " | previewVisible=" + previewVisible);

        isHovering = false;
        hoverTimer = 0f;
        previewVisible = false;

        if (preview != null)
        {
            Debug.Log("HOVER EXIT | hiding preview");
            preview.Hide();
        }
        else
        {
            Debug.Log("HOVER EXIT WARNING | preview is NULL");
        }
    }

    void Update()
    {
        if (!isHovering)
            return;

        hoverTimer += Time.deltaTime;

        Debug.Log("FAVOR SLOT TIMER | slot=" + favorSlotIndex +
                  " | timer=" + hoverTimer.ToString("0.00") +
                  " / " + hoverDelay.ToString("0.00") +
                  " | previewVisible=" + previewVisible);

        if (previewVisible)
        {
            return;
        }

        if (hoverTimer < hoverDelay)
            return;

        Debug.Log("HOVER DELAY PASSED | attempting preview | slot=" + favorSlotIndex);

        bool success = ShowPreviewForSlot();

        if (success)
        {
            previewVisible = true;
            Debug.Log("PREVIEW SUCCESSFUL | slot=" + favorSlotIndex +
                      " | shown at timer=" + hoverTimer.ToString("0.00"));
        }
        else
        {
            Debug.Log("PREVIEW FAILED | slot=" + favorSlotIndex +
                      " | timer=" + hoverTimer.ToString("0.00"));
        }
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

        if (preview == null)
        {
            Debug.Log("PREVIEW CANCELLED | preview is NULL");
            return false;
        }

        Debug.Log("PREVIEW DATA REQUEST | slot=" + favorSlotIndex);

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

        Debug.Log("PREVIEW SHOW CALLED | slot=" + favorSlotIndex);

        return true;
    }

    void OnMouseDown()
    {
        Debug.Log("FAVOR SLOT CLICK DETECTED");

        if (preview != null)
        {
            Debug.Log("CLICK | hiding preview");
            preview.Hide();
        }

        if (favorAreaHover == null)
        {
            Debug.Log("CLICK FAILED: favorAreaHover is null");
            return;
        }

        if (favorAreaHover.deckManager == null)
        {
            Debug.Log("CLICK FAILED: favorAreaHover.deckManager is null");
            return;
        }

        if (favorAreaHover.helperObject != null)
        {
            Debug.Log("CLICK | hiding helper object");
            favorAreaHover.helperObject.SetActive(false);
        }

        Debug.Log("CLICK | calling OnFavorAreaClicked");
        favorAreaHover.deckManager.OnFavorAreaClicked();
    }
}