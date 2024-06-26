using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    [HideInInspector] public float maxRange = 50f;
    private Vector2 initialPosition;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Shoot(Vector2 direction)
    {
        initialPosition = transform.position;
        Debug.Log(direction);
        Debug.Log(rb);
        Debug.Log(speed);
        rb.velocity = direction * speed;
    }

    private void Update()
    {
        float distanceTraveled = Vector2.Distance(initialPosition, transform.position);
        if (distanceTraveled >= maxRange)
        {
            gameObject.SetActive(false);
        }
    }

}