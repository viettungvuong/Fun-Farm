using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SlimeControl : MonoBehaviour
{
    private float moveSpeed;
    public Tilemap plantTilemap;

    List<Vector3Int> plantPositions;
    private float targetTimeLimit = 10f; // Time limit to reach a plant in seconds
    private float timeSpent = 0f;
    Vector3Int? targetPlantPosition;

    private int damage = 10;

    void Start()
    {
        plantPositions = new List<Vector3Int>();
        targetPlantPosition = null;
    }

    // Update is called once per frame
    void Update()
    {
        moveSpeed = MapManager.instance.GetWalkingSpeed(transform.position) * 0.3f;

        if (targetPlantPosition == null) {
            // double check whether new plant on the map
            plantPositions.AddRange(PlantManager.instance.FindAllPlants());

            if (plantPositions.Count>0){
                targetPlantPosition = SetRandomTargetPosition(); // random plant on the tilemap
            }
            else{
                return;
            }
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


    Vector3Int SetRandomTargetPosition()
    {
        int random = Random.Range(0,plantPositions.Count);

        return plantPositions[random];
    }

    // slime only damage to plant, not player
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Plant")){ // đụng plant
            Plant plant = PlantManager.instance.GetPlantAt(other.gameObject.transform.position);
            if (plant!=null){
                PlantManager.instance.DamagePlant(plant, damage);
            }
        }
    }
}
