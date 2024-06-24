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
        if (PlayerUnit.playerMode==PlayerMode.CREATIVE){
            enabled = false;
            return;
        }
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

    private void FixedUpdate() {
        if (TimeManage.instance.IsDay()){
            enemyIndicatorIcon.sprite = slimeSprite;
        }
        else{
            enemyIndicatorIcon.sprite = skeletonSprite;
        }
    }


    protected void Spawn(int number)
    {

        BoundsInt groundBounds = groundTilemap.cellBounds;
        int offset = 1;

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
                    case 0: // Top edge
                        randomCell = new Vector3Int(
                            Random.Range(groundBounds.xMin + offset, groundBounds.xMax - offset),
                            groundBounds.yMax - offset,
                            0
                        );
                        break;
                    case 1: // Bottom edge
                        randomCell = new Vector3Int(
                            Random.Range(groundBounds.xMin + offset, groundBounds.xMax - offset),
                            groundBounds.yMin + offset,
                            0
                        );
                        break;
                    case 2: // Left edge
                        randomCell = new Vector3Int(
                            groundBounds.xMin + offset,
                            Random.Range(groundBounds.yMin + offset, groundBounds.yMax - offset),
                            0
                        );
                        break;
                    case 3: // Right edge
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
                || Physics2D.OverlapCircle(spawnPosition, 2f, obstacleLayer)) && attempts < 200);

            if (attempts >= 200)
            {
                continue;
            }

            GameObject spawned = ObjectPooling.SpawnFromPool(objectTag, spawnPosition);
            spawned.SetActive(true);

            if (spawnPosition.x > player.transform.position.x)
            {
                spawned.GetComponent<SpriteRenderer>().flipX = true;
            }
        }
    }
}
