using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    PlayerMove playerMove;
    Animator animator;
    Unit playerUnit;
    [HideInInspector] public bool isAttacking = false;
    private float nextAttackTime = 0f;
    public float cooldownTime = 0.5f;
    public float attackRange = 1.0f; 
    public LayerMask enemyLayers; 

    void Start()
    {
        if (PlayerUnit.playerMode == PlayerMode.CREATIVE)
        {
            enabled = false;
            return;
        }
        playerMove = GetComponent<PlayerMove>();
        animator = GetComponent<Animator>();
        playerUnit = GetComponent<Unit>();
    }

    void Update()
    {
        if (GameController.HomeScene() == false)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1f / cooldownTime;

            isAttacking = true;

            StopAllCoroutines(); // stop other coroutines 
            StartCoroutine(AttackCoroutine());
        }
    }

    private void PerformRaycastAttack(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, attackRange, enemyLayers);
        if (hit.collider != null)
        {
            Unit enemyUnit = hit.collider.GetComponent<Unit>();
            if (enemyUnit != null)
            {
                enemyUnit.TakeDamage(playerUnit.damage);
            }
        }
    }

    public IEnumerator AttackCoroutine()
    {
        string animationName;
        Orientation orientation = playerMove.orientation;
        Vector2 attackDirection;

        switch (orientation)
        {
            case Orientation.UP:
                animationName = "PlayerAttackUp";
                attackDirection = Vector2.up;
                break;
            case Orientation.DOWN:
                animationName = "PlayerAttackDown";
                attackDirection = Vector2.down;
                break;
            case Orientation.LEFT:
                animationName = "PlayerAttackHorizontal";
                attackDirection = Vector2.left;
                break;
            case Orientation.RIGHT:
                animationName = "PlayerAttackHorizontal";
                attackDirection = Vector2.right;
                break;
            default:
                animationName = "PlayerAttackHorizontal";
                attackDirection = Vector2.right;
                break;
        }

        animator.SetBool("idle", false);
        animator.Play(animationName);

        // Perform the raycast attack in the determined direction
        PerformRaycastAttack(attackDirection);

        yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName));
        animator.SetBool("idle", true);
        isAttacking = false;
    }
}
