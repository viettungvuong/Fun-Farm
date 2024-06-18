using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WoodGenerate : MonoBehaviour
{
    public Tilemap groundTilemap;
    public int woodNumber;
    public int intervalBetweenFenceRefills = 5;
    private int nextMinuteRefill = 0;
    void Start()
    {
        // SpawnWood(woodNumber);
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeManage.instance.currentMinute==nextMinuteRefill){
            nextMinuteRefill+= intervalBetweenFenceRefills;
            if (nextMinuteRefill>=60){
                nextMinuteRefill -= 60;
            }

            SpawnWood(woodNumber); // spawn wood every 5 mins
        }
    }

    private void SpawnWood(int number)
    {
        string tag = "Wood";

        BoundsInt groundBounds = groundTilemap.cellBounds;
        int offset = 2;
        for (int i = 0; i < number; i++) // number of enemies spawn
        {
            // Find a random position within the tilemap bounds
            Vector3 spawnPosition;
            int attempts = 0; // fail-safe mechanism

            do
            {
                Vector3Int randomCell = new Vector3Int(
                    Random.Range(groundBounds.xMin + offset, groundBounds.xMax - offset),
                    Random.Range(groundBounds.yMin + offset, groundBounds.yMax - offset),
                    0
                );

                // Convert cell position to world position
                spawnPosition = groundTilemap.CellToWorld(randomCell);

                attempts++;
            } while (Physics2D.OverlapPoint(spawnPosition)&&attempts<200); // make sure the spawn position not on any collider

            if (attempts>=200){
                continue; // skip this enemy
            }


            GameObject spawnedWood = ObjectPooling.SpawnFromPool(tag, spawnPosition);
            spawnedWood.SetActive(true);

        }
    }
}
