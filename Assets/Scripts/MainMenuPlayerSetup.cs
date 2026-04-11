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

        playerSlots.Add(new MenuPlayerSlot
        {
            playerName = "Player 1",
            playerType = MenuPlayerType.Human,
            botIdentity = BotIdentity.None
        });

        playerSlots.Add(new MenuPlayerSlot
        {
            playerName = "Jerek",
            playerType = MenuPlayerType.AI,
            botIdentity = BotIdentity.Jerek
        });

        playerSlots.Add(new MenuPlayerSlot
        {
            playerName = "Closed",
            playerType = MenuPlayerType.Closed,
            botIdentity = BotIdentity.None
        });

        playerSlots.Add(new MenuPlayerSlot
        {
            playerName = "Closed",
            playerType = MenuPlayerType.Closed,
            botIdentity = BotIdentity.None
        });
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

            ApplySlotDefaultsAfterTypeChange(1);
            SyncBotAssignments();

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayMenuClick();

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

        ApplySlotDefaultsAfterTypeChange(slotIndex);
        SyncBotAssignments();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuClick();
    }

    public void CycleBotIdentity(int slotIndex, int direction)
    {
        if (slotIndex < 0 || slotIndex >= playerSlots.Count)
            return;

        if (slotIndex < 1 || slotIndex > 3)
            return;

        if (playerSlots[slotIndex].playerType != MenuPlayerType.AI)
            return;

        if (slotIndex == 3)
            return; // Player 4 is auto-assigned from the remaining bot

        List<BotIdentity> availableBots = GetAvailableBotChoicesForSlot(slotIndex);

        if (availableBots.Count <= 1)
            return;

        BotIdentity current = playerSlots[slotIndex].botIdentity;
        int currentIndex = availableBots.IndexOf(current);

        if (currentIndex < 0)
            currentIndex = 0;

        int nextIndex = currentIndex + (direction > 0 ? 1 : -1);

        if (nextIndex >= availableBots.Count)
            nextIndex = 0;
        else if (nextIndex < 0)
            nextIndex = availableBots.Count - 1;

        playerSlots[slotIndex].botIdentity = availableBots[nextIndex];
        playerSlots[slotIndex].playerName = GetBotDisplayName(playerSlots[slotIndex].botIdentity);

        SyncBotAssignments();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuClick();
    }

    public bool IsSlotInteractable(int slotIndex)
    {
        if (slotIndex == 0)
            return false;

        if (slotIndex == 3 && playerSlots[2].playerType == MenuPlayerType.Closed)
            return false;

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
                return playerType == MenuPlayerType.AI ? "Trikstan" : "Player 3";

            case 3:
                return playerType == MenuPlayerType.AI ? "Dr. Giggles MD" : "Player 4";
        }

        return "Closed";
    }

    private void ApplySlotDefaultsAfterTypeChange(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= playerSlots.Count)
            return;

        MenuPlayerSlot slot = playerSlots[slotIndex];

        if (slot.playerType == MenuPlayerType.Human)
        {
            slot.botIdentity = BotIdentity.None;
            slot.playerName = GetDefaultNameForSlot(slotIndex, MenuPlayerType.Human);
            return;
        }

        if (slot.playerType == MenuPlayerType.Closed)
        {
            slot.botIdentity = BotIdentity.None;
            slot.playerName = "Closed";
            return;
        }

        if (slot.playerType == MenuPlayerType.AI)
        {
            if (slotIndex == 1)
            {
                if (slot.botIdentity == BotIdentity.None)
                    slot.botIdentity = BotIdentity.Jerek;
            }
            else if (slotIndex == 2)
            {
                if (slot.botIdentity == BotIdentity.None)
                    slot.botIdentity = GetFirstUnusedBot(slotIndex);
            }
            else if (slotIndex == 3)
            {
                slot.botIdentity = GetRemainingBotForPlayer4();
            }

            slot.playerName = GetBotDisplayName(slot.botIdentity);
        }
    }

    private void SyncBotAssignments()
    {
        // Ensure Player 2 has a valid unique bot if AI
        if (playerSlots[1].playerType == MenuPlayerType.AI)
        {
            if (playerSlots[1].botIdentity == BotIdentity.None)
                playerSlots[1].botIdentity = BotIdentity.Jerek;

            playerSlots[1].playerName = GetBotDisplayName(playerSlots[1].botIdentity);
        }
        else
        {
            playerSlots[1].botIdentity = BotIdentity.None;
            playerSlots[1].playerName = "Player 2";
        }

        // Ensure Player 3 has a valid unique bot if AI
        if (playerSlots[2].playerType == MenuPlayerType.AI)
        {
            List<BotIdentity> p3Choices = GetAvailableBotChoicesForSlot(2);

            if (!p3Choices.Contains(playerSlots[2].botIdentity))
                playerSlots[2].botIdentity = p3Choices.Count > 0 ? p3Choices[0] : BotIdentity.None;

            playerSlots[2].playerName = GetBotDisplayName(playerSlots[2].botIdentity);
        }
        else if (playerSlots[2].playerType == MenuPlayerType.Human)
        {
            playerSlots[2].botIdentity = BotIdentity.None;
            playerSlots[2].playerName = "Player 3";
        }
        else
        {
            playerSlots[2].botIdentity = BotIdentity.None;
            playerSlots[2].playerName = "Closed";
        }

        // Player 4 auto-fills with remaining bot if AI
        if (playerSlots[3].playerType == MenuPlayerType.AI)
        {
            playerSlots[3].botIdentity = GetRemainingBotForPlayer4();
            playerSlots[3].playerName = GetBotDisplayName(playerSlots[3].botIdentity);
        }
        else if (playerSlots[3].playerType == MenuPlayerType.Human)
        {
            playerSlots[3].botIdentity = BotIdentity.None;
            playerSlots[3].playerName = "Player 4";
        }
        else
        {
            playerSlots[3].botIdentity = BotIdentity.None;
            playerSlots[3].playerName = "Closed";
        }

        // If Player 3 is closed, Player 4 must also be closed
        if (playerSlots[2].playerType == MenuPlayerType.Closed)
        {
            playerSlots[3].playerType = MenuPlayerType.Closed;
            playerSlots[3].botIdentity = BotIdentity.None;
            playerSlots[3].playerName = "Closed";
        }
    }

    private List<BotIdentity> GetAvailableBotChoicesForSlot(int slotIndex)
    {
        List<BotIdentity> allBots = GetAllBotIdentities();
        List<BotIdentity> result = new List<BotIdentity>();

        for (int i = 0; i < allBots.Count; i++)
        {
            BotIdentity candidate = allBots[i];

            bool usedByAnotherAI = false;

            for (int slot = 1; slot <= 3; slot++)
            {
                if (slot == slotIndex)
                    continue;

                if (slot >= playerSlots.Count)
                    continue;

                if (playerSlots[slot].playerType != MenuPlayerType.AI)
                    continue;

                if (playerSlots[slot].botIdentity == candidate)
                {
                    usedByAnotherAI = true;
                    break;
                }
            }

            if (!usedByAnotherAI)
                result.Add(candidate);
        }

        return result;
    }

    private BotIdentity GetFirstUnusedBot(int slotIndex)
    {
        List<BotIdentity> choices = GetAvailableBotChoicesForSlot(slotIndex);

        if (choices.Count > 0)
            return choices[0];

        return BotIdentity.None;
    }

    private BotIdentity GetRemainingBotForPlayer4()
    {
        List<BotIdentity> choices = GetAvailableBotChoicesForSlot(3);

        if (choices.Count > 0)
            return choices[0];

        return BotIdentity.None;
    }

    private List<BotIdentity> GetAllBotIdentities()
    {
        return new List<BotIdentity>
        {
            BotIdentity.Jerek,
            BotIdentity.Trikstan,
            BotIdentity.DrGigglesMD
        };
    }

    public string GetBotDisplayName(BotIdentity identity)
    {
        switch (identity)
        {
            case BotIdentity.Jerek:
                return "Jerek";

            case BotIdentity.Trikstan:
                return "Trikstan";

            case BotIdentity.DrGigglesMD:
                return "Dr. Giggles MD";

            default:
                return "Bot";
        }
    }
}