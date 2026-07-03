using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class StatisticsUI : MonoBehaviour
{
    [Header("Win/Loss")]
    [SerializeField] private TMP_Text vs2Text;
    [SerializeField] private TMP_Text vs3Text;
    [SerializeField] private TMP_Text vs4Text;

    [Header("Totals")]
    [SerializeField] private TMP_Text currentTitleText;
    [SerializeField] private TMP_Text totalFavorText;
    [SerializeField] private TMP_Text lifetimeScoreText;
    [SerializeField] private TMP_Text highestScoreText;
    [SerializeField] private TMP_Text nextLifetimeUnlockText;

    [Header("Rows")]
    [SerializeField] private Transform rowsContainer;
    [SerializeField] private GameObject rowTemplate;

    [Header("Icons")]
    [SerializeField] private Sprite thiefIcon;
    [SerializeField] private Sprite wizardIcon;
    [SerializeField] private Sprite engineerIcon;
    [SerializeField] private Sprite laborerIcon;
    [SerializeField] private Sprite scribeIcon;
    [SerializeField] private Sprite beastmasterIcon;

    [Header("Manual Row Layout")]
    [SerializeField] private float iconX = -120f;
    [SerializeField] private float nameX = -90f;
    [SerializeField] private float favorX = 500f;
    [SerializeField] private float serviceX = 930f;

    private string GetTitleFromHighScore(int highScore)
{
    if (highScore >= 250) return "Savior of Bumblebrook";
    if (highScore >= 200) return "Legend of Bumblebrook";
    if (highScore >= 150) return "Master of Mayhem";
    if (highScore >= 125) return "Chaos Artist";
    if (highScore >= 100) return "Prankster";
    if (highScore >= 75) return "Mischief Maker";
    if (highScore >= 50) return "Trickster";
    if (highScore >= 25) return "Jester";
    return "Clown";
}

    private void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        PlayerProgressSave data = SaveSystem.Load();

        if (data == null)
        {
            Debug.LogError("StatisticsUI: SaveSystem.Load() returned null.");
            return;
        }

        if (vs2Text != null)
            vs2Text.text = $"{data.wins2P}-{data.losses2P}";

        if (vs3Text != null)
            vs3Text.text = $"{data.wins3P}-{data.losses3P}";

        if (vs4Text != null)
            vs4Text.text = $"{data.wins4P}-{data.losses4P}";

        int totalFavor = 0;

        if (data.favorPointsByType != null)
        {
            for (int i = 0; i < data.favorPointsByType.Count; i++)
            {
                totalFavor += data.favorPointsByType[i].totalFavorPointsGained;
            }
        }

        if (totalFavorText != null)
            totalFavorText.text = $"Total Influence Gained: {totalFavor}";

        if (lifetimeScoreText != null)
            lifetimeScoreText.text = $"Lifetime Notoriety: {data.lifetimeFinalScorePoints}";

        if (nextLifetimeUnlockText != null)
            nextLifetimeUnlockText.text =
                GetNextLifetimeUnlockText(data.lifetimeFinalScorePoints);

        if (highestScoreText != null)
            highestScoreText.text = $"Highest Single Game Score: {data.highestSingleGameScore}";

        if (currentTitleText != null)
            currentTitleText.text = "Current Title: " + GetTitleFromHighScore(data.highestSingleGameScore);

        BuildRows(data);
    }

    private void BuildRows(PlayerProgressSave data)
    {
        if (rowsContainer == null || rowTemplate == null)
        {
            Debug.LogError("StatisticsUI: Rows container or row template is not assigned.");
            return;
        }

        for (int i = rowsContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(rowsContainer.GetChild(i).gameObject);
        }

        Dictionary<string, int> favorLookup = new Dictionary<string, int>();

        if (data.favorPointsByType != null)
        {
            foreach (FavorPointsEntry entry in data.favorPointsByType)
            {
                favorLookup[entry.pranksterType] = entry.totalFavorPointsGained;
            }
        }

        Dictionary<string, int> serviceLookup = new Dictionary<string, int>();

        if (data.availableServiceUseCountsByType != null)
        {
            foreach (AvailableServiceUseCountEntry entry in data.availableServiceUseCountsByType)
            {
                serviceLookup[entry.pranksterType] = entry.totalAvailableServiceUses;
            }
        }

        CreateRow("Thief", "Rogues", thiefIcon, favorLookup, serviceLookup);
        CreateRow("Wizard", "Mystics", wizardIcon, favorLookup, serviceLookup);
        CreateRow("Engineer", "Planners", engineerIcon, favorLookup, serviceLookup);
        CreateRow("Laborer", "Roughnecks", laborerIcon, favorLookup, serviceLookup);
        CreateRow("Scribe", "Scholars", scribeIcon, favorLookup, serviceLookup);
        CreateRow("BeastMaster", "Wranglers", beastmasterIcon, favorLookup, serviceLookup);
    }

    private void CreateRow(
        string saveKey,
        string displayName,
        Sprite icon,
        Dictionary<string, int> favorLookup,
        Dictionary<string, int> serviceLookup)
    {
        GameObject row = Instantiate(rowTemplate, rowsContainer);
        row.SetActive(true);

        ApplyRowLayout(row.transform);

        Transform iconTransform = row.transform.Find("Icon");
        Transform nameTransform = row.transform.Find("NameText");
        Transform favorTransform = row.transform.Find("FavorValueText");
        Transform serviceTransform = row.transform.Find("ServiceValueText");

        if (iconTransform == null || nameTransform == null ||
            favorTransform == null || serviceTransform == null)
        {
            Debug.LogError("StatisticsUI: Row template is missing required children.");
            return;
        }

        Image iconImage = iconTransform.GetComponent<Image>();
        TMP_Text nameText = nameTransform.GetComponent<TMP_Text>();
        TMP_Text favorText = favorTransform.GetComponent<TMP_Text>();
        TMP_Text serviceText = serviceTransform.GetComponent<TMP_Text>();

        if (iconImage != null)
            iconImage.sprite = icon;

        if (nameText != null)
            nameText.text = displayName;

        int favor = favorLookup.ContainsKey(saveKey) ? favorLookup[saveKey] : 0;
        int serviceUses = serviceLookup.ContainsKey(saveKey) ? serviceLookup[saveKey] : 0;

        if (favorText != null)
            favorText.text = favor.ToString();

        if (serviceText != null)
            serviceText.text = serviceUses.ToString();
    }

    private void ApplyRowLayout(Transform rowTransform)
    {
        RectTransform rowRect = rowTransform as RectTransform;

        if (rowRect != null)
        {
            rowRect.anchorMin = new Vector2(0f, 1f);
            rowRect.anchorMax = new Vector2(1f, 1f);
            rowRect.pivot = new Vector2(0.5f, 1f);
            rowRect.offsetMin = new Vector2(0f, rowRect.offsetMin.y);
            rowRect.offsetMax = new Vector2(0f, rowRect.offsetMax.y);
        }

        RectTransform iconRect = rowTransform.Find("Icon") as RectTransform;
        RectTransform nameRect = rowTransform.Find("NameText") as RectTransform;
        RectTransform favorRect = rowTransform.Find("FavorValueText") as RectTransform;
        RectTransform serviceRect = rowTransform.Find("ServiceValueText") as RectTransform;

        if (iconRect != null)
        {
            iconRect.anchorMin = new Vector2(0f, 0.5f);
            iconRect.anchorMax = new Vector2(0f, 0.5f);
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            iconRect.anchoredPosition = new Vector2(iconX, 0f);
        }

        if (nameRect != null)
        {
            nameRect.anchorMin = new Vector2(0f, 0.5f);
            nameRect.anchorMax = new Vector2(0f, 0.5f);
            nameRect.pivot = new Vector2(0f, 0.5f);
            nameRect.anchoredPosition = new Vector2(nameX, 0f);
        }

        if (favorRect != null)
        {
            favorRect.anchorMin = new Vector2(0f, 0.5f);
            favorRect.anchorMax = new Vector2(0f, 0.5f);
            favorRect.pivot = new Vector2(0.5f, 0.5f);
            favorRect.anchoredPosition = new Vector2(favorX, 0f);
        }

        if (serviceRect != null)
        {
            serviceRect.anchorMin = new Vector2(0f, 0.5f);
            serviceRect.anchorMax = new Vector2(0f, 0.5f);
            serviceRect.pivot = new Vector2(0.5f, 0.5f);
            serviceRect.anchoredPosition = new Vector2(serviceX, 0f);
        }
    }

    private string FormatDisplayName(string type)
    {
        if (type == "BeastMaster")
            return "Beast Master";

        return type;
    }

    private string GetNextLifetimeUnlockText(int lifetimeNotoriety)
{
    if (lifetimeNotoriety < 100)
        return "Next Unlock: 100 - Use Inactive Services";

    if (lifetimeNotoriety < 200)
        return "Next Unlock: 200 - Swap Wanted Posters";

    if (lifetimeNotoriety < 350)
        return "Next Unlock: 350 - Pester Mayor";

    if (lifetimeNotoriety < 500)
        return "Next Unlock: 500 - Distract Barnaby";

    if (lifetimeNotoriety < 750)
        return "Next Unlock: 750 - Crew Size Discount";

    if (lifetimeNotoriety < 1000)
        return "Next Unlock: 1000 - Inactive Services Discount";

    if (lifetimeNotoriety < 1250)
        return "Next Unlock: 1250 - Crew Size Discount";

    if (lifetimeNotoriety < 1500)
        return "Next Unlock: 1500 - Inactive Services Discount";

    return "All Lifetime Unlocks Earned";
}

}