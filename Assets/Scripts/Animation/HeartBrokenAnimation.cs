using UnityEngine;

public class HearBrokenAnimation : MonoBehaviour
{
    public float moveDuration = 0.75f; // Duration of the jump
    public Vector2 direction = Vector2.up; // Direction of the jump, default is upwards

    private Vector2 startPosition;
    private Vector2 endPosition;
    private float startTime;

    void Start()
    {
        startPosition = transform.position;

        endPosition = startPosition + direction*0.5f;
        startTime = Time.time;

        //Debug.Log($"Start Position: {startPosition}");
        //Debug.Log($"End Position: {endPosition}");

    }

    void Update()
    {
        float t = (Time.time - startTime) / moveDuration;
        t = Mathf.Clamp01(t); // Ensure t stays within 0 and 1
        transform.position = Vector2.Lerp(startPosition, endPosition, t);

        if (t >= 1f)
        {

            // Coin has finished the jump, you can now destroy it or perform another action
            Destroy(gameObject);
        }
    }
}
