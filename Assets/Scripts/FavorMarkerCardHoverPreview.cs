using UnityEngine;

public class FavorMarkerCardHoverPreview : MonoBehaviour
{
    public DeckManager deckManager;
    public FavorCardPreviewPanel previewPanel;

    public int favorSlotIndex = 0;
    public float hoverDelay = 1f;

    [Header("Fixed Preview Placement")]
    public Vector3 fixedPreviewPosition = new Vector3(0f, 0f, 0f);
    public Vector3 fixedPreviewScale = new Vector3(1f, 1f, 1f);

    private bool isHovering = false;
    private float hoverTimer = 0f;
    private Vector3 lastMousePosition;
    private bool previewShowing = false;

    void OnMouseEnter()
    {
        Debug.Log("MARKER HOVER ENTER | slot=" + favorSlotIndex);

        if (!CanPreview())
        {
            Debug.Log("HOVER BLOCKED | slot=" + favorSlotIndex);
            return;
        }

        isHovering = true;
        hoverTimer = 0f;
        previewShowing = false;
        lastMousePosition = Input.mousePosition;

        Debug.Log("HOVER TIMER STARTED | slot=" + favorSlotIndex);
    }

    void OnMouseExit()
    {
        Debug.Log("MARKER HOVER EXIT | slot=" + favorSlotIndex);
        ResetState();
    }

    void Update()
    {
        if (!isHovering)
            return;

        if (!CanPreview())
        {
            Debug.Log("UPDATE STOPPED - CanPreview FALSE | slot=" + favorSlotIndex);
            ResetState();
            return;
        }

        if (!previewShowing)
        {
            hoverTimer += Time.deltaTime;

            Debug.Log("HOVER TIMER: " + hoverTimer.ToString("F2"));

            if (hoverTimer >= hoverDelay)
            {
                Debug.Log("TIMER COMPLETE - SHOWING PREVIEW");
                ShowPreview();

                lastMousePosition = Input.mousePosition;
            }

            return;
        }

        float movement = Vector3.Distance(Input.mousePosition, lastMousePosition);

        if (movement > 2f)
        {
            Debug.Log("PREVIEW HIDDEN - MOUSE MOVED | movement=" + movement);
            ResetState();
        }
    }

    void ShowPreview()
    {
        Debug.Log("SHOW PREVIEW CALLED | slot=" + favorSlotIndex);

        if (previewPanel == null)
        {
            Debug.Log("PREVIEW FAILED: previewPanel NULL");
            return;
        }

        if (deckManager == null)
        {
            Debug.Log("PREVIEW FAILED: deckManager NULL");
            return;
        }

        PranksterDeckEntry card = deckManager.GetCurrentPlayerFavorCardAtSlot(favorSlotIndex);

        if (card == null)
        {
            Debug.Log("PREVIEW FAILED: card NULL");
            return;
        }

        Debug.Log("PREVIEW CARD: " + card.pranksterType + " | tier=" + card.tier);

        previewPanel.Show(card, fixedPreviewPosition);
        previewPanel.transform.localScale = fixedPreviewScale;

        previewShowing = true;

        Debug.Log("PREVIEW SHOWN SUCCESSFULLY");
    }

    void ResetState()
    {
        isHovering = false;
        hoverTimer = 0f;
        previewShowing = false;

        if (previewPanel != null)
            previewPanel.Hide();
    }

    bool CanPreview()
    {
        if (deckManager == null)
            return false;

        if (deckManager.IsInteractionBlocked())
            return false;

        return deckManager.GetCurrentPlayerFavorCardAtSlot(favorSlotIndex) != null;
    }

    void OnMouseDown()
    {
        if (deckManager == null)
            return;

        if (deckManager.IsInteractionBlocked())
            return;

        Debug.Log("MARKER CLICKED -> PASS TO FAVOR AREA");

        deckManager.OnFavorAreaClicked();
    }
}