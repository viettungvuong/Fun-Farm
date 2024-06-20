using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class WoodGenerate : MonoBehaviour
{
    private Tilemap groundTilemap;
    public int woodNumber;
    public int intervalBetweenFenceRefills = 30;
    private int nextMinuteRefill = 0;

    private void Start()
    {

        SceneManager.sceneLoaded += OnSceneLoaded;

        InitializeMap();
    }

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
        groundTilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
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
        
        for (int i = 0; i < number; i++)
        {
            Vector3 spawnPosition;
            int attempts = 0;

            do
            {
                Vector3Int randomCell = new Vector3Int(
                    Random.Range(groundBounds.xMin + offset, groundBounds.xMax - offset),
                    Random.Range(groundBounds.yMin + offset, groundBounds.yMax - offset),
                    0
                );

                spawnPosition = groundTilemap.CellToWorld(randomCell);


                attempts++;
                
            } while ((MapManager.instance.Plantable(spawnPosition)||PlantManager.instance.Planted(spawnPosition)
            ||Physics2D.OverlapCircle(spawnPosition, 3f)) && attempts < 200);

            if (attempts >= 200)
            {
                continue;
            }

            GameObject spawnedWood = ObjectPooling.SpawnFromPool(tag, spawnPosition);
            spawnedWood.SetActive(true);
        }
    }

}
