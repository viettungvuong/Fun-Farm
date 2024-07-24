using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class ItemUnit : ScriptableObject {
    public Tile tile;

    public int money;
    public int quantity = 0;
}