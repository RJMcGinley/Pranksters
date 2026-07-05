using UnityEngine;
using System.Collections;

public class SendRecruitToBarnabyHelper : MonoBehaviour
{
    public float hiddenX = -100f;
    public float visibleX = 0f;
    public float moveSpeed = 10f;

    private Transform helperTransform;
    private Vector3 targetPosition;
    private Coroutine replayCoroutine;

    void Awake()
    {
        helperTransform = transform;

        Vector3 startPos = helperTransform.localPosition;
        startPos.x = hiddenX;
        helperTransform.localPosition = startPos;

        targetPosition = startPos;
    }

    void Update()
    {
        helperTransform.localPosition = Vector3.Lerp(
            helperTransform.localPosition,
            targetPosition,
            Time.deltaTime * moveSpeed
        );
    }

    public void Show()
    {
        targetPosition.x = visibleX;
    }

    public void Hide()
    {
        targetPosition.x = hiddenX;
    }

    public void ReplaySlide()
    {
        if (replayCoroutine != null)
            StopCoroutine(replayCoroutine);

        replayCoroutine = StartCoroutine(ReplaySlideRoutine());
    }

    private IEnumerator ReplaySlideRoutine()
    {
        targetPosition.x = hiddenX;

        if (helperTransform != null)
        {
            Vector3 resetPos = helperTransform.localPosition;
            resetPos.x = hiddenX;
            helperTransform.localPosition = resetPos;
        }

        yield return null;

        Show();
        replayCoroutine = null;
    }
}