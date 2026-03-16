using UnityEngine;

public class DrawDeckDisplay : MonoBehaviour
{
    public GameObject topCard;
    public GameObject layer1;
    public GameObject layer2;
    public GameObject layer3;

    public DeckManager deckManager;

    void Update()
    {
        int count = deckManager.GetDeckCount();

        topCard.SetActive(count > 0);
        layer1.SetActive(count > 10);
        layer2.SetActive(count > 25);
        layer3.SetActive(count > 40);
    }
}