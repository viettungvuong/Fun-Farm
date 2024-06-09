using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class Plant : ScriptableObject
{
    public Tile[] tiles;
    public Vector3Int gridPosition;
    [HideInInspector] public int currentStage = 0;
}
