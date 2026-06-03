using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PrankCompletionShowcasePanel : MonoBehaviour
{
    [Header("References")]
    public GameObject rootObject;
    public Image prankImage;

    [Header("Animation")]
    public float duration = 1.75f;
    public float startScale = 1f;
    public float endScale = 1.12f;

    private Coroutine showcaseCoroutine;
    private Vector3 originalScale;

    void Start()
    {
        if (rootObject == null)
            rootObject = gameObject;

        originalScale = rootObject.transform.localScale;

        rootObject.SetActive(false);
    }

    public void Show(Sprite sprite, float customDuration)
    {
        if (sprite == null)
            return;

        if (showcaseCoroutine != null)
            StopCoroutine(showcaseCoroutine);

        showcaseCoroutine = StartCoroutine(ShowSequence(sprite, customDuration));
    }

    private IEnumerator ShowSequence(Sprite sprite, float customDuration)
    {
        prankImage.sprite = sprite;

        rootObject.SetActive(true);

        float elapsed = 0f;

        while (elapsed < customDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / customDuration);
            float scale = Mathf.Lerp(startScale, endScale, t);

            rootObject.transform.localScale =
                originalScale * scale;

            yield return null;
        }

        rootObject.transform.localScale = originalScale;

        rootObject.SetActive(false);

        showcaseCoroutine = null;
    }

    public void Hide()
    {
        if (showcaseCoroutine != null)
        {
            StopCoroutine(showcaseCoroutine);
            showcaseCoroutine = null;
        }

        if (rootObject == null)
            rootObject = gameObject;

        rootObject.transform.localScale = originalScale;
        rootObject.SetActive(false);
    }
}