using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMove : MonoBehaviour
{
    Orientation orientation;
    Animator animator;
    SpriteRenderer spriteRenderer;

    public float moveSpeed;
    private float moveXSpeed = 0, moveYSpeed = 0;
    public Transform player; // Reference to the player's transform

    void Start()
    {
        orientation = Orientation.RIGHT;
        SetOrientation(orientation);

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position)<0.5f){
            StartCoroutine(AttackCoroutine());
        }
        else{
            MoveTowardsPlayer();
        }



    }

    private void SetOrientation(Orientation newOrientation)
    {
        if (orientation != newOrientation)
        {
            orientation = newOrientation;
            animator.SetBool("up", orientation == Orientation.UP);
            animator.SetBool("down", orientation == Orientation.DOWN);
            animator.SetBool("horizontal", orientation == Orientation.LEFT || orientation == Orientation.RIGHT);
            spriteRenderer.flipX = orientation == Orientation.LEFT;

            // Start the walk coroutine when orientation changes
            StopAllCoroutines();
            StartCoroutine(WalkCoroutine());
        }
    }

    private void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        Vector3 moveDirection = direction.normalized * moveSpeed * Time.deltaTime;

        // orientation based on move direction
        if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y))
        {
            if (moveDirection.x > 0)
            {
                SetOrientation(Orientation.RIGHT);
            }
            else
            {
                SetOrientation(Orientation.LEFT);
            }
        }
        else
        {
            if (moveDirection.y > 0)
            {
                SetOrientation(Orientation.UP);
            }
            else
            {
                SetOrientation(Orientation.DOWN);
            }
        }

        // move towards player
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }


    private IEnumerator WalkCoroutine()
    {
        string animationName;

        switch (orientation)
        {
            case Orientation.UP:
                animationName = "SkeletonWalkUp";
                break;
            case Orientation.DOWN:
                animationName = "SkeletonWalkDown";
                break;
            default:
                animationName = "SkeletonWalkHorizontal";
                break;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName(animationName)) // make sure not playing the current animation
        // this ensures animation not reset when pressing
        {
            animator.SetTrigger("walk");
            yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName));
            animator.ResetTrigger("walk");
        }
    }

    private IEnumerator AttackCoroutine()
    {
        string animationName;

        switch (orientation)
        {
            case Orientation.UP:
                animationName = "SkeletonAttackUp";
                break;
            case Orientation.DOWN:
                animationName = "SkeletonAttackDown";
                break;
            default:
                animationName = "SkeletonAttackHorizontal";
                break;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName(animationName)) // make sure not playing the current animation
        // this ensures animation not reset when pressing
        {
            animator.SetTrigger("attack");
            yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName));
            animator.ResetTrigger("attack");
        }
    }
}
