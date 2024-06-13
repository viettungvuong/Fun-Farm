using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SlimeControl : MonoBehaviour
{
    public float speed;
    public Tilemap plantTilemap;

    Unit unit;

    void Start()
    {
        unit = GetComponent<Unit>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = SetRandomTargetPosition(); // random position on the tilemap

        if (Vector3.Distance(transform.position, targetPosition) >= 0.001f)
        {
            var step =  speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        }
    }

    Vector3 SetRandomTargetPosition()
    {
        // get the bounds of the tilemap
        BoundsInt bounds = plantTilemap.cellBounds;

        int randomY = Random.Range(bounds.yMin, bounds.yMax);

        Vector3Int randomCellPosition = new Vector3Int(bounds.xMax, randomY);
        return plantTilemap.CellToWorld(randomCellPosition);
    }

    // slime only damage to plant, not player
}
