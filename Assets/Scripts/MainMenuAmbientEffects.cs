using System.Collections;
using UnityEngine;

public class MainMenuAmbientEffects : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform[] leafPrefabs;
    [SerializeField] private RectTransform leafParent;

    [Header("Spawn Timing")]
    [SerializeField] private float minimumSpawnDelay = 5f;
    [SerializeField] private float maximumSpawnDelay = 10f;

    [Header("Leaf Size")]
    [SerializeField] private float minimumScale = 0.1f;
    [SerializeField] private float maximumScale = 0.3f;

    [Header("Travel")]
    [SerializeField] private float minimumTravelDuration = 5f;
    [SerializeField] private float maximumTravelDuration = 10f;
    [SerializeField] private float maximumVerticalDrift = 60f;

    [Header("Wobble")]
    [SerializeField] private float minimumWobbleAmount = 10f;
    [SerializeField] private float maximumWobbleAmount = 30f;
    [SerializeField] private float minimumWobbleSpeed = 1.5f;
    [SerializeField] private float maximumWobbleSpeed = 3.5f;

    [Header("Rotation")]
    [SerializeField] private float minimumRotationSpeed = -55f;
    [SerializeField] private float maximumRotationSpeed = 55f;

    private Coroutine spawnRoutine;

    private void OnEnable()
    {
        spawnRoutine = StartCoroutine(SpawnLeavesRoutine());
    }

    private void OnDisable()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private IEnumerator SpawnLeavesRoutine()
    {
        while (true)
        {
            float delay = Random.Range(
                minimumSpawnDelay,
                maximumSpawnDelay
            );

            yield return new WaitForSecondsRealtime(delay);

            SpawnLeaf();
        }
    }

    private void SpawnLeaf()
    {
        if (leafPrefabs == null || leafPrefabs.Length == 0 || leafParent == null)
        {
            Debug.LogWarning(
                "MainMenuAmbientEffects: Leaf Prefab or Leaf Parent is not assigned."
            );

            return;
        }

        if (leafPrefabs == null || leafPrefabs.Length == 0)
        {
            Debug.LogWarning("MainMenuAmbientEffects: No leaf prefabs assigned.");
            return;
        }

        RectTransform selectedPrefab =
            leafPrefabs[Random.Range(0, leafPrefabs.Length)];

        RectTransform leaf =
            Instantiate(selectedPrefab, leafParent);

        Rect parentRect = leafParent.rect;

        float horizontalPadding = 600f;

        bool useLeftSide = Random.value > 0.5f;

        float edgeWidth = parentRect.width * 0.32f;

        float startX;
        float endX;

        if (useLeftSide)
        {
            startX = parentRect.xMin - horizontalPadding;
            endX = parentRect.xMin + edgeWidth;
        }
        else
        {
            startX = parentRect.xMax + horizontalPadding;
            endX = parentRect.xMax - edgeWidth;
        }

        float startY = ChooseEdgeHeight(parentRect);

        float endY = startY + Random.Range(
            -maximumVerticalDrift,
            maximumVerticalDrift
        );

        leaf.anchoredPosition = new Vector2(startX, startY);

        float randomScale = Random.Range(
            minimumScale,
            maximumScale
        );

        leaf.localScale = Vector3.one * randomScale;

        leaf.localEulerAngles = new Vector3(
        0f,
        0f,
        Random.Range(0f, 360f)
    );

        float travelDuration = Random.Range(
            minimumTravelDuration,
            maximumTravelDuration
        );

        float rotationSpeed = Random.Range(
            minimumRotationSpeed,
            maximumRotationSpeed
        );

        float wobbleAmount = Random.Range(
            minimumWobbleAmount,
            maximumWobbleAmount
        );

        float wobbleSpeed = Random.Range(
            minimumWobbleSpeed,
            maximumWobbleSpeed
        );

        StartCoroutine(
            MoveLeafRoutine(
                leaf,
                new Vector2(endX, endY),
                travelDuration,
                rotationSpeed,
                wobbleAmount,
                wobbleSpeed
            )
        );
    }

    private IEnumerator MoveLeafRoutine(
        RectTransform leaf,
        Vector2 targetPosition,
        float duration,
        float rotationSpeed,
        float wobbleAmount,
        float wobbleSpeed
    )
    {
        Vector2 startPosition = leaf.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (leaf == null)
                yield break;

            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            Vector2 position = Vector2.Lerp(
                startPosition,
                targetPosition,
                t
            );

            position.y += Mathf.Sin(
                t * Mathf.PI * 2f * wobbleSpeed
            ) * wobbleAmount;

            leaf.anchoredPosition = position;

            leaf.Rotate(
                0f,
                0f,
                rotationSpeed * Time.unscaledDeltaTime
            );

            yield return null;
        }

        if (leaf != null)
            Destroy(leaf.gameObject);
    }

    private float ChooseEdgeHeight(Rect parentRect)
    {
        bool useUpperCanopy = Random.value > 0.25f;

        if (useUpperCanopy)
        {
            return Random.Range(
                parentRect.yMin + parentRect.height * 0.68f,
                parentRect.yMax - parentRect.height * 0.05f
            );
        }

        return Random.Range(
            parentRect.yMin + parentRect.height * 0.05f,
            parentRect.yMin + parentRect.height * 0.28f
        );
    }
}