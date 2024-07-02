using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public enum Orientation
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
    None
}

[Serializable]
public struct FootprintTile {
    public TileBase original;
    public TileBase footprint;
}

public class PlayerMove : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Animator animator;
    Rigidbody2D rb;

    private Tilemap groundTilemap, highlightTilemap;
    public Tile highlightTile;
    private GameObject plantPanel;

    private Vector3 minBounds;
    private Vector3 maxBounds;
    private float moveSpeed;
    private float moveXSpeed = 0f;
    private float moveYSpeed = 0f;
    private Vector3 previousPos;
    public SpriteRenderer arrowSprite;

    [HideInInspector] public Orientation orientation;

    private bool changingAnimation = false;
    private PlayerPlant playerPlant;
    private PlayerAttack playerAttack;
    private PlayerGun playerGun;

    public List<FootprintTile> footprints;
    private Queue<Pair<Pair<TileBase, Vector3Int>, DateTime>> footprintQueue; // save tiles player has been previously

    public Sprite[] spriteOrientation;

    public ParticleSystem dustTrail;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        InitializeGroundTilemap();

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        footprintQueue = new Queue<Pair<Pair<TileBase, Vector3Int>, DateTime>>();

        minBounds = groundTilemap.localBounds.min;
        maxBounds = groundTilemap.localBounds.max;

        previousPos = rb.position;

        orientation = Orientation.DOWN;

        playerPlant = GetComponent<PlayerPlant>();
        playerAttack = GetComponent<PlayerAttack>();
        playerGun = GetComponent<PlayerGun>();

        plantPanel = GameObject.Find("PlantPanel");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-initialize the groundTilemap when a new scene is loaded
        InitializeGroundTilemap();
    }

    private void InitializeGroundTilemap()
    {
        groundTilemap = GameObject.Find("Ground").GetComponent<Tilemap>();

        if (GameController.HomeScene())
        {
            highlightTilemap = GameObject.Find("Highlight").GetComponent<Tilemap>();
        }

        minBounds = groundTilemap.localBounds.min;
        maxBounds = groundTilemap.localBounds.max;
    }

    void Update()
    {
        moveSpeed = MapManager.instance.GetWalkingSpeed(rb.position); // walking speed based on terrain
        bool holdArrowKey = false;
        Orientation prevOrientation = orientation;

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKey(KeyCode.UpArrow)) // press up
        {
            SetOrientation(Orientation.UP);

            if (Input.GetKey(KeyCode.UpArrow)) // hold up
            {
                moveYSpeed = moveSpeed;
                holdArrowKey = true;
            }
            else
            {
                holdArrowKey = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKey(KeyCode.DownArrow)) // down
        {
            SetOrientation(Orientation.DOWN);
            if (Input.GetKey(KeyCode.DownArrow))
            {
                moveYSpeed = -moveSpeed;
                holdArrowKey = true;
            }
            else
            {
                holdArrowKey = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow)
        || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
        {
            SetOrientation(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKey(KeyCode.RightArrow) ? Orientation.RIGHT : Orientation.LEFT);
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
            {
                holdArrowKey = true;
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    moveXSpeed = moveSpeed;
                }
                else if (Input.GetKey(KeyCode.LeftArrow))
                {
                    moveXSpeed = -moveSpeed;
                }
            }
            else
            {
                holdArrowKey = false;
            }
        }

        if (holdArrowKey) // holding arrow key => walking
        {
            DustTrailPlay();
            if (!changingAnimation)
            { // not playing orientation change animation => currently idle

                if (prevOrientation != orientation) // changing orientation when walking
                {
                    StartOrientationChange(); // play animation to change orientation
                }
                else if (prevOrientation == orientation)  // not changing orientation but still walking
                {
                    animator.Play(GetWalkAnimationName());
                }
            }

        }
        else // not hold arrow key => idle
        {
            if (SceneManager.GetActiveScene().name != "SceneHome")
            {
                animator.Play(GetIdleAnimationName()); // idle
            }
            else
            {
                if (playerPlant.isPlanting == false && playerAttack.isAttacking == false && playerGun.isShooting == false)
                { // not planting or attacking then start the idle animation
                    animator.Play(GetIdleAnimationName()); // idle
                }
            }
        }

        if (!TimeManage.instance.IsDay() && !GameController.HomeScene())
        { // go home when the night comes
            VillagePlayerSpawn.GoBackHome(transform);
            SceneManager.LoadScene("SceneHome");
        }

        // CheckFootprint(); // check all footprints in game
    }

    private void DustTrailPlay(){
        // change dust trail color
        var main = dustTrail.main;
        main.startColor = MapManager.instance.GetTrailColor(rb.position);

        dustTrail.Play();
    }

    private void FixedUpdate()
    {
        Vector2 newPosition = rb.position + new Vector2(moveXSpeed, moveYSpeed) * Time.fixedDeltaTime;

        newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

        rb.MovePosition(newPosition);

        moveXSpeed = 0f;
        moveYSpeed = 0f;

        Vector3Int cellPosition = groundTilemap.WorldToCell(rb.position);

        if (GameController.HomeScene() && highlightTilemap != null)
        {
            highlightTilemap.SetTile(groundTilemap.WorldToCell(previousPos), null); // delete highlight on previous pos

            if (GameController.HomeScene() && MapManager.instance.Plantable(rb.position)) // plantable position
            {
                // Only highlight if highlightTile is assigned
                if (highlightTile != null)
                {
                    highlightTilemap.SetTile(cellPosition, highlightTile);
                }
                plantPanel.SetActive(true);
            }
            else
            {
                highlightTilemap.SetTile(cellPosition, null);
                plantPanel.SetActive(false);// hide planting panel when the position is not plantable
            }
            highlightTilemap.RefreshAllTiles();
        }

        previousPos = rb.position;

        // AddFootprint(previousPos); // add footprint
    }

    private void AddFootprint(Vector3 pos)
    {
        Vector3Int cellPosition = groundTilemap.WorldToCell(pos);

        TileBase footprintTile = null;
        TileBase tile = groundTilemap.GetTile(cellPosition);
        foreach (FootprintTile ft in footprints)
        {
            if (ft.original == tile)
            {
                footprintTile = ft.footprint;
            }
        }
        if (footprintTile == null)
        {
            return;
        }
        groundTilemap.SetTile(cellPosition, footprintTile);

        // add to queue to manage time
        footprintQueue.Enqueue(new Pair<Pair<TileBase, Vector3Int>, DateTime>(new Pair<TileBase, Vector3Int>(tile, cellPosition), DateTime.Now));
    }

    private void CheckFootprint()
    {
        // footprint disappear after 1 min
        TimeSpan footprintLifetime = TimeSpan.FromSeconds(10);

        while (footprintQueue.Count > 0)
        {
            Pair<Pair<TileBase, Vector3Int>, DateTime> footprint = footprintQueue.Peek();
            if (DateTime.Now - footprint.Second > footprintLifetime)
            {
                // remove the footprint from the tilemap
                Vector3Int position = footprint.First.Second;
                TileBase originalTile = footprint.First.First;
                groundTilemap.SetTile(position, originalTile);
                footprintQueue.Dequeue();
            }
            else
            {
                break;
            }
        }
    }

    public void SetOrientation(Orientation newOrientation)
    {
        const float xTrailLeft = -0.3f;
        const float xTrailRight = 0.3f;

        void FlipSprite(bool flip)
        {
            spriteRenderer.flipX = flip;
            float x;
            if (flip){
                x = xTrailRight;
            }
            else{
                x = xTrailLeft;
            }
            float y = dustTrail.transform.localPosition.y;
            dustTrail.transform.localPosition = new Vector3(x, y);
        }


        orientation = newOrientation;

        switch (orientation)
        {
            case Orientation.UP:
                spriteRenderer.sprite = spriteOrientation[1];
                arrowSprite.flipY = true; // flip arrow
                arrowSprite.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case Orientation.DOWN:
                spriteRenderer.sprite = spriteOrientation[0];
                arrowSprite.flipY = false;
                arrowSprite.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case Orientation.LEFT:
                spriteRenderer.sprite = spriteOrientation[2];
                arrowSprite.flipY = false;
                arrowSprite.transform.rotation = Quaternion.Euler(0, 0, -90);
                FlipSprite(true); // Flip sprite to the left
                break;
            case Orientation.RIGHT:
                spriteRenderer.sprite = spriteOrientation[2];
                arrowSprite.flipY = false;
                arrowSprite.transform.rotation = Quaternion.Euler(0, 0, 90);
                FlipSprite(false); // Flip sprite to the right
                break;
        }

        // StartOrientationChange(); // play animation to change orientation
    }

    private IEnumerator WalkCoroutine()
    {
        animator.Play(GetWalkAnimationName());
        yield return null;
        switch (orientation){
            case Orientation.UP:
                spriteRenderer.sprite = spriteOrientation[1];

                break;
            case Orientation.DOWN:
                spriteRenderer.sprite = spriteOrientation[0];
                break;
            case Orientation.LEFT:
                spriteRenderer.sprite = spriteOrientation[2];
                spriteRenderer.flipX = true;
                break;
            case Orientation.RIGHT:
                spriteRenderer.sprite = spriteOrientation[2];
                spriteRenderer.flipX = false;
                break;
        }

    }

    private void StartOrientationChange()
    {
        changingAnimation = true; // orientation change
        animator.Play(GetIdleAnimationName());
        Invoke(nameof(EndOrientationChange), GameController.GetAnimationLength(animator, GetIdleAnimationName()));
    }

    private string GetWalkAnimationName()
    {
        switch (orientation)
        {
            case Orientation.UP:
                return "PlayerWalkUp";
            case Orientation.DOWN:
                return "PlayerWalkDown";
            default:
                return "PlayerWalkHorizontal";
        }
    }

    private string GetIdleAnimationName()
    {
        switch (orientation)
        {
            case Orientation.UP:
                return "PlayerIdleUp";
            case Orientation.DOWN:
                return "PlayerIdleDown";
            default:
                return "PlayerIdleHorizontal";
        }
    }

    private void EndOrientationChange()
    {
        changingAnimation = false;
        animator.Play(GetIdleAnimationName());
    }
}