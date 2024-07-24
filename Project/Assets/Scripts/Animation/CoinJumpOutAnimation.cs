using UnityEngine;

public class CoinJumpOutAnimation : MonoBehaviour
{
    public float jumpHeight = 0.5f; // Height the coin will jump
    public float moveDuration = 1.0f; // Duration of the jump
    public Vector2 direction = Vector2.up; // Direction of the jump, default is upwards

    private Vector2 startPosition;
    private Vector2 endPosition;
    private float startTime;
    private AudioSource audioSource;

    void Start()
    {
        startPosition = transform.position;

        endPosition = startPosition + direction;
        startTime = Time.time;

        audioSource = GetComponent<AudioSource>();
        audioSource.Play();

        Debug.Log($"Start Position: {startPosition}");
        Debug.Log($"End Position: {endPosition}");

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
