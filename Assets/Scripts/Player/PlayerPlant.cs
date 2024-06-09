using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerPlant : MonoBehaviour
{
    public Tilemap plantTilemap;

    public void PlantTree(Vector3 worldPosition, Plant plant){
        bool plantable = MapManager.instance.Plantable(worldPosition);
        if (!plantable){
            return;
        }
        Vector3Int cellPosition = plantTilemap.WorldToCell(worldPosition);
        plant.gridPosition = cellPosition; // store position
        plantTilemap.SetTile(cellPosition, plant.tiles[0]);

        PlantManager.instance.AddPlant(worldPosition, plant);
    }
}
