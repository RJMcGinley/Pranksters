using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public static TutorialController Instance { get; private set; }

    [Header("Tutorial Objects")]
    [SerializeField] private GameObject tutorialRoot;
    [SerializeField] private GameObject scoutTutorial;
    [SerializeField] private GameObject endTurnTutorial;
    [SerializeField] private GameObject turnTwoTutorial;
    [SerializeField] private GameObject barnabyHideoutTutorial;
    [SerializeField] private GameObject availableServicesTutorial;
    [SerializeField] private GameObject playerHideoutTutorial;
    [SerializeField] private GameObject prankCardTutorial;
    [SerializeField] private GameObject wantedBoardTutorial;
    [SerializeField] private GameObject jailAndJailBreakTutorial;
    [SerializeField] private GameObject betweenRoundsTutorial;

    [Header("Highlight Suppression")]
    [SerializeField] private GameObject discardPileHighlight;
    [SerializeField] private GameObject drawPileHighlight;
    [SerializeField] private GameObject favorAreaHighlight;
    [SerializeField] private HideoutController playerHideoutController;
    [SerializeField] private GameObject availableServices2Tutorial;


    private bool discardPileHighlightWasActive;
    private bool discardPileHighlightIsSuppressed;
    private bool drawPileHighlightWasActive;
    private bool drawPileHighlightIsSuppressed;
    private bool favorAreaHighlightWasActive;
    private bool favorAreaHighlightIsSuppressed;
    private bool hideoutHighlightsAreSuppressed;
    

    private bool isTutorialOpen;
    private System.Action onTutorialClosed;

    public bool IsTutorialOpen => isTutorialOpen;
    private bool hasShownEndTurnTutorial;
    private bool hasShownTurnTwoTutorial;
    private int humanTurnsStarted;
    private bool hasShownBarnabyHideoutTutorial;
    private bool hasShownAvailableServicesTutorial;
    private bool hasShownAvailableServices2Tutorial;
    private bool hasShownPlayerHideoutTutorial;
    private bool hasShownPrankCardTutorial;
    private bool hasShownWantedBoardTutorial;
    private bool hasShownJailAndJailBreakTutorial;
    private bool hasShownBetweenRoundsTutorial;
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError(
                "More than one TutorialController exists in the scene.",
                gameObject);

            Destroy(gameObject);
            return;
        }

        Instance = this;

        ValidateReferences();

        if (tutorialRoot != null)
            tutorialRoot.SetActive(false);
    }

    /// Tutorials

    public void ShowScoutTutorial()
{
    if (isTutorialOpen)
        return;

    if (tutorialRoot == null || scoutTutorial == null)
    {
        Debug.LogError(
            "The Scout tutorial cannot be displayed because one or more references are missing.",
            gameObject);

        return;
    }

    isTutorialOpen = true;

    SuppressFavorAreaHighlight();

    scoutTutorial.SetActive(true);
    tutorialRoot.SetActive(true);
}

    public void ShowEndTurnTutorial()
{
    if (hasShownEndTurnTutorial || isTutorialOpen)
        return;

    if (tutorialRoot == null || endTurnTutorial == null)
    {
        Debug.LogError(
            "The End Turn tutorial cannot be displayed because one or more references are missing.",
            gameObject);

        return;
    }

    hasShownEndTurnTutorial = true;
    isTutorialOpen = true;

    endTurnTutorial.SetActive(true);
    tutorialRoot.SetActive(true);
}

public void ShowTurnTwoTutorial()
{
    if (hasShownTurnTwoTutorial || isTutorialOpen)
        return;

    if (tutorialRoot == null || turnTwoTutorial == null)
    {
        Debug.LogError(
            "The Turn Two tutorial cannot be displayed because one or more references are missing.",
            gameObject);

        return;
    }

    hasShownTurnTwoTutorial = true;
    isTutorialOpen = true;

    turnTwoTutorial.SetActive(true);
    tutorialRoot.SetActive(true);
}

public void ShowBarnabyHideoutTutorial()
{
    if (hasShownBarnabyHideoutTutorial || isTutorialOpen)
        return;

    if (tutorialRoot == null || barnabyHideoutTutorial == null)
    {
        Debug.LogError(
            "The Barnaby Hideout tutorial cannot be displayed because one or more references are missing.",
            gameObject);

        return;
    }

    hasShownBarnabyHideoutTutorial = true;
    isTutorialOpen = true;

    barnabyHideoutTutorial.SetActive(true);
    tutorialRoot.SetActive(true);
}

public bool ShowAvailableServicesTutorial(
    System.Action closedCallback = null)
{
    if (hasShownAvailableServicesTutorial || isTutorialOpen)
        return false;

    if (tutorialRoot == null || availableServicesTutorial == null)
    {
        Debug.LogError(
            "The Available Services tutorial cannot be displayed because one or more references are missing.",
            gameObject);

        return false;
    }

    hasShownAvailableServicesTutorial = true;
    isTutorialOpen = true;
    onTutorialClosed = closedCallback;

    SuppressDiscardPileHighlight();
    SuppressDrawPileHighlight();
    SuppressPlayerHideoutHighlights();

    availableServicesTutorial.SetActive(true);
    tutorialRoot.SetActive(true);

    return true;
}

public void ShowAvailableServices2Tutorial()
{
    if (hasShownAvailableServices2Tutorial || isTutorialOpen)
        return;

    if (tutorialRoot == null || availableServices2Tutorial == null)
    {
        Debug.LogError(
            "The Available Services 2 tutorial cannot be displayed because one or more references are missing.",
            gameObject);

        return;
    }

    hasShownAvailableServices2Tutorial = true;
    isTutorialOpen = true;

    availableServices2Tutorial.SetActive(true);
    tutorialRoot.SetActive(true);
}

public bool ShowPlayerHideoutTutorial()
{
    if (hasShownPlayerHideoutTutorial || isTutorialOpen)
        return false;

    if (tutorialRoot == null || playerHideoutTutorial == null)
    {
        Debug.LogError(
            "The Player Hideout tutorial cannot be displayed because one or more references are missing.",
            gameObject);

        return false;
    }

    hasShownPlayerHideoutTutorial = true;
    isTutorialOpen = true;

    SuppressDiscardPileHighlight();

    playerHideoutTutorial.SetActive(true);
    tutorialRoot.SetActive(true);

    return true;
}

public bool ShowPrankCardTutorial()
{
    if (hasShownPrankCardTutorial || isTutorialOpen)
        return false;

    if (tutorialRoot == null || prankCardTutorial == null)
    {
        Debug.LogError(
            "The Prank Card tutorial cannot be displayed because one or more references are missing.",
            gameObject);

        return false;
    }

    hasShownPrankCardTutorial = true;
    isTutorialOpen = true;

    prankCardTutorial.SetActive(true);
    tutorialRoot.SetActive(true);

    return true;
}

public bool ShowWantedBoardTutorial()
{
    if (hasShownWantedBoardTutorial || isTutorialOpen)
        return false;

    if (tutorialRoot == null || wantedBoardTutorial == null)
    {
        Debug.LogError(
            "The Wanted Board tutorial cannot be displayed because one or more references are missing.",
            gameObject);

        return false;
    }

    hasShownWantedBoardTutorial = true;
    isTutorialOpen = true;

    wantedBoardTutorial.SetActive(true);
    tutorialRoot.SetActive(true);

    return true;
}

public bool ShowJailAndJailBreakTutorial()
{
    if (hasShownJailAndJailBreakTutorial || isTutorialOpen)
        return false;

    if (tutorialRoot == null || jailAndJailBreakTutorial == null)
    {
        Debug.LogError(
            "The Jail and Jailbreak tutorial cannot be displayed because one or more references are missing.",
            gameObject);

        return false;
    }

    hasShownJailAndJailBreakTutorial = true;
    isTutorialOpen = true;

    jailAndJailBreakTutorial.SetActive(true);
    tutorialRoot.SetActive(true);

    return true;
}

public bool ShowBetweenRoundsTutorial()
{
    if (hasShownBetweenRoundsTutorial || isTutorialOpen)
        return false;

    if (tutorialRoot == null || betweenRoundsTutorial == null)
    {
        Debug.LogError(
            "The Between Rounds tutorial cannot be displayed because one or more references are missing.",
            gameObject);

        return false;
    }

    hasShownBetweenRoundsTutorial = true;
    isTutorialOpen = true;

    betweenRoundsTutorial.SetActive(true);
    tutorialRoot.SetActive(true);

    return true;
}

///Controls and Validation

   public void CloseCurrentTutorial()
{
    if (!isTutorialOpen)
        return;

    if (AudioManager.Instance != null)
        AudioManager.Instance.PlayUIClick();

    if (scoutTutorial != null)
        scoutTutorial.SetActive(false);

    if (endTurnTutorial != null)
        endTurnTutorial.SetActive(false);

    if (turnTwoTutorial != null)
        turnTwoTutorial.SetActive(false);

    if (barnabyHideoutTutorial != null)
        barnabyHideoutTutorial.SetActive(false);

    if (availableServicesTutorial != null)
        availableServicesTutorial.SetActive(false);

    if (playerHideoutTutorial != null)
        playerHideoutTutorial.SetActive(false);

    if (availableServices2Tutorial != null)
        availableServices2Tutorial.SetActive(false);

    if (prankCardTutorial != null)
        prankCardTutorial.SetActive(false);

    if (wantedBoardTutorial != null)
        wantedBoardTutorial.SetActive(false);

    if (jailAndJailBreakTutorial != null)
        jailAndJailBreakTutorial.SetActive(false);

    if (betweenRoundsTutorial != null)
        betweenRoundsTutorial.SetActive(false);

    if (tutorialRoot != null)
        tutorialRoot.SetActive(false);

    RestoreDiscardPileHighlight();
    RestoreDrawPileHighlight();
    RestorePlayerHideoutHighlights();
    RestoreFavorAreaHighlight();

    isTutorialOpen = false;

    System.Action closedCallback = onTutorialClosed;
    onTutorialClosed = null;

    closedCallback?.Invoke();
}

private void ValidateReferences()
{
    if (tutorialRoot == null)
    {
        Debug.LogError(
            "Tutorial Root has not been assigned.",
            gameObject);
    }

    if (scoutTutorial == null)
    {
        Debug.LogError(
            "Scout Tutorial has not been assigned.",
            gameObject);
    }

    if (endTurnTutorial == null)
    {
        Debug.LogError(
            "End Turn Tutorial has not been assigned.",
            gameObject);
    }

    if (turnTwoTutorial == null)
    {
        Debug.LogError(
            "Turn Two Tutorial has not been assigned.",
            gameObject);
    }

    if (barnabyHideoutTutorial == null)
    {
        Debug.LogError(
            "Barnaby Hideout Tutorial has not been assigned.",
            gameObject);
    }

    if (availableServicesTutorial == null)
    {
        Debug.LogError(
            "Available Services Tutorial has not been assigned.",
            gameObject);
    }

    if (discardPileHighlight == null)
    {
        Debug.LogWarning(
            "Discard Pile Highlight has not been assigned.",
            gameObject);
    }

    if (drawPileHighlight == null)
    {
        Debug.LogWarning(
            "Draw Pile Highlight has not been assigned.",
            gameObject);
    }

    if (playerHideoutTutorial == null)
        Debug.LogError("Player Hideout Tutorial reference is missing.", gameObject);

    if (prankCardTutorial == null)
        Debug.LogError("Prank Card Tutorial reference is missing.", gameObject);

    if (wantedBoardTutorial == null)
        Debug.LogError("Wanted Board Tutorial reference is missing.", gameObject);

    if (jailAndJailBreakTutorial == null)
        Debug.LogError("Jail And Jail Break Tutorial reference is missing.", gameObject);

    if (betweenRoundsTutorial == null)
    {
        Debug.LogError(
            "Between Rounds Tutorial reference is missing.",
            gameObject);
    }
}

public void NotifyHumanTurnStarted()
{
    humanTurnsStarted++;

    Debug.Log("Human turns started: " + humanTurnsStarted);

    if (humanTurnsStarted == 1)
        ShowScoutTutorial();
    else if (humanTurnsStarted == 2)
        ShowTurnTwoTutorial();
}

///Highlight Suppression

private void SuppressDrawPileHighlight()
{
    if (drawPileHighlight == null)
        return;

    drawPileHighlightWasActive = drawPileHighlight.activeSelf;
    drawPileHighlightIsSuppressed = true;

    drawPileHighlight.SetActive(false);
}

private void RestoreDrawPileHighlight()
{
    if (!drawPileHighlightIsSuppressed)
        return;

    if (drawPileHighlight != null)
        drawPileHighlight.SetActive(drawPileHighlightWasActive);

    drawPileHighlightIsSuppressed = false;
}

private void SuppressDiscardPileHighlight()
{
    if (discardPileHighlight == null)
        return;

    discardPileHighlightWasActive = discardPileHighlight.activeSelf;
    discardPileHighlightIsSuppressed = true;

    discardPileHighlight.SetActive(false);
}

private void RestoreDiscardPileHighlight()
{
    if (!discardPileHighlightIsSuppressed)
        return;

    if (discardPileHighlight != null)
        discardPileHighlight.SetActive(discardPileHighlightWasActive);

    discardPileHighlightIsSuppressed = false;
}

private void SuppressFavorAreaHighlight()
{
    if (favorAreaHighlight == null)
        return;

    favorAreaHighlightWasActive = favorAreaHighlight.activeSelf;
    favorAreaHighlightIsSuppressed = true;

    favorAreaHighlight.SetActive(false);
}

private void RestoreFavorAreaHighlight()
{
    if (!favorAreaHighlightIsSuppressed)
        return;

    if (favorAreaHighlight != null)
        favorAreaHighlight.SetActive(favorAreaHighlightWasActive);

    favorAreaHighlightIsSuppressed = false;
}

private void SuppressPlayerHideoutHighlights()
{
    if (playerHideoutController == null)
        return;

    hideoutHighlightsAreSuppressed = true;
    playerHideoutController.SetAllSlotHighlightsVisible(false);
}

private void RestorePlayerHideoutHighlights()
{
    if (!hideoutHighlightsAreSuppressed)
        return;

    if (playerHideoutController != null)
        playerHideoutController.RefreshSlotHighlights();

    hideoutHighlightsAreSuppressed = false;
}
}