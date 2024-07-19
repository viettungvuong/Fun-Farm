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

    private IEnumerator boom(){
        spriteRenderer.sprite = boomSprite;
        yield return new WaitForSeconds(1f);
        spriteRenderer.sprite = originalSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Enemy"))
        {
            Unit enemyUnit = other.GetComponent<Unit>();
            if (enemyUnit != null)
            {
                enemyUnit.TakeDamage(damage);

                HitFlash hitFlash = other.GetComponent<HitFlash>();
                if (hitFlash != null)
                {
                    hitFlash.Flash();
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        Debug.Log("Collide");
        StartCoroutine(boom()); 
        
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