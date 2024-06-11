using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantManager : MonoBehaviour
{
    [SerializeField] private Tilemap map;


    private Dictionary<Vector3Int, Plant> plantPos; // position of plants
    private Dictionary<Plant, DateTime> lastLevelTime, lastWateredTime; // last time this plant was leveled and planted
    public static PlantManager instance;
    private int maxStage = 2;
    private const int plantDamage = 20;


    private void Awake()
    {
        instance = this;
    }

    void Start(){
        plantPos = new Dictionary<Vector3Int, Plant>();
        lastLevelTime = new Dictionary<Plant, DateTime>();
        lastWateredTime = new Dictionary<Plant, DateTime>();
    }

    private void FixedUpdate() {
        CheckPlantLevel();
        CheckDeterioration(plantDamage);
    }

    private void CheckPlantLevel(){
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

    private void CheckDeterioration(int damage){
        DateTime now = DateTime.Now;

        // update in temp dictionary
        var updates = new Dictionary<Plant, DateTime>();

        foreach (var entry in lastWateredTime)
        {
            Plant plant = entry.Key;

            if (plant.health <= 0)
            {
                continue; // max level so do nothing
            }

            DateTime lastTime = entry.Value;

            double secondsDifference = (now - lastTime).TotalSeconds;

            if (secondsDifference > plant.deteriorateTime)
            {
                plant.health -= damage; // reduce health of plant
            }
        }
    }

    public int GetPlantLevel(Vector3 worldPosition){
        Vector3Int gridPosition = map.WorldToCell(worldPosition);

        if (gridPosition == null||plantPos.ContainsKey(gridPosition)==false)
            return -1;

        return plantPos[gridPosition].currentStage;
    }

    public bool AddPlant(Vector3 worldPosition, Plant plant){
        Vector3Int gridPosition = map.WorldToCell(worldPosition);

        if (gridPosition == null)
            return false;

        // check whether another plant is here later

        plantPos.Add(gridPosition, plant);
        lastLevelTime.Add(plant, DateTime.Now);
        lastWateredTime.Add(plant, DateTime.Now);
        return true; 
    }

    public bool WaterPlant(Vector3 worldPosition){
        Vector3Int gridPosition = map.WorldToCell(worldPosition);

        if (gridPosition == null)
            return false;

        if (plantPos.ContainsKey(gridPosition)==false||plantPos[gridPosition]==null){
            return false; // no plant here to water
        }

        Plant plant = plantPos[gridPosition]; // get plant at position

        if (lastWateredTime.ContainsKey(plant)==false){
            lastWateredTime.Add(plant, DateTime.Now);
        }
        else{
            lastWateredTime[plant] = DateTime.Now;
        }
        return true;
    }
}
