using System.Collections.Generic;

public class Player
{
    public List<PranksterDeckEntry> hand = new List<PranksterDeckEntry>();
    public List<PranksterType> favorArea = new List<PranksterType>();
    public List<PrankCard> completedPranks = new List<PrankCard>();
    public int favorPoints = 0;
    public int renownPoints = 0;
    public int finalScore = 0;
    public string playerName = "";

    public bool isBot = false;
}