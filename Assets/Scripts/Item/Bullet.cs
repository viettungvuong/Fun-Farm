using System.Collections;
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

    private SpriteRenderer spriteRenderer;
    private Sprite originalSprite;
    public Sprite boomSprite;

    private bool hit = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSprite = spriteRenderer.sprite;
    }

    public void Shoot(Vector2 direction)
    {
        initialPosition = transform.position;
        this.direction = direction;
        rb.velocity = direction * speed;
    }

    private IEnumerator Boom()
    {
        spriteRenderer.sprite = boomSprite;
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        hit = true;
        rb.velocity = Vector2.zero; // stop the bullet movement

        if (other.gameObject.CompareTag("Enemy"))
        {
            Unit enemyUnit = other.gameObject.GetComponent<Unit>();
            if (enemyUnit != null)
            {
                enemyUnit.TakeDamage(damage);

                HitFlash hitFlash = other.gameObject.GetComponent<HitFlash>();
                if (hitFlash != null)
                {
                    hitFlash.Flash();
                }
            }
        }

        Debug.Log("Collide");
        StartCoroutine(Boom()); 
    }

    private void Update()
    {
        if (hit)
        {
            return;
        }
        
        float distanceTraveled = Vector2.Distance(initialPosition, transform.position);
        if (distanceTraveled >= maxRange)
        {
            gameObject.SetActive(false);
        }
    }
}