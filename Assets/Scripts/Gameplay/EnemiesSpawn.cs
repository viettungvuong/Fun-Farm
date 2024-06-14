using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemiesSpawn : MonoBehaviour
{
    public Tilemap groundTilemap;
    public Transform player;
    public int intervalBetweenSpawns = 20;
    private int nextMinuteSpawn = 0;
    // every 20 minute passes by then randomly whether should it spawn new slimes and new skeletons
    void Update(){
        if (TimeManage.instance.currentMinute==nextMinuteSpawn){
            if (TimeManage.instance.IsDay()){ // day time spawn slime
                int slimes = Random.Range(1, 5);

                SpawnEnemy(slimes, "Slime");

            } else{
                int skeletons = Random.Range(1, 4);

                SpawnEnemy(skeletons, "Skeleton");
            }

            nextMinuteSpawn += intervalBetweenSpawns;
            if (nextMinuteSpawn>=60){
                nextMinuteSpawn = 0;
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
        for (int i = 0; i < number; i++)
        {
            // Find a random position within the tilemap bounds
            Vector3Int randomCell = new Vector3Int(
                Random.Range(groundBounds.xMin, groundBounds.xMax),
                Random.Range(groundBounds.yMin, groundBounds.yMax),
                0
            );

            // Convert cell position to world position
            Vector3 spawnPosition = groundTilemap.CellToWorld(randomCell);

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
