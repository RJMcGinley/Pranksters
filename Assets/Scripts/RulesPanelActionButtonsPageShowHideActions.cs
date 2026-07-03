using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RulesPanelActionButtonsPageShowHideActions : MonoBehaviour
{
    [System.Serializable]
    public class ActionEntry
    {
        public Image actionImage;
        public TMP_Text descriptionText;

        public Sprite unlockedSprite;
        public Sprite lockedSprite;

        [HideInInspector]
        public string originalDescription;
    }

    [TextArea]
    [SerializeField] private string lockedDescription =
        "Your notoriety hasn't grown enough to take this action.";

    [Header("Action Entries")]
    [SerializeField] private ActionEntry useInactiveServices;
    [SerializeField] private ActionEntry swapWantedPosters;
    [SerializeField] private ActionEntry pesterMayor;
    [SerializeField] private ActionEntry distractBarnaby;

    private void Awake()
    {
        CacheOriginalDescriptions();
    }

    public void Refresh()
    {
        RefreshEntry(useInactiveServices, SaveSystem.HasInactiveInfluenceServiceUnlock());
        RefreshEntry(swapWantedPosters, SaveSystem.HasSwapWantedPostersUnlock());
        RefreshEntry(pesterMayor, SaveSystem.HasPesterMayorUnlock());
        RefreshEntry(distractBarnaby, SaveSystem.HasDistractBarnabyUnlock());
    }

    private void CacheOriginalDescriptions()
    {
        CacheOriginalDescription(useInactiveServices);
        CacheOriginalDescription(swapWantedPosters);
        CacheOriginalDescription(pesterMayor);
        CacheOriginalDescription(distractBarnaby);
    }

    private void CacheOriginalDescription(ActionEntry entry)
    {
        if (entry == null || entry.descriptionText == null)
            return;

        entry.originalDescription = entry.descriptionText.text;
    }

    private void RefreshEntry(ActionEntry entry, bool unlocked)
    {
        if (entry == null)
            return;

        if (entry.actionImage != null)
            entry.actionImage.sprite = unlocked ? entry.unlockedSprite : entry.lockedSprite;

        if (entry.descriptionText != null)
            entry.descriptionText.text = unlocked ? entry.originalDescription : lockedDescription;
    }
}