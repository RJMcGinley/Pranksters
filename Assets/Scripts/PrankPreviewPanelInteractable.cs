using UnityEngine;
using UnityEngine.EventSystems;

public class PrankPreviewPanelInteractable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public PrankPreviewPanel previewPanel;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("PREVIEW HOVER ZONE ENTER on " + gameObject.name);

        if (previewPanel != null)
            previewPanel.NotifyPanelEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("PREVIEW HOVER ZONE EXIT on " + gameObject.name);

        if (previewPanel != null)
            previewPanel.NotifyPanelExit();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("PREVIEW HOVER ZONE CLICK on " + gameObject.name);

        if (previewPanel != null)
            previewPanel.OnPanelClicked();
    }
}