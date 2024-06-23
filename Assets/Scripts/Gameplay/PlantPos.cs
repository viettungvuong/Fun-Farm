using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class PlantPos : MonoBehaviour
{
    private Dictionary<Vector3Int, PlantedPlant> matrix;
    private Tilemap plantTilemap;
    public static PlantPos instance;

    private void Awake() {
        if (instance==null){
            instance = this;
        }
        else{
            Destroy(this);
        }
    }

    private void Start() {
        plantTilemap = GameObject.Find("PlantTilemap").GetComponent<Tilemap>();

        matrix = new Dictionary<Vector3Int, PlantedPlant>();
    }

    public void AddPlant(Vector3Int cellPosition, PlantedPlant plant){
        if (matrix.ContainsKey(cellPosition)==false){
            matrix.Add(cellPosition, plant);
        }else{
            matrix[cellPosition] = plant;
        }

    }

    public void RemovePlant(Vector3Int cellPosition){
        if (matrix.ContainsKey(cellPosition)==false){
            return;
        }else{
            matrix.Remove(cellPosition);
        }
    }

    public void LoadToTilemap(){
        plantTilemap.ClearAllTiles();

        foreach (var entry in matrix)
        {
            Vector3Int cellPosition = entry.Key;
            PlantedPlant plant = entry.Value;

            if (plant != null)
            {
                plantTilemap.SetTile(cellPosition, plant.tiles[plant.currentStage]);
            }
        }
    }

    
}
