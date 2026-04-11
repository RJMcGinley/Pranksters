using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectionRowUI : MonoBehaviour
{
    public MainMenuPlayerSetup playerSetup;

    [Header("Identity")]
    public int slotIndex; // 0 = Player1, 1 = Player2, 2 = Player3, 3 = Player4

    [Header("UI References")]
    public Button selectorButton;
    public Image stateIconImage;
    public TextMeshProUGUI nameText;
    public GameObject editNameButtonObject;

    [Header("State Icons")]
    public Sprite humanIconSprite;
    public Sprite botIconSprite;
    public Sprite blankClosedSprite;

    public GameObject botCycleButtonObject;
    public PlayerNameEditUI nameEditUI;

    //public PlayerSelectionMenuUI menuUI;

    public void Initialize()
    {
    ApplyVisuals();
    }   

    public void OnSelectorClicked()
    {

        if (playerSetup == null)
            return;

        if (!playerSetup.IsSlotInteractable(slotIndex))
            return;

        playerSetup.CyclePlayerType(slotIndex, 1);

        var slot = playerSetup.playerSlots[slotIndex];
        slot.playerName = playerSetup.GetDefaultNameForSlot(slotIndex, slot.playerType);

        RefreshAllRows();
    }

    public void ApplyVisuals()
    {
        if (playerSetup == null)
            return;

        var slot = playerSetup.playerSlots[slotIndex];

        if (nameText != null)
            nameText.text = slot.playerName;

        if (stateIconImage != null)
        {
            switch (slot.playerType)
            {
                case MenuPlayerType.Human:
                    stateIconImage.sprite = humanIconSprite;
                    stateIconImage.enabled = true;
                    break;

                case MenuPlayerType.AI:
                    stateIconImage.sprite = botIconSprite;
                    stateIconImage.enabled = true;
                    break;

                case MenuPlayerType.Closed:
                    stateIconImage.sprite = blankClosedSprite;
                    stateIconImage.enabled = blankClosedSprite != null;
                    break;
            }
        }

        if (editNameButtonObject != null)
        editNameButtonObject.SetActive(slot.playerType == MenuPlayerType.Human);

        if (botCycleButtonObject != null)
        botCycleButtonObject.SetActive(slot.playerType == MenuPlayerType.AI);
    }

    void Start()
    {
    ApplyVisuals();
    }

    void RefreshAllRows()
    {
    PlayerSelectionRowUI[] rows = FindObjectsByType<PlayerSelectionRowUI>(FindObjectsSortMode.None);

    foreach (PlayerSelectionRowUI row in rows)
    {
        row.ApplyVisuals();
    }
    }

    public void OnEditNameClicked()
    {
    if (nameEditUI == null || playerSetup == null)
        return;

    var slot = playerSetup.playerSlots[slotIndex];

    if (slot.playerType != MenuPlayerType.Human)
        return;

    nameEditUI.OpenForRow(this, slot.playerName);
    }


}