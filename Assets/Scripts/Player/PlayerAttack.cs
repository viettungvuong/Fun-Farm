using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    PlayerMove playerMove;
    Animator animator;
    [HideInInspector] public bool isAttacking = false;
    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){  // press space to attack
            animator.SetBool("idle", false);
            isAttacking = true;
            StartCoroutine(AttackCoroutine());
        }
    }

    public IEnumerator AttackCoroutine(){
        string animationName;
        PlayerOrientation orientation = playerMove.orientation;
        switch (orientation)
        {
            case PlayerOrientation.UP:
            {               
                animationName = "PlayerAttackUp";
                break;
            }
            case PlayerOrientation.DOWN:
            {               
                animationName = "PlayerAttackDown";
                break;
            }
            default:
            {               
                animationName = "PlayerAttackHorizontal";
                break;
            }
        }
        Debug.Log(animationName);

        animator.Play(animationName);
        yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName));
        animator.SetBool("idle", true);
        isAttacking = false;
    }
}
