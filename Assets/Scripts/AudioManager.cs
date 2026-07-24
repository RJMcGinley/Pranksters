using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PrankCompletionAudioEntry
{
    public string prankTitle;

    [Range(0f, 1f)]
    public float completePrankVolume = 1f;

    public AudioClip[] clips;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("SFX Source")]
    public AudioSource sfxSource;

    [Header("Music Source")]
    public AudioSource musicSource;

    [Header("Music Clip")]
    public AudioClip backgroundMusicClip;
    public AudioClip rebelWorkshopMusic;
    public AudioClip forestClearingMusic;
    public AudioClip sewerHideoutMusic;
    public AudioClip outsideTheWallsMusic;
    public AudioClip treetopHideoutMusic;

    [Header("Current Clips")]
    [SerializeField] private AudioClip invalidSelectionClip;
    public AudioClip drawDeckHoverClip;
    public AudioClip drawCardActionClip;
    public AudioClip discardPileHoverClip;
    public AudioClip discardCardClip;
    public AudioClip player1TurnClip;
    public AudioClip player2TurnClip;
    public AudioClip player3TurnClip;
    public AudioClip player4TurnClip;
    public AudioClip[] hmmmDecisionsClips;
    public AudioClip[] favorVoiceClips;
    public AudioClip[] swapCompleteVoiceClips;
    private int lastSwapCompleteVoiceIndex = -1;
    public AudioClip readyButtonClip;
    public AudioClip favorHoverClip;
    public AudioClip favorClickClip;
    public AudioClip favorRewardClip;
    public AudioClip cancelActionClip;
    public AudioClip completePrankBannerDropClip;
    public AudioClip completePrankClip; 
    [SerializeField] private AudioClip notAnOptionClip;
    public AudioClip chooseRecruitReturnVoice;
    public AudioClip chooseAPrankToGetRidOfClip;
    public AudioClip chooseOpponentToPoachRecruitFromClip;
    public AudioClip chooseOpponentToGatherTheirReferralsClip;
    public AudioClip houndsCouldntTrackThemDownClip;
    public AudioClip chooseARecruitForTheDogsToTrackDownClip;
    public AudioClip thatPlayerDoesntHaveAnyRecruitsClip;
    public AudioClip yourOpponentDoesntHaveAnyRecruitsClip;
    public AudioClip yourOpponentsDontHaveAnyRecruitsClip;
    public AudioClip availableServiceScoringActionClip;
    public AudioClip spendInfluenceClip;
    public AudioClip jailDoorClosingClip;
    [SerializeField] private AudioClip cardLandingOnTable;
    [SerializeField] private AudioClip rulesPageTurn;

    [Header("Fart Sounds")]
    public AudioClip[] fartSounds;
    public bool fartSoundsEnabled = true;

    [Header("UI Click Sounds")]
    public AudioClip uiClickClip;
    public AudioClip menuClickClip;
    public AudioClip confirmClickClip;
    public AudioClip backClickClip;
    
    [Header("Unlock / Reveal")]
    public AudioClip unlockRevealClip;

    [Header("Prank Completion Sounds")]
    public PrankCompletionAudioEntry[] prankCompletionAudioEntries;

    [Header("Pester The Mayor Sound Clips")]
    [SerializeField] private AudioClip[] pesterMayorEarlyClips;
    [SerializeField] private AudioClip[] pesterMayorMiddleClips;
    [SerializeField] private AudioClip[] pesterMayorLateClips;

    public AudioClip mayorsGrowingFrustrationClip;

    

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayLocationMusic(GameLocationType.RebelWorkshop);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic()
    {
        if (musicSource == null || backgroundMusicClip == null) return;

        musicSource.clip = backgroundMusicClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
    if (musicSource == null) return;

    musicSource.volume = volume;

    Debug.Log("SetMusicVolume | volume=" + volume +
              " | isPlaying=" + musicSource.isPlaying +
              " | clip=" + (musicSource.clip != null ? musicSource.clip.name : "null"));

    if (volume > 0.001f && !musicSource.isPlaying)
    {
        if (musicSource.clip == null && backgroundMusicClip != null)
            musicSource.clip = backgroundMusicClip;

        musicSource.loop = true;
        musicSource.Play();
    }
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource == null) return;
        sfxSource.volume = volume;
    }

    public void SetFartSoundsEnabled(bool enabled)
    {
        fartSoundsEnabled = enabled;
    }

    public void PlayDrawDeckHover()
    {
        PlaySFX(drawDeckHoverClip);
    }

    public void PlayDrawCardAction()
    {
        PlaySFX(drawCardActionClip);
    }

    private int lastHmmDecisionsIndex = -1;

    public void PlayHmmDecisions()
    {
    if (sfxSource == null || hmmmDecisionsClips == null || hmmmDecisionsClips.Length == 0)
        return;

    int index;

    if (hmmmDecisionsClips.Length == 1)
    {
        index = 0;
    }
    else
    {
        do
        {
            index = Random.Range(0, hmmmDecisionsClips.Length);
        }
        while (index == lastHmmDecisionsIndex);
    }

    lastHmmDecisionsIndex = index;
    sfxSource.PlayOneShot(hmmmDecisionsClips[index]);
    }

    public void PlayDiscardPileHover()
    {
        PlaySFX(discardPileHoverClip);
    }

    public void PlayRandomFart()
    {
        if (!fartSoundsEnabled) return;
        if (sfxSource == null || fartSounds == null || fartSounds.Length == 0) return;

        int index = Random.Range(0, fartSounds.Length);
        sfxSource.PlayOneShot(fartSounds[index]);
    }

    [Header("Bot Turn Voice Clips")]
    public AudioClip jerekTurnClip;
    public AudioClip trikstanTurnClip;
    public AudioClip drGigglesTurnClip;

public void PlayPlayerTurnVoice(string playerName, int playerIndex, bool isBot)
{
    string name = "";

    if (!string.IsNullOrEmpty(playerName))
        name = playerName.Trim().ToLower();

    Debug.Log("TURN VOICE | name=" + playerName + " | normalized=" + name + " | isBot=" + isBot);

    if (isBot)
    {
        if (name.Contains("jerek") && jerekTurnClip != null)
        {
            PlaySFX(jerekTurnClip);
            return;
        }

        if (name.Contains("trikstan") && trikstanTurnClip != null)
        {
            PlaySFX(trikstanTurnClip);
            return;
        }

        if (name.Contains("giggles") && drGigglesTurnClip != null)
        {
            PlaySFX(drGigglesTurnClip);
            return;
        }
    }

    switch (playerIndex)
    {
        case 0:
            PlaySFX(player1TurnClip);
            break;
        case 1:
            PlaySFX(player2TurnClip);
            break;
        case 2:
            PlaySFX(player3TurnClip);
            break;
        case 3:
            PlaySFX(player4TurnClip);
            break;
    }
}

    public void PlayDiscardCard()
    {
        PlaySFX(discardCardClip);
    }

    public void PlayReadyButton()
    {
        PlaySFX(readyButtonClip);
    }

    public void PlayFavorHover()
    {
        if (sfxSource != null && favorHoverClip != null)
            sfxSource.PlayOneShot(favorHoverClip);
    }

    public void PlayFavorClick()
    {
        if (favorClickClip != null && sfxSource != null)
            sfxSource.PlayOneShot(favorClickClip);
    }

    public void PlayFavorReward()
    {
        if (favorRewardClip != null && sfxSource != null)
            sfxSource.PlayOneShot(favorRewardClip);
    }

    private int lastFavorVoiceIndex = -1;

    public void PlayFavorVoiceLine()
    {
        if (sfxSource == null || favorVoiceClips == null || favorVoiceClips.Length == 0)
            return;

        int index;

        if (favorVoiceClips.Length == 1)
        {
            index = 0;
        }
        else
        {
            do
            {
                index = Random.Range(0, favorVoiceClips.Length);
            }
            while (index == lastFavorVoiceIndex);
        }

        lastFavorVoiceIndex = index;
        sfxSource.PlayOneShot(favorVoiceClips[index]);
    }

    public void PlayCancelAction()
    {
        if (cancelActionClip != null && sfxSource != null)
            sfxSource.PlayOneShot(cancelActionClip);
    }

    public void PlayCompletePrankBannerDrop()
    {
        if (completePrankBannerDropClip != null && sfxSource != null)
            sfxSource.PlayOneShot(completePrankBannerDropClip);
    }

    public void PlayCompletePrank()
    {
        if (completePrankClip != null && sfxSource != null)
            sfxSource.PlayOneShot(completePrankClip);
    }

    public void PlayUIClick()
    {
        PlaySFX(uiClickClip);
    }

    public void PlayMenuClick()
    {
        PlaySFX(menuClickClip);
    }

    public void PlayConfirmClick()
    {
        PlaySFX(confirmClickClip);
    }

    public void PlayBackClick()
    {
        PlaySFX(backClickClip);
    }

    public void PlaySwapCompleteVoiceLine()
    {
        if (sfxSource == null || swapCompleteVoiceClips == null || swapCompleteVoiceClips.Length == 0)
         return;

        int index;

        if (swapCompleteVoiceClips.Length == 1)
        {
            index = 0;
        }
        else
        {
            do
            {
                index = Random.Range(0, swapCompleteVoiceClips.Length);
            }
            while (index == lastSwapCompleteVoiceIndex);
        }

        lastSwapCompleteVoiceIndex = index;
        sfxSource.PlayOneShot(swapCompleteVoiceClips[index]);
    }

    public void PlayUnlockReveal()
    {
        if (unlockRevealClip != null && sfxSource != null)
            sfxSource.PlayOneShot(unlockRevealClip);
    }

    public void PlayNotAnOption()
    {
        PlaySFX(notAnOptionClip);
    }

    public void PlayChooseRecruitReturnVoice()
    {
        PlaySFX(chooseRecruitReturnVoice);
    }

    public void PlayChooseAPrankToGetRidOf()
    {
        PlaySFX(chooseAPrankToGetRidOfClip);
    }
    
    public void PlayChooseOpponentToPoachRecruitFrom()
    {
        PlaySFX(chooseOpponentToPoachRecruitFromClip);
    }

    public void PlayChooseOpponentToGatherTheirReferrals()
    {
        PlaySFX(chooseOpponentToGatherTheirReferralsClip);
    }

    public void PlayHoundsCouldntTrackThemDown()
    {
        PlaySFX(houndsCouldntTrackThemDownClip);
    }

    public void PlayChooseARecruitForTheDogsToTrackDown()
    {
        PlaySFX(chooseARecruitForTheDogsToTrackDownClip);
    }

    public void PlayThatPlayerDoesntHaveAnyRecruits()
    {
        PlaySFX(thatPlayerDoesntHaveAnyRecruitsClip);
    }

    public void PlayYourOpponentDoesntHaveAnyRecruits()
    {
        PlaySFX(yourOpponentDoesntHaveAnyRecruitsClip);
    }

    public void PlayYourOpponentsDontHaveAnyRecruits()
    {
        PlaySFX(yourOpponentsDontHaveAnyRecruitsClip);
    }

    public void PlayAvailableServiceScoringAction()
    {
        PlaySFX(availableServiceScoringActionClip);
    }

    public float PlayPrankCompletionSound(string prankTitle)
    {
        PlayCompletePrank();

        float fallbackDuration = 1.75f;

        if (sfxSource == null)
        {
            Debug.LogWarning("Prank audio failed: sfxSource is NULL");
            return fallbackDuration;
        }

        Debug.Log("Looking for prank audio title: [" + prankTitle + "]");

        for (int i = 0; i < prankCompletionAudioEntries.Length; i++)
        {
            PrankCompletionAudioEntry entry = prankCompletionAudioEntries[i];

            if (entry == null)
                continue;

            Debug.Log("Checking prank audio entry: [" + entry.prankTitle + "]");

            if (entry.prankTitle != prankTitle)
                continue;

            if (entry.clips == null || entry.clips.Length == 0)
            {
                Debug.LogWarning("Prank audio entry found but has no clips: [" + prankTitle + "]");
                return fallbackDuration;
            }

            AudioClip chosenClip = entry.clips[Random.Range(0, entry.clips.Length)];

            if (chosenClip != null)
            {
                Debug.Log("Playing prank audio clip: " + chosenClip.name);
                sfxSource.PlayOneShot(chosenClip);
                return chosenClip.length;
            }

            Debug.LogWarning("Prank audio entry found but selected clip is NULL: [" + prankTitle + "]");
            return fallbackDuration;
        }

        Debug.LogWarning("No prank audio entry found for title: [" + prankTitle + "]");
        return fallbackDuration;
    }

    public void PlaySpendInfluence()
    {
        PlaySFX(spendInfluenceClip);
    }

    public float PlayPesterMayorVoice(int pesterUseCount)
    {
        AudioClip[] clipPool;

        if (pesterUseCount <= 1)
        {
            clipPool = pesterMayorEarlyClips;
        }
        else if (pesterUseCount <= 3)
        {
            clipPool = pesterMayorMiddleClips;
        }
        else
        {
            clipPool = pesterMayorLateClips;
        }

        if (clipPool == null || clipPool.Length == 0)
            return 0f;

        AudioClip selectedClip = clipPool[Random.Range(0, clipPool.Length)];

        PlaySFX(selectedClip);

        return selectedClip.length;
    }

    public void PlayJailDoorClosing()
    {
        PlaySFX(jailDoorClosingClip);
    }

    public void PlayCardLandingOnTable()
    {
        PlaySFX(cardLandingOnTable);
    }

    public void PlayRulesPageTurn()
    {
        PlaySFX(rulesPageTurn);
    }

    public void PlayMayorsGrowingFrustration()
    {
        if (mayorsGrowingFrustrationClip != null && sfxSource != null)
            sfxSource.PlayOneShot(mayorsGrowingFrustrationClip);
    }

    public void PlayInvalidSelection()
    {
        if (sfxSource == null || invalidSelectionClip == null)
            return;

        sfxSource.PlayOneShot(invalidSelectionClip, 0.6f);
    }

    public void PlayLocationMusic(GameLocationType locationType)
    {
        if (musicSource == null)
        {
            Debug.LogWarning("AudioManager: musicSource is not assigned.");
            return;
        }

        AudioClip selectedClip = null;

        switch (locationType)
        {
            case GameLocationType.RebelWorkshop:
                selectedClip = rebelWorkshopMusic;
                break;

            case GameLocationType.SewerHideout:
                selectedClip = sewerHideoutMusic;
                break;

            case GameLocationType.ForestClearing:
                selectedClip = forestClearingMusic;
                break;

            case GameLocationType.OutsideTheWalls:
                selectedClip = outsideTheWallsMusic;
                break;

            case GameLocationType.Treetop:
                selectedClip = treetopHideoutMusic;
                break;
        }

        if (selectedClip == null)
        {
            Debug.LogWarning(
                "AudioManager: no music clip assigned for location " +
                locationType);

            return;
        }

        if (musicSource.clip == selectedClip && musicSource.isPlaying)
            return;

        musicSource.clip = selectedClip;
        musicSource.loop = true;
        musicSource.Play();

        Debug.Log(
            "AudioManager: playing location music | location=" +
            locationType +
            " | clip=" +
            selectedClip.name);
    }

}