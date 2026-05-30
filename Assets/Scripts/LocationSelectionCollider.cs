using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class LocationSelectionCollider : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private LocationSelectionPanelController panelController;

    [Header("Location Data")]
    [SerializeField] private string locationName;
    [SerializeField] private GameLocationType locationType;

    [TextArea]
    [SerializeField] private string flavorText;

    [TextArea]
    [SerializeField] private string effectText;

    [Header("Hover")]
    [SerializeField] private float hoverDelay = 0.5f;
    [SerializeField] private GameObject glowObject;

    private Coroutine hoverCoroutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (glowObject != null)
            glowObject.SetActive(true);

        if (hoverCoroutine != null)
            StopCoroutine(hoverCoroutine);

        hoverCoroutine = StartCoroutine(HoverPopupRoutine());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine);
            hoverCoroutine = null;
        }

        if (glowObject != null)
            glowObject.SetActive(false);

        if (panelController != null)
            panelController.HidePopup();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Location selected: " + locationName);

        if (panelController != null)
        {
            panelController.SelectLocation(locationType);
        }
    }   

    private IEnumerator HoverPopupRoutine()
    {
        yield return new WaitForSeconds(hoverDelay);

        if (panelController != null)
        {
            panelController.ShowPopup(
                locationName,
                flavorText,
                effectText
            );
        }
    }

    private void Start()
    {
        if (glowObject != null)
            glowObject.SetActive(false);
    }


}