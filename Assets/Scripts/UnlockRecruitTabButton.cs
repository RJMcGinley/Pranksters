using UnityEngine;
using UnityEngine.UI;

public class UnlockRecruitTabButton : MonoBehaviour
{
    [Header("References")]
    public UnlockCollectionMenuController menuController;
    public Image iconImage;

    [Header("Recruit")]
    public PranksterType recruitType;

    public void ClickTab()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUIClick();

        if (menuController == null)
        {
            Debug.LogWarning("UnlockRecruitTabButton: menuController is not assigned.");
            return;
        }

        menuController.ShowRecruitFromTab(recruitType);
    }

    public void SetSelected(bool selected)
    {
        if (iconImage == null)
            return;

        iconImage.color = selected
            ? Color.white
            : new Color(0.5f, 0.5f, 0.5f, 1f);

        iconImage.transform.localScale = selected
            ? new Vector3(1.2f, 1.2f, 1.2f)
            : Vector3.one;
    }
}