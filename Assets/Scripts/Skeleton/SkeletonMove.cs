using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMove : MonoBehaviour
{
    Orientation orientation;
    Animator animator;
    SpriteRenderer spriteRenderer;

    public float moveSpeed;
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
        if (Vector3.Distance(player.transform.position, transform.position)<=1.5f){
            StartCoroutine(AttackCoroutine());
        }
        else{
            animator.SetBool("walk", true);
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
        }
    }

   private void MoveTowardsPlayer()
    {
        if (player == null) return;

        // ensures that skeleton only moves in 4 direction: up, down, right, left
        Vector3 direction = player.position - transform.position;

        // axis of movement
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Move horizontally
            if (direction.x > 0)
            {
                transform.position += Vector3.right * moveSpeed * Time.deltaTime;
                SetOrientation(Orientation.RIGHT);

            }
            else
            {

                transform.position += Vector3.left * moveSpeed * Time.deltaTime;
                SetOrientation(Orientation.LEFT);
            }
        }
        else
        {
            // Move vertically
            if (direction.y > 0)
            {
                transform.position += Vector3.up * moveSpeed * Time.deltaTime;
                SetOrientation(Orientation.UP);
            }
            else
            {

                transform.position += Vector3.down * moveSpeed * Time.deltaTime;
                SetOrientation(Orientation.DOWN);
            }
        }
    }


    // private IEnumerator WalkCoroutine()
    // {
    //     string animationName;

    //     switch (orientation)
    //     {
    //         case Orientation.UP:
    //             animationName = "SkeletonWalkUp";
    //             break;
    //         case Orientation.DOWN:
    //             animationName = "SkeletonWalkDown";
    //             break;
    //         default:
    //             animationName = "SkeletonWalkHorizontal";
    //             break;
    //     }

    //     AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    //     if (!stateInfo.IsName(animationName)) // make sure not playing the current animation
    //     // this ensures animation not reset when pressing
    //     {
    //         animator.SetTrigger("walk");
    //         yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName));
    //         animator.ResetTrigger("walk");
    //     }
    // }

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
            animator.SetBool("walk", false);
            animator.SetTrigger("attack");
            yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName));
            // animator.ResetTrigger("attack");
        }
    }
}
