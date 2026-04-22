using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("SFX Source")]
    public AudioSource sfxSource;

    [Header("Music Source")]
    public AudioSource musicSource;

    [Header("Music Clip")]
    public AudioClip backgroundMusicClip;

    [Header("Current Clips")]
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
        PlayMusic();
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

    public void PlayPlayerTurnVoice(int playerIndex)
    {
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
    
}