using UnityEngine;

public class PrankHoverPreview : MonoBehaviour
{
    public Sprite previewSprite;
    public PrankPreviewPanel previewPanel;

    void OnMouseEnter()
    {
        if (previewPanel == null)
        {
            Debug.LogWarning("PrankHoverPreview: previewPanel is not assigned on " + gameObject.name);
            return;
        }

        if (previewSprite == null)
        {
            Debug.LogWarning("PrankHoverPreview: previewSprite is not assigned on " + gameObject.name);
            return;
        }

        previewPanel.Show(previewSprite);
    }

    void OnMouseExit()
    {
        if (previewPanel != null)
            previewPanel.Hide();
    }
}