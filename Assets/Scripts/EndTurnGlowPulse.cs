using UnityEngine;
using UnityEngine.UI;

public class EndTurnGlowPulse : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float minAlpha = 0.25f;
    [SerializeField] private float maxAlpha = 0.65f;

    private Image glowImage;

    private void Awake()
    {
        glowImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (glowImage == null) return;

        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);

        Color c = glowImage.color;
        c.a = alpha;
        glowImage.color = c;
    }
}