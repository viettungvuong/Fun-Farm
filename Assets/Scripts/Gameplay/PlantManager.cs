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
    }

    void Start(){
        plantFromTiles = new Dictionary<Vector3Int, Plant>();
        lastLevelTime = new Dictionary<Plant, DateTime>();
    }

    private void FixedUpdate() {
        DateTime now = DateTime.Now;

        // update in temp dictionary
        var updates = new Dictionary<Plant, DateTime>();

        foreach (var entry in lastLevelTime)
        {
            Plant plant = entry.Key;

            if (plant.currentStage >= maxStage)
            {
                continue; // max level so do nothing
            }

            DateTime lastTime = entry.Value;

            double secondsDifference = (now - lastTime).TotalSeconds;

            if (secondsDifference > plant.levelUpTime)
            {
                plant.currentStage++; // level up
                map.SetTile(plant.gridPosition, plant.tiles[plant.currentStage]);
                updates[plant] = now; // collect the update
            }
        }

        // change update to dictionary
        foreach (var update in updates)
        {
            lastLevelTime[update.Key] = update.Value;
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
