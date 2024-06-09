using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerPlant : MonoBehaviour
{
    public Tilemap plantTilemap;

    public void PlantTree(Vector3 worldPosition, Tile plantTile){
        bool plantable = MapManager.instance.Plantable(worldPosition);
        if (!plantable){
            return;
        }
        Vector3Int cellPosition = plantTilemap.WorldToCell(worldPosition); 
        plantTilemap.SetTile(cellPosition, plantTile);

    }
}
