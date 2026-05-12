using UnityEngine;

public class AvailableServiceActionCollider : MonoBehaviour
{
    [Header("Action Identity")]
    [SerializeField] private string actionName;

    [Header("Availability")]
    [SerializeField] private bool isAvailable;

    [Header("Hover Description")]
    [SerializeField] private GameObject[] actionTitleTextObjects;
    [SerializeField] private TMPro.TMP_Text descriptionText;
    [SerializeField] [TextArea] private string actionDescription;
    [SerializeField] private float hoverDelay = 0.5f;

    private bool isHovering;
    private float hoverTimer;

    private void Start()
    {
        SetAvailable(isAvailable);
    }

    private void OnMouseDown()
    {
        if (!isAvailable)
            return;

        Debug.Log("Available Service action clicked: " + actionName);
    }

    public void SetAvailable(bool available)
{
    isAvailable = available;
}

    private void Update()
{
    if (!isHovering)
        return;

    hoverTimer += Time.deltaTime;

    if (hoverTimer >= hoverDelay)
    {
        ShowDescription();
    }
}

private void OnMouseEnter()
{
    isHovering = true;
    hoverTimer = 0f;
}

private void OnMouseExit()
{
    isHovering = false;
    hoverTimer = 0f;

    HideDescription();
}

private void ShowDescription()
{
    if (actionTitleTextObjects != null)
    {
        foreach (GameObject obj in actionTitleTextObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    if (descriptionText != null)
    {
        descriptionText.gameObject.SetActive(true);
        descriptionText.text = actionDescription;
    }
}

private void HideDescription()
{
    if (actionTitleTextObjects != null)
    {
        foreach (GameObject obj in actionTitleTextObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }

    if (descriptionText != null)
    {
        descriptionText.gameObject.SetActive(false);
    }
}
}
