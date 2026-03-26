using UnityEngine;

public class PopupArm : MonoBehaviour
{
    public float hiddenY = -100f;
    public float visibleY = 0f;
    public float moveSpeed = 10f;

    private RectTransform rectTransform;
    private Vector2 targetPosition;

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
    }

    public void Hide()
    {
        targetPosition.y = hiddenY;
    }
}