using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class Plant : ScriptableObject
{
    public Tile[] tiles;
    [HideInInspector] public Vector3Int gridPosition;
    public double levelUpTime;
    [HideInInspector] public int currentStage=0;
    [HideInInspector] public int health=100;
    public double deteriorateTime;
}
