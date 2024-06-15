using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class FenceUnit : ScriptableObject{
    public Tile tile;
    public int health = 100;
}
