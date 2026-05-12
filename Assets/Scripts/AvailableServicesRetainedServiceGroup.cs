using System;
using System.Collections.Generic;

[Serializable]
public class AvailableServicesRetainedServiceGroup
{
    public PranksterType serviceType;

    public List<PranksterDeckEntry> assignedCards =
        new List<PranksterDeckEntry>();
}