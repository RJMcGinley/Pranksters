using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WantedBoardPanelController : MonoBehaviour
{
    [Header("Board Cells")]
    public WantedBoardCell[] cells;

    [Header("Buttons")]
    public WantedSelectionButton[] selectionButtons;

    public Button postWantedSignsButton;

    [Header("UI")]
    public TMP_Text placementResultText;

    private WantedDirection currentDirection;

    private WantedSelectionType selectedType;
    private int selectedIndex = -1;
    private List<PranksterType> pendingRecruits;

    [Header("Recruit Icons")]
    public Sprite thiefIcon;
    public Sprite wizardIcon;
    public Sprite engineerIcon;
    public Sprite beastmasterIcon;
    public Sprite laborerIcon;
    public Sprite scribeIcon;

    private void Awake()
    {
        foreach (var button in selectionButtons)
        {
            if (button != null)
                button.Initialize(this);
        }

        if (postWantedSignsButton != null)
            postWantedSignsButton.interactable = false;
    }

    public void OpenForPrank(PrankCard prank)
    {
        Debug.Log("OPEN FOR PRANK CALLED");
        gameObject.SetActive(true);

        currentDirection = prank.wantedDirection;
        pendingRecruits = prank.requiredPranksters;

        selectedIndex = -1;

        ClearHighlights();

        if (postWantedSignsButton != null)
            postWantedSignsButton.interactable = false;

        if (placementResultText != null)
            placementResultText.text = "";

        RefreshArrowVisibility();
    }

    void RefreshArrowVisibility()
    {
        foreach (var button in selectionButtons)
        {
            if (button == null)
            {
                Debug.LogWarning("Wanted selection button reference is missing.");
                continue;
            }

            bool show = false;

            switch (currentDirection)
            {
                case WantedDirection.Horizontal:
                    show = button.selectionType == WantedSelectionType.Row;
                    break;

                case WantedDirection.Vertical:
                    show = button.selectionType == WantedSelectionType.Column;
                    break;

                case WantedDirection.Diagonal:
                    show = button.selectionType == WantedSelectionType.Diagonal;
                    break;
            }

            button.gameObject.SetActive(show);
        }
    }

    public void SelectPlacement(
        WantedSelectionType type,
        int index)
    {
        selectedType = type;
        selectedIndex = index;

        ClearHighlights();

        List<WantedBoardCell> selectedCells = GetCellsForSelection(type, index);

        int jailPreviewCount = 0;

        for (int i = 0; i < selectedCells.Count; i++)
        {
            WantedBoardCell cell = selectedCells[i];

            bool willJail =
                pendingRecruits != null &&
                pendingRecruits.Count == 4 &&
                cell.HasOccupant &&
                cell.Occupant == pendingRecruits[i];

            if (willJail)
            {
                cell.SetHighlight(false);
                cell.SetJailPreview(true);
                jailPreviewCount++;
            }
            else
            {
                cell.SetHighlight(true);
                cell.SetJailPreview(false);
            }
        }

        if (placementResultText != null)
        {
            placementResultText.text =
                jailPreviewCount + " recruit(s) will be jailed";
        }

        if (postWantedSignsButton != null)
            postWantedSignsButton.interactable = true;
    }

    List<WantedBoardCell> GetCellsForSelection(
        WantedSelectionType type,
        int index)
    {
        List<WantedBoardCell> result =
            new List<WantedBoardCell>();

        if (cells.Length != 16)
            return result;

        if (type == WantedSelectionType.Row)
        {
            int start = index * 4;

            result.Add(cells[start + 0]);
            result.Add(cells[start + 1]);
            result.Add(cells[start + 2]);
            result.Add(cells[start + 3]);
        }
        else if (type == WantedSelectionType.Column)
        {
            result.Add(cells[index + 0]);
            result.Add(cells[index + 4]);
            result.Add(cells[index + 8]);
            result.Add(cells[index + 12]);
        }
        else
        {
            if (index == 0)
            {
                result.Add(cells[0]);
                result.Add(cells[5]);
                result.Add(cells[10]);
                result.Add(cells[15]);
            }
            else
            {
                result.Add(cells[3]);
                result.Add(cells[6]);
                result.Add(cells[9]);
                result.Add(cells[12]);
            }
        }

        return result;
    }

    void ClearHighlights()
    {
        foreach (var cell in cells)
        {
            if (cell != null)
            {
                cell.SetHighlight(false);
                cell.SetJailPreview(false);
            }
        }
    }

    [ContextMenu("Test Horizontal")]
    public void TestHorizontal()
    {
        OpenForPrank(new PrankCard
        {
            wantedDirection = WantedDirection.Horizontal,
            requiredPranksters = new List<PranksterType>
            {
                PranksterType.Thief,
                PranksterType.Wizard,
                PranksterType.Engineer,
                PranksterType.Laborer
            }
        });
    }

    [ContextMenu("Test Fill A1 As Thief")]
    public void TestFillA1AsThief()
    {
        cells[0].SetOccupant(PranksterType.Thief, GetIconForType(PranksterType.Thief));
    }

    Sprite GetIconForType(PranksterType type)
    {
        switch (type)
        {
            case PranksterType.Thief:
                return thiefIcon;

            case PranksterType.Wizard:
                return wizardIcon;

            case PranksterType.Engineer:
                return engineerIcon;

            case PranksterType.BeastMaster:
                return beastmasterIcon;

            case PranksterType.Laborer:
                return laborerIcon;

            case PranksterType.Scribe:
                return scribeIcon;

            default:
                return null;
        }
    }
}