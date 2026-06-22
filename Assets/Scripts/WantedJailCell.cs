using UnityEngine;
using UnityEngine.UI;

public class WantedJailCell : MonoBehaviour
{
    public Image jailedRecruitImage;

    public void Clear()
    {
        if (jailedRecruitImage != null)
        {
            jailedRecruitImage.sprite = null;
            jailedRecruitImage.enabled = false;
            jailedRecruitImage.gameObject.SetActive(false);
        }
    }

    public void SetJailedRecruit(Sprite icon)
    {
        if (jailedRecruitImage != null)
        {
            jailedRecruitImage.gameObject.SetActive(true);
            jailedRecruitImage.sprite = icon;
            jailedRecruitImage.enabled = true;
        }
    }
}