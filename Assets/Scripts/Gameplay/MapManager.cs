using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    private Tilemap map, expandableMap;

    [SerializeField] private List<TileData> tileDatas;

    private Dictionary<TileBase, TileData> dataFromTiles;
    public static MapManager instance;

    private const float defaultSpeed = 1f;

    private void Awake()
    {
        DontDestroyOnLoadManager.DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeMap();
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
        map = GameObject.Find("Ground").GetComponent<Tilemap>();

        if (GameController.HomeScene()){
            expandableMap = GameObject.Find("ExpandableGround").GetComponent<Tilemap>();
        }
        else{
            expandableMap = null;
        }


        dataFromTiles = new Dictionary<TileBase, TileData>();

        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData); // save data of tiles
            }
        }
    }

    public float GetWalkingSpeed(Vector3 worldPosition)
    {
       if (expandableMap==null){
            Vector3Int gridPosition = map.WorldToCell(worldPosition);

            TileBase tile = map.GetTile(gridPosition);

            if (tile == null)
            {
                return defaultSpeed;
            }

            if (!dataFromTiles.ContainsKey(tile))
            {
                return defaultSpeed;
            }


            return dataFromTiles[tile].walkSpeed;
        }
        else{
            Vector3Int gridPosition = expandableMap.WorldToCell(worldPosition);

            TileBase tile = expandableMap.GetTile(gridPosition);

            if (tile == null)
            {
                // if tile is not found in expandableMap, try map
                gridPosition = map.WorldToCell(worldPosition);
                tile = map.GetTile(gridPosition);

                if (tile == null || !dataFromTiles.ContainsKey(tile))
                    return defaultSpeed;
            }

            if (!dataFromTiles.ContainsKey(tile))
            {
                return defaultSpeed;
            }


            return dataFromTiles[tile].walkSpeed;
        }

    }

    public Color GetTrailColor(Vector3 worldPosition)
    {
        if (expandableMap==null){
            Vector3Int gridPosition = map.WorldToCell(worldPosition);

            TileBase tile = map.GetTile(gridPosition);

            if (tile == null)
            {
                return Color.white;
            }

            if (!dataFromTiles.ContainsKey(tile))
            {
                return Color.white;
            }

            Color dustTrailColor = dataFromTiles[tile].trailColor;

            return dustTrailColor;
        }
        else{
            Vector3Int gridPosition = expandableMap.WorldToCell(worldPosition);

            TileBase tile = expandableMap.GetTile(gridPosition);

            if (tile == null)
            {
                // if tile is not found in expandableMap, try map
                gridPosition = map.WorldToCell(worldPosition);
                tile = map.GetTile(gridPosition);

                if (tile == null||!dataFromTiles.ContainsKey(tile))
                    return Color.white;
            }

            if (!dataFromTiles.ContainsKey(tile))
            {
                return Color.white;
            }

            Color dustTrailColor = dataFromTiles[tile].trailColor;

            return dustTrailColor;
        }

    }

    public bool Plantable(Vector3 worldPosition)
    {
        if (GameController.HomeScene()==false){
            return false;
        }
        Vector3Int gridPosition = expandableMap.WorldToCell(worldPosition);

        TileBase tile = expandableMap.GetTile(gridPosition);

        if (tile == null || !dataFromTiles.ContainsKey(tile))
            return false;

        bool plantable = dataFromTiles[tile].plantable;

        return plantable;
    }
}
