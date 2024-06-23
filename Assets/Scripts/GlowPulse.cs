using UnityEngine;

public class GlowPulse : MonoBehaviour
{
    public Material glowMaterial;
    public float pulseSpeed = 2.0f; 
    public float minGlowIntensity = 0.5f; 
    public float maxGlowIntensity = 3.0f; 
    public float moveSpeed = 2.0f; 
    public float moveAmount = 0.1f; 

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        // pulse the glow intensity
        float glowIntensity = Mathf.Lerp(minGlowIntensity, maxGlowIntensity, (Mathf.Sin(Time.time * pulseSpeed) + 1.0f) / 2.0f);
        glowMaterial.SetFloat("_GlowIntensity", glowIntensity);

        // vertical movement (jumping effect)
        float newY = originalPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveAmount;
        transform.localPosition = new Vector3(originalPosition.x, newY, originalPosition.z);
    }
}
