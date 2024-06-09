using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantManager : MonoBehaviour
{
    [SerializeField] private Tilemap map;


    private Dictionary<Vector3Int, Plant> plantFromTiles; // position of plants
    private Dictionary<Plant, DateTime> lastLevelTime; // last time this plant was updated
    public static PlantManager instance;
    private int maxStage = 2;


    private void Awake()
    {
        instance = this;

        plantFromTiles = new Dictionary<Vector3Int, Plant>();
        lastLevelTime = new Dictionary<Plant, DateTime>();
    }

    private void LateUpdate() {
        DateTime now = DateTime.Now;
        
        foreach (var entry in lastLevelTime)
        {
            Plant plant = entry.Key;

            if (plant.currentStage>=maxStage){
                continue; // max level so do nothing
            }

            DateTime lastTime = entry.Value;

            double secondsDifference = (now - lastTime).TotalSeconds;
            Debug.Log(secondsDifference);

            if (secondsDifference>plant.levelUpTime){
                plant.currentStage++; // level up
                map.SetTile(plant.gridPosition, plant.tiles[plant.currentStage]);
                lastLevelTime[plant] = now; // reupdate last level time
            }
        }
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
        lastLevelTime.Add(plant, DateTime.Now);       
    }
}
