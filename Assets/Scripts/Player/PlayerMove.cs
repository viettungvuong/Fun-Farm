using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum PlayerOrientation{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

public class PlayerMove : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Animator animator;
    Rigidbody2D rb;

    public Tilemap groundTilemap;
    public Tilemap highlightTilemap; // for highlighting
    public Tile highlightTile;
    public GameObject panel;

    private Vector3 minBounds;
    private Vector3 maxBounds;
    private float moveSpeed;
    private float moveXSpeed = 0f;
    private float moveYSpeed = 0f;
    private Vector3 previousPos;

    [HideInInspector] public PlayerOrientation orientation;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = MapManager.instance.GetWalkingSpeed(transform.position);

        minBounds = groundTilemap.localBounds.min;
        maxBounds = groundTilemap.localBounds.max;

        previousPos = rb.position;
    }

    void Update()
    {
        moveSpeed = MapManager.instance.GetWalkingSpeed(transform.position); // walking speed based on terrain

        if (Input.GetKeyDown(KeyCode.UpArrow)) // up
        {
            SetOrientation(PlayerOrientation.UP);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) // down
        {
            SetOrientation(PlayerOrientation.DOWN);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetOrientation(Input.GetKeyDown(KeyCode.RightArrow) ? PlayerOrientation.RIGHT : PlayerOrientation.LEFT);
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
        {
            string animationName;

            switch (orientation){
                case PlayerOrientation.UP:{
                        animationName = "PlayerWalkUp";
                        break;
                }
                case PlayerOrientation.DOWN:{
                        animationName = "PlayerWalkDown";
                        break;
                }
                default:{
                        animationName = "PlayerWalkHorizontal";
                        break;
                }
            }

            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (!stateInfo.IsName(animationName)) // make sure not playing the current animation
            // this ensures animation not reset when pressing
            {
                animator.Play(animationName);
            }
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveYSpeed = moveSpeed;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            moveYSpeed = -moveSpeed;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveXSpeed = moveSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveXSpeed = -moveSpeed;
        }
    }

    private void FixedUpdate()
    {
        Vector2 newPosition = rb.position + new Vector2(moveXSpeed, moveYSpeed) * Time.fixedDeltaTime;

        // move within groundTilemap bound
        newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

        rb.MovePosition(newPosition);

        moveXSpeed = 0f;
        moveYSpeed = 0f; // reset speed

        Vector3Int cellPosition = groundTilemap.WorldToCell(rb.position);
        highlightTilemap.SetTile(groundTilemap.WorldToCell(previousPos), null); // delete highlight on previous pos

        if (MapManager.instance.Plantable(rb.position)) // plantable position
        {
            highlightTilemap.SetTile(cellPosition, highlightTile);
            panel.SetActive(true);
        }
        else
        {
            highlightTilemap.SetTile(cellPosition, null);
            panel.SetActive(false); // hide planting panel when the position is not plantable
        }
        highlightTilemap.RefreshAllTiles();

        previousPos = rb.position;
    }

    private void SetOrientation(PlayerOrientation newOrientation)
    {
        orientation = newOrientation;
        animator.SetBool("up", orientation == PlayerOrientation.UP);
        animator.SetBool("down", orientation == PlayerOrientation.DOWN);
        animator.SetBool("horizontal", orientation == PlayerOrientation.LEFT || orientation == PlayerOrientation.RIGHT);
        spriteRenderer.flipX = orientation == PlayerOrientation.LEFT;

    }
}
