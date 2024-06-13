using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlantManager : MonoBehaviour
{
    [SerializeField] private Tilemap map;
    public Tile[] plantTiles;

    private Dictionary<Vector3Int, Plant> plantPos; // position of plants
    private Dictionary<Plant, DateTime> lastLevelTime, lastCheckFreshTime; 
    // last time this plant was leveled and watered
    public static PlantManager instance;
    private int maxStage = 4;
    private const int plantDamage = 50;
    public Slider healthSliderPrefab;


    private void Awake()
    {
        instance = this;
    }

    void Start(){
        plantPos = new Dictionary<Vector3Int, Plant>();
        lastLevelTime = new Dictionary<Plant, DateTime>();
        lastCheckFreshTime = new Dictionary<Plant, DateTime>();
    }

    private void FixedUpdate() {
        CheckPlantLevel();
        CheckDeterioration(plantDamage);
    }

    private void ColorPlant(Plant plant, Color color){
        TileBase tile = map.GetTile(plant.gridPosition);
        
        if (tile != null)
        {
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            
            TilemapRenderer tilemapRenderer = map.GetComponent<TilemapRenderer>();
            tilemapRenderer.GetPropertyBlock(materialPropertyBlock);

            // set black color for deterioration
            materialPropertyBlock.SetColor("_Color", color);
        }
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
    public bool Planted(Vector3 worldPosition){
        Vector3Int gridPosition = map.WorldToCell(worldPosition);

        TileBase tile = map.GetTile(gridPosition);

        return tile != null;
    }

    public bool Planted(Vector3Int cellPosition){

        TileBase tile = map.GetTile(cellPosition);

        return tile != null;
    }

    public DateTime? GetLastTimeWatered(Plant plant){
        if (lastCheckFreshTime.ContainsKey(plant)==false){
            return null;
        }
        else{
            return lastCheckFreshTime[plant];
        }
    }

    public List<Vector3Int> FindAllPlants()
    {
        BoundsInt bounds = map.cellBounds;
        List<Vector3Int> positions = new List<Vector3Int>();

        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                TileBase tile = map.GetTile(cellPosition);
                
                if (plantTiles.Contains(tile)==true)
                {
                    positions.Add(cellPosition); // this position has plant
                }
                // if (tile!=null)
                // {
                //     positions.Add(cellPosition); // this position has plant
                // }
            }
        }

        return positions;
    }

    private void CheckDeterioration(int damage){
        DateTime now = DateTime.Now;

        // update in temp dictionary
        var updates = new Dictionary<Plant, DateTime>();

        foreach (var entry in lastCheckFreshTime)
        {
            Plant plant = entry.Key;

            if (plant.health <= 0)
            {
                continue; // die
            }

            DateTime lastTime = entry.Value;

            double secondsDifference = (now - lastTime).TotalSeconds;

            if (secondsDifference > plant.deteriorateTime)
            {
                DamagePlant(plant, damage);

                updates[plant] = DateTime.Now; // collect the update

            }
        }

        // change update to dictionary
        foreach (var update in updates)
        {
            lastLevelTime[update.Key] = update.Value;
        }
    }

    public void DamagePlant(Plant plant, int damage){
        plant.health -= damage; // reduce health of plant
        ColorPlant(plant, Color.black);

        if (plant.health<=0){

        }
    }

    public Plant GetPlantAt(Vector3 worldPosition){
        if (!Planted(worldPosition)){
            return null;
        }

        else{
            return plantPos[map.WorldToCell(worldPosition)];
        }
    }

    public Plant GetPlantAt(Vector3Int cellPosition){
        if (!Planted(map.CellToWorld(cellPosition))){
            return null;
        }

        else{
            return plantPos[cellPosition];
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
        lastCheckFreshTime.Add(plant, DateTime.Now);

        PlantHealthBar plantHealthBar = gameObject.AddComponent<PlantHealthBar>();
        plantHealthBar.Initialize(plant, map, healthSliderPrefab); // add another plant health bar

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

        if (lastCheckFreshTime.ContainsKey(plant)==false){
            lastCheckFreshTime.Add(plant, DateTime.Now);
        }
        else{
            lastCheckFreshTime[plant] = DateTime.Now;
        }
        plant.health += plantDamage; 
        
        ColorPlant(plant, Color.white); // fresh plant again

        return true;
    }
}
