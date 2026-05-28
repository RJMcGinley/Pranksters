using TMPro;
using UnityEngine;

public class LocationSelectionPanelController : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panelRoot;

    [Header("Popup")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI locationNameText;
    [SerializeField] private TextMeshProUGUI flavorText;
    [SerializeField] private TextMeshProUGUI effectText;

    [Header("Game Flow")]
    [SerializeField] private GameBackgroundManager backgroundManager;

    private System.Action<GameLocationType> onLocationSelected;

    private void Awake()
    {
        Hide();

        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    public void Show()
    {
        if (panelRoot != null)
            panelRoot.SetActive(true);

        HidePopup();
    }

    public void Hide()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    public void ShowPopup(string locationName, string flavor, string effect)
    {
        if (locationNameText != null)
            locationNameText.text = locationName;

        if (flavorText != null)
            flavorText.text = flavor;

        if (effectText != null)
            effectText.text = effect;

        if (popupPanel != null)
            popupPanel.SetActive(true);
    }

    public void HidePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    public void Open(System.Action<GameLocationType> locationSelectedCallback)
    {
        onLocationSelected = locationSelectedCallback;
        Show();
    }

    public void SelectLocation(GameLocationType locationType)
    {
        Debug.Log("LocationSelectionPanelController: selected location = " + locationType);

        if (backgroundManager != null)
        {
            backgroundManager.SetLocation(locationType);
        }
        else
        {
            Debug.LogWarning("LocationSelectionPanelController: backgroundManager is not assigned.");
        }

        Hide();

        System.Action<GameLocationType> callback = onLocationSelected;
        onLocationSelected = null;

        callback?.Invoke(locationType);
    }
}