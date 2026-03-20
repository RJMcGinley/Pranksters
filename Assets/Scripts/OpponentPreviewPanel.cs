using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OpponentPreviewPanel : MonoBehaviour
{
    [Header("Root")]
    public GameObject rootObject;

    [Header("Text")]
    public TextMeshProUGUI opponentLabelText;
    public TextMeshProUGUI pranksCompletedText;
    public TextMeshProUGUI renownPointsText;
    public TextMeshProUGUI favorPointsText;

    [Header("Favor Slots")]
    public Image favorSlot1Image;
    public Image favorSlot2Image;
    public Image favorSlot3Image;

    [Header("Prankster Icon Sprites")]
    public Sprite thiefIcon;
    public Sprite engineerIcon;
    public Sprite laborerIcon;
    public Sprite scribeIcon;
    public Sprite wizardIcon;
    public Sprite beastmasterIcon;

    public void ShowFromPlayerInfoPanel(PlayerInfoPanel sourcePanel)
    {
        if (sourcePanel == null)
            return;

        if (opponentLabelText != null && sourcePanel.playerLabelText != null)
            opponentLabelText.text = sourcePanel.playerLabelText.text;

        if (pranksCompletedText != null && sourcePanel.pranksCompletedText != null)
            pranksCompletedText.text = sourcePanel.pranksCompletedText.text;

        if (renownPointsText != null && sourcePanel.renownPointsText != null)
            renownPointsText.text = sourcePanel.renownPointsText.text;

        if (favorPointsText != null && sourcePanel.favorPointsText != null)
            favorPointsText.text = sourcePanel.favorPointsText.text;

        CopyFavorSlot(sourcePanel.favorSlot1Image, favorSlot1Image);
        CopyFavorSlot(sourcePanel.favorSlot2Image, favorSlot2Image);
        CopyFavorSlot(sourcePanel.favorSlot3Image, favorSlot3Image);

        if (rootObject != null)
            rootObject.SetActive(true);
        else
            gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (rootObject != null)
            rootObject.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    void CopyFavorSlot(Image source, Image target)
    {
        if (target == null)
            return;

        if (source != null && source.gameObject.activeSelf && source.sprite != null)
        {
            target.gameObject.SetActive(true);
            target.sprite = source.sprite;
        }
        else
        {
            target.gameObject.SetActive(false);
        }
    }
}