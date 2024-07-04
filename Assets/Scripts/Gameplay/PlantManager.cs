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

    private void CheckDeterioration(){
        // update in temp dictionary
        var updates = new Dictionary<PlantedPlant, DateTime>();

        List<PlantedPlant> plantsToRemove = new List<PlantedPlant>();

        foreach (var entry in nextDeteriorate)
        {
            try {
                PlantedPlant plant = entry.Key;

                if (plant.health <= 0 || plant.currentStage == plant.maxStage)
                {
                    continue; // die or max stage
                }

                Pair nextTime = entry.Value;

                if (TimeManage.instance.currentHour == nextTime.First &&
                TimeManage.instance.currentMinute == nextTime.Second){
                    DamagePlant(plant);
                    plantsToRemove.Add(plant);
                    PlantPos.instance.RemovePlant(plant, plant.gridPosition);
                } 
            } catch (Exception err){
                Debug.LogError($"Error updating plant water: {err.Message}");
            }
            
        }

        foreach (PlantedPlant plant in plantsToRemove)
        {
            nextDeteriorate.Remove(plant);
            nextLevelTime.Remove(plant);
            RemovePlant(plant);
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

    public bool DetectPlant(Vector3 worldPosition)
    {
        float detectionRadius = 0.2f;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(worldPosition, detectionRadius);
        
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Plant"))
            {
                Vector3Int cellPosition = plantMap.WorldToCell(hitCollider.transform.position);
                PlantedPlant plant = GetPlantAt(cellPosition);
                if (plant != null)
                {
                    return true;
                }
            }
        }
        
        return false;
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
        nextDeteriorate.Remove(plant);
        Debug.Log("Modified last check fresh time");
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
                    healthBar.gameObject.SetActive(false);
                }
                plantHealthBars.Remove(plant);
            }
            else
            {
                Debug.LogWarning("Plant health bar not found in dictionary for plant: " + plant.gridPosition);
            }
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

        if (gridPosition == null||Planted(worldPosition))
            return false;

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
