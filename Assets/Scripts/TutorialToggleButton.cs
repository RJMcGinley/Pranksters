using UnityEngine;

public class TutorialToggleButton : MonoBehaviour
{
    public GameObject checkmarkObject;

    void Start()
    {
        PlayerProgressSave data = SaveSystem.Load();

        if (checkmarkObject != null)
            checkmarkObject.SetActive(data.tutorialTipsEnabled);
    }

    public void ToggleTutorialTips()
    {
        PlayerProgressSave data = SaveSystem.Load();

        bool newState = !data.tutorialTipsEnabled;

        data.tutorialTipsEnabled = newState;
        SaveSystem.Save(data);

        if (checkmarkObject != null)
            checkmarkObject.SetActive(newState);
    }
}