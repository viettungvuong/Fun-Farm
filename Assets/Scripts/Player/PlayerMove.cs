using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    // Update is called once per frame
    void Update()
    {
        moveSpeed = MapManager.instance.GetWalkingSpeed(transform.position); // walking speed based on terrain

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
        Vector2 newPosition = rb.position + new Vector2(moveXSpeed, moveYSpeed) * Time.fixedDeltaTime;

        // move within groundTilemap bound
        newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

        rb.MovePosition(newPosition);

        moveXSpeed = 0f;
        moveYSpeed = 0f;

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
}
