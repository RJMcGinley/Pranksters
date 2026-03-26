using UnityEngine;
using UnityEngine.UI;

public class PrankPreviewPanel : MonoBehaviour
{
    public GameObject rootObject;
    public Image previewImage;
    public DeckManager deckManager;

    public void Show(Sprite sprite)
    {
        Debug.Log("SHOW PREVIEW called with sprite: " + sprite.name);

        if (previewImage != null)
            previewImage.sprite = sprite;

        // 🔹 TURN OFF HIGHLIGHTS
        if (deckManager != null)
        {
            deckManager.PushHighlightSuppression();
        }

        if (rootObject != null)
            rootObject.SetActive(true);
        else
            gameObject.SetActive(true);
    }

    public void Hide()
    {
        Debug.Log("HIDE PREVIEW called");

        // 🔹 TURN HIGHLIGHTS BACK ON
        if (deckManager != null)
        {
            deckManager.PopHighlightSuppression();
        }

        if (rootObject != null)
            rootObject.SetActive(false);
        else
            gameObject.SetActive(false);
    }
}