using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MayorFrustrationController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image mayorImage;
    [SerializeField] private TMP_Text scoreText;

    private int currentDisplayedFrustrationLevel = -1;

    [Header("Mayor Frustration Sprites")]
    [SerializeField] private Sprite level1Sprite;
    [SerializeField] private Sprite level2Sprite;
    [SerializeField] private Sprite level3Sprite;
    [SerializeField] private Sprite level4Sprite;
    [SerializeField] private Sprite level5Sprite;

    [Header("Shake Effect")]
    [SerializeField] private RectTransform shakeTarget;
    [SerializeField] private float shakeDuration = 0.25f;
    [SerializeField] private float shakeStrength = 8f;

    private Vector2 originalAnchoredPosition;
    private Coroutine shakeCoroutine;

    private void Start()
    {
        if (shakeTarget != null)
            originalAnchoredPosition = shakeTarget.anchoredPosition;
    }
    
    public void Refresh(int combinedMischief, int breakingPoint)
    {
        if (breakingPoint <= 0)
            breakingPoint = 150;

        int frustrationLevel = GetFrustrationLevel(combinedMischief, breakingPoint);

        if (currentDisplayedFrustrationLevel == -1)
        {
            currentDisplayedFrustrationLevel = frustrationLevel;

            if (mayorImage != null)
                mayorImage.sprite = GetSpriteForLevel(frustrationLevel);
        }
        else if (frustrationLevel != currentDisplayedFrustrationLevel)
        {
            currentDisplayedFrustrationLevel = frustrationLevel;

            if (mayorImage != null)
                mayorImage.sprite = GetSpriteForLevel(frustrationLevel);

            PlayShakeEffect();

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayMayorsGrowingFrustration();
        }

        if (scoreText != null)
            scoreText.text = combinedMischief + "/" + breakingPoint;
    }

    private int GetFrustrationLevel(int combinedMischief, int breakingPoint)
    {
        if (breakingPoint == 150)
        {
            if (combinedMischief >= 120) return 5;
            if (combinedMischief >= 90) return 4;
            if (combinedMischief >= 60) return 3;
            if (combinedMischief >= 30) return 2;
            return 1;
        }

        if (breakingPoint == 200)
        {
            if (combinedMischief >= 160) return 5;
            if (combinedMischief >= 120) return 4;
            if (combinedMischief >= 80) return 3;
            if (combinedMischief >= 40) return 2;
            return 1;
        }

        if (breakingPoint == 250)
        {
            if (combinedMischief >= 200) return 5;
            if (combinedMischief >= 150) return 4;
            if (combinedMischief >= 100) return 3;
            if (combinedMischief >= 50) return 2;
            return 1;
        }

        float progress = (float)combinedMischief / breakingPoint;

        if (progress >= 0.8f) return 5;
        if (progress >= 0.6f) return 4;
        if (progress >= 0.4f) return 3;
        if (progress >= 0.2f) return 2;
        return 1;
    }

    private Sprite GetSpriteForLevel(int frustrationLevel)
    {
        switch (frustrationLevel)
        {
            case 5:
                return level5Sprite;

            case 4:
                return level4Sprite;

            case 3:
                return level3Sprite;

            case 2:
                return level2Sprite;

            default:
                return level1Sprite;
        }
    }

    private void PlayShakeEffect()
    {
        if (shakeTarget == null)
            return;

        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-shakeStrength, shakeStrength);
            float y = Random.Range(-shakeStrength, shakeStrength);

            shakeTarget.anchoredPosition = originalAnchoredPosition + new Vector2(x, y);

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeTarget.anchoredPosition = originalAnchoredPosition;
        shakeCoroutine = null;
    }
}