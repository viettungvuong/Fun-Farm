using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Tilemap map;

    [SerializeField] private List<TileData> tileDatas;

    private Dictionary<TileBase, TileData> dataFromTiles;
    public static MapManager instance;

    private const float defaultSpeed = 2f;


    private void Awake()
    {
        instance = this;

        dataFromTiles = new Dictionary<TileBase, TileData>();

        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData); // save data of tiles
            }
        }
    }

    public float GetWalkingSpeed(Vector3 worldPosition){
        Vector3Int gridPosition = map.WorldToCell(worldPosition);

        TileBase tile = map.GetTile(gridPosition);

        if (tile == null)
            return 1f;

        if (dataFromTiles.ContainsKey(tile)==false){
            return defaultSpeed;
        }

        float walkingSpeed = dataFromTiles[tile].walkSpeed;

        return walkingSpeed;
    }

    public bool Plantable(Vector3 worldPosition){
        Vector3Int gridPosition = map.WorldToCell(worldPosition);

        TileBase tile = map.GetTile(gridPosition);

        if (tile == null)
            return false;

        bool plantable = dataFromTiles[tile].plantable;

        return plantable;       
    }


}
