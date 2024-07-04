using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class PlantPos : MonoBehaviour
{
    private Dictionary<Vector3Int, PlantedPlant> plantPos;
    private Dictionary<PlantedPlant, Pair> nextLevelTime, nextDeteriorate;

    public Dictionary<Vector3Int, PlantedPlant> SerializedPlantPos => plantPos;
    public Dictionary<PlantedPlant, Pair> SerializednextLevelTime => nextLevelTime; // int bcs Datetime is unserializable
    public Dictionary<PlantedPlant, Pair> SerializednextDeteriorate => nextDeteriorate;
    private Tilemap plantTilemap;
    public static PlantPos instance;

    [SerializeField][HideInInspector] private SerializableDictionary<Vector3Int, PlantedPlant> serializedPlantPos;
    [SerializeField][HideInInspector] private SerializableDictionary<PlantedPlant, Pair> serializednextLevelTime;
    [SerializeField][HideInInspector] private SerializableDictionary<PlantedPlant, Pair> serializednextDeteriorate;

    private void Awake() {
        if (instance==null){
            instance = this;
        }
        else{
            Destroy(this);
        }
        serializedPlantPos = new SerializableDictionary<Vector3Int, PlantedPlant>();
        serializednextLevelTime = new SerializableDictionary<PlantedPlant, Pair>();
        serializednextDeteriorate = new SerializableDictionary<PlantedPlant, Pair>();

        plantTilemap = GameObject.Find("PlantTilemap").GetComponent<Tilemap>();

        plantPos = serializedPlantPos.ToDictionary();
        nextDeteriorate = serializednextDeteriorate.ToDictionary();
        nextLevelTime = serializednextLevelTime.ToDictionary();
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

            nextLevelTime.Remove(plant);
            nextDeteriorate.Remove(plant);
            UpdateSerializedMatrix(nextDeteriorate, serializednextDeteriorate);
            UpdateSerializedMatrix(nextLevelTime, serializednextLevelTime);

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

    public void LevelPlant(PlantedPlant plant, Pair nextTime){
        if (nextLevelTime.ContainsKey(plant)){
            nextLevelTime[plant] = nextTime;
        }
        else{
            nextLevelTime.Add(plant, nextTime);
        }
        UpdateSerializedMatrix(nextLevelTime, serializednextLevelTime);
    }

    public void HealthPlant(PlantedPlant plant, Pair nextTime){
        if (nextDeteriorate.ContainsKey(plant)){
            nextDeteriorate[plant] = nextTime;
        }
        else{
            nextDeteriorate.Add(plant, nextTime);
        }
        UpdateSerializedMatrix(nextDeteriorate, serializednextDeteriorate);
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
        }
        nextDeteriorate = new Dictionary<PlantedPlant, Pair>();
        foreach (var entry in serializednextDeteriorate.entries){
            PlantedPlant plant = entry.key;
            Pair dateTime = entry.value;
            nextDeteriorate.Add(plant, dateTime);
        }
        nextLevelTime = new Dictionary<PlantedPlant, Pair>();
        foreach (var entry in serializednextLevelTime.entries){
            PlantedPlant plant = entry.key;
            Pair dateTime = entry.value;
            nextLevelTime.Add(plant, dateTime);

        }
    }



}
