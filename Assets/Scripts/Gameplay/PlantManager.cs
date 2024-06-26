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
    private Dictionary<PlantedPlant, DateTime> lastLevelTime, lastCheckFreshTime;
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
        lastLevelTime = new Dictionary<PlantedPlant, DateTime>();
        lastCheckFreshTime = new Dictionary<PlantedPlant, DateTime>();
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
        this.lastCheckFreshTime = new Dictionary<PlantedPlant, DateTime>();
        this.lastLevelTime = new Dictionary<PlantedPlant, DateTime>();
        this.plantHealthBars = new Dictionary<PlantedPlant, PlantHealthBar>();
        
        foreach (var entry in plantPos.SerializedLastLevelTime){
            PlantedPlant plant = entry.Key;
            double timeStamp = entry.Value;

            this.lastLevelTime.Add(plant, GameController.DeserializeDateTime(timeStamp));
        }

        foreach (var entry in plantPos.SerializedLastCheckFreshTime){
            PlantedPlant plant = entry.Key;
            double timeStamp = entry.Value;

            this.lastCheckFreshTime.Add(plant, GameController.DeserializeDateTime(timeStamp));
        }

        foreach (var entry in lastLevelTime.ToList())
        {
            PlantedPlant plant = entry.Key;
            DateTime lastTime = entry.Value;

            lastLevelTime.Remove(plant);
            DateTime freshValue = lastCheckFreshTime[plant];
            lastCheckFreshTime.Remove(plant);

            if (!plantPos.SerializedPlantPos.ContainsKey(plant.gridPosition)){
                continue;
            }

            PlantPos.instance.RemovePlant(plant, plant.gridPosition); // làm sạch

            Vector3Int cellPosition = plant.gridPosition;

            plant.LoadSavetime(); // load save time
            plant.LoadTilesFromPaths();

            this.plantPos.Add(cellPosition, plant); // add to plant pos with updated tiles

            lastLevelTime.Add(plant, lastTime); // update giá trị mới của plant về save time
            lastCheckFreshTime.Add(plant, freshValue);

            plantPos.LevelPlant(plant, lastTime);
            plantPos.AddPlant(plant.gridPosition, plant);
            plantPos.HealthPlant(plant, freshValue);

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


    private void CheckPlantLevel()
    {
        DateTime now = DateTime.Now;

        // update in temp dictionary
        var updates = new Dictionary<PlantedPlant, DateTime>();

        foreach (var entry in lastLevelTime)
        {
            PlantedPlant plant = entry.Key;

            try
            {
                if (plant.currentStage >= plant.maxStage)
                {
                    continue; // max level so do nothing
                }

                DateTime lastTime = entry.Value;
                double secondsDifference=Math.Abs((now - lastTime).TotalSeconds);
                if (plant.lastSavedTime!=null&&plant.lastOpenedTime!=null){ // when saving

                    if (plant.lastOpenedTime > lastTime){
                        double unneededDifference = Math.Abs(((DateTime)plant.lastOpenedTime - (DateTime)plant.lastSavedTime).TotalSeconds);
                        secondsDifference -= unneededDifference;
                    }

                }

                if (secondsDifference > plant.levelUpTime)
                {
                    plant.currentStage++;
                    if (plant.currentStage <=  plant.maxStage)
                    {
                        plantMap.SetTile(plant.gridPosition, plant.tiles[plant.currentStage]);
                        PlantPos.instance.LevelPlant(plant, now);
                        updates[plant] = now; // collect the update

                        if (plant.currentStage == plant.maxStage)
                        {
                            PlantTilemapShader tileShaderManager = GetComponent<PlantTilemapShader>();
                            tileShaderManager.ApplyShaderToTile(plant.gridPosition, plant.tiles[plant.currentStage].sprite, "PlantMax", 2);
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
            try {
                PlantedPlant plant = entry.Key;

                if (plant.health <= 0 || plant.currentStage == plant.maxStage)
                {
                    continue; // die or max stage
                }

                DateTime lastTime = entry.Value;

                double secondsDifference=Math.Abs((now - lastTime).TotalSeconds);
                if (plant.lastSavedTime!=null&&plant.lastOpenedTime!=null){ // when saving
                    if (plant.lastOpenedTime > lastTime){
                        double unneededDifference = Math.Abs(((DateTime)plant.lastOpenedTime - (DateTime)plant.lastSavedTime).TotalSeconds);
                        secondsDifference -= unneededDifference;
                    }
                }

                if (secondsDifference > plant.deteriorateTime)
                {
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
        lastCheckFreshTime.Remove(plant);
        lastLevelTime.Remove(plant);

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
            ColorPlant(plant, Color.black);

            RemovePlant(plant);

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

        DateTime now = DateTime.Now;
        plantPos.Add(gridPosition, plant);
        lastLevelTime.Add(plant, now);
        lastCheckFreshTime.Add(plant, now);

        PlantPos.instance.AddPlant(gridPosition, plant); // add to serializable matrix
        PlantPos.instance.LevelPlant(plant, now);
        PlantPos.instance.HealthPlant(plant, now);

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
        // if (lastCheckFreshTime.ContainsKey(plant)){
        //     lastCheckFreshTime.Remove(plant);
        // }

        if (lastCheckFreshTime.ContainsKey(plant)==false){
            return false;
        }

        lastCheckFreshTime[plant] = now;
    
        PlantPos.instance.HealthPlant(plant, now);
        
        ColorPlant(plant, Color.white); // fresh plant again

        return true;
    }

    public int GetNumberOfPlants(){
        return plantPos.Keys.ToList().Count;
    }
}
