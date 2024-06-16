using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerDefend : MonoBehaviour
{
    public static PlayerDefend instance;
    private Tilemap groundDefenseTilemap;
    Rigidbody2D rb;

    public FenceUnit fenceHorizontal, fenceVertical;

    private Dictionary<Vector3Int, FenceUnit> fences;

    private const int maxNumberOfFences = 4;
    private int numberOfFences = maxNumberOfFences;
    public TextMeshProUGUI fenceText;

    private PlayerMove playerMove;
    public int intervalBetweenFenceRefills = 20;
    private int nextMinuteRefill = 8;

    private void Awake() {
                if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        fences = new Dictionary<Vector3Int, FenceUnit>();

        groundDefenseTilemap = GameObject.Find("GroundDefense").GetComponent<Tilemap>();
        playerMove = GetComponent<PlayerMove>();
    }

    private bool buildFenceFlag = false;

    void Update()
    {
        if (TimeManage.instance.currentMinute==nextMinuteRefill){
            nextMinuteRefill+= intervalBetweenFenceRefills;
            if (nextMinuteRefill>=60){
                nextMinuteRefill -= 60;
            }

            numberOfFences = maxNumberOfFences; // refill number of fences

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
}
