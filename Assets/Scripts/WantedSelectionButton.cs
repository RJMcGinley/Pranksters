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
        if (panelController == null)
            return;

        panelController.SelectPlacement(selectionType, selectionIndex);
    }
}