using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerDefend : MonoBehaviour
{
    public static PlayerDefend instance;
    private Tilemap groundDefenseTilemap;
    Rigidbody2D rb;

    public FenceUnit fence;

    private Dictionary<Vector3Int, FenceUnit> fences;

    private void Awake() {
        instance = this;
    }

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        fences = new Dictionary<Vector3Int, FenceUnit>();

        groundDefenseTilemap = GameObject.Find("GroundDefense").GetComponent<Tilemap>();
    }

    void Update(){
        if (Input.GetKey(KeyCode.D)){
            // build defence

            BuildDefendFence(fence);
        }
    }

    public void BuildDefendFence(FenceUnit fence){
        Vector3Int gridPosition = groundDefenseTilemap.WorldToCell(rb.position);

        if (fences.ContainsKey(gridPosition)){
            return;
        }

        groundDefenseTilemap.SetTile(gridPosition, fence.tile);

        FenceUnit cloneFence = Instantiate(fence);
        cloneFence.health = 100;

        fences.Add(gridPosition, cloneFence);
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
