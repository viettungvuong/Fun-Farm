using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Animator animator;
    Rigidbody2D rb;

    public float moveSpeed;

    private float moveXSpeed = 0f;
    private float moveYSpeed = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.UpArrow)){ // up
            animator.SetBool("up", true);
            animator.SetBool("down", false);
            animator.SetBool("horizontal", false);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) // down
        {
            animator.SetBool("up", false);
            animator.SetBool("down", true);
            animator.SetBool("horizontal", false);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)||Input.GetKeyDown(KeyCode.LeftArrow)){
            animator.SetBool("horizontal", true);
            if (Input.GetKeyDown(KeyCode.RightArrow)){
                spriteRenderer.flipX = false;
            }
            else{
                spriteRenderer.flipX = true;
            }
        }

        if (Input.GetKey(KeyCode.UpArrow)){
            animator.SetTrigger("upWalk");
            moveYSpeed = moveSpeed;
        }
        else if (Input.GetKey(KeyCode.DownArrow)){
            animator.SetTrigger("downWalk");
            moveYSpeed = -moveSpeed;
        }
        else if (Input.GetKey(KeyCode.RightArrow)||Input.GetKey(KeyCode.LeftArrow)){
            animator.SetTrigger("horizontalWalk");
            if (Input.GetKey(KeyCode.RightArrow)){
                spriteRenderer.flipX = false;
                moveXSpeed = moveSpeed;
            }
            else{
                spriteRenderer.flipX = true;
                moveXSpeed = -moveSpeed;
            }
        }
    }

    private void FixedUpdate() {
        rb.velocity = new Vector2(moveXSpeed, moveYSpeed);
        moveXSpeed = 0f;
        moveYSpeed = 0f;
    }
}
