using UnityEngine;
using UnityEngine.UI;

public class PrankPreviewPanel : MonoBehaviour
{
    public GameObject rootObject;
    public Image previewImage;

    public void Show(Sprite sprite)
    {
        Debug.Log("SHOW PREVIEW called with sprite: " + sprite.name);

        if (previewImage != null)
            previewImage.sprite = sprite;

        if (rootObject != null)
            rootObject.SetActive(true);
        else
            gameObject.SetActive(true);
    }

    public void Hide()
    {
        Debug.Log("HIDE PREVIEW called");

        if (rootObject != null)
            rootObject.SetActive(false);
        else
            gameObject.SetActive(false);
    }
}