using TMPro;
using UnityEngine;

public class PlayerNameEditUI : MonoBehaviour
{
    public TMP_InputField inputField;
    public PlayerSelectionRowUI currentRow;

    public void OpenForRow(PlayerSelectionRowUI row, string currentName)
    {
        currentRow = row;

        gameObject.SetActive(true);

        if (inputField != null)
        {
            inputField.text = currentName;
            inputField.ActivateInputField();
            inputField.Select();
        }
    }

    public void ConfirmName()
    {
        if (currentRow == null || inputField == null || currentRow.playerSetup == null)
        {
            gameObject.SetActive(false);
            return;
        }

        string newName = inputField.text.Trim();

        if (string.IsNullOrEmpty(newName))
        {
            newName = currentRow.playerSetup.GetDefaultNameForSlot(
                currentRow.slotIndex,
                currentRow.playerSetup.playerSlots[currentRow.slotIndex].playerType
            );
        }

        currentRow.playerSetup.playerSlots[currentRow.slotIndex].playerName = newName;
        currentRow.ApplyVisuals();

        gameObject.SetActive(false);
        currentRow = null;
    }

    public void CancelEdit()
    {
        gameObject.SetActive(false);
        currentRow = null;
    }

    void Update()
    {
    if (!gameObject.activeSelf)
        return;

    if (Input.GetKeyDown(KeyCode.Escape))
    {
        CancelEdit();
    }
    }


}