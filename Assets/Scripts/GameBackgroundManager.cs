using UnityEngine;

public class GameBackgroundManager : MonoBehaviour
{
    [Header("Background Renderer")]
    [SerializeField] private SpriteRenderer backgroundRenderer;

    [Header("Background Options")]
    [SerializeField] private Sprite[] possibleBackgrounds;

    private int lastBackgroundIndex = -1;

    public void ChooseRandomBackground()
    {
        if (backgroundRenderer == null)
        {
            Debug.LogWarning("GameBackgroundManager: backgroundRenderer is not assigned.");
            return;
        }

        if (possibleBackgrounds == null || possibleBackgrounds.Length == 0)
        {
            Debug.LogWarning("GameBackgroundManager: no possible backgrounds assigned.");
            return;
        }

        int randomIndex = Random.Range(0, possibleBackgrounds.Length);

        if (possibleBackgrounds.Length > 1)
        {
            int safety = 0;

            while (randomIndex == lastBackgroundIndex && safety < 20)
            {
                randomIndex = Random.Range(0, possibleBackgrounds.Length);
                safety++;
            }
        }

        backgroundRenderer.sprite = possibleBackgrounds[randomIndex];
        lastBackgroundIndex = randomIndex;

        Debug.Log("Game background changed to: " + possibleBackgrounds[randomIndex].name);
    }
}