using TMPro;
using UnityEngine;

public class PlayerNameEditUI : MonoBehaviour
{
    public TMP_InputField inputField;
    public PlayerSelectionRowUI currentRow;

    public void OpenForRow(PlayerSelectionRowUI row, string currentName)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuClick();

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
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayConfirmClick();

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
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBackClick();

        gameObject.SetActive(false);
        currentRow = null;
    }

    void Update()
{
    if (!gameObject.activeSelf)
        return;

    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
    {
        ConfirmName();
        return;
    }

    if (Input.GetKeyDown(KeyCode.Escape))
    {
        CancelEdit();
    }
}
}