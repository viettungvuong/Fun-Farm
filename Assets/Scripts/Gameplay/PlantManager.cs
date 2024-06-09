using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantManager : MonoBehaviour
{
    [SerializeField] private Tilemap map;

    private Dictionary<Vector3Int, Plant> plantFromTiles; // position of plants
    public static PlantManager instance;


    private void Awake()
    {
        instance = this;

        plantFromTiles = new Dictionary<Vector3Int, Plant>();
    }

    public int GetPlantLevel(Vector3 worldPosition){
        Vector3Int gridPosition = map.WorldToCell(worldPosition);

        if (gridPosition == null||plantFromTiles.ContainsKey(gridPosition)==false)
            return -1;

        return plantFromTiles[gridPosition].currentStage;
    }

    public void AddPlant(Vector3 worldPosition, Plant plant){
        Vector3Int gridPosition = map.WorldToCell(worldPosition);

        if (gridPosition == null)
            return;

        // check whether another plant is here later

        plantFromTiles.Add(gridPosition, plant);      
    }
}
