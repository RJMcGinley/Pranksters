using UnityEngine;

public class AvailableServiceSlotPreviewHover : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AvailableServicesServiceCardSlot serviceCardSlot;
    [SerializeField] private FavorAreaPreview previewPanel;

    [Header("Hover Timing")]
    [SerializeField] private float hoverDelay = 0.5f;

    private bool isHovering;
    private float hoverTimer;
    private bool previewShown;

    private void Update()
    {
        if (!isHovering || previewShown)
            return;

        hoverTimer += Time.deltaTime;

        if (hoverTimer >= hoverDelay)
            ShowPreview();
    }

    private void OnMouseEnter()
    {
        isHovering = true;
        hoverTimer = 0f;
        previewShown = false;
    }

    private void OnMouseExit()
    {
        isHovering = false;
        hoverTimer = 0f;
        previewShown = false;

        if (previewPanel != null)
            previewPanel.Hide();
    }

    private void ShowPreview()
    {
        previewShown = true;

        if (serviceCardSlot == null || previewPanel == null)
            return;

        PranksterDeckEntry card = serviceCardSlot.GetAssignedCardCopy();

        if (card == null)
            return;

        previewPanel.Show(card);
    }
}