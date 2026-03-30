using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("SFX Source")]
    public AudioSource sfxSource;

    [Header("Current Clips")]
    public AudioClip drawDeckHoverClip;
    public AudioClip drawCardActionClip;
    public AudioClip discardPileHoverClip;
    public AudioClip discardCardClip;
    public AudioClip player1TurnClip;
    public AudioClip player2TurnClip;
    public AudioClip player3TurnClip;
    public AudioClip player4TurnClip;
    public AudioClip hmmmDecisionsClip;
    public AudioClip readyButtonClip;
    public AudioClip favorHoverClip;
    public AudioClip favorClickClip;
    public AudioClip favorRewardClip;
    public AudioClip cancelActionClip;
    public AudioClip completePrankBannerDropClip;

    [Header("Fart Sounds")]
    public AudioClip[] fartSounds;
    public bool fartSoundsEnabled = true;

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

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayDrawDeckHover()
    {
        PlaySFX(drawDeckHoverClip);
    }

    public void PlayDrawCardAction()
    {
        PlaySFX(drawCardActionClip);
    }

    public void PlayHmmDecisions()
    {
        if (hmmmDecisionsClip == null) return;
        PlaySFX(hmmmDecisionsClip);
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
}