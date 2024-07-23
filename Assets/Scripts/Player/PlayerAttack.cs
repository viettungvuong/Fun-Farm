using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerMove playerMove;
    private PlayerGun playerGun;
    Animator animator;
    Unit playerUnit;
    [HideInInspector] public bool isAttacking = false;
    private float nextAttackTime = 0f;
    public float cooldownTime = 0.2f;
    public float attackRange = 1.0f; 
    public LayerMask enemyLayers; 
    private Collider2D attackCollider;
    private AudioSource audioSource;
    private AudioClip swordSound; 

    void Start()
    {
        if (PlayerUnit.playerMode == PlayerMode.CREATIVE)
        {
            enabled = false;
            return;
        }
        playerMove = GetComponent<PlayerMove>();
        playerGun = GetComponent<PlayerGun>();
        animator = GetComponent<Animator>();
        playerUnit = GetComponent<Unit>();

        audioSource = gameObject.GetComponent<AudioSource>();
        swordSound = Resources.Load<AudioClip>("Audio/sword");

        attackCollider = GetComponent<Collider2D>();
        attackCollider.enabled = false; // Ensure the collider is disabled initially
    }

    void Update()
    {
        if (PlayerUnit.playerMode==PlayerMode.CREATIVE){
            enabled = false;
            return;
        }
        if (GameController.HomeScene() == false || playerGun.currentWeapon != Weapon.SWORD)
        {
            return;
        }
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1f / cooldownTime;

            isAttacking = true;

            StopAllCoroutines(); // stop other coroutines 
            StartCoroutine(AttackCoroutine());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy")&&isAttacking)
        {
            Unit enemyUnit = other.GetComponent<Unit>();
            if (enemyUnit != null)
            {
                enemyUnit.TakeDamage(playerUnit.damage);

                HitFlash hitFlash = other.GetComponent<HitFlash>();
                if (hitFlash != null)
                {
                    hitFlash.Flash();
                }
            }
        }
    }

    public IEnumerator AttackCoroutine()
    {
        string animationName;
        Orientation orientation = playerMove.orientation;

        switch (orientation)
        {
            case Orientation.UP:
                animationName = "PlayerAttackUp";
                break;
            case Orientation.DOWN:
                animationName = "PlayerAttackDown";
                break;
            case Orientation.LEFT:
                animationName = "PlayerAttackHorizontal";
                break;
            case Orientation.RIGHT:
                animationName = "PlayerAttackHorizontal";
                break;
            default:
                animationName = "PlayerAttackHorizontal";
                break;
        }

        animator.SetBool("idle", false);
        animator.Play(animationName);

        audioSource.clip = swordSound;
        audioSource.Play();

        // Enable the attack collider at the start of the attack
        attackCollider.enabled = true;

        yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName));

        // Disable the attack collider at the end of the attack
        attackCollider.enabled = false;

        animator.SetBool("idle", true);
        isAttacking = false;
    }
}