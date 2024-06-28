using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    [HideInInspector] public float maxRange = 50f;
    [HideInInspector] public int damage;
    private Vector2 initialPosition;
    private Vector2 direction;
    private Rigidbody2D rb;
    private Camera cam;
    public LayerMask enemyLayers;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();

    }

    public void Shoot(Vector2 direction)
    {
        initialPosition = transform.position;
        this.direction = direction;
        rb.velocity = direction * speed;
    }

    private void Update()
    {
        float distanceTraveled = Vector2.Distance(initialPosition, transform.position);
        if (distanceTraveled >= maxRange)
        {
            gameObject.SetActive(false);
        }
        else{
            // Raycast check hit
            float offset = 2f;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distanceTraveled + offset , enemyLayers);
            if (hit.collider!=null)
            {
                Unit unit = hit.transform.GetComponent<Unit>();
                if (unit != null)
                {
                    unit.TakeDamage(damage);

                    HitFlash hitFlash = hit.transform.GetComponent<HitFlash>();
                    if (hitFlash != null)
                    {
                        hitFlash.Flash();
                    }
                }
            }
        }
    }

}