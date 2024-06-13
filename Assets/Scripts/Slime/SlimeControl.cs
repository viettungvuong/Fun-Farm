using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SlimeControl : MonoBehaviour
{
    private float moveSpeed;
    public Tilemap plantTilemap;
    public Tile[] plantTiles;

    List<Vector3Int> plantPositions;
    private float targetTimeLimit = 10f; // Time limit to reach a plant in seconds
    private float timeSpent = 0f;
    Vector3Int? targetPlantPosition;

    void Start()
    {
        plantPositions = new List<Vector3Int>();
        targetPlantPosition = null;
        FindAllPlants();

        if (plantPositions.Count>0){
            targetPlantPosition = SetRandomTargetPosition(); // random plant on the tilemap
        }

    }

    // Update is called once per frame
    void Update()
    {
        moveSpeed = MapManager.instance.GetWalkingSpeed(transform.position);

        if (targetPlantPosition==null){

            return;
        }

        if (Vector3.Distance(transform.position, plantTilemap.CellToWorld((Vector3Int)targetPlantPosition)) >= 0.001f)
        {
            var step = moveSpeed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, plantTilemap.CellToWorld((Vector3Int)targetPlantPosition), step);
            timeSpent += Time.deltaTime;

            if (timeSpent >= targetTimeLimit)
            {
                targetPlantPosition = SetRandomTargetPosition();
                timeSpent = 0f; // Reset timer for the new target
            }
        }
        else
        {
            // If we have reached the plant, eat it and find a new target
            // EatPlant(targetPlantPosition);
            targetPlantPosition = SetRandomTargetPosition();
            timeSpent = 0f; // Reset timer for the new target
        }
    }

    private void FindAllPlants()
    {
        BoundsInt bounds = plantTilemap.cellBounds;

        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                TileBase tile = plantTilemap.GetTile(cellPosition);
                
                if (plantTiles.Contains(tile)==true)
                {
                    plantPositions.Add(cellPosition); // this position has plant
                }
                // if (tile!=null)
                // {
                //     plantPositions.Add(cellPosition); // this position has plant
                // }
            }
        }
    }

    Vector3Int SetRandomTargetPosition()
    {
        int random = Random.Range(0,plantPositions.Count);

        return plantPositions[random];
    }

    // slime only damage to plant, not player
}
