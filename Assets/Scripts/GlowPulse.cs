using UnityEngine;

public class GlowPulse : MonoBehaviour
{    public float moveSpeed = 2.0f; 
    public float moveAmount = 0.1f; 

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        // vertical movement (jumping effect)
        float newY = originalPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveAmount;
        transform.localPosition = new Vector3(originalPosition.x, newY, originalPosition.z);
    }
}
