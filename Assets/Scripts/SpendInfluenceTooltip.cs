using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpendInfluenceTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Tooltip Panel")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    [Header("Cost Source")]
    [SerializeField] private LifetimeNotorietyCrewSizeButton crewSizeButton;
    [SerializeField] private SpendInfluenceButton_UseInactiveServices inactiveServicesButton;
    [SerializeField] private LifetimeNotorietyPesterMayorButton pesterMayorButton;

    [Header("Content")]
    [SerializeField] private string tooltipTitle;

    [TextArea]
    [SerializeField] private string tooltipDescription;

    [Header("Timing")]
    [SerializeField] private float hoverDelay = 0.5f;

    private Coroutine showRoutine;
    private bool pointerInside;

    private void Awake()
    {
        HideTooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerInside = true;

        if (showRoutine != null)
            StopCoroutine(showRoutine);

        showRoutine = StartCoroutine(ShowAfterDelay());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerInside = false;

        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
            showRoutine = null;
        }

        HideTooltip();
    }

    private IEnumerator ShowAfterDelay()
    {
        yield return new WaitForSeconds(hoverDelay);

        showRoutine = null;

        if (!pointerInside)
            yield break;

        ShowTooltip();
    }

    private void ShowTooltip()
    {
        if (titleText != null)
            titleText.text = tooltipTitle;

        if (descriptionText != null)
        {
            int cost = 0;
            int gain = 0;
            bool hasGain = false;

            if (crewSizeButton != null)
            {
                cost = crewSizeButton.GetCurrentCost();
            }
            else if (inactiveServicesButton != null)
            {
                cost = inactiveServicesButton.GetCurrentCost();
            }
            else if (pesterMayorButton != null)
            {
                cost = pesterMayorButton.GetCurrentCost();
                gain = pesterMayorButton.GetCurrentMischiefGain();
                hasGain = true;
            }

            descriptionText.text =
                tooltipDescription +
                "\n\nCost: " + cost + " Influence";

            if (hasGain)
                descriptionText.text += "\nGain: " + gain + " Mischief";
        }

        if (tooltipPanel != null)
            tooltipPanel.SetActive(true);
    }

    private void HideTooltip()
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }
}