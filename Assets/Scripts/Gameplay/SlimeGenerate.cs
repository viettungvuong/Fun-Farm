using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class SlimeGenerate : MonoBehaviour
{
    private Tilemap groundTilemap;
    public GameObject player;

    public LayerMask enemyLayer, playerLayer, obstacleLayer;
    public int slimeNumber;
    public int intervalBetweenSpawns = 40;
    private int nextMinuteRefill = 15;

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
        if (TimeManage.instance.currentMinute==nextMinuteRefill&&TimeManage.instance.IsDay()==true){
            nextMinuteRefill+= intervalBetweenSpawns;
            if (nextMinuteRefill>=60){
                nextMinuteRefill -= 60;
            }

            SpawnSkeleton(slimeNumber); // spawn wood every 5 mins
        }
    }

    private void SpawnSkeleton(int number)
    {
        string tag = "Slime";

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
            ||Physics2D.OverlapCircle(spawnPosition, 1.5f, enemyLayer)
            ||Physics2D.OverlapCircle(spawnPosition, 7f, playerLayer)
            ||Physics2D.OverlapCircle(spawnPosition, 2f, obstacleLayer)) && attempts < 200);

            if (attempts >= 200)
            {
                continue;
            }

            GameObject spawnedEnemy = ObjectPooling.SpawnFromPool(tag, spawnPosition);
            spawnedEnemy.SetActive(true);

            if (spawnPosition.x > player.transform.position.x)
            {
                spawnedEnemy.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
    }
}
