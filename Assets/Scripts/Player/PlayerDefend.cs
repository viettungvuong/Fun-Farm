using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public enum FenceOrientation{
    vertical,
    horizontal,
    NONE
}

public class PlayerDefend : MonoBehaviour
{
    public static PlayerDefend instance;
    private Tilemap groundDefenseTilemap;
    [HideInInspector] public bool isTakingWood = false;
    private Rigidbody2D rb;

    public Tile fenceHorizontal, fenceVertical;

   


    public TextMeshProUGUI fenceText;

    private PlayerMove playerMove;
    private Animator animator;

    private Dictionary<Vector3Int, FenceOrientation> fences;

    private int numberOfFences = 0;
    public int intervalBetweenRefill = 45;
    private int nextMinuteRefill = 5;

    public PlayerDefendData Serialize(){
        PlayerDefendData playerDefendData = new PlayerDefendData
        {
            numberOfFences = numberOfFences,
            nextMinuteRefill = nextMinuteRefill
        };

        if (fences == null){
            fences = new Dictionary<Vector3Int, FenceOrientation>();
        }

        playerDefendData.fences.FromDictionary(fences);

        return playerDefendData;
    }

    public void Reload(PlayerDefendData playerDefendData){
        numberOfFences = playerDefendData.numberOfFences;

        nextMinuteRefill = playerDefendData.nextMinuteRefill;

        

        foreach (var entry in playerDefendData.fences.entries){
            Vector3Int vt3 = entry.key;
            FenceOrientation fenceOrientation = entry.value;
            fences.Add(vt3, fenceOrientation);
        }
    }

    private void Awake() {
        if (PlayerUnit.playerMode==PlayerMode.CREATIVE){
            enabled = false;
            return;
        }

        if (instance==null){
            instance = this;
        }
        else{
            Destroy(this);
        }

        rb = GetComponent<Rigidbody2D>();
        fences = new Dictionary<Vector3Int, FenceOrientation>();
        playerMove = GetComponent<PlayerMove>();
        animator = GetComponent<Animator>();

    }
    private void OnDestroy()
    {
 
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
 
        InitializeMap();
    }

    private void InitializeMap()
    {
        if (GameController.HomeScene()){
            groundDefenseTilemap = GameObject.Find("GroundDefense").GetComponent<Tilemap>();
        }

    }


    void Start(){

        SceneManager.sceneLoaded += OnSceneLoaded;

        InitializeMap();

    }

    private bool buildFenceFlag = false;

    void Update()
    {
        if (TimeManage.instance.currentMinute==nextMinuteRefill){
            nextMinuteRefill+= intervalBetweenRefill;
            if (nextMinuteRefill>=60){
                nextMinuteRefill -= 60;
            }

            numberOfFences += 2;
            if (fenceText!=null){
                fenceText.text = numberOfFences.ToString();
            }

        }
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
                BuildDefendFence(FenceOrientation.horizontal);
            }
            else{
                BuildDefendFence(FenceOrientation.vertical);
            }

            buildFenceFlag = false; // ensure it only builds once per key press
        }
    }

    public void BuildDefendFence(FenceOrientation fence)
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

        // temporarily disable the collider
        Collider2D tilemapCollider = groundDefenseTilemap.GetComponent<Collider2D>();
        if (tilemapCollider != null)
        {
            tilemapCollider.enabled = false;
        }

        Tile tile;
        if (fence==FenceOrientation.horizontal){
            tile = fenceHorizontal;
        }
        else{
            tile = fenceVertical;
        }

        groundDefenseTilemap.SetTile(gridPosition, tile);

        fences.Add(gridPosition, fence);
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


    public FenceOrientation GetDefenceAt(Vector3 worldPosition){
        Vector3Int cellPosition = groundDefenseTilemap.WorldToCell(worldPosition);
        
        if (!fences.ContainsKey(cellPosition)){
            return FenceOrientation.NONE;
        }
        else{
            return fences[cellPosition];
        }
    }

    public FenceOrientation GetDefenceAt(Vector3Int cellPosition){
        if (!fences.ContainsKey(cellPosition)){
            return FenceOrientation.NONE;
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

    // private IEnumerator GetWoodCoroutine(GameObject wood)
    // {
    //     Vector3 woodPosition = wood.transform.position;
    //     Orientation woodToPlayer()
    //     {
    //         Vector3 direction = woodPosition - (Vector3)rb.position;
    //         direction.Normalize();

    //         // Determine the orientation based on the direction vector
    //         if (Mathf.Abs(direction.y) >= Mathf.Abs(direction.x))
    //         {
    //             if (direction.y > 0)
    //             {
    //                 return Orientation.UP;
    //             }
    //             else
    //             {
    //                 return Orientation.DOWN;
    //             }
    //         }
    //         else
    //         {
    //             if (direction.x > 0)
    //             {
    //                 return Orientation.RIGHT;
    //             }
    //             else
    //             {
    //                 return Orientation.LEFT;
    //             }
    //         }
    //     }


    //     isTakingWood = true;
    //     string animationName;

    //     switch (woodToPlayer()){
    //         case Orientation.UP:{
    //                 animationName = "PlayerHarvestUp";
    //                 break;
    //         }
    //         case Orientation.DOWN:{
    //                 animationName = "PlayerHarvestDown";
    //                 break;
    //         }
    //         default:{
    //                 animationName = "PlayerHarvestHorizontal";
    //                 break;
    //         }
    //     }
    //     animator.SetBool("idle", false);
    //     animator.Play(animationName);
    //     // wait for animation to complete
    //     yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName)+0.5f);
    //     animator.SetBool("idle", true);

    //     isTakingWood = false;

    //     woodTaken++;

    //     if (woodTaken==3){
    //         numberOfFences++;
    //         woodTaken = 0;
    //     }
    //     fenceText.text = numberOfFences.ToString();
    //     woodTakenText.text = woodTaken.ToString() + "/3";
    //     wood.SetActive(false); // hide wood after being harvested
    // }

    // private void OnCollisionEnter2D(Collision2D other) {
    //     if (other.gameObject.CompareTag("Wood")){ // take wood
    //         // play harvest down animation
    //         StopAllCoroutines();
    //         StartCoroutine(GetWoodCoroutine(other.gameObject));
            
    //     }
    // }
}
