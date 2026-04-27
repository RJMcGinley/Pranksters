using UnityEngine;

public class PreviewFavorSlotClick : MonoBehaviour
{
    public DeckManager deckManager;
    public OpponentPreviewPanel previewPanel;
    public int favorSlotIndex;

    [Header("Player Favor Collider Suppression")]
    public GameObject activeFavorDisplay;

    public float hoverDelay = 0.5f;

    private bool isHovering = false;
    private float hoverTimer = 0f;
    private bool previewShown = false;
    private Vector3 lastMousePosition;

    void OnMouseEnter()
    {
        if (previewPanel == null)
            return;

        SetPlayerFavorColliders(false);

        isHovering = true;
        hoverTimer = 0f;
        previewShown = false;
        lastMousePosition = Input.mousePosition;

        previewPanel.StopFavorSlotPreview();
    }

    void OnMouseExit()
    {
        StopHover();
    }

    void Update()
    {
        if (!isHovering || previewPanel == null)
            return;

        if (previewShown)
        {
            if (Input.mousePosition.x != lastMousePosition.x ||
                Input.mousePosition.y != lastMousePosition.y)
            {
                StopHover();
            }

            return;
        }

        hoverTimer += Time.deltaTime;

        if (hoverTimer < hoverDelay)
            return;

        previewPanel.StartFavorSlotPreview(favorSlotIndex);
        previewShown = true;
        lastMousePosition = Input.mousePosition;
    }

    void OnMouseDown()
    {
        Debug.Log("PREVIEW FAVOR SLOT CLICKED: " + favorSlotIndex);

        StopHover();

        if (deckManager == null || previewPanel == null)
            return;

        if (!previewPanel.IsLockedForSwap())
            return;

        deckManager.ResolveSwapTargetChoice(favorSlotIndex);
    }

    void StopHover()
    {
        isHovering = false;
        hoverTimer = 0f;
        previewShown = false;

        if (previewPanel != null)
            previewPanel.StopFavorSlotPreview();

        SetPlayerFavorColliders(true);
    }

    void SetPlayerFavorColliders(bool enabledState)
    {
        if (activeFavorDisplay == null)
            return;

        Collider2D[] colliders = activeFavorDisplay.GetComponentsInChildren<Collider2D>(true);

        foreach (Collider2D col in colliders)
            col.enabled = enabledState;
    }
}