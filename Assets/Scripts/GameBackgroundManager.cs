using UnityEngine;

public enum GameLocationType
{
    RebelWorkshop,
    SewerHideout,
    ForestClearing,
    OutsideTheWalls,
    Treetop
}

[System.Serializable]
public class GameLocationData
{
    public GameLocationType locationType;
    public string locationName;
    public Sprite backgroundSprite;
}

public class GameBackgroundManager : MonoBehaviour
{
    [Header("Background Renderer")]
    [SerializeField] private SpriteRenderer backgroundRenderer;

    [Header("Locations")]
    [SerializeField] private GameLocationData[] locations;

    private GameLocationType currentLocation = GameLocationType.RebelWorkshop;

    public GameLocationType CurrentLocation => currentLocation;

    private void Start()
    {
        SetLocation(GameLocationType.RebelWorkshop);
    }

    public void SetLocation(GameLocationType locationType)
    {
        if (backgroundRenderer == null)
        {
            Debug.LogWarning("GameBackgroundManager: backgroundRenderer is not assigned.");
            return;
        }

        GameLocationData location = GetLocationData(locationType);

        if (location == null || location.backgroundSprite == null)
        {
            Debug.LogWarning("GameBackgroundManager: missing location/background for " + locationType);
            return;
        }

        backgroundRenderer.sprite = location.backgroundSprite;
        currentLocation = locationType;

        Debug.Log("Game location changed to: " + location.locationName);
    }

    public GameLocationData[] GetAllLocations()
    {
        return locations;
    }

    public GameLocationData GetLocationData(GameLocationType locationType)
    {
        if (locations == null)
            return null;

        foreach (GameLocationData location in locations)
        {
            if (location != null && location.locationType == locationType)
                return location;
        }

        return null;
    }
}