using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BuyLand : MonoBehaviour
{
    public static BuyLand Instance { get; private set; } // Singleton instance

    private Tilemap expandableGroundTilemap, decorMap;
    public TileBase newTile;
    public GameObject buyLandPanel;
    public Vector3Int? currentCellPosition;

    // Store purchased tile positions
    private List<Vector3Int> purchasedTilePositions = new List<Vector3Int>();
    public BuyLandData Serialize(){
        BuyLandData buyLandData = new BuyLandData
        {
            purchased = purchasedTilePositions
        };

        return buyLandData;
    }

    public void Reload(BuyLandData buyLandData){
        InitializeMap();

        purchasedTilePositions = buyLandData.purchased;

        Redraw();
    }

    private void Redraw(){
        foreach (Vector3Int position in purchasedTilePositions)
        {
            expandableGroundTilemap.SetTile(position, newTile);

            Vector3 vt3 = expandableGroundTilemap.CellToWorld(position);
            Vector3Int decorPos = decorMap.WorldToCell(vt3); // remove decor when purchasing a land

            decorMap.SetTile(decorPos, null);
        }
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
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
        if (GameController.HomeScene()){
            expandableGroundTilemap = GameObject.Find("ExpandableGround").GetComponent<Tilemap>();
            decorMap = GameObject.Find("Decor").GetComponent<Tilemap>();
            Redraw();
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (!GameController.HomeScene())
        {
            return;
        }

        Vector3Int currentCellPosition = expandableGroundTilemap.WorldToCell(transform.position);

        if (CanBuyLand(currentCellPosition))
        {
            buyLandPanel.SetActive(true);
            this.currentCellPosition = currentCellPosition;
        }
        else
        {
            buyLandPanel.SetActive(false);
            this.currentCellPosition = null;
        }
    }

    bool CanBuyLand(Vector3Int currentCellPosition)
    {
        Vector3Int leftCellPosition = currentCellPosition + Vector3Int.left;
        TileBase leftTile = expandableGroundTilemap.GetTile(leftCellPosition);
        TileBase currentTile = expandableGroundTilemap.GetTile(currentCellPosition);

        return leftTile != null && currentTile == null; // can buy land if the left tile is of expandableGround and current tile is null (not bought)
    }

    public bool BuyLandHere()
    {
        if (!currentCellPosition.HasValue)
        {
            return false;
        }

        expandableGroundTilemap.SetTile(currentCellPosition.Value, newTile);

        Vector3 vt3 = expandableGroundTilemap.CellToWorld(currentCellPosition.Value);
        Vector3Int decorPos = decorMap.WorldToCell(vt3);

        decorMap.SetTile(decorPos, null);

        // save vector3int of purchased tiles
        purchasedTilePositions.Add(currentCellPosition.Value);


        return true;
    }
}
