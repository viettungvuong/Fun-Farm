using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static PlayerUnit;

public class PlayerPlant : MonoBehaviour
{
    [HideInInspector] public bool isPlanting = false;
    private Tilemap plantTilemap;

    PlayerMove playerMove;
    SpriteRenderer spriteRenderer;
    Animator animator;
    Rigidbody2D rb;
    PlayerUnit playerUnit;

    public AudioClip audioClip;

    public AudioSource audioSource;

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeMap();
    }

    void InitializeMap()
    {
        if (GameController.HomeScene())
        {
            plantTilemap = GameObject.Find("PlantTilemap").GetComponent<Tilemap>();
        }
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeMap();
        animator = GetComponent<Animator>();
        playerMove = GetComponent<PlayerMove>();
        rb = GetComponent<Rigidbody2D>();
        playerUnit = GetComponent<PlayerUnit>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        audioSource = gameObject.GetComponent<AudioSource>();
        audioClip = Resources.Load<AudioClip>("Audio/watering");
        

    }

    void Update()
    {
        if (!GameController.HomeScene())
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            WaterTree(rb.position);
        }
    }
    
    #region plant
    public void PlantTree(Vector3 worldPosition, Plant plant)
    {
        bool plantable = MapManager.instance.Plantable(worldPosition);
        if (!plantable)
        {
            return;
        }

        if (!playerUnit.moneyManager.SufficientMoney(plant.buyMoney))
        {
            return;
        }

        playerUnit.moneyManager.UseMoney(plant.buyMoney);
        StartCoroutine(PlantTreeCoroutine(worldPosition, new PlantedPlant(plant, plantTilemap.WorldToCell(worldPosition))));
    }

    private Orientation GetTileToPlayerOrientation(Vector3 playerPosition)
    {
        Vector3 tileCenter = plantTilemap.GetCellCenterWorld(plantTilemap.WorldToCell(playerPosition));
        Vector3 direction = tileCenter - playerPosition;

        if (playerMove.orientation==Orientation.LEFT||playerMove.orientation==Orientation.RIGHT)
        {
            if (direction.x > 0)
            {
                return Orientation.RIGHT;
            }
            else
            {
                return Orientation.LEFT;
            }
        }
        else
        {
            if (direction.y > 0)
            {
                return Orientation.UP;
            }
            else
            {
                return Orientation.DOWN;
            }
        }
    }

   private IEnumerator PlantTreeCoroutine(Vector3 worldPosition, PlantedPlant plant)
    {
        Vector3Int plantCellPosition = plantTilemap.WorldToCell(worldPosition);
        plant.gridPosition = plantCellPosition;
        Vector3Int playerCellPosition = plantTilemap.WorldToCell(rb.position);

        bool plantEligible = PlantManager.instance.AddPlant(worldPosition, plant);
        if (!plantEligible)
        {
            yield break;
        }


        isPlanting = true;
        string animationName;

        Orientation orientation = GetTileToPlayerOrientation(playerCellPosition);
        spriteRenderer.flipX = false;
        switch (orientation)
        {
            case Orientation.UP:
                animationName = "PlayerPlantUp";
                break;
            case Orientation.DOWN:
                animationName = "PlayerPlantDown";
                break;
            default:
                animationName = "PlayerPlantHorizontal";
                if (orientation==Orientation.LEFT){
                    spriteRenderer.flipX = true;
                }
                break;
        }

        animator.Play(animationName);

        yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName));

        isPlanting = false;
 
        plantTilemap.SetTile(plantCellPosition, plant.tiles[plant.currentStage]);
    }
    #endregion

    #region water
    const float waterUsage = 0.15f;

    public void WaterTree(Vector3 worldPosition)
    {
        bool plantable = MapManager.instance.Plantable(worldPosition);
        if (!plantable)
        {
            return;
        }
        bool planted = PlantManager.instance.Planted(worldPosition);
        if (!planted)
        {
            return;
        }
        // if (playerUnit.remainingWater<waterUsage){
        //     return;
        // }
        audioSource.clip = audioClip;
        StartCoroutine(WaterTreeCoroutine(worldPosition));
    }

    private IEnumerator WaterTreeCoroutine(Vector3 worldPosition)
    {
        bool water = PlantManager.instance.WaterPlant(worldPosition);
        if (!water)
        {
            yield break;
        }

        isPlanting = true;
        string animationName;


        Orientation orientation = GetTileToPlayerOrientation(plantTilemap.WorldToCell(worldPosition));
        spriteRenderer.flipX = false;
        switch (orientation)
        {
            case Orientation.UP:
                animationName = "PlayerWaterUp";
                break;
            case Orientation.DOWN:
                animationName = "PlayerWaterDown";
                break;
            default:
                animationName = "PlayerWaterHorizontal";
                if (orientation==Orientation.LEFT){
                    spriteRenderer.flipX = true;
                }
                break;
        }

        animator.Play(animationName);
        audioSource.Play();
        playerUnit.waterManager.UseWater(waterUsage); // use amount of water
        yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName) + 1f);
        isPlanting = false;
    }
    #endregion
}