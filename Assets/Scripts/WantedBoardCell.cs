using UnityEngine;
using UnityEngine.UI;

public class WantedBoardCell : MonoBehaviour
{
    [Header("Visuals")]
    public Image recruitIconImage;
    public GameObject highlightObject;
    public GameObject jailPreviewObject;

    public bool HasOccupant { get; private set; }
    public PranksterType Occupant { get; private set; }

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
    }
}