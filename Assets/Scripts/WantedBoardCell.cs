using UnityEngine;
using UnityEngine.UI;

public class WantedBoardCell : MonoBehaviour
{
    [Header("Visuals")]
    public Image recruitIconImage;
    public GameObject highlightObject;
    public GameObject jailPreviewObject;
    public Image previewIconImage;

    public bool HasOccupant { get; private set; }
    public PranksterType Occupant { get; private set; }
    public WantedBoardPanelController controller;
    

    public void Clear()
    {
        HasOccupant = false;

        if (recruitIconImage != null)
        {
            recruitIconImage.sprite = null;
            recruitIconImage.enabled = false;
            recruitIconImage.gameObject.SetActive(false);
        }

        SetHighlight(false);
        SetJailPreview(false);
        ClearPreview();
    }

    public void SetOccupant(PranksterType type, Sprite icon)
    {
        HasOccupant = true;
        Occupant = type;

        if (recruitIconImage != null)
        {
            recruitIconImage.gameObject.SetActive(true);
            recruitIconImage.sprite = icon;
            recruitIconImage.enabled = true;
        }
    }

    public void SetHighlight(bool active)
    {
        if (highlightObject != null)
            highlightObject.SetActive(active);
    }

    public void SetJailPreview(bool active)
    {
        if (jailPreviewObject != null)
            jailPreviewObject.SetActive(active);

        if (active)
            ClearPreview();
    }

    public void SetPreviewOccupant(PranksterType type, Sprite icon)
    {
        if (previewIconImage != null)
        {
            previewIconImage.gameObject.SetActive(true);
            previewIconImage.sprite = icon;
            previewIconImage.enabled = true;
        }
    }

    public void ClearPreview()
    {
        if (previewIconImage != null)
        {
            previewIconImage.sprite = null;
            previewIconImage.enabled = false;
            previewIconImage.gameObject.SetActive(false);
        }
    }

    public void OnClicked()
    {
        Debug.Log(gameObject.name + " was clicked.");

        if (controller != null)
            controller.OnCellClicked(this);
    }
}