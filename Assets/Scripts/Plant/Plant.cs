using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class Plant : ScriptableObject
{
    public Tile[] tiles;
    public double levelUpTime;
    [HideInInspector] public int maxHealth = 100;
    public int buyMoney;
    public int harvestMoney;
    public double deteriorateTime;
}

[Serializable]
public class PlantedPlant
{
    public PlantedPlant(Plant plant, Vector3Int gridPosition)
    {
        tiles = plant.tiles;

        this.gridPosition = gridPosition;

        levelUpTime = plant.levelUpTime;

        maxHealth = plant.maxHealth;
        health = maxHealth;

        maxStage = tiles.ToList().Count;

        buyMoney = plant.buyMoney;
        harvestMoney = plant.harvestMoney;
        deteriorateTime = plant.deteriorateTime;
    }


    public Tile[] tiles;
    public Vector3Int gridPosition;
    public double levelUpTime;
    public int currentStage=0;
    public int maxHealth=100;
    public int health;
    public int maxStage;
    public int buyMoney;
    public int harvestMoney;
    public double deteriorateTime;
}
