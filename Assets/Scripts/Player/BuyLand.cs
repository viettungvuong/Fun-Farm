using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BuyLand : MonoBehaviour
{
    public Tilemap expandableGroundTilemap;
    public TileBase newTile;
    public GameObject buyLandPanel; // Reference to the buy land panel

    // Update is called once per frame
    void Update()
    {
        Vector3Int currentCellPosition = expandableGroundTilemap.WorldToCell(transform.position);

        if (CanBuyLand(currentCellPosition))
        {
            buyLandPanel.SetActive(true);

            // if (Input.GetKeyDown(KeyCode.B))
            // {
            //     BuyLandAt(targetCellPosition);
            //     buyLandPanel.SetActive(false); 
            // }
        }
        else
        {
            buyLandPanel.SetActive(false);
        }
    }

    bool CanBuyLand(Vector3Int currentCellPosition)
    {
        Vector3Int leftCellPosition = currentCellPosition + Vector3Int.left;
        TileBase leftTile = expandableGroundTilemap.GetTile(leftCellPosition);
        TileBase currentTile = expandableGroundTilemap.GetTile(currentCellPosition);

        return leftTile != null && currentTile == null; // can buy land if the left tile is of expandeableGround
        // and current tile is null (not bought)
    }

    void BuyLandAt(Vector3Int cellPosition)
    {
        expandableGroundTilemap.SetTile(cellPosition, newTile);
    }
}