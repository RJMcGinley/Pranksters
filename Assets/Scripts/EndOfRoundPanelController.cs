using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndOfRoundPanelController : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelRoot;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public Button startNextRoundButton;
    public Button chooseNextLocationButton;

    private Action onStartNextRound;
    private Action onChooseNextLocation;

    private void Awake()
    {
        if (startNextRoundButton != null)
            startNextRoundButton.onClick.AddListener(OnStartNextRoundClicked);

        if (chooseNextLocationButton != null)
            chooseNextLocationButton.onClick.AddListener(OnChooseNextLocationClicked);
    }

    public void Show(
    string dealerName,
    string firstPlayerName,
    string influenceWinnerText,
    bool playerChoosesLocation,
    Action startNextRoundCallback,
    Action chooseNextLocationCallback)
{
    Debug.Log("EndOfRoundPanelController.Show CALLED | panelRoot = " +
              (panelRoot != null ? panelRoot.name : "NULL"));

    onStartNextRound = startNextRoundCallback;
    onChooseNextLocation = chooseNextLocationCallback;

    if (titleText != null)
        titleText.text = "Round Complete!";

    if (bodyText != null)
    {
        string body =
            "All recruit cards will be reshuffled.\n" +
            "Four new pranks will be dealt.\n\n" +
            dealerName + " will be the dealer.\n" +
            firstPlayerName + " will start the next round.";

        if (!string.IsNullOrWhiteSpace(influenceWinnerText))
            body += "\n\n" + influenceWinnerText;

        bodyText.text = body;
    }

    if (startNextRoundButton != null)
        startNextRoundButton.gameObject.SetActive(!playerChoosesLocation);

    if (chooseNextLocationButton != null)
        chooseNextLocationButton.gameObject.SetActive(playerChoosesLocation);

    if (panelRoot != null)
    {
        panelRoot.SetActive(true);

        Debug.Log(
            "EndOfRoundPanelController.Show AFTER SetActive | " +
            "activeSelf = " + panelRoot.activeSelf +
            " | activeInHierarchy = " + panelRoot.activeInHierarchy +
            " | parent = " + (panelRoot.transform.parent != null ? panelRoot.transform.parent.name : "NO PARENT")
        );
    }
    else
    {
        Debug.LogError("EndOfRoundPanelController.Show FAILED: panelRoot is NULL.");
    }
}

    private void OnStartNextRoundClicked()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        Action callback = onStartNextRound;
        onStartNextRound = null;
        onChooseNextLocation = null;

        callback?.Invoke();
    }

    private void OnChooseNextLocationClicked()
    {
        Debug.Log("Choose Next Location clicked.");

        if (panelRoot != null)
            panelRoot.SetActive(false);

        Action callback = onChooseNextLocation;
        onChooseNextLocation = null;

        callback?.Invoke();
    }
}