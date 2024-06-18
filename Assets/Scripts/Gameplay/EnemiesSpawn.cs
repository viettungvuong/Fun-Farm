using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class EnemiesSpawn : MonoBehaviour
{
    private Tilemap groundTilemap;
    public Transform player;
    public int intervalBetweenSpawns;
    private int nextMinuteSpawn = 0;
    private bool timeToSpawn = false;
    // every 20 minute passes by then randomly whether should it spawn new slimes and new skeletons

    
    private void Start()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;

        InitializeMap();


    }

    private void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-initialize the map when a new scene is loaded
        InitializeMap();
    }

    private void InitializeMap()
    {
        groundTilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
    }

    void Update(){
        if (TimeManage.instance.currentMinute==nextMinuteSpawn){
            timeToSpawn = true;
        }

        if (timeToSpawn){
            if (TimeManage.instance.IsDay()){ // day time spawn slime
                int slimes = UnityEngine.Random.Range(1, 3);

                SpawnEnemy(slimes, "Slime");

            } else{
                int skeletons = UnityEngine.Random.Range(1, 3);

                SpawnEnemy(skeletons, "Skeleton");
            }
            timeToSpawn = false;
            nextMinuteSpawn += intervalBetweenSpawns;
            if (nextMinuteSpawn>=60){
                nextMinuteSpawn -= 60;
            }
        }

    }

    private void SpawnEnemy(int number, string enemyTag)
    {
        if (enemyTag != "Slime" && enemyTag != "Skeleton")
        {
            return;
        }

        BoundsInt groundBounds = groundTilemap.cellBounds;
        for (int i = 0; i < number; i++) // number of enemies spawn
        {
            // Find a random position within the tilemap bounds
            Vector3 spawnPosition;
            int attempts = 0; // fail-safe mechanism

            do
            {
                Vector3Int randomCell = new Vector3Int(
                    UnityEngine.Random.Range(groundBounds.xMin, groundBounds.xMax),
                    UnityEngine.Random.Range(groundBounds.yMin, groundBounds.yMax),
                    0
                );

                // Convert cell position to world position
                spawnPosition = groundTilemap.CellToWorld(randomCell);

                attempts++;
            } while (Physics2D.OverlapPoint(spawnPosition)&&attempts<200); // make sure the spawn position not on any collider

            if (attempts>=200){
                continue; // skip this enemy
            }


            GameObject spawnedEnemy = ObjectPooling.SpawnFromPool(enemyTag, spawnPosition);
            spawnedEnemy.SetActive(true);

            // enemy is to the right of player
            if (spawnPosition.x > player.position.x)
            {
                spawnedEnemy.GetComponent<SpriteRenderer>().flipX = true;
            }

        }
    }
}
