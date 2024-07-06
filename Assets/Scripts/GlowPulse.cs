using UnityEngine;


public class GlowPulse : MonoBehaviour
{
    public Material glowMaterial;
    public float minIntensity = 0.0f;
    public float maxIntensity = 10.0f;
    public float swaySpeed = 1.0f;

    public float moveSpeed = 2.0f;
    public float moveAmount = 0.1f;

    private Vector3 originalPosition;
    private float currentIntensity;
    private bool increasing = true;

    private void Start()
    {
        originalPosition = transform.localPosition;

        if (glowMaterial == null)
        {
            Debug.LogError("Glow material not assigned.");
            enabled = false;
            return;
        }
        currentIntensity = minIntensity;
    }

    private void Update()
    {
        // Vertical movement (jumping effect)
        float newY = originalPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveAmount;
        transform.localPosition = new Vector3(originalPosition.x, newY, originalPosition.z);

        // Glow intensity swaying
        if (increasing)
        {
            currentIntensity += swaySpeed * Time.deltaTime;
            if (currentIntensity >= maxIntensity)
            {
                currentIntensity = maxIntensity;
                increasing = false;
            }
        }
        else
        {
            currentIntensity -= swaySpeed * Time.deltaTime;
            if (currentIntensity <= minIntensity)
            {
                currentIntensity = minIntensity;
                increasing = true;
            }
        }

        glowMaterial.SetFloat("_GlowIntensity", currentIntensity);
    }
}
