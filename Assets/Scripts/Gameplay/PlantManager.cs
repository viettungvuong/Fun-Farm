using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlantManager : MonoBehaviour
{
    private Tilemap plantMap;

    private Dictionary<Vector3Int, PlantedPlant> plantPos; // position of plants
    private Dictionary<PlantedPlant, Pair> nextLevelTime, nextDeteriorate;
    private Dictionary<PlantedPlant, PlantHealthBar> plantHealthBars;
    // last time this plant was leveled and watered

    public static PlantManager instance;

    public Slider healthSliderPrefab;

    public static bool firstOpen = true;


    private void Awake()
    {
        if (instance==null){
            instance = this;
        }
        else{
            Destroy(this);
        }
        plantPos = new Dictionary<Vector3Int, PlantedPlant>();
        nextLevelTime = new Dictionary<PlantedPlant, Pair>();
        nextDeteriorate = new Dictionary<PlantedPlant, Pair>();
        plantHealthBars = new Dictionary<PlantedPlant, PlantHealthBar>();

    }

    private void Start() {
 
        SceneManager.sceneLoaded += OnSceneLoaded;

        InitializeMap();
        firstOpen = false;

        currentDeteriorateMin = TimeManage.instance.currentMinute;
    }


    private void OnDestroy()
    {
 
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
 
        InitializeMap();
    }


    public void Load(){
        PlantPos plantPos = PlantPos.instance;

        this.plantPos = new Dictionary<Vector3Int, PlantedPlant>();
        this.nextDeteriorate = new Dictionary<PlantedPlant, Pair>();
        this.nextLevelTime = new Dictionary<PlantedPlant, Pair>();
        this.plantHealthBars = new Dictionary<PlantedPlant, PlantHealthBar>();
        

        foreach (var entry in plantPos.SerializedPlantPos) // load plants
        {
            Vector3Int cellPosition = entry.Key;
            PlantedPlant plant = entry.Value;

            plant.LoadTilesFromPaths();

            this.plantPos.Add(cellPosition, plant); // add to plant pos with updated tiles

            if (!plantPos.SerializednextLevelTime.ContainsKey(plant)||!plantPos.SerializednextDeteriorate.ContainsKey(plant)){
                this.plantPos.Remove(cellPosition);
                plantPos.RemovePlant(plant, cellPosition);
                continue;
            }
            Pair nextLevel = plantPos.SerializednextLevelTime[plant];

            nextLevelTime.Add(plant, nextLevel); 
            // plantPos.LevelPlant(plant, nextLevel);

            Pair nextDeteriorateTime = plantPos.SerializednextDeteriorate[plant];
            nextDeteriorate.Add(plant, nextDeteriorateTime);
            // plantPos.HealthPlant(plant, nextDeteriorateTime);

            plantMap = GameObject.Find("PlantTilemap").GetComponent<Tilemap>(); // draw on tilemap
            plantMap.SetTile(plant.gridPosition, plant.tiles[plant.currentStage]);
  

            PlantHealthBar plantHealthBar = gameObject.AddComponent<PlantHealthBar>();
            plantHealthBar.Initialize(plant, plantMap, healthSliderPrefab); // add healthbar to plant
            plantHealthBars.Add(plant, plantHealthBar);
        
        }

        // plantPos.LoadToTilemap(); // redraw tilemap

    }


    private void InitializeMap()
    {
        if (GameController.HomeScene()){
            plantMap = GameObject.Find("PlantTilemap").GetComponent<Tilemap>();

            if (firstOpen==false){ // first open then don't load these
                foreach (var entry in plantPos)
                {
                    Vector3Int position = entry.Key;
                    PlantedPlant plant = entry.Value;

                    // reput plant 
                    plantMap.SetTile(position,plant.tiles[plant.currentStage]);

                    // // reinitialize plant health bar
                    PlantHealthBar plantHealthBar = gameObject.AddComponent<PlantHealthBar>();
                    plantHealthBar.Initialize(plant, plantMap, healthSliderPrefab); // add another plant health bar
                    if (plantHealthBars.ContainsKey(plant)==false){
                        plantHealthBars.Add(plant, plantHealthBar);
                    }
                    else{
                        plantHealthBars[plant] = plantHealthBar;
                    }

                }
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

    private Pair AddTime(Pair time, int nextTime)
    {
        int currentHours = time.First;
        int currentMinutes = time.Second;

        int totalMinutes = currentHours * 60 + currentMinutes + nextTime;

        // Calculate new hours and minutes
        int newHours = (totalMinutes / 60) % 24; 
        int newMinutes = totalMinutes % 60;      

        return new Pair(newHours, newMinutes);
    }

    #region plantlevel
    private void CheckPlantLevel()
    {
        // update in temp dictionary
        var updates = new Dictionary<PlantedPlant, Pair>();

        foreach (var entry in nextLevelTime)
        {
            PlantedPlant plant = entry.Key;

            try
            {
                if (plant.currentStage >= plant.maxStage)
                {
                    
                    continue; // max level so do nothing
                }

                Pair nextTime = entry.Value;

                if (TimeManage.instance.currentHour == nextTime.First &&
                TimeManage.instance.currentMinute == nextTime.Second)
                {
                    plant.currentStage++;
                    Pair current = new Pair(TimeManage.instance.currentHour, TimeManage.instance.currentMinute);
                    if (plant.currentStage <=  plant.maxStage)
                    {
                        plantMap.SetTile(plant.gridPosition, plant.tiles[plant.currentStage]);
                        Pair next = AddTime(current, plant.levelUpTime);
                        PlantPos.instance.LevelPlant(plant, next);
                        updates[plant] = next; // update next level time

                        if (plant.currentStage == plant.maxStage)
                        {
                            PlantTilemapShader tileShaderManager = GetComponent<PlantTilemapShader>();
                            Debug.Log("FINAL TILE: 1 -> " + plant.tiles[plant.currentStage]);
                            // Tile finalTile = Resources.Load<Tile>(plant.tiles[plant.currentStage].name + "_Harvest.asset");
                            // Debug.Log("FINAL TILE: 2 -> " + finalTile);
                            plantMap.SetTile(plant.gridPosition, null); // delete from map for the max plant shader
                            tileShaderManager.ApplyShaderToTile(plant, "PlantMax", 2);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error updating plant level: {e.Message}");
            }
        }

        foreach (var update in updates)
        {
            nextLevelTime[update.Key] = update.Value;
        }
    }
    #endregion

    public bool Planted(Vector3 worldPosition){
        Vector3Int gridPosition = plantMap.WorldToCell(worldPosition);

        return plantPos.ContainsKey(gridPosition);
    }

    public bool Planted(Vector3Int cellPosition){
        return plantPos.ContainsKey(cellPosition);
    }

    public Pair GetnextTimeDeteriorate(PlantedPlant plant){
        if (nextDeteriorate.ContainsKey(plant)==false){
            return null;
        }
        else{
            return nextDeteriorate[plant];
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

    #region deteriorate
    private static int currentDeteriorateMin; // ensure rain deteriorate delay only apply once per min
    private void CheckDeterioration(){
        // update in temp dictionary

        List<PlantedPlant> plantsToRemove = new List<PlantedPlant>();
        Dictionary<PlantedPlant, Pair> updatedNextDeteriorate = new Dictionary<PlantedPlant, Pair>();

        foreach (var entry in nextDeteriorate)
        {
            try {
                PlantedPlant plant = entry.Key;

                if (plant.health <= 0 || plant.currentStage == plant.maxStage)
                {
                    continue; // die or max stage
                }

                Pair nextTime = entry.Value;

                if (currentDeteriorateMin!=TimeManage.instance.currentMinute&&Weather.instance.currentWeather == WeatherType.Rainy) // rainy then delay deteriorate
                {
                    nextTime.Second += 1;
                    if (nextTime.Second >= 60)
                    {
                        nextTime.Second -= 60;
                        nextTime.First += 1;
                        if (nextTime.First >= 24)
                        {
                            nextTime.First -= 24;
                        }
                    }
                    updatedNextDeteriorate[plant] = nextTime;
                }

                if (TimeManage.instance.currentHour == nextTime.First &&
                TimeManage.instance.currentMinute == nextTime.Second){
                    DamagePlant(plant); // remove plant sau
                    plantsToRemove.Add(plant);
                    PlantPos.instance.RemovePlant(plant, plant.gridPosition);
                } 
            } catch (Exception err){
                Debug.LogError($"Error updating plant water: {err.Message}");
            }
            
        }

        currentDeteriorateMin = TimeManage.instance.currentMinute;

        foreach (var entry in updatedNextDeteriorate)
        {
            nextDeteriorate[entry.Key] = entry.Value;
        }

        //foreach (PlantedPlant plant in plantsToRemove)
        //{
        //    //nextDeteriorate.Remove(plant);
        //    //nextLevelTime.Remove(plant);
        //    RemovePlant(plant);
        //}
    }
    #endregion

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

    public PlantedPlant DetectPlant(Vector3 worldPosition)
    {
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(0, 0, 0), // current cell
            new Vector3Int(1, 0, 0), // right
            new Vector3Int(-1, 0, 0), // left
            new Vector3Int(0, 1, 0), // up
            new Vector3Int(0, -1, 0), // down
            new Vector3Int(1, 1, 0), // top-right
            new Vector3Int(-1, 1, 0), // top-left
            new Vector3Int(1, -1, 0), // bottom-right
            new Vector3Int(-1, -1, 0) // bottom-left
        };

        float threshold = 1f;

        Vector3Int originCellPosition = plantMap.WorldToCell(worldPosition);
        PlantedPlant nearestPlant = null;
        float nearestDistance = float.MaxValue;

        foreach (Vector3Int direction in directions)
        {
            Vector3Int cellPosition = originCellPosition + direction;
            PlantedPlant plant = GetPlantAt(cellPosition);

            if (plant != null)
            {
                Vector3 cellWorldPosition = plantMap.CellToWorld(cellPosition);
                float distance = Vector3.Distance(worldPosition, cellWorldPosition);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPlant = plant;
                }
            }
        }

        if (nearestDistance>threshold){
            return null;
        }

        return nearestPlant;
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
        Debug.Log(plantPos.Keys.Count);
        nextDeteriorate.Remove(plant);
        nextLevelTime.Remove(plant);

        if (removeOnMap){
            plantMap.SetTile(plant.gridPosition, null);
            // remove gameobject of plant max stage
            PlantPos.instance.RemovePlant(plant, plant.gridPosition);
        }

    }

    public void DamagePlant(PlantedPlant plant)
    {
        try
        {
            if (plant==null){
                return;
            }

            ColorPlant(plant, Color.black);

            // RemovePlant(plant);

            if (plantHealthBars.ContainsKey(plant))
            {
                var healthBar = plantHealthBars[plant];
                if (healthBar != null)
                {
                    healthBar.healthSlider.gameObject.SetActive(false); // plant dies => disable health slider
                    //healthBar.gameObject.SetActive(false);
                }
                plantHealthBars.Remove(plant);
            }
            else
            {
                Debug.LogWarning("Plant health bar not found in dictionary for plant: " + plant.gridPosition);
            }

            RemovePlant(plant);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error damaging plant: {e.Message}");
        }
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

        if (gridPosition == null || Planted(worldPosition))
        {
            Debug.Log("cannot plant");
            return false;
        }


        plantPos.Add(gridPosition, plant);

        Pair current = new Pair(TimeManage.instance.currentHour, TimeManage.instance.currentMinute);

        Pair nextLevel = AddTime(current, plant.levelUpTime);
        nextLevelTime.Add(plant, nextLevel);
        PlantPos.instance.LevelPlant(plant, nextLevel);

        Pair nextDeteriorateTime = AddTime(current, plant.deteriorateTime);
        nextDeteriorate.Add(plant, nextDeteriorateTime);
        PlantPos.instance.HealthPlant(plant, nextDeteriorateTime);

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
        DateTime now = DateTime.Now;
        // plant.lastOpenedTime = null;
        // plant.lastSavedTime = null; // no longer needed to keep this
        // if (nextDeteriorate.ContainsKey(plant)){
        //     nextDeteriorate.Remove(plant);
        // }

        if (nextDeteriorate.ContainsKey(plant)==false){
            return false;
        }
        Pair current = new Pair(TimeManage.instance.currentHour, TimeManage.instance.currentMinute);
        Pair nextDeteriorateTime = AddTime(current, plant.deteriorateTime);
        nextDeteriorate[plant] = nextDeteriorateTime;
    
        PlantPos.instance.HealthPlant(plant, nextDeteriorateTime);
        
        ColorPlant(plant, Color.white); // fresh plant again

        return true;
    }

    public int GetNumberOfPlants(){
        return plantPos.Keys.ToList().Count;
    }
}
