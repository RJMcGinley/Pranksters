using System.Collections.Generic;

public class Player
{
    public List<PranksterDeckEntry> hand = new List<PranksterDeckEntry>();
    public List<PranksterDeckEntry> favorArea = new List<PranksterDeckEntry>();
    public List<PrankCard> completedPranks = new List<PrankCard>();
    public int favorPoints = 0;
    public int renownPoints = 0;
    public int finalScore = 0;
    public string playerName = "";
    public List<AvailableServicesRetainedServiceGroup> retainedServices = new List<AvailableServicesRetainedServiceGroup>();
    public List<PranksterType> activeScoringServiceTypes = new List<PranksterType>();
    public List<PranksterType> activeOngoingServiceTypes = new List<PranksterType>();
    public int lifetimeNotorietyMaxHandSizeBonus = 0;
    public bool lifetimeNotorietyCrewUpgradeUsedThisGame = false;

    public int maxHandSize = 4;

    public bool isBot = false;
}