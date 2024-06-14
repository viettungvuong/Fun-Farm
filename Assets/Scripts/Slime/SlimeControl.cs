using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SlimeControl : MonoBehaviour
{
    private float moveSpeed;
    private Tilemap plantTilemap;

    List<Vector3Int> plantPositions;
    private float targetTimeLimit = 20f; // Time limit to reach a plant in seconds
    private float timeSpent = 0f;
    Vector3Int? targetPlantPosition;

    public float cooldownTime = 1.0f; // Cooldown after a slime moves
    private float nextMoveTime = 0f;  // To track the next allowed move time


    void Start()
    {
        plantPositions = new List<Vector3Int>();
        targetPlantPosition = null;

        plantTilemap = GameObject.Find("PlantTilemap").GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time >= nextMoveTime) // Only move if cooldown period has passed
        {
            moveSpeed = MapManager.instance.GetWalkingSpeed(transform.position) * 0.5f;

            if (targetPlantPosition != null && PlantManager.instance.GetPlantAt((Vector3Int)targetPlantPosition) == null)
            {
                targetPlantPosition = null;
            }

            if (targetPlantPosition == null)
            {
                // Double check whether new plant on the map
                var plants = PlantManager.instance.FindAllPlants(notIncludeMax: true);
                plantPositions.AddRange(plants);
                plantPositions = plantPositions.Distinct().ToList();

                if (plantPositions.Count > 0)
                {
                    targetPlantPosition = SetRandomTargetPosition(); // Random plant on the tilemap
                }
                else
                {
                    return;
                }
            }

            if (Vector3.Distance(transform.position, plantTilemap.CellToWorld((Vector3Int)targetPlantPosition)) >= 0.0001f)
            {
                var step = moveSpeed * Time.deltaTime; // Calculate distance to move
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
                // If we have reached the plant, damage it and find a new target
                PlantManager.instance.DamagePlant(PlantManager.instance.GetPlantAt((Vector3Int)targetPlantPosition));
                targetPlantPosition = SetRandomTargetPosition();
                timeSpent = 0f; // Reset timer for the new target

                // Set the next move time to current time plus cooldown
                nextMoveTime = Time.time + cooldownTime;
            }
        }
    }

    Vector3Int SetRandomTargetPosition()
    {
        int random = Random.Range(0, plantPositions.Count);
        return plantPositions[random];
    }

    // Slime only damages the plant, not the player
    private void OnCollisionEnter2D(Collision2D other)
    {
        // if (other.gameObject.CompareTag("Plant")) { // If colliding with a plant
        //     Plant plant = PlantManager.instance.GetPlantAt(other.gameObject.transform.position);
        //     if (plant != null) {
        //         PlantManager.instance.DamagePlant(plant);
        //     }
        // }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ReferenceEquals(other.gameObject, plantTilemap.gameObject))
        {
            // If colliding with a plant
            Plant plant = PlantManager.instance.GetPlantAt(other.gameObject.transform.position);
            if (plant != null)
            {
                PlantManager.instance.DamagePlant(plant);
            }
        }
    }
}
