using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class SkeletonGenerate : MonoBehaviour
{
    private Tilemap groundTilemap;
    public GameObject player;

    public LayerMask enemyLayer, playerLayer, obstacleLayer;
    public int skeletonNumber;
    public int intervalBetweenSpawns = 60;
    private int nextMinuteRefill = 0;

    public static int skeletons = 0;

    static bool hasSpawned = false;

    private void Start()
    {
        if (PlayerUnit.playerMode == PlayerMode.CREATIVE)
        {
            enabled = false;
            return;
        }
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

        if (GameController.HomeScene() == false)
        {
            nextMinuteRefill += 1;
            if (nextMinuteRefill >= 60)
            {
                nextMinuteRefill -= 60;
            }
            return;
        }
        if (TimeManage.instance.currentMinute == nextMinuteRefill && TimeManage.instance.IsDay() == false && !hasSpawned)
        {
            hasSpawned = true;
            nextMinuteRefill += intervalBetweenSpawns;
            if (nextMinuteRefill >= 60)
            {
                nextMinuteRefill -= 60;
            }

            SpawnSkeleton(skeletonNumber);
            skeletons += skeletonNumber;
        }

        if (TimeManage.instance.currentMinute != nextMinuteRefill)
        {
            hasSpawned = false;
        }
    }

    private void SpawnSkeleton(int number)
    {
        string tag = "Skeleton";

        BoundsInt groundBounds = groundTilemap.cellBounds;
        int offset = 2;

        for (int i = 0; i < number; i++)
        {
            Vector3 spawnPosition;
            int attempts = 0;

            do
            {
                Vector3Int randomCell;
                int edge = Random.Range(0, 4); // 0: top, 1: bottom, 2: left, 3: right

                switch (edge)
                {
                    case 0: // top
                        randomCell = new Vector3Int(
                            Random.Range(groundBounds.xMin + offset, groundBounds.xMax - offset),
                            groundBounds.yMax - offset,
                            0
                        );
                        break;
                    case 1: // bottom
                        randomCell = new Vector3Int(
                            Random.Range(groundBounds.xMin + offset, groundBounds.xMax - offset),
                            groundBounds.yMin + offset,
                            0
                        );
                        break;
                    case 2: // left
                        randomCell = new Vector3Int(
                            groundBounds.xMin + offset,
                            Random.Range(groundBounds.yMin + offset, groundBounds.yMax - offset),
                            0
                        );
                        break;
                    case 3: // right
                        randomCell = new Vector3Int(
                            groundBounds.xMax - offset,
                            Random.Range(groundBounds.yMin + offset, groundBounds.yMax - offset),
                            0
                        );
                        break;
                    default:
                        randomCell = new Vector3Int();
                        break;
                }

                spawnPosition = groundTilemap.CellToWorld(randomCell);

                attempts++;

            } while ((MapManager.instance.Plantable(spawnPosition) || PlantManager.instance.Planted(spawnPosition)
                || Physics2D.OverlapCircle(spawnPosition, 1.5f, enemyLayer)
                || Physics2D.OverlapCircle(spawnPosition, 7f, playerLayer)
                || Physics2D.OverlapCircle(spawnPosition, 1f, obstacleLayer)) && attempts < 200);

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
