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

    void Start()
    {
        if (PlayerUnit.playerMode==PlayerMode.CREATIVE){
            enabled = false;
            return;
        }
        playerMove = GetComponent<PlayerMove>();
        animator = GetComponent<Animator>();
        playerUnit = GetComponent<Unit>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.HomeScene()==false){
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space)&& Time.time >= nextAttackTime){  // press space to attack
            nextAttackTime = Time.time + 1f / cooldownTime;

            isAttacking = true;

            StopAllCoroutines(); // stop other coroutines 
            StartCoroutine(AttackCoroutine());

            // Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayers);
            // foreach (Collider enemy in hitEnemies)
            // {
            //     enemy.GetComponent<Unit>().TakeDamage(playerUnit.damage);
            // }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Enemy")){
            if (isAttacking){
                other.gameObject.GetComponent<Unit>().TakeDamage(playerUnit.damage);
            }
        }
    }

    // private void OnTriggerStay2D(Collider2D other) {
    //     if (other.gameObject.CompareTag("Enemy")){
    //         if (isAttacking){
    //             other.gameObject.GetComponent<Unit>().TakeDamage(playerUnit.damage);
    //         }
    //     }
    // }

    public IEnumerator AttackCoroutine(){
        string animationName;
        Orientation orientation = playerMove.orientation;
        switch (orientation)
        {
            case Orientation.UP:
            {               
                animationName = "PlayerAttackUp";
                break;
            }
            case Orientation.DOWN:
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

        animator.SetBool("idle", false);
        animator.Play(animationName);
        yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName));
        animator.SetBool("idle", true);
        isAttacking = false;
    }
}
