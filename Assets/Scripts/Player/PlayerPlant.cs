using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PlayerPlant : MonoBehaviour
{
    [HideInInspector] public bool isPlanting = false;
    private Tilemap plantTilemap;

    PlayerMove playerMove;
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

        audioSource = gameObject.GetComponent<AudioSource>();
        

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

    public void PlantTree(Vector3 worldPosition, Plant plant)
    {
        bool plantable = MapManager.instance.Plantable(worldPosition);
        if (!plantable)
        {
            return;
        }

        if (!playerUnit.SufficientMoney(plant.buyMoney))
        {
            return;
        }

        playerUnit.UseMoney(plant.buyMoney);
        StartCoroutine(PlantTreeCoroutine(worldPosition, new PlantedPlant(plant, plantTilemap.WorldToCell(worldPosition))));
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

        // direction between plant and player
        Orientation GetTileToPlayerOrientation()
        {
            Vector3Int direction = plantCellPosition - playerCellPosition;

            if (direction.x==0&&direction.y==0){ // the same position
                return playerMove.orientation;
            }

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                return direction.x >= 0 ? Orientation.RIGHT : Orientation.LEFT;
            }
            else
            {
                return direction.y >= 0 ? Orientation.UP : Orientation.DOWN;
            }
        }

        isPlanting = true;
        string animationName;

        switch (GetTileToPlayerOrientation())
        {
            case Orientation.UP:
                animationName = "PlayerPlantUp";
                break;
            case Orientation.DOWN:
                animationName = "PlayerPlantDown";
                break;
            default:
                animationName = "PlayerPlantHorizontal";
                break;
        }

        animator.Play(animationName);

        yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName));

        isPlanting = false;
 
        plantTilemap.SetTile(plantCellPosition, plant.tiles[plant.currentStage]);
    }

    const double waterUsage = 0.15;

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

        switch (playerMove.orientation)
        {
            case Orientation.UP:
                animationName = "PlayerWaterUp";
                break;
            case Orientation.DOWN:
                animationName = "PlayerWaterDown";
                break;
            default:
                animationName = "PlayerWaterHorizontal";
                break;
        }

        animator.Play(animationName);
        audioSource.Play();
        yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName) + 1f);
        isPlanting = false;
    }
}