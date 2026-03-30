using UnityEngine;
using System.Collections;

public class PopupArm : MonoBehaviour
{
    public float hiddenY = -100f;
    public float visibleY = 0f;
    public float moveSpeed = 10f;

    private RectTransform rectTransform;
    private Vector2 targetPosition;
    private Coroutine replayCoroutine;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        Vector2 startPos = rectTransform.anchoredPosition;
        startPos.y = hiddenY;
        rectTransform.anchoredPosition = startPos;
        targetPosition = startPos;
    }

    void Update()
    {
        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            targetPosition,
            Time.deltaTime * moveSpeed
        );
    }

    public void Show()
    {
        targetPosition.y = visibleY;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayCompletePrankBannerDrop();
    }

    public void Hide()
    {
        targetPosition.y = hiddenY;
    }

    public void ReplayDrop()
    {
        if (replayCoroutine != null)
            StopCoroutine(replayCoroutine);

        replayCoroutine = StartCoroutine(ReplayDropRoutine());
    }

    private IEnumerator ReplayDropRoutine()
    {
        targetPosition.y = hiddenY;

        if (rectTransform != null)
        {
            Vector2 resetPos = rectTransform.anchoredPosition;
            resetPos.y = hiddenY;
            rectTransform.anchoredPosition = resetPos;
        }

        yield return null;

        Show();
        replayCoroutine = null;
    }
}