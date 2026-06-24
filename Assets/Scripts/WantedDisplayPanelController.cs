using UnityEngine;
using UnityEngine.UI;

public class WantedDisplayPanelController : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panelRoot;

    [Header("Display Cells")]
    public Image[] cellImages;

    public void Show()
    {
        if (panelRoot != null)
            panelRoot.SetActive(true);
    }

    public void Hide()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    public void RefreshFromWantedCells(WantedBoardCell[] sourceCells)
    {
        if (sourceCells == null || cellImages == null)
            return;

        int count = Mathf.Min(sourceCells.Length, cellImages.Length);

        for (int i = 0; i < count; i++)
        {
            if (cellImages[i] == null || sourceCells[i] == null)
                continue;

            if (sourceCells[i].HasOccupant && sourceCells[i].recruitIconImage != null)
            {
                cellImages[i].sprite = sourceCells[i].recruitIconImage.sprite;
                cellImages[i].enabled = true;
                cellImages[i].gameObject.SetActive(true);
            }
            else
            {
                cellImages[i].sprite = null;
                cellImages[i].enabled = false;
                cellImages[i].gameObject.SetActive(false);
            }
        }
    }
}