using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockRevealPanelController : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panelRoot;
    public TMP_Text titleText;
    public TMP_Text bodyText;
    public Button acknowledgeButton;

    [Header("Reveal Card")]
    public RectTransform revealFrame;
    public GameObject revealCardPrefab;

    private readonly List<PranksterUnlockEntry> unlocksToShow = new List<PranksterUnlockEntry>();
    private int currentIndex = 0;
    private Action onFinished;

    private GameObject currentRevealCardInstance;
    private int lastPlayedRevealSoundIndex = -1;
    private bool isShowingUnlockSequence = false;

    public void ShowUnlocks(List<PranksterUnlockEntry> unlocks, Action finishedCallback)
    {
        unlocksToShow.Clear();

        if (unlocks != null)
            unlocksToShow.AddRange(unlocks);

        currentIndex = 0;
        lastPlayedRevealSoundIndex = -1;
        isShowingUnlockSequence = true;
        onFinished = finishedCallback;

        if (panelRoot != null)
            panelRoot.SetActive(true);

        if (acknowledgeButton != null)
        {
            acknowledgeButton.interactable = true;
            acknowledgeButton.onClick.RemoveAllListeners();
            acknowledgeButton.onClick.AddListener(OnAcknowledgeClicked);
        }

        ShowCurrentUnlock();
    }

    public void OnAcknowledgeClicked()
{
    Debug.Log("ACK CLICK | currentIndex before advance = " + currentIndex + " | total unlocks = " + unlocksToShow.Count);

    // Move past the currently displayed unlock
    currentIndex++;

    Debug.Log("ACK CLICK | currentIndex after advance = " + currentIndex + " | total unlocks = " + unlocksToShow.Count);

    // If another unlock exists, show it and let ShowCurrentUnlock play the audio once
    if (currentIndex < unlocksToShow.Count)
    {
        Debug.Log("ACK CLICK | another unlock exists, showing next unlock");
        ShowCurrentUnlock();
        return;
    }

    // No more unlocks remain, so finish without replaying unlock audio
    Debug.Log("ACK CLICK | no more unlocks, closing panel and returning to main menu");

    isShowingUnlockSequence = false;

    ClearCurrentRevealCard();

    if (panelRoot != null)
    {
        panelRoot.SetActive(false);
        Debug.Log("ACK CLICK | panelRoot set false");
    }

    if (onFinished != null)
    {
        Debug.Log("ACK CLICK | invoking onFinished");
        onFinished.Invoke();
    }
    else
    {
        Debug.LogWarning("ACK CLICK | onFinished is NULL");
    }
}

    private void ShowCurrentUnlock()
{
    if (unlocksToShow == null || unlocksToShow.Count == 0)
    {
        Debug.LogWarning("UnlockRevealPanelController.ShowCurrentUnlock called with no unlocks.");
        return;
    }

    if (currentIndex < 0 || currentIndex >= unlocksToShow.Count)
    {
        Debug.LogWarning("UnlockRevealPanelController.ShowCurrentUnlock currentIndex out of range: " + currentIndex);
        return;
    }

    PranksterUnlockEntry entry = unlocksToShow[currentIndex];

    string unlockTitle;
    string flavorText;

    if (entry.category == PranksterUnlockCategory.PrankCompletion)
    {
        unlockTitle = PranksterSpriteDatabase.GetTierTitle(entry.tier);
        flavorText = PranksterSpriteDatabase.GetTierFlavorText(entry.tier);
    }
    else if (entry.category == PranksterUnlockCategory.FavorOffer)
    {
        unlockTitle = PranksterSpriteDatabase.GetFavorTierTitle(entry.tier);
        flavorText = PranksterSpriteDatabase.GetFavorTierFlavorText(entry.tier);
    }
    else
    {
        unlockTitle = PranksterSpriteDatabase.GetDiscardTierTitle(entry.tier);
        flavorText = PranksterSpriteDatabase.GetDiscardTierFlavorText(entry.tier);
    }

    Sprite unlockSprite;
    string pranksterName = entry.pranksterType == "BeastMaster" ? "Beastmaster" : entry.pranksterType;

    if (entry.category == PranksterUnlockCategory.FavorOffer)
    {
        string suffix = "";

        switch (entry.tier)
        {
            case 1: suffix = "Assistant"; break;
            case 2: suffix = "Strategist"; break;
            case 3: suffix = "Advisor"; break;
            default: suffix = ""; break;
        }

        unlockSprite = Resources.Load<Sprite>("UnlockCards/" + pranksterName + suffix);

        if (unlockSprite == null)
        {
            Debug.LogWarning("Missing favor unlock sprite: UnlockCards/" + pranksterName + suffix);
            unlockSprite = PranksterSpriteDatabase.GetSprite(entry.pranksterType, 0);
        }
    }
    else if (entry.category == PranksterUnlockCategory.Discard)
    {
        string suffix = "";

        switch (entry.tier)
        {
            case 1: suffix = "Hustler"; break;
            case 2: suffix = "Opportunist"; break;
            case 3: suffix = "Manipulator"; break;
            default: suffix = ""; break;
        }

        unlockSprite = Resources.Load<Sprite>("UnlockCards/" + pranksterName + suffix);

        if (unlockSprite == null)
        {
            Debug.LogWarning("Missing discard unlock sprite: UnlockCards/" + pranksterName + suffix);
            unlockSprite = PranksterSpriteDatabase.GetSprite(entry.pranksterType, 0);
        }
    }
    else
    {
        unlockSprite = PranksterSpriteDatabase.GetSprite(entry.pranksterType, entry.tier);
    }

    if (titleText != null)
        titleText.text = unlockTitle;

    if (bodyText != null)
        bodyText.text = flavorText;

    SpawnRevealCard(unlockSprite);

    if (isShowingUnlockSequence && lastPlayedRevealSoundIndex != currentIndex)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUnlockReveal();

        lastPlayedRevealSoundIndex = currentIndex;
    }

    Debug.Log("Showing unlock | pranksterType = " + entry.pranksterType +
              " | tier = " + entry.tier +
              " | category = " + entry.category +
              " | title = " + unlockTitle);
}

    private void SpawnRevealCard(Sprite unlockSprite)
    {
        ClearCurrentRevealCard();

        if (revealFrame == null)
        {
            Debug.LogWarning("UnlockRevealPanelController: revealFrame is not assigned.");
            return;
        }

        if (revealCardPrefab == null)
        {
            Debug.LogWarning("UnlockRevealPanelController: revealCardPrefab is not assigned.");
            return;
        }

        currentRevealCardInstance = Instantiate(revealCardPrefab, revealFrame);

        RectTransform instanceRect = currentRevealCardInstance.GetComponent<RectTransform>();
        if (instanceRect != null)
        {
            instanceRect.anchorMin = new Vector2(0.5f, 0.5f);
            instanceRect.anchorMax = new Vector2(0.5f, 0.5f);
            instanceRect.pivot = new Vector2(0.5f, 0.5f);
            instanceRect.anchoredPosition = Vector2.zero;
            instanceRect.localScale = Vector3.one;
            instanceRect.localRotation = Quaternion.identity;
            instanceRect.sizeDelta = new Vector2(100f, 140f);
        }

        RevealCardUI revealCardUI = currentRevealCardInstance.GetComponent<RevealCardUI>();
        if (revealCardUI != null)
        {
            revealCardUI.SetSprite(unlockSprite);
        }
        else
        {
            Debug.LogWarning("UnlockRevealPanelController: RevealCardUI not found on revealCardPrefab instance.");
        }
    }

    private void ClearCurrentRevealCard()
    {
        if (currentRevealCardInstance != null)
        {
            Destroy(currentRevealCardInstance);
            currentRevealCardInstance = null;
        }
    }
}