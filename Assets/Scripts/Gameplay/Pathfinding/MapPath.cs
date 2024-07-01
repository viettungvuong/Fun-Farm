using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class MapPath : MonoBehaviour
{
    private Tilemap groundTilemap;
    public LayerMask obstacleLayer;
    private Dictionary<Vector3Int, Node> grid;
    public static MapPath instance;

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

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeMap();
        InitializeGrid();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeMap();
        InitializeGrid();
    }

    private void InitializeMap()
    {
        groundTilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
    }

    void InitializeGrid()
    {
        grid = new Dictionary<Vector3Int, Node>();
        BoundsInt bounds = groundTilemap.cellBounds;
        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                Vector3 worldPosition = groundTilemap.CellToWorld(position);

                bool isWalkable = !Physics2D.OverlapCircle(worldPosition, 0.3f, obstacleLayer);
                
                grid[position] = new Node(position, isWalkable);
            }
        }
    }

    public Node GetNode(Vector3Int position)
    {
        grid.TryGetValue(position, out Node node);
        return node;
    }

    public Node GetNode(Vector3 position)
    {
        Vector3Int cellPosition = groundTilemap.WorldToCell(position);
        return GetNode(cellPosition);
    }
}