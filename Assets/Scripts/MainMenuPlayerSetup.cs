using System.Collections.Generic;
using UnityEngine;

public class MainMenuPlayerSetup : MonoBehaviour
{
    public List<MenuPlayerSlot> playerSlots = new List<MenuPlayerSlot>();

    void Awake()
    {
        InitializeDefaults();
    }

    public void InitializeDefaults()
    {
    if (playerSlots == null)
        playerSlots = new List<MenuPlayerSlot>();

    playerSlots.Clear();

    playerSlots.Add(new MenuPlayerSlot { playerName = "Player 1", playerType = MenuPlayerType.Human });
    playerSlots.Add(new MenuPlayerSlot { playerName = "Jerek", playerType = MenuPlayerType.AI });
    playerSlots.Add(new MenuPlayerSlot { playerName = "Closed", playerType = MenuPlayerType.Closed });
    playerSlots.Add(new MenuPlayerSlot { playerName = "Closed", playerType = MenuPlayerType.Closed });
    }

    public void CyclePlayerType(int slotIndex, int direction)
    {
        if (slotIndex < 0 || slotIndex >= playerSlots.Count)
            return;

        if (slotIndex == 0)
            return;

        if (slotIndex == 1)
        {
            playerSlots[1].playerType = playerSlots[1].playerType == MenuPlayerType.Human
                ? MenuPlayerType.AI
                : MenuPlayerType.Human;
            return;
        }

        if (slotIndex == 3 && playerSlots[2].playerType == MenuPlayerType.Closed)
            return;

        MenuPlayerType current = playerSlots[slotIndex].playerType;
        MenuPlayerType next = current;

        if (direction > 0)
        {
            if (current == MenuPlayerType.Closed) next = MenuPlayerType.Human;
            else if (current == MenuPlayerType.Human) next = MenuPlayerType.AI;
            else next = MenuPlayerType.Closed;
        }
        else
        {
            if (current == MenuPlayerType.Closed) next = MenuPlayerType.AI;
            else if (current == MenuPlayerType.AI) next = MenuPlayerType.Human;
            else next = MenuPlayerType.Closed;
        }

        playerSlots[slotIndex].playerType = next;

        if (slotIndex == 2 && playerSlots[2].playerType == MenuPlayerType.Closed)
        {   
            playerSlots[3].playerType = MenuPlayerType.Closed;
            playerSlots[3].playerName = GetDefaultNameForSlot(3, MenuPlayerType.Closed);
        }
    }

    public bool IsSlotInteractable(int slotIndex)
    {
        if (slotIndex == 0) return false;
        if (slotIndex == 3 && playerSlots[2].playerType == MenuPlayerType.Closed) return false;
        return true;
    }

    public int GetActivePlayerCount()
    {
        int count = 0;

        for (int i = 0; i < playerSlots.Count; i++)
        {
            if (playerSlots[i].playerType != MenuPlayerType.Closed)
                count++;
        }

        return count;
    }

    public string GetDefaultNameForSlot(int slotIndex, MenuPlayerType playerType)
    {   
    switch (slotIndex)
    {
        case 0:
            return "Player 1";

        case 1:
            return playerType == MenuPlayerType.AI ? "Jerek" : "Player 2";

        case 2:
            if (playerType == MenuPlayerType.AI) return "Trikstan";
            if (playerType == MenuPlayerType.Human) return "Player 3";
            return "Closed";

        case 3:
            if (playerType == MenuPlayerType.AI) return "Dr. Giggles MD";
            if (playerType == MenuPlayerType.Human) return "Player 4";
            return "Closed";
    }

    return "Closed";
    }


}