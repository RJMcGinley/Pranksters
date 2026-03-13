using System.Collections.Generic;

public static class PrankDatabase
{
    public static List<PrankCard> CreatePrankDeck()
    {
        return new List<PrankCard>
        {
            new PrankCard
            {
                title = "Cabbage Cash",
                requiredPranksters = new List<PranksterType> { PranksterType.Scribe, PranksterType.Scribe, PranksterType.Laborer, PranksterType.Laborer },
                renownPoints = 10,
                favorMultiplier = 1
            },

            new PrankCard
            {
                title = "False Fruit",
                requiredPranksters = new List<PranksterType> { PranksterType.Wizard, PranksterType.Wizard, PranksterType.Laborer, PranksterType.Laborer },
                renownPoints = 10,
                favorMultiplier = 1
            },

            new PrankCard
            {
                title = "Floating Footwear",
                requiredPranksters = new List<PranksterType> { PranksterType.Thief, PranksterType.Thief, PranksterType.Thief, PranksterType.Wizard },
                renownPoints = 12,
                favorMultiplier = 1
            },

            new PrankCard
            {
                title = "Foaming Fountain",
                requiredPranksters = new List<PranksterType> { PranksterType.Engineer, PranksterType.Engineer, PranksterType.Engineer, PranksterType.Laborer },
                renownPoints = 12,
                favorMultiplier = 1
            },

            new PrankCard
            {
                title = "Fowl Play Exchange",
                requiredPranksters = new List<PranksterType> { PranksterType.BeastMaster, PranksterType.BeastMaster, PranksterType.BeastMaster, PranksterType.BeastMaster },
                renownPoints = 15,
                favorMultiplier = 0
            },

            new PrankCard
            {
                title = "Grave Greetings",
                requiredPranksters = new List<PranksterType> { PranksterType.Engineer, PranksterType.Engineer, PranksterType.Scribe, PranksterType.Scribe },
                renownPoints = 10,
                favorMultiplier = 1
            },

            new PrankCard
            {
                title = "Grim Grafitti",
                requiredPranksters = new List<PranksterType> { PranksterType.Scribe, PranksterType.Scribe, PranksterType.Scribe, PranksterType.Thief },
                renownPoints = 12,
                favorMultiplier = 1
            },

            new PrankCard
            {
                title = "Guano-teed Delivery",
                requiredPranksters = new List<PranksterType> { PranksterType.Wizard, PranksterType.Thief, PranksterType.BeastMaster, PranksterType.Scribe },
                renownPoints = 8,
                favorMultiplier = 2
            },

            new PrankCard
            {
                title = "Headless Hounds",
                requiredPranksters = new List<PranksterType> { PranksterType.Wizard, PranksterType.Wizard, PranksterType.Wizard, PranksterType.BeastMaster },
                renownPoints = 12,
                favorMultiplier = 1
            },

            new PrankCard
            {
                title = "Ink Stink",
                requiredPranksters = new List<PranksterType> { PranksterType.BeastMaster, PranksterType.BeastMaster, PranksterType.BeastMaster, PranksterType.Thief },
                renownPoints = 12,
                favorMultiplier = 1
            },

            new PrankCard
            {
                title = "Manure Mischief",
                requiredPranksters = new List<PranksterType> { PranksterType.Engineer, PranksterType.BeastMaster, PranksterType.Laborer, PranksterType.Thief },
                renownPoints = 8,
                favorMultiplier = 1
            },

            new PrankCard
            {
                title = "Mayor's Moat",
                requiredPranksters = new List<PranksterType> { PranksterType.BeastMaster, PranksterType.Engineer, PranksterType.Wizard, PranksterType.Scribe },
                renownPoints = 8,
                favorMultiplier = 2
            },

            new PrankCard
            {
                title = "Morning Surprise",
                requiredPranksters = new List<PranksterType> { PranksterType.Thief, PranksterType.Thief, PranksterType.BeastMaster, PranksterType.BeastMaster },
                renownPoints = 10,
                favorMultiplier = 1
            },

            new PrankCard
            {
                title = "Municipal Paper Shuffle",
                requiredPranksters = new List<PranksterType> { PranksterType.Scribe, PranksterType.Scribe, PranksterType.Scribe, PranksterType.Scribe },
                renownPoints = 15,
                favorMultiplier = 0
            },

            new PrankCard
            {
                title = "Operation Featherfall",
                requiredPranksters = new List<PranksterType> { PranksterType.BeastMaster, PranksterType.BeastMaster, PranksterType.Engineer, PranksterType.Engineer },
                renownPoints = 10,
                favorMultiplier = 1
            },

            new PrankCard
            {
                title = "Privies on the Porch",
                requiredPranksters = new List<PranksterType> { PranksterType.Laborer, PranksterType.Laborer, PranksterType.Laborer, PranksterType.Laborer },
                renownPoints = 15,
                favorMultiplier = 0
            },

            new PrankCard
            {
                title = "Ram Stampede",
                requiredPranksters = new List<PranksterType> { PranksterType.BeastMaster, PranksterType.Engineer, PranksterType.Laborer, PranksterType.Scribe },
                renownPoints = 8,
                favorMultiplier = 2
            },

            new PrankCard
            {
                title = "Sinus Scorcher",
                requiredPranksters = new List<PranksterType> { PranksterType.Thief, PranksterType.Wizard, PranksterType.Engineer, PranksterType.Laborer },
                renownPoints = 8,
                favorMultiplier = 1
            },

            new PrankCard
            {
                title = "Soiled Spring",
                requiredPranksters = new List<PranksterType> { PranksterType.Thief, PranksterType.Laborer, PranksterType.Scribe, PranksterType.Wizard },
                renownPoints = 8,
                favorMultiplier = 2
            },

            new PrankCard
            {
                title = "Statue Swap",
                requiredPranksters = new List<PranksterType> { PranksterType.Engineer, PranksterType.Engineer, PranksterType.Engineer, PranksterType.Engineer },
                renownPoints = 15,
                favorMultiplier = 0
            },

            new PrankCard
            {
                title = "The Angry Moon",
                requiredPranksters = new List<PranksterType> { PranksterType.Wizard, PranksterType.Wizard, PranksterType.Wizard, PranksterType.Wizard },
                renownPoints = 15,
                favorMultiplier = 0
            },

            new PrankCard
            {
                title = "The Shocking Toll",
                requiredPranksters = new List<PranksterType> { PranksterType.Thief, PranksterType.Thief, PranksterType.Wizard, PranksterType.Wizard },
                renownPoints = 10,
                favorMultiplier = 1
            },

            new PrankCard
            {
                title = "The Tilted Tumble",
                requiredPranksters = new List<PranksterType> { PranksterType.Laborer, PranksterType.Laborer, PranksterType.Laborer, PranksterType.Engineer },
                renownPoints = 12,
                favorMultiplier = 1
            },

            new PrankCard
            {
                title = "Wooden Nickels",
                requiredPranksters = new List<PranksterType> { PranksterType.Thief, PranksterType.Thief, PranksterType.Thief, PranksterType.Thief },
                renownPoints = 15,
                favorMultiplier = 0
            }
        };
    }
}