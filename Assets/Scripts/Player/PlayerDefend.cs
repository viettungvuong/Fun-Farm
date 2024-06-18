using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerDefend : MonoBehaviour
{
    public static PlayerDefend instance;
    private Tilemap groundDefenseTilemap;
    public bool isTakingWood = false;
    private Rigidbody2D rb;

    public FenceUnit fenceHorizontal, fenceVertical;

    private Dictionary<Vector3Int, FenceUnit> fences;


    public TextMeshProUGUI fenceText;

    private PlayerMove playerMove;
    private Animator animator;

    private int woodTaken = 0;
    private int numberOfFences = 0;

    private void Awake() {
        instance = this;
    }

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        fences = new Dictionary<Vector3Int, FenceUnit>();

        groundDefenseTilemap = GameObject.Find("GroundDefense").GetComponent<Tilemap>();
        playerMove = GetComponent<PlayerMove>();
        animator = GetComponent<Animator>();
    }

    private bool buildFenceFlag = false;

    void Update()
    {
        if (GameController.HomeScene()==false){
            return; // only in home scene
        }


        if (Input.GetKeyDown(KeyCode.D))
        {
            buildFenceFlag = true;
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            buildFenceFlag = false;
        }

        if (buildFenceFlag)
        {
            // build defense
            if (playerMove.orientation==Orientation.UP||playerMove.orientation==Orientation.DOWN){
                BuildDefendFence(fenceHorizontal);
            }
            else{
                BuildDefendFence(fenceVertical);
            }

            buildFenceFlag = false; // Ensure it only builds once per key press
        }
    }

    public void BuildDefendFence(FenceUnit fence)
    {
        if (numberOfFences <= 0)
        {
            return;
        }

        Vector3Int gridPosition = groundDefenseTilemap.WorldToCell(rb.position);

        if (fences.ContainsKey(gridPosition))
        {
            return;
        }

        // Temporarily disable the collider
        Collider2D tilemapCollider = groundDefenseTilemap.GetComponent<Collider2D>();
        if (tilemapCollider != null)
        {
            tilemapCollider.enabled = false;
        }

        groundDefenseTilemap.SetTile(gridPosition, fence.tile);

        FenceUnit cloneFence = Instantiate(fence);
        cloneFence.health = 100;

        fences.Add(gridPosition, cloneFence);
        numberOfFences--; // minus one available fence

        // rb.position += new Vector2(0.5f, 0.5f); // slightly move to avoid collide the fence => cannot move player

        // Start the coroutine to re-enable the collider after a delay
        if (tilemapCollider != null)
        {
            StartCoroutine(ReenableColliderAfterDelay(tilemapCollider, 1f));
        }
    }

    private IEnumerator ReenableColliderAfterDelay(Collider2D collider, float delay)
    {
        yield return new WaitForSeconds(delay);
        collider.enabled = true;
    }

    private void LateUpdate() {
        fenceText.text = numberOfFences.ToString();
    }


    public FenceUnit GetDefenceAt(Vector3 worldPosition){
        Vector3Int cellPosition = groundDefenseTilemap.WorldToCell(worldPosition);
        
        if (!fences.ContainsKey(cellPosition)){
            return null;
        }
        else{
            return fences[cellPosition];
        }
    }

    public FenceUnit GetDefenceAt(Vector3Int cellPosition){
        if (!fences.ContainsKey(cellPosition)){
            return null;
        }
        else{
            return fences[cellPosition];
        }
    }

    public void DestroyFence(Vector3Int cellPosition){
        groundDefenseTilemap.SetTile(cellPosition, null);
    }

    public void DestroyFence(Vector3 worldPosition){
        Vector3Int cellPosition = groundDefenseTilemap.WorldToCell(worldPosition);
        
        groundDefenseTilemap.SetTile(cellPosition, null);
    }

    private IEnumerator GetWoodCoroutine(GameObject wood)
    {
        Vector3 woodPosition = wood.transform.position;
        Orientation woodToPlayer()
        {
            // Calculate the direction vector from the tile to the player
            Vector3 direction = (Vector3)rb.position - woodPosition;
            
            // Normalize the direction vector to get the direction in terms of unit vectors
            direction.Normalize();

            // Determine the orientation based on the direction vector
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x > 0)
                {
                    return Orientation.RIGHT;
                }
                else
                {
                    return Orientation.LEFT;
                }
            }
            else
            {
                if (direction.y > 0)
                {
                    return Orientation.UP;
                }
                else
                {
                    return Orientation.DOWN;
                }
            }
        }


        isTakingWood = true;
        string animationName;

        switch (woodToPlayer()){
            case Orientation.UP:{
                    animationName = "PlayerHarvestUp";
                    break;
            }
            case Orientation.DOWN:{
                    animationName = "PlayerHarvestDown";
                    break;
            }
            default:{
                    animationName = "PlayerHarvestHorizontal";
                    break;
            }
        }
        animator.SetBool("idle", false);
        animator.Play(animationName);
        // wait for animation to complete
        yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName)+0.5f);
        animator.SetBool("idle", true);

        isTakingWood = false;

        woodTaken++;

        if (woodTaken==3){
            numberOfFences++;
        }

        wood.SetActive(false); // hide wood
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Wood")){ // take wood
            // play harvest down animation
            StopAllCoroutines();
            StartCoroutine(GetWoodCoroutine(other.gameObject));
            
        }
    }
}
