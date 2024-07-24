using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;  
    public float blinkSpeed = 10.0f; 
    public float blinkDuration = 0.5f;  
    public Color blinkColor = Color.white; 

    private bool isBlinking = false;
    private float blinkTimer = 0.0f;
    private Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        if (isBlinking)
        {
            blinkTimer += Time.deltaTime;

            float blink = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
            Color currentColor = Color.Lerp(originalColor, blinkColor, blink);
            spriteRenderer.color = currentColor;

            if (blinkTimer >= blinkDuration)
            {
                StopBlink();
            }
        }
    }

    public void Flash()
    {
        isBlinking = true;
        blinkTimer = 0.0f;
    }

    void StopBlink()
    {
        isBlinking = false;
        spriteRenderer.color = originalColor;  // Reset to the original color
    }
}