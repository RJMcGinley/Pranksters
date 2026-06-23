using UnityEngine;

public class WantedSelectionButton : MonoBehaviour
{
    public WantedSelectionType selectionType;

    public int selectionIndex;

    private WantedBoardPanelController panelController;

    public void Initialize(WantedBoardPanelController controller)
    {
        panelController = controller;
    }

    public void OnClicked()
{
    Debug.Log("WANTED BUTTON CLICKED | object=" + gameObject.name +
              " | type=" + selectionType +
              " | index=" + selectionIndex);

    if (panelController == null)
    {
        Debug.LogWarning("WANTED BUTTON CLICK FAILED | panelController is null on " + gameObject.name);
        return;
    }

    panelController.SelectPlacement(selectionType, selectionIndex);
}
}