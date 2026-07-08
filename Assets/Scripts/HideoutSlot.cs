using UnityEngine;
using System.Collections;

public class HideoutSlot : MonoBehaviour
{
    [SerializeField] private HideoutController hideoutController;
    [SerializeField] private int slotIndex;
    [SerializeField] private GameObject swapRecruitsHelper;
    [SerializeField] private float hoverDelay = 0.5f;

    private Coroutine hoverCoroutine;
    private bool previewVisible;
    private Collider2D slotCollider;

    private void Awake()
    {
        slotCollider = GetComponent<Collider2D>();
    }

    public void SetColliderEnabled(bool isEnabled)
    {
        if (slotCollider != null)
            slotCollider.enabled = isEnabled;

        
        ClearHoverState();
    }

    private void OnMouseEnter()
    {
        if (hideoutController == null)
            return;

        if (hoverCoroutine != null)
            StopCoroutine(hoverCoroutine);

        hoverCoroutine = StartCoroutine(HoverDelayRoutine());
    }

    private void OnMouseExit()
    {
        ClearHoverState();
    }

    private void ClearHoverState()
    {
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine);
            hoverCoroutine = null;
        }

        if (previewVisible)
        {
            hideoutController.HideCardPreview();
            previewVisible = false;
        }

        if (swapRecruitsHelper != null)
            swapRecruitsHelper.SetActive(false);
    }

    private IEnumerator HoverDelayRoutine()
    {
        yield return new WaitForSeconds(hoverDelay);

        hideoutController.ShowCardPreview(slotIndex);
        previewVisible = true;

        if (swapRecruitsHelper != null)
            swapRecruitsHelper.SetActive(true);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayFavorHover();

        hoverCoroutine = null;
    }

    private void OnMouseDown()
    {
        if (hideoutController == null)
            return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUIClick();

        hideoutController.StartSwapFromSlot(slotIndex);
    }
}