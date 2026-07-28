using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Transform")]
    [SerializeField] private RectTransform targetTransform;
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float hoverRotation = 1.5f;
    [SerializeField] private float transitionDuration = 0.12f;

    [Header("Hover Position")]
    [SerializeField] private bool moveOnHover = true;
    [SerializeField] private Vector2 hoverOffset = new Vector2(8f, 0f);

    [Header("Hover Brightness")]
    [SerializeField] private Image targetImage;
    [SerializeField] private bool brightenOnHover = true;
    [SerializeField] private float hoverBrightness = 1.12f;

    [Header("Hover Audio")]
    [SerializeField] private AudioClip hoverSound;

    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Vector2 originalAnchoredPosition;
    private Color originalColor;

    private Coroutine activeTransition;
    private bool isHovered;

    private void Awake()
    {
        if (targetTransform == null)
            targetTransform = transform as RectTransform;

        if (targetImage == null)
            targetImage = GetComponent<Image>();

        if (targetTransform != null)
        {
            originalScale = targetTransform.localScale;
            originalRotation = targetTransform.localRotation;
            originalAnchoredPosition = targetTransform.anchoredPosition;
        }

        if (targetImage != null)
            originalColor = targetImage.color;
    }

    private void OnDisable()
    {
        isHovered = false;

        if (activeTransition != null)
        {
            StopCoroutine(activeTransition);
            activeTransition = null;
        }

        RestoreImmediately();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isHovered)
            return;

        isHovered = true;

        PlayHoverSound();
        StartTransition(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isHovered)
            return;

        isHovered = false;
        StartTransition(false);
    }

    private void StartTransition(bool hovering)
    {
        if (activeTransition != null)
            StopCoroutine(activeTransition);

        activeTransition = StartCoroutine(TransitionRoutine(hovering));
    }

    private IEnumerator TransitionRoutine(bool hovering)
    {
        if (targetTransform == null)
            yield break;

        Vector3 startScale = targetTransform.localScale;
        Quaternion startRotation = targetTransform.localRotation;
        Vector2 startPosition = targetTransform.anchoredPosition;

        Color startColor = targetImage != null
            ? targetImage.color
            : Color.white;

        Vector3 targetScale = hovering
            ? originalScale * hoverScale
            : originalScale;

        Quaternion targetRotation = hovering
            ? originalRotation * Quaternion.Euler(0f, 0f, hoverRotation)
            : originalRotation;

        Vector2 targetPosition = hovering && moveOnHover
            ? originalAnchoredPosition + hoverOffset
            : originalAnchoredPosition;

        Color targetColor = originalColor;

        if (hovering && brightenOnHover && targetImage != null)
        {
            targetColor = new Color(
                Mathf.Clamp01(originalColor.r * hoverBrightness),
                Mathf.Clamp01(originalColor.g * hoverBrightness),
                Mathf.Clamp01(originalColor.b * hoverBrightness),
                originalColor.a
            );
        }

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = transitionDuration <= 0f
                ? 1f
                : Mathf.Clamp01(elapsed / transitionDuration);

            t = Mathf.SmoothStep(0f, 1f, t);

            targetTransform.localScale =
                Vector3.Lerp(startScale, targetScale, t);

            targetTransform.localRotation =
                Quaternion.Lerp(startRotation, targetRotation, t);

            targetTransform.anchoredPosition =
                Vector2.Lerp(startPosition, targetPosition, t);

            if (targetImage != null)
                targetImage.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }

        targetTransform.localScale = targetScale;
        targetTransform.localRotation = targetRotation;
        targetTransform.anchoredPosition = targetPosition;

        if (targetImage != null)
            targetImage.color = targetColor;

        activeTransition = null;
    }

    private void PlayHoverSound()
    {
        if (hoverSound == null)
            return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(hoverSound);
    }

    private void RestoreImmediately()
    {
        if (targetTransform != null)
        {
            targetTransform.localScale = originalScale;
            targetTransform.localRotation = originalRotation;
            targetTransform.anchoredPosition = originalAnchoredPosition;
        }

        if (targetImage != null)
            targetImage.color = originalColor;
    }
}