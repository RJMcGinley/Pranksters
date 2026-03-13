using System.Collections.Generic;

public class Player
{
    public List<PranksterType> hand = new List<PranksterType>();
    public List<PranksterType> favorArea = new List<PranksterType>();
    public List<PrankCard> completedPranks = new List<PrankCard>();
    public int favorPoints = 0;
    public int finalScore = 0;
}