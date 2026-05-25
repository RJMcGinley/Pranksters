using TMPro;
using UnityEngine;

[System.Serializable]
public class InstructionLayout
{
    public Vector2 parchmentPosition;
    public Vector2 pointerPosition;
    public float pointerRotationZ;
}

public class AvailableServiceInstructionPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI instructionText;

    [SerializeField] private RectTransform parchmentRect;
    [SerializeField] private RectTransform pointerRect;

    [Header("instruction Layout")]
    [SerializeField] private InstructionLayout engineerLayout;
    [SerializeField] private InstructionLayout beastmasterLayout;
    //[SerializeField] private InstructionLayout wizardLayout;
    [SerializeField] private InstructionLayout thiefLayout;
    [SerializeField] private InstructionLayout scribeLayout;
    //[SerializeField] private InstructionLayout laborerLayout;

    public void Show(string message)
    {
        gameObject.SetActive(true);

        if (instructionText != null)
            instructionText.text = message;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ShowEngineerInstruction(string message)
{
    ApplyLayout(engineerLayout);
        Show(message);
    }    

    public void ShowBeastmasterInstruction(string message)
{
    ApplyLayout(beastmasterLayout);
    Show(message);
}

//public void ShowWizardInstruction(string message)
//{
//    ApplyLayout(wizardLayout);
//    Show(message);
//}

public void ShowThiefInstruction(string message)
{
    ApplyLayout(thiefLayout);
    Show(message);
}

public void ShowScribeInstruction(string message)
{
    ApplyLayout(scribeLayout);
    Show(message);
}

//public void ShowLaborerInstruction(string message)
//{
//    ApplyLayout(laborerLayout);
//    Show(message);
//}

    private void ApplyLayout(InstructionLayout layout)
    {
        if (parchmentRect != null)
            parchmentRect.anchoredPosition = layout.parchmentPosition;

        if (pointerRect != null)
        {
            pointerRect.anchoredPosition = layout.pointerPosition;
            pointerRect.localEulerAngles = new Vector3(0f, 0f, layout.pointerRotationZ);
        }
    }
}