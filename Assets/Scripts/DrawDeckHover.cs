using UnityEngine;

public class DrawDeckHover : MonoBehaviour
{
    public PopupArm popupArm;

    void OnMouseEnter()
    {
        if (popupArm != null)
            popupArm.Show();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDrawDeckHover();
    }

    void OnMouseExit()
    {
        if (popupArm != null)
            popupArm.Hide();
    }
}