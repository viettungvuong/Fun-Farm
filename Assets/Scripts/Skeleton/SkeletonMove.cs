using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SkeletonMove : MonoBehaviour
{
    Orientation orientation;
    Animator animator;
    SpriteRenderer spriteRenderer;
    Unit unit;

    public float moveSpeed;
    public GameObject player;
    public GameObject[] torches;
    int torchSabotaged = 0;
    private int numTorches;

    private GameObject currentTargetTorch;

    void Start()
    {
        orientation = Orientation.RIGHT;
        SetOrientation(orientation);

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        unit = GetComponent<Unit>();

        numTorches = torches.Length;
        if (numTorches > 0)
        {
            currentTargetTorch = torches[torchSabotaged];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeManage.instance.IsDay() == false && torchSabotaged < numTorches)
        {
            HandleTorchSabotage();
        }
        else
        {
            HandlePlayerAttack();
        }
    }

    private void HandleTorchSabotage()
    {
        if (currentTargetTorch != null)
        {
            float distanceToTorch = Vector3.Distance(currentTargetTorch.transform.position, transform.position);
            if (distanceToTorch <= 1.5f)
            {
                StartCoroutine(AttackCoroutine(true));
                torchSabotaged++;
                Light2D torchLight = currentTargetTorch.GetComponent<Light2D>();
                torchLight.intensity = 0.1f;
                Torch torch = currentTargetTorch.GetComponent<Torch>();
                torch.sabotaged = true;
                if (torchSabotaged < numTorches)
                {
                    currentTargetTorch = torches[torchSabotaged];
                }
                else
                {
                    currentTargetTorch = null;
                }
            }
            else
            {
                animator.SetBool("walk", true);
                MoveTowards(currentTargetTorch.transform);
            }
        }
    }

    private void HandlePlayerAttack()
    {
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (distanceToPlayer <= 1.5f)
        {
            StartCoroutine(AttackCoroutine(false));
        }
        else
        {
            animator.SetBool("walk", true);
            MoveTowards(player.transform);
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

    private void MoveTowards(Transform targetTransform)
    {
        // ensures that skeleton only moves in 4 direction: up, down, right, left
        Vector3 direction = targetTransform.position - transform.position;

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

    public IEnumerator AttackCoroutine(bool torchSabotage = false)
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
        {
            animator.SetBool("walk", false);
            animator.SetTrigger("attack");
            yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName));

            animator.ResetTrigger("attack");
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.name=="Player"){ // hit player
            Unit playerUnit = other.gameObject.GetComponent<Unit>();
            // inflict damage on player
            playerUnit.TakeDamage(unit.damage);
        }
    }
}
