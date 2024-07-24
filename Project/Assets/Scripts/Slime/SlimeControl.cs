using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class SlimeControl : MonoBehaviour
{
    private float moveSpeed;
    private Tilemap groundTilemap, plantTilemap;

    List<Vector3Int> plantPositions;
    private float targetTimeLimit = 20f; // Time limit to reach a plant in seconds
    private float timeSpent = 0f;
    Vector3Int? targetPlantPosition;

    public float cooldownTime = 1.0f; // Cooldown after a slime moves
    private float nextMoveTime = 0f;  // To track the next allowed move time

    Animator animator;
    Rigidbody2D rb;

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeMap();
    }

    private void InitializeMap()
    {
        plantTilemap = GameObject.Find("PlantTilemap").GetComponent<Tilemap>();
        groundTilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        InitializeMap();

        DontDestroyOnLoadManager.DontDestroyOnLoad(gameObject);

        plantPositions = new List<Vector3Int>();
        targetPlantPosition = null;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (GameController.HomeScene())
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            gameObject.transform.localScale = new Vector3(0, 0, 0); // hide
        }
    }

    int PlantCount()
    {
        var plants = PlantManager.instance.FindAllPlants(notIncludeMax: true);
        plantPositions = plants.Distinct().ToList();
        return plantPositions.Count;
    }

    void FixedUpdate()
    {
        if (GameController.HomeScene() == false)
        {
            return; // only work in home scene
        }

        if (TimeManage.instance.IsDay() == false)
        {
            GetComponent<Unit>().Die();
        }

        PlantedPlant plant = PlantManager.instance.DetectPlant(rb.position);
        if (plant != null) // detect any plant
        {
            PlantManager.instance.DamagePlant(plant);

            Vector3Int cellPosition = plantTilemap.WorldToCell(rb.position);
            if (cellPosition == targetPlantPosition)
            {
                targetPlantPosition = SetRandomTargetPosition(); // reset target if reached
            }

            timeSpent = 0f;

            nextMoveTime = Time.time + cooldownTime;
        }

        if (Time.time >= nextMoveTime) // Only move if cooldown period has passed
        {
            moveSpeed = MapManager.instance.GetWalkingSpeed(rb.position) * 2f;

            if (targetPlantPosition != null && PlantManager.instance.GetPlantAt((Vector3Int)targetPlantPosition) == null)
            {
                targetPlantPosition = null;
            } // the target plant is no longer there

            if (targetPlantPosition == null)
            {
                if (PlantCount() > 0)
                {
                    targetPlantPosition = SetRandomTargetPosition(); // Random plant on the tilemap
                }
                else
                {
                    Vector3 moveBack = GetEdgePosition(); // Move to the edge of the tilemap
                    MoveTowardsPosition(moveBack);
                    return;
                }
            }

            Vector3 plantPosition = plantTilemap.CellToWorld((Vector3Int)targetPlantPosition);
            MoveTowardsPosition(plantPosition);
        }
    }

    void MoveTowardsPosition(Vector3 targetPosition)
    {
        if (Vector3.Distance(rb.position, targetPosition) >= 0.001f)
        { // move towards target position
            var step = moveSpeed * Time.deltaTime; // calculate distance to move
            Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, step);
            rb.MovePosition(newPosition);
            timeSpent += Time.deltaTime;

            if (timeSpent >= targetTimeLimit)
            {
                if (PlantCount() > 0)
                {
                    targetPlantPosition = SetRandomTargetPosition();
                }
                else
                {
                    targetPlantPosition = null;
                }

                timeSpent = 0f; // reset timer for the new target
            }
        }
    }

    Vector3 GetEdgePosition()
    {
        BoundsInt groundBounds = groundTilemap.cellBounds;
        int offset = 1;
        int middleY = groundBounds.yMin + (groundBounds.size.y / 2);
        return groundTilemap.CellToWorld(new Vector3Int(
             groundBounds.xMax - offset,
             Random.Range(groundBounds.yMin + offset, middleY - offset),
             0
         ));
    }

    Vector3Int SetRandomTargetPosition()
    {
        int random = Random.Range(0, plantPositions.Count);
        return plantPositions[random];
    }
}
