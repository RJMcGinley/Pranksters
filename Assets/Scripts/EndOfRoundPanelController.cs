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

    private Action onStartNextRound;

    private void Awake()
{
    if (startNextRoundButton != null)
        startNextRoundButton.onClick.AddListener(OnStartNextRoundClicked);
}

    public void Show(string dealerName, string firstPlayerName, Action startNextRoundCallback)
    {
        Debug.Log("EndOfRoundPanelController.Show CALLED | panelRoot = " + 
          (panelRoot != null ? panelRoot.name : "NULL"));

        onStartNextRound = startNextRoundCallback;

        if (titleText != null)
            titleText.text = "Round Complete!";

        if (bodyText != null)
        {
            bodyText.text =
                "All prankster cards will be reshuffled.\n\n" +
                "Four new pranks will be dealt.\n\n" +
                dealerName + " will be the dealer.\n\n" +
                firstPlayerName + " will start the next round.";
        }

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

        callback?.Invoke();
    }
}