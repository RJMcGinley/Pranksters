using UnityEngine;

public class HandCardDragReorder : MonoBehaviour
{
    public DeckManager deckManager;
    public int cardIndex;

    private bool isDragging;
    private bool hasMovedEnoughToDrag;
    public SettingsMenuController settingsMenuController;

    private Vector3 originalLocalPosition;
    private Vector3 originalScale;
    private Vector3 mouseDownWorldPosition;

    private SpriteRenderer[] spriteRenderers;
    private int[] originalSortingOrders;

    private PranksterDeckEntry draggedCard;
    private int currentTargetIndex = -1;

    private const float dragStartThreshold = 0.15f;
    private const float dragYOffset = 0.1f;
    private const int draggedSortingOrderBoost = 100;

    void Awake()
    {
        originalLocalPosition = transform.localPosition;
        originalScale = transform.localScale;

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        originalSortingOrders = new int[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
            originalSortingOrders[i] = spriteRenderers[i].sortingOrder;
    }

    void OnMouseDown()
    {
        if (settingsMenuController != null &&
            settingsMenuController.IsPanelBlockingInteraction())
            return;
        if (deckManager == null || !deckManager.CanReorderHand())
            return;

        draggedCard = deckManager.GetCurrentPlayerHandCard(cardIndex);

        if (draggedCard == null)
            return;

        isDragging = true;
        hasMovedEnoughToDrag = false;
        currentTargetIndex = cardIndex;

        originalLocalPosition = transform.localPosition;
        originalScale = transform.localScale;
        mouseDownWorldPosition = GetMouseWorldPosition();

        transform.localScale = originalScale * 1.08f;

        SetDraggedSorting(true);
    }

    void OnMouseDrag()
    {
        if (!isDragging)
            return;

        Vector3 mouseWorld = GetMouseWorldPosition();

        if (!hasMovedEnoughToDrag)
        {
            float distance = Vector3.Distance(mouseWorld, mouseDownWorldPosition);

            if (distance < dragStartThreshold)
                return;

            hasMovedEnoughToDrag = true;
        }

        Vector3 parentLocalPosition = transform.parent.InverseTransformPoint(mouseWorld);
        parentLocalPosition.y = originalLocalPosition.y + dragYOffset;
        parentLocalPosition.z = originalLocalPosition.z;

        transform.localPosition = parentLocalPosition;

        int targetIndex = GetTargetIndexFromLocalX(transform.localPosition.x);

        if (targetIndex != currentTargetIndex)
        {
            currentTargetIndex = targetIndex;

            deckManager.PreviewReorderCurrentPlayerHand(
                draggedCard,
                currentTargetIndex,
                gameObject
            );
        }
    }

    void OnMouseUp()
    {
        if (!isDragging)
            return;

        isDragging = false;

        transform.localScale = originalScale;
        SetDraggedSorting(false);

        deckManager.FinalizeCurrentPlayerHandReorder();

        draggedCard = null;
        currentTargetIndex = -1;
    }

    private void SetDraggedSorting(bool isDragged)
    {
        if (spriteRenderers == null)
            return;

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null)
                continue;

            if (isDragged)
                spriteRenderers[i].sortingOrder = originalSortingOrders[i] + draggedSortingOrderBoost;
            else
                spriteRenderers[i].sortingOrder = originalSortingOrders[i];
        }
    }

    private int GetTargetIndexFromLocalX(float localX)
    {
        Player player = deckManager.turnManager.GetCurrentPlayer();

        if (player == null || player.hand == null || player.hand.Count == 0)
            return cardIndex;

        int count = player.hand.Count;

        float spacing = GetEstimatedSpacing(count);
        float startX = -(count - 1) * spacing / 2f;

        int targetIndex = Mathf.RoundToInt((localX - startX) / spacing);
        return Mathf.Clamp(targetIndex, 0, count - 1);
    }

    private float GetEstimatedSpacing(int count)
    {
        if (count >= 8)
            return 1.25f;

        if (count == 7)
            return 1.45f;

        if (count == 6)
            return 1.65f;

        if (count == 5)
            return 1.9f;

        return 2.1f;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;

        Camera cam = Camera.main;

        if (cam == null)
            return transform.position;

        float distanceFromCamera = Mathf.Abs(cam.transform.position.z - transform.position.z);
        mousePosition.z = distanceFromCamera;

        return cam.ScreenToWorldPoint(mousePosition);
    }
}