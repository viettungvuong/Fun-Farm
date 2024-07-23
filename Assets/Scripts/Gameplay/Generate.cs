using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public abstract class Generate : MonoBehaviour
{
    private Tilemap groundTilemap;
    private GameObject player;

    private LayerMask enemyLayer, playerLayer, obstacleLayer;

    public Image enemyIndicatorIcon;
    public Sprite slimeSprite, skeletonSprite;
    public TextMeshProUGUI remainingTimeText;


    protected int number;
    protected string objectTag;

    protected virtual void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");

        enemyLayer = LayerMask.NameToLayer("Enemies");
        playerLayer = LayerMask.NameToLayer("Player");
        obstacleLayer = LayerMask.NameToLayer("Obstacle");
    }

    protected virtual void Start()
    {

        SceneManager.sceneLoaded += OnSceneLoaded;

        InitializeMap();
    }

    protected void OnDestroy()
    {
 
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    protected void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
 
        InitializeMap();
    }

    protected void InitializeMap()
    {
        groundTilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
    }

    protected string timeString(int nextMinuteRefill){
        int minsDiff;
        if (nextMinuteRefill>TimeManage.instance.currentMinute){
            minsDiff = nextMinuteRefill - TimeManage.instance.currentMinute;
        }
        else{
            minsDiff = nextMinuteRefill+60 - TimeManage.instance.currentMinute;
        }
        int hours = minsDiff / 60;
        int mins = minsDiff - hours * 60;
        string minsString = (mins < 10) ? "0" + mins : mins.ToString();
        return hours + ":" + minsString;
    }


    protected void Spawn(int number)
    {

        BoundsInt groundBounds = groundTilemap.cellBounds;
        int offset = 1;
        int middleY = groundBounds.yMin + (groundBounds.size.y / 2);

        for (int i = 0; i < number; i++)
        {
            Vector3 spawnPosition;
            int attempts = 0;

            do
            {
                Vector3Int randomCell;

                randomCell = new Vector3Int(
                    groundBounds.xMax - offset,
                    Random.Range(groundBounds.yMin + offset, middleY - offset),
                    0
                );
                spawnPosition = groundTilemap.CellToWorld(randomCell);

                attempts++;

            } while ((MapManager.instance.Plantable(spawnPosition) || PlantManager.instance.Planted(spawnPosition)
                || Physics2D.OverlapCircle(spawnPosition, 1.5f, enemyLayer)
                || Physics2D.OverlapCircle(spawnPosition, 5f, playerLayer)
                || Physics2D.OverlapCircle(spawnPosition, 2f, obstacleLayer)) && attempts < 200);

            if (attempts >= 200)
            {
                continue;
            }

            GameObject spawned = ObjectPooling.SpawnFromPool(objectTag, spawnPosition);
            spawned.SetActive(true);
            // HealthBar healthBar = spawned.GetComponent<HealthBar>();
            // if (healthBar!=null){
            //     healthBar.Enable();
            // }

            if (spawnPosition.x > player.transform.position.x)
            {
                spawned.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
    }
}
