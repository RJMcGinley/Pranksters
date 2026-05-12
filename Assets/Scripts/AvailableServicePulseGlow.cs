using UnityEngine;

public class AvailableServicePulseGlow : MonoBehaviour
{
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float minScale = 0.9f;
    [SerializeField] private float maxScale = 1.1f;

    private Vector3 baseScale;

    private void Start()
    {
        baseScale = transform.localScale;
    }

    private void Update()
    {
        float pulse = Mathf.Lerp(
            minScale,
            maxScale,
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f
        );

        transform.localScale = baseScale * pulse;
    }
}