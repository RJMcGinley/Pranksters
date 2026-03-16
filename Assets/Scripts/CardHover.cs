using UnityEngine;

public class CardHover : MonoBehaviour
{
    private Vector3 originalScale;

    private SpriteRenderer artRenderer;
    private SpriteRenderer frameRenderer;

    private int originalArtSortingOrder;
    private int originalFrameSortingOrder;

    public float hoverScale = 1.6f;
    public int hoverSortingBoost = 100;

    void Start()
    {
        originalScale = transform.localScale;

        Transform artTransform = transform.Find("CardArt");
        Transform frameTransform = transform.Find("CardFrame");

        if (artTransform != null)
            artRenderer = artTransform.GetComponent<SpriteRenderer>();

        if (frameTransform != null)
            frameRenderer = frameTransform.GetComponent<SpriteRenderer>();

        if (artRenderer != null)
            originalArtSortingOrder = artRenderer.sortingOrder;

        if (frameRenderer != null)
            originalFrameSortingOrder = frameRenderer.sortingOrder;
    }

    void OnMouseEnter()
    {
        transform.localScale = originalScale * hoverScale;

        if (artRenderer != null)
            artRenderer.sortingOrder = originalArtSortingOrder + hoverSortingBoost;

        if (frameRenderer != null)
            frameRenderer.sortingOrder = originalFrameSortingOrder + hoverSortingBoost;
    }

    void OnMouseExit()
    {
        transform.localScale = originalScale;

        if (artRenderer != null)
            artRenderer.sortingOrder = originalArtSortingOrder;

        if (frameRenderer != null)
            frameRenderer.sortingOrder = originalFrameSortingOrder;
    }
}