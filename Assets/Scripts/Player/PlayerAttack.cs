using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    PlayerOrientation orientation;
    Animator animator;
    [HideInInspector] public bool isAttacking = false;
    void Start()
    {
        orientation = GetComponent<PlayerMove>().orientation;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){  // press space to attack
            animator.SetBool("idle", false);
            StartCoroutine(AttackCoroutine());
        }
    }

    public IEnumerator AttackCoroutine(){
        string animationName;
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

        animator.Play(animationName);
        yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName));
        animator.SetBool("idle", true);
    }
}
