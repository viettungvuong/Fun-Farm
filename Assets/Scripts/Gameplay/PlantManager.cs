using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[Serializable]
public class PlantManager : MonoBehaviour
{
    private Tilemap plantMap;

    private Dictionary<Vector3Int, PlantedPlant> plantPos; // position of plants
    private Dictionary<PlantedPlant, DateTime> lastLevelTime, lastCheckFreshTime;
    private Dictionary<PlantedPlant, PlantHealthBar> plantHealthBars;
    // last time this plant was leveled and watered

    public static PlantManager instance;

    public Slider healthSliderPrefab;


    private void Awake()
    {
        if (instance==null){
            instance = this;
        }
        else{
            Destroy(this);
        }


    }

    private void Start() {
        plantPos = new Dictionary<Vector3Int, PlantedPlant>();
        lastLevelTime = new Dictionary<PlantedPlant, DateTime>();
        lastCheckFreshTime = new Dictionary<PlantedPlant, DateTime>();
        plantHealthBars = new Dictionary<PlantedPlant, PlantHealthBar>();

 
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
            plantMap = GameObject.Find("PlantTilemap").GetComponent<Tilemap>();

            foreach (var entry in plantPos)
            {
                Vector3Int position = entry.Key;
                PlantedPlant plant = entry.Value;

                // reput plant 
                plantMap.SetTile(position,plant.tiles[plant.currentStage]);

                // reinitialize plant health bar
                PlantHealthBar plantHealthBar = gameObject.AddComponent<PlantHealthBar>();
                plantHealthBar.Initialize(plant, plantMap, healthSliderPrefab); // add another plant health bar
                plantHealthBars.Add(plant, plantHealthBar);
            }
        }

    }


    private void FixedUpdate() {
        CheckPlantLevel();
        CheckDeterioration();
    }

    private void ColorPlant(PlantedPlant plant, Color color){

        plantMap.RemoveTileFlags(plant.gridPosition, TileFlags.LockColor);
        plantMap.SetColor(plant.gridPosition, color);

    }


    private void CheckPlantLevel(){

        DateTime now = DateTime.Now;

        // update in temp dictionary
        var updates = new Dictionary<PlantedPlant, DateTime>();

        foreach (var entry in lastLevelTime)
        {
            PlantedPlant plant = entry.Key;

            if (plant.currentStage >= plant.maxStage)
            {
                continue; // max level so do nothing
            }

            DateTime lastTime = entry.Value;

            double secondsDifference = (now - lastTime).TotalSeconds;

            if (secondsDifference > plant.levelUpTime)
            {
                plant.currentStage++; // level up
                plantMap.SetTile(plant.gridPosition, plant.tiles[plant.currentStage]);
                updates[plant] = now; // collect the update
            }
        }

        foreach (var update in updates)
        {
            lastLevelTime[update.Key] = update.Value;
        }
    }
    public bool Planted(Vector3 worldPosition){
        Vector3Int gridPosition = plantMap.WorldToCell(worldPosition);

        return plantPos.ContainsKey(gridPosition);
    }

    public bool Planted(Vector3Int cellPosition){
        return plantPos.ContainsKey(cellPosition);
    }

    public DateTime? GetLastTimeWatered(PlantedPlant plant){
        if (lastCheckFreshTime.ContainsKey(plant)==false){
            return null;
        }
        else{
            return lastCheckFreshTime[plant];
        }
    }

    public List<Vector3Int> FindAllPlants(bool notIncludeMax=false)
    {
        BoundsInt bounds = plantMap.cellBounds;
        List<Vector3Int> positions = new List<Vector3Int>();

        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y);
                // TileBase tile = map.GetTile(cellPosition);
                
                if (plantPos.ContainsKey(cellPosition))
                {
                    if (notIncludeMax){
                        PlantedPlant plant = GetPlantAt(cellPosition);
                        if (plant.currentStage==plant.maxStage){
                            continue; // not include plant at max stage
                        }
                    }
                    positions.Add(cellPosition); // this position has plant
                }
            }
        }

        return positions;
    }

    private void CheckDeterioration(){
        DateTime now = DateTime.Now;

        // update in temp dictionary
        var updates = new Dictionary<PlantedPlant, DateTime>();

        List<PlantedPlant> plantsToRemove = new List<PlantedPlant>();

        foreach (var entry in lastCheckFreshTime)
        {
            PlantedPlant plant = entry.Key;

            if (plant.health <= 0 || plant.currentStage == plant.maxStage)
            {
                continue; // die or max stage
            }

            DateTime lastTime = entry.Value;

            double secondsDifference = (now - lastTime).TotalSeconds;

            if (secondsDifference > plant.deteriorateTime)
            {
                DamagePlant(plant);
                plantsToRemove.Add(plant);
            }
        }

        foreach (PlantedPlant plant in plantsToRemove)
        {
            lastCheckFreshTime.Remove(plant);
            lastLevelTime.Remove(plant);
        }
    }

    public bool DetectPlant(Vector3Int cellPosition){
        TileBase tile = plantMap.GetTile(cellPosition);

        if (tile != null)
        {
            PlantedPlant plant = GetPlantAt(cellPosition);

            return plant != null;
        }
        else{
            return false;
        }
    }

    public bool DetectPlant(Vector3 worldlPosition){
        Vector3Int cellPosition = plantMap.WorldToCell(worldlPosition);
        TileBase tile = plantMap.GetTile(cellPosition);

        if (tile != null)
        {
            PlantedPlant plant = GetPlantAt(cellPosition);

            return plant != null;
        }
        else{
            return false;
        }
    }

    public bool DetectPlantMaxStage(Vector3Int cellPosition){
        TileBase tile = plantMap.GetTile(cellPosition);

        if (tile != null)
        {
            PlantedPlant plant = GetPlantAt(cellPosition);

            return plant != null && plant.currentStage == plant.maxStage;
        }
        else{
            return false;
        }
    }

    public bool DetectPlantMaxStage(Vector3 worldlPosition){
        Vector3Int cellPosition = plantMap.WorldToCell(worldlPosition);
        TileBase tile = plantMap.GetTile(cellPosition);

        if (tile != null)
        {
            PlantedPlant plant = GetPlantAt(cellPosition);

            return plant != null && plant.currentStage == plant.maxStage;
        }
        else{
            return false;
        }
    }

    public void RemovePlant(PlantedPlant plant, bool removeOnMap=false){
        plantPos.Remove(plant.gridPosition); // remove plant

        if (removeOnMap){
            plantMap.SetTile(plant.gridPosition, null);
            PlantPos.instance.RemovePlant(plant.gridPosition);
        }

    }

    public void DamagePlant(PlantedPlant plant){
        ColorPlant(plant, Color.black);

        RemovePlant(plant);
        plantHealthBars[plant].healthSlider.gameObject.SetActive(false); // plant die => disable health slider
        plantHealthBars[plant].gameObject.SetActive(false);
        plantHealthBars.Remove(plant);
    }

    public PlantedPlant GetPlantAt(Vector3 worldPosition){
        if (!Planted(worldPosition)){
            return null;
        }

        else{
            return plantPos[plantMap.WorldToCell(worldPosition)];
        }
    }

    public PlantedPlant GetPlantAt(Vector3Int cellPosition){
        if (!Planted(plantMap.CellToWorld(cellPosition))){
            return null;
        }

        else{
            return plantPos[cellPosition];
        }
    }

    public int GetPlantLevel(Vector3 worldPosition){
        Vector3Int gridPosition = plantMap.WorldToCell(worldPosition);

        if (gridPosition == null||plantPos.ContainsKey(gridPosition)==false)
            return -1;

        return plantPos[gridPosition].currentStage;
    }

    public bool AddPlant(Vector3 worldPosition, PlantedPlant plant){
        Vector3Int gridPosition = plantMap.WorldToCell(worldPosition);

        if (gridPosition == null)
            return false;

        // check whether another plant is here later

        plantPos.Add(gridPosition, plant);
        lastLevelTime.Add(plant, DateTime.Now);
        lastCheckFreshTime.Add(plant, DateTime.Now);

        PlantPos.instance.AddPlant(gridPosition, plant); // add to serializable matrix

        PlantHealthBar plantHealthBar = gameObject.AddComponent<PlantHealthBar>();
        plantHealthBar.Initialize(plant, plantMap, healthSliderPrefab); // add another plant health bar
        plantHealthBars.Add(plant, plantHealthBar);

        return true; 
    }

    public bool WaterPlant(Vector3 worldPosition){
        Vector3Int gridPosition = plantMap.WorldToCell(worldPosition);

        if (gridPosition == null)
            return false;

        if (plantPos.ContainsKey(gridPosition)==false||plantPos[gridPosition]==null){
            return false; // no plant here to water
        }

        PlantedPlant plant = plantPos[gridPosition]; // get plant at position

        if (lastCheckFreshTime.ContainsKey(plant)==false){
            lastCheckFreshTime.Add(plant, DateTime.Now);
        }
        else{
            lastCheckFreshTime[plant] = DateTime.Now;
        }
        
        ColorPlant(plant, Color.white); // fresh plant again

        return true;
    }

    public int GetNumberOfPlants(){
        return plantPos.Keys.ToList().Count;
    }
}
