using UnityEngine;

public class WhoopieToggleButton : MonoBehaviour
{
    public GameObject checkmarkObject;

    void Start()
    {
        // Sync visual with actual state at start
        if (AudioManager.Instance != null && checkmarkObject != null)
        {
            checkmarkObject.SetActive(AudioManager.Instance.fartSoundsEnabled);
        }
    }

    public void ToggleWhoopie()
    {
        if (AudioManager.Instance == null) return;

        bool newState = !AudioManager.Instance.fartSoundsEnabled;

        AudioManager.Instance.SetFartSoundsEnabled(newState);

        if (checkmarkObject != null)
            checkmarkObject.SetActive(newState);
    }
}