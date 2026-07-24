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
    [SerializeField] private LifetimeNotorietyDistractBarnabyButton distractBarnabyButton;

    [Header("Content")]
    [SerializeField] private string tooltipTitle;

    [TextArea]
    [SerializeField] private string tooltipDescription;

    [Header("Timing")]
    [SerializeField] private float hoverDelay = 0.5f;

    [Header("Highlight Handling")]
    [SerializeField] private GameObject discardPileHighlight;
    [SerializeField] private DeckManager deckManager;

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

        bool tooltipWasVisible =
            tooltipPanel != null &&
            tooltipPanel.activeSelf;

        HideTooltip();

        if (tooltipWasVisible && deckManager != null)
            deckManager.RefreshAllHighlights();
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
    if (discardPileHighlight != null)
        discardPileHighlight.SetActive(false);

    if (titleText != null)
        titleText.text = tooltipTitle;

    if (descriptionText != null)
    {
        int cost = 0;
        int gain = 0;
        bool hasCost = false;
        bool hasGain = false;

        if (crewSizeButton != null)
        {
            cost = crewSizeButton.GetCurrentCost();
            hasCost = true;
        }
        else if (inactiveServicesButton != null)
        {
            cost = inactiveServicesButton.GetCurrentCost();
            hasCost = true;
        }
        else if (pesterMayorButton != null)
        {
            cost = pesterMayorButton.GetCurrentCost();
            gain = pesterMayorButton.GetCurrentMischiefGain();
            hasCost = true;
            hasGain = true;
        }
        else if (distractBarnabyButton != null)
        {
            cost = distractBarnabyButton.GetCurrentCost();
            hasCost = true;
        }

        descriptionText.text = tooltipDescription;

        if (hasCost)
            descriptionText.text += "\n\nCost: " + cost + " Influence";

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