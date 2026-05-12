using UnityEngine;

public class AvailableServiceCloseCollider : MonoBehaviour
{
    [SerializeField] private AvailableServicesPanelController servicesPanelController;

    private void OnMouseDown()
    {
        if (servicesPanelController == null)
        {
            Debug.LogWarning("AvailableServiceCloseCollider has no servicesPanelController assigned.");
            return;
        }

        servicesPanelController.OnCloseServicePanelClicked();
    }
}