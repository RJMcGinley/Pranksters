using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopScreenController : MonoBehaviour
{
    [Header("Values")]
    public int mayorBreakingPoint = 150;
    public int bestPrankChain = 0;

    public int specialistsUnlocked = 0;
    public int totalSpecialists = 54;
    public int freeVersionAccess = 15;
    private bool fullRosterUnlocked = false;

    [Header("Text")]
    public TextMeshProUGUI mayorText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI specialistsUnlockedText;
    public TextMeshProUGUI freeVersionText;
    public TextMeshProUGUI purchasedLabelText;

    [Header("Purchase UI")]
    public GameObject unlockFullRosterButton;

    [Header("Score Bar")]
    public RectTransform scoreBarBackground;
    
    [Header("Score Bar")]
    public Slider scoreProgressSlider;

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        LoadSavedProgress();

        if (mayorText != null)
            mayorText.text = "Mayor’s Breaking Point: " + mayorBreakingPoint;

        if (scoreText != null)
            scoreText.text = "Your Best Pranking: " + bestPrankChain + " / " + mayorBreakingPoint;

        if (specialistsUnlockedText != null)
            specialistsUnlockedText.text = "Specialists Unlocked: " + specialistsUnlocked + " / " + totalSpecialists;

        if (freeVersionText != null)
        {
            if (fullRosterUnlocked)
                freeVersionText.text = "Full Roster Access: Unlocked";
            else
                freeVersionText.text = "Free Version Access: " + freeVersionAccess;
        }

        if (unlockFullRosterButton != null)
            unlockFullRosterButton.SetActive(!fullRosterUnlocked);

        if (purchasedLabelText != null)
        {
            purchasedLabelText.gameObject.SetActive(fullRosterUnlocked);
            purchasedLabelText.text = "Full Roster Unlocked";
        }

        UpdateScoreBar();
    }

    void UpdateScoreBar()
    {
        if (scoreProgressSlider == null)
            return;

        scoreProgressSlider.minValue = 0;
        scoreProgressSlider.maxValue = mayorBreakingPoint;
        scoreProgressSlider.value = Mathf.Clamp(bestPrankChain, 0, mayorBreakingPoint);
    }

    void LoadSavedProgress()
    {
        PlayerProgressSave data = SaveSystem.Load();

        if (data == null)
        {
            bestPrankChain = 0;
            specialistsUnlocked = 0;
            freeVersionAccess = 15;
            return;
        }

        bestPrankChain = data.highestSingleGameScore;
        specialistsUnlocked = SaveSystem.GetTotalEarnedUnlockCount();
        freeVersionAccess = SaveSystem.GetFreeVersionUnlockLimit();
        fullRosterUnlocked = data.fullGamePurchased;
    }

    public void UnlockFullRoster()
    {
        Debug.Log("FAKE PURCHASE: Unlocking full roster");

        SaveSystem.SetFullGamePurchased(true);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayConfirmClick();

        Refresh();
    }
}