using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class PlantPos : MonoBehaviour
{
    private Dictionary<Vector3Int, PlantedPlant> plantPos;
    private Dictionary<PlantedPlant, double> lastLevelTime, lastCheckFreshTime;

    public Dictionary<Vector3Int, PlantedPlant> SerializedPlantPos => plantPos;
    public Dictionary<PlantedPlant, double> SerializedLastLevelTime => lastLevelTime; // double bcs Datetime is unserializable
    public Dictionary<PlantedPlant, double> SerializedLastCheckFreshTime => lastCheckFreshTime;
    private Tilemap plantTilemap;
    public static PlantPos instance;

    [SerializeField][HideInInspector] private SerializableDictionary<Vector3Int, PlantedPlant> serializedPlantPos;
    [SerializeField][HideInInspector] private SerializableDictionary<PlantedPlant, double> serializedLastLevelTime;
    [SerializeField][HideInInspector] private SerializableDictionary<PlantedPlant, double> serializedLastCheckFreshTime;

    private void Awake() {
        if (instance==null){
            instance = this;
        }
        else{
            Destroy(this);
        }
        serializedPlantPos = new SerializableDictionary<Vector3Int, PlantedPlant>();
        serializedLastLevelTime = new SerializableDictionary<PlantedPlant, double>();
        serializedLastCheckFreshTime = new SerializableDictionary<PlantedPlant, double>();

        plantTilemap = GameObject.Find("PlantTilemap").GetComponent<Tilemap>();

        plantPos = serializedPlantPos.ToDictionary();
        lastCheckFreshTime = serializedLastCheckFreshTime.ToDictionary();
        lastLevelTime = serializedLastLevelTime.ToDictionary();
    }


    // private void OnDestroy()
    // {
    //     serializedPlantPos.FromDictionary(plantPos);
    // }

    public void AddPlant(Vector3Int cellPosition, PlantedPlant plant)
    {
        if (!plantPos.ContainsKey(cellPosition))
        {
            plantPos.Add(cellPosition, plant);
        }
        else
        {
            plantPos[cellPosition] = plant;
        }
        UpdateSerializedMatrix(plantPos,serializedPlantPos);
    }

    public void RemovePlant(PlantedPlant plant, Vector3Int cellPosition)
    {
        if (!plantPos.ContainsKey(cellPosition))
        {
            return;
        }
        else
        {
            plantPos.Remove(cellPosition);
            UpdateSerializedMatrix(plantPos,serializedPlantPos); // remap to serializable dict

            lastLevelTime.Remove(plant);
            lastCheckFreshTime.Remove(plant);
            UpdateSerializedMatrix(lastCheckFreshTime, serializedLastCheckFreshTime);
            UpdateSerializedMatrix(lastLevelTime, serializedLastLevelTime);

        }
    }

    public void LoadToTilemap()
    {
        plantTilemap.ClearAllTiles();

        foreach (var entry in plantPos)
        {
            Vector3Int cellPosition = entry.Key;
            PlantedPlant plant = entry.Value;

            if (plant != null)
            {
                plantTilemap.SetTile(cellPosition, plant.tiles[plant.currentStage]);
            }
        }
    }

    public void LevelPlant(PlantedPlant plant, DateTime dateTime){
        if (lastLevelTime.ContainsKey(plant)==false){
            lastLevelTime.Add(plant, GameController.SerializeDateTime(dateTime));
        }
        else{
            lastLevelTime[plant] = GameController.SerializeDateTime(dateTime);
        }
        UpdateSerializedMatrix(lastLevelTime, serializedLastLevelTime);
    }

    public void HealthPlant(PlantedPlant plant, DateTime dateTime){
        if (lastCheckFreshTime.ContainsKey(plant)==false){
            lastCheckFreshTime.Add(plant, GameController.SerializeDateTime(dateTime));
        }
        else{
            lastCheckFreshTime[plant] = GameController.SerializeDateTime(dateTime);
        }
        UpdateSerializedMatrix(lastCheckFreshTime, serializedLastCheckFreshTime);
    }

    private void UpdateSerializedMatrix<T, V>(Dictionary<T,V> dictionary, SerializableDictionary<T, V> serializableDictionary)
    {
        serializableDictionary.FromDictionary(dictionary);
    }

    public void Deserialize(){
        plantPos = new Dictionary<Vector3Int, PlantedPlant>();
        foreach (var entry in serializedPlantPos.entries){
            Vector3Int position = entry.key;
            PlantedPlant plant = entry.value;
            plantPos.Add(position, plant);

            DateTime now = DateTime.Now;
            plant.lastOpenedTime = now;
        }
        lastCheckFreshTime = new Dictionary<PlantedPlant, double>();
        foreach (var entry in serializedLastCheckFreshTime.entries){
            PlantedPlant plant = entry.key;
            double dateTime = entry.value;
            lastCheckFreshTime.Add(plant, dateTime);

            DateTime now = DateTime.Now;
            plant.lastOpenedTime = now;
        }
        lastLevelTime = new Dictionary<PlantedPlant, double>();
        foreach (var entry in serializedLastLevelTime.entries){
            PlantedPlant plant = entry.key;
            double dateTime = entry.value;
            lastLevelTime.Add(plant, dateTime);

            DateTime now = DateTime.Now;
            plant.lastOpenedTime = now;
        }
    }

    public void SetSaveTime(){ // for accurate deterioration check
        DateTime now = DateTime.Now;

        List<Vector3Int> keysToUpdate = new List<Vector3Int>(plantPos.Keys);

        foreach (Vector3Int key in keysToUpdate)
        {
            PlantedPlant plant = plantPos[key];

            plant.SetSaveTime(now);

            plantPos[key] = plant;

            if (lastCheckFreshTime.ContainsKey(plant))
            {
                double value = lastCheckFreshTime[plant];
                lastCheckFreshTime.Remove(plant);

                plant.SetSaveTime(now);

                lastCheckFreshTime.Add(plant, value);
            }

            if (lastLevelTime.ContainsKey(plant))
            {
                double value = lastLevelTime[plant];
                lastLevelTime.Remove(plant);

                plant.SetSaveTime(now);

                lastLevelTime.Add(plant, value);
            }
        }

    }

}
