using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Callbacks;
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

    Animator animator;

    Rigidbody2D rb;


    void Start()
    {
        plantPositions = new List<Vector3Int>();
        targetPlantPosition = null;

        plantTilemap = GameObject.Find("PlantTilemap").GetComponent<Tilemap>();

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time >= nextMoveTime) // Only move if cooldown period has passed
        {
            moveSpeed = MapManager.instance.GetWalkingSpeed(rb.position) * 0.5f;

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

            Vector3 targetWorldPosition = plantTilemap.CellToWorld((Vector3Int)targetPlantPosition);

            if (Vector3.Distance(rb.position, targetWorldPosition) >= 0.0001f)
            {
                var step = moveSpeed * Time.deltaTime; // Calculate distance to move
                rb.MovePosition(Vector3.MoveTowards(rb.position, targetWorldPosition, step));
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

    const int damage = 50;

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Defense")){
            // try to attack the fence or find alternative way (which one is faster to go)
            // get fence at
            // fence.health -= damage if fence.health > 0
            // if fence.health == 0
            // groundDefense.settile(null)
            FenceUnit fenceUnit = PlayerDefend.instance.GetDefenceAt(rb.position);
            if (fenceUnit!=null){
                if (fenceUnit.health > 0){
                    fenceUnit.health -= damage;

                    if (fenceUnit.health == 0){
                        PlayerDefend.instance.DestroyFence(rb.position); // destroy fence
                    }
                }

            }
        }
    }
}
