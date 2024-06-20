using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    private Tilemap map;

    [SerializeField] private List<TileData> tileDatas;

    private Dictionary<TileBase, TileData> dataFromTiles;
    public static MapManager instance;

    private const float defaultSpeed = 2f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() {
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
        Vector3Int gridPosition = map.WorldToCell(worldPosition);

        TileBase tile = map.GetTile(gridPosition);

        if (tile == null)
            return 1f;

        if (dataFromTiles.ContainsKey(tile) == false)
        {
            return defaultSpeed;
        }

        float walkingSpeed = dataFromTiles[tile].walkSpeed;

        return walkingSpeed;
    }

    public bool Plantable(Vector3 worldPosition)
    {
        Vector3Int gridPosition = map.WorldToCell(worldPosition);

        TileBase tile = map.GetTile(gridPosition);

        if (tile == null || dataFromTiles.ContainsKey(tile)==false)
            return false;

        bool plantable = dataFromTiles[tile].plantable;

        return plantable;
    }
}
