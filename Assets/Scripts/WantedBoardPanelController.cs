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

    private int previewJailCount;
    
    private System.Action onPlacementCommitted;

    [Header("Jail")]
    public WantedJailController wantedJailController;
    [Header("Display Panel")]
    public WantedDisplayPanelController wantedDisplayPanelController;

    private System.Action<string> onLossTriggered;

    private void Awake()
    {
        foreach (var button in selectionButtons)
        {
            if (button != null)
                button.Initialize(this);
        }

        if (postWantedSignsButton != null)
            postWantedSignsButton.interactable = false;

        if (postWantedSignsButton != null)
        {
            postWantedSignsButton.onClick.AddListener(
                CommitPlacement);
        }
    }

    public void OpenForPrank(
    PrankCard prank,
    System.Action onCommitted = null,
    System.Action<string> onLoss = null)
    {
        onLossTriggered = onLoss;
        Debug.Log("OPEN FOR PRANK CALLED");

        if (wantedDisplayPanelController != null)
            wantedDisplayPanelController.Hide();

        gameObject.SetActive(true);

        onPlacementCommitted = onCommitted;

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
        int bestOpenCount = -1;

        foreach (var button in selectionButtons)
        {
            if (button == null)
                continue;

            bool directionMatches = DoesButtonMatchCurrentDirection(button);

            if (!directionMatches)
                continue;

            List<WantedBoardCell> selectedCells =
                GetCellsForSelection(
                    button.selectionType,
                    button.selectionIndex);

            int openCount = CountOpenCells(selectedCells);

            if (openCount > bestOpenCount)
                bestOpenCount = openCount;
        }

        foreach (var button in selectionButtons)
        {
            if (button == null)
            {
                Debug.LogWarning("Wanted selection button reference is missing.");
                continue;
            }

            bool show = false;

            if (DoesButtonMatchCurrentDirection(button))
            {
                List<WantedBoardCell> selectedCells =
                    GetCellsForSelection(
                        button.selectionType,
                        button.selectionIndex);

                int openCount = CountOpenCells(selectedCells);

                show =
                    openCount == bestOpenCount &&
                    openCount > 0;
            }

            button.gameObject.SetActive(show);
        }

        if (bestOpenCount <= 0)
        {
            TriggerWantedBoardLoss("No valid placement has open spaces.");
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

        previewJailCount = 0;

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
                previewJailCount++;
            }
            else
            {
                cell.SetHighlight(true);
                cell.SetJailPreview(false);

                if (pendingRecruits != null && pendingRecruits.Count == 4)
                {
                    PranksterType recruit = pendingRecruits[i];

                    cell.SetPreviewOccupant(
                        recruit,
                        GetIconForType(recruit));
                }
            }
        }

        if (placementResultText != null)
        {
            placementResultText.text =
                previewJailCount + " recruit(s) will be jailed";
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
                cell.ClearPreview();
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

    void CommitPlacement()
{
    List<WantedBoardCell> selectedCells =
        GetCellsForSelection(
            selectedType,
            selectedIndex);

    for (int i = 0; i < selectedCells.Count; i++)
    {
        WantedBoardCell cell = selectedCells[i];
        PranksterType recruit = pendingRecruits[i];

        bool sameType =
            cell.HasOccupant &&
            cell.Occupant == recruit;

        if (sameType)
        {
            cell.Clear();

            if (wantedJailController != null)
            {
                wantedJailController.AddJailedRecruit(recruit);

                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayJailDoorClosing();

                Debug.Log(
                    "RECRUIT JAILED | Total Jailed = "
                    + wantedJailController.GetJailedRecruitCount());
            }
        }
        else
        {
            cell.SetOccupant(
                recruit,
                GetIconForType(recruit));
        }
    }

    ClearHighlights();

    if (wantedJailController != null &&
        wantedJailController.GetJailedRecruitCount() >= 3)
    {
        TriggerWantedBoardLoss("Jail is full.");
        return;
    }

    if (IsWantedBoardFull())
    {
        TriggerWantedBoardLoss("Wanted Board is full.");
        return;
    }

    if (placementResultText != null)
        placementResultText.text = "";

    if (postWantedSignsButton != null)
        postWantedSignsButton.interactable = false;

    gameObject.SetActive(false);

    if (wantedDisplayPanelController != null)
    {
        wantedDisplayPanelController.RefreshFromWantedCells(cells);
        wantedDisplayPanelController.Show();
    }

    if (onPlacementCommitted != null)
    {
        onPlacementCommitted.Invoke();
        onPlacementCommitted = null;
    }
}

bool DoesButtonMatchCurrentDirection(WantedSelectionButton button)
{
    switch (currentDirection)
    {
        case WantedDirection.Horizontal:
            return button.selectionType == WantedSelectionType.Row;

        case WantedDirection.Vertical:
            return button.selectionType == WantedSelectionType.Column;

        case WantedDirection.Diagonal:
            return button.selectionType == WantedSelectionType.Diagonal;

        default:
            return false;
    }
}

int CountOpenCells(List<WantedBoardCell> selectedCells)
{
    int count = 0;

    foreach (WantedBoardCell cell in selectedCells)
    {
        if (cell != null && !cell.HasOccupant)
            count++;
    }

    return count;
}

void TriggerWantedBoardLoss(string reason)
{
    Debug.Log("WANTED BOARD LOSS | " + reason);

    gameObject.SetActive(false);

    if (onLossTriggered != null)
    {
        onLossTriggered.Invoke(reason);
        onLossTriggered = null;
    }
}

bool IsWantedBoardFull()
{
    foreach (WantedBoardCell cell in cells)
    {
        if (cell != null && !cell.HasOccupant)
            return false;
    }

    return true;
}

public void ResetWantedBoardState()
{
    currentDirection = WantedDirection.Horizontal;
    selectedType = WantedSelectionType.Row;
    selectedIndex = -1;
    pendingRecruits = null;
    previewJailCount = 0;

    onPlacementCommitted = null;
    onLossTriggered = null;

    foreach (WantedBoardCell cell in cells)
    {
        if (cell != null)
            cell.Clear();
    }

    foreach (WantedSelectionButton button in selectionButtons)
    {
        if (button != null)
            button.gameObject.SetActive(false);
    }

    if (placementResultText != null)
        placementResultText.text = "";

    if (postWantedSignsButton != null)
        postWantedSignsButton.interactable = false;

    gameObject.SetActive(false);

    if (wantedDisplayPanelController != null)
    {
        wantedDisplayPanelController.RefreshFromWantedCells(cells);
        wantedDisplayPanelController.Show();
    }
}
}