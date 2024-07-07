using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class Plant : ScriptableObject
{
    public Tile[] tiles;
    public int levelUpTime;
    [HideInInspector] public int maxHealth = 100;
    public int buyMoney;
    public int harvestMoney;
    public int deteriorateTime;
}

[Serializable]
public class PlantedPlant
{
    public PlantedPlant() {
        tilePaths = new List<string>();
        tiles = new List<Tile>();
    }

    public PlantedPlant(Plant plant, Vector3Int gridPosition)
    {
        tilePaths = new List<string>();

        tiles = plant.tiles.ToList();

        string temp = "TILE: ";
        foreach (var tile in tiles)
        {
            temp += tile.name;
        }
        Debug.Log(temp);
        
        SaveTilePaths();

        this.gridPosition = gridPosition;

        levelUpTime = plant.levelUpTime;

        maxHealth = plant.maxHealth;
        health = maxHealth;

        maxStage = tiles.ToList().Count-1;

        buyMoney = plant.buyMoney;
        harvestMoney = plant.harvestMoney;
        deteriorateTime = plant.deteriorateTime;
    }

    public void SaveTilePaths()
    {
        tilePaths.Clear();
        foreach (var tile in tiles)
        {
            tilePaths.Add(tile.name);
        }
    }

    public void LoadTilesFromPaths()
    {
        tiles.Clear();
        foreach (var path in tilePaths)
        {
            Debug.Log(path);
            var tile = Resources.Load<Tile>(path);
            tiles.Add(tile);
        }
    }


    [NonSerialized] public List<Tile> tiles;
    [SerializeField] private List<string> tilePaths; // tiles are un-serializable so we have to store path
    public Vector3Int gridPosition;
    public int levelUpTime;
    public int currentStage=0;
    public int maxHealth=100;
    public int health;
    public int maxStage;
    public int buyMoney;
    public int harvestMoney;
    public int deteriorateTime;

    public override int GetHashCode()
    {
        int hash = 17;

        hash = hash * 31 + gridPosition.GetHashCode();

        foreach (var path in tilePaths)
        {
            hash = hash * 31 + (path != null ? path.GetHashCode() : 0);
        }

        return hash;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        PlantedPlant other = (PlantedPlant)obj;

        return gridPosition.Equals(other.gridPosition);
    }
}
