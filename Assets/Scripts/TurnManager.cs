using UnityEngine;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public List<Player> players = new List<Player>();
    public int currentPlayerIndex = 0;

    void Awake()
{
}

    public void StartGame(int numberOfPlayers)
    {
        players.Clear();

        for (int i = 0; i < numberOfPlayers; i++)
        {
            players.Add(new Player());
        }

        currentPlayerIndex = 0;

        Debug.Log("Game started with " + numberOfPlayers + " players.");
    }

    public Player GetCurrentPlayer()
    {
        return players[currentPlayerIndex];
    }

    public void NextPlayer()
    {
        currentPlayerIndex++;

        if (currentPlayerIndex >= players.Count)
        {
            currentPlayerIndex = 0;
        }

        Debug.Log("It is now Player " + (currentPlayerIndex + 1) + "'s turn.");
    }
}