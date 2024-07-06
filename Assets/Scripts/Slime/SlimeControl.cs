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
    private Tilemap plantTilemap;

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

    private void Update() {
        if (GameController.HomeScene()){
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
        else{
            gameObject.transform.localScale = new Vector3(0, 0, 0); // hide
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameController.HomeScene()==false){
            return; // only work in home scene
        }

        if ((targetPlantPosition!=null&&(Vector3)rb.position==plantTilemap.CellToWorld((Vector3Int)targetPlantPosition))||PlantManager.instance.DetectPlant(rb.position)){ // detect any plant
            Debug.Log("Detect plant");
            PlantedPlant plant = PlantManager.instance.GetPlantAt(rb.position);
            PlantManager.instance.DamagePlant(plant);

            Vector3Int cellPosition = plantTilemap.WorldToCell(rb.position);
            if (cellPosition == targetPlantPosition){
                targetPlantPosition = SetRandomTargetPosition(); // reset target if reached
            }

            timeSpent = 0f; 

            nextMoveTime = Time.time + cooldownTime;
        }

        if (Time.time >= nextMoveTime) // Only move if cooldown period has passed
        {
            moveSpeed = MapManager.instance.GetWalkingSpeed(rb.position) * 0.5f;

            if (targetPlantPosition != null && PlantManager.instance.GetPlantAt((Vector3Int)targetPlantPosition) == null)
            {
                targetPlantPosition = null;
            } // the target plant is no longer there

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
                    return; // stop because no target plant
                }
            }
            Vector3 plantPosition = plantTilemap.CellToWorld((Vector3Int)targetPlantPosition);
            if (Vector3.Distance(rb.position, plantPosition) >= 0.001f)
            { // move towards target plant

                var step = moveSpeed * Time.deltaTime; // calculate distance to move
                Vector2 newPosition = Vector2.MoveTowards(rb.position, plantPosition, step);
                rb.MovePosition(newPosition);                
                timeSpent += Time.deltaTime;

                if (timeSpent >= targetTimeLimit)
                {
                    targetPlantPosition = SetRandomTargetPosition();
                    timeSpent = 0f; // reset timer for the new target
                }
            }

        }
    }

    Vector3Int SetRandomTargetPosition()
    {
        int random = Random.Range(0, plantPositions.Count);
        return plantPositions[random];
    }

    // Slime only damages the plant, not the player


    // private void OnCollisionEnter2D(Collision2D other) {
    //     if (other.gameObject.CompareTag("Wood")||other.gameObject.CompareTag("Tree")){
    //         Vector2 displacement = other.contacts[0].normal * 0.1f;
    //         while (other.collider.bounds.Intersects(GetComponent<Collider2D>().bounds)) {
    //             transform.position += (Vector3)displacement;
    //         }

    //         // move by displacement vector to move out of the wood
    //     }
    // }


}
