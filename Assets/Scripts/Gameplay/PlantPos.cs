using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class PlantPos : MonoBehaviour
{
    private Dictionary<Vector3Int, PlantedPlant> plantPos;
    private Dictionary<PlantedPlant, DateTime> lastLevelTime, lastCheckFreshTime;
    private Tilemap plantTilemap;
    public static PlantPos instance;

    [SerializeField][HideInInspector] private SerializableDictionary<Vector3Int, PlantedPlant> serializedPlantPos;
    [SerializeField][HideInInspector] private SerializableDictionary<PlantedPlant, DateTime> serializedLastLevelTime;
    [SerializeField][HideInInspector] private SerializableDictionary<PlantedPlant, DateTime> serializedLastCheckFreshTime;
    public SerializableDictionary<Vector3Int, PlantedPlant> SerializedPlantPos => serializedPlantPos;
    public SerializableDictionary<PlantedPlant, DateTime> SerializedLastLevelTime => serializedLastLevelTime;
    public SerializableDictionary<PlantedPlant, DateTime> SerializedLastCheckFreshTime => serializedLastCheckFreshTime;
    private void Awake() {
        if (instance==null){
            instance = this;
        }
        else{
            Destroy(this);
        }
    }

    private void Start()
    {
        serializedPlantPos = new SerializableDictionary<Vector3Int, PlantedPlant>();
        serializedLastLevelTime = new SerializableDictionary<PlantedPlant, DateTime>();
        serializedLastCheckFreshTime = new SerializableDictionary<PlantedPlant, DateTime>();

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
            lastLevelTime.Add(plant, dateTime);
        }
        else{
            lastLevelTime[plant] = dateTime;
        }
        UpdateSerializedMatrix(lastLevelTime, serializedLastLevelTime);
    }

    public void HealthPlant(PlantedPlant plant, DateTime dateTime){
        if (lastCheckFreshTime.ContainsKey(plant)==false){
            lastCheckFreshTime.Add(plant, dateTime);
        }
        else{
            lastCheckFreshTime[plant] = dateTime;
        }
        UpdateSerializedMatrix(lastCheckFreshTime, serializedLastCheckFreshTime);
    }

    private void UpdateSerializedMatrix<T, V>(Dictionary<T,V> dictionary, SerializableDictionary<T, V> serializableDictionary)
    {
        serializableDictionary.FromDictionary(dictionary);
    }

    public void Deserialize(){
        plantPos = serializedPlantPos.ToDictionary();
        lastCheckFreshTime = serializedLastCheckFreshTime.ToDictionary();
        lastLevelTime = serializedLastLevelTime.ToDictionary();
    }



}
