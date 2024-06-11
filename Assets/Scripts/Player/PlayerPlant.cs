using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerPlant : MonoBehaviour
{
    [HideInInspector] public bool isPlanting = false;
    public Tilemap plantTilemap;

    PlayerMove playerMove;
    Animator animator;
    Rigidbody2D rb;

    void Start(){
        animator = GetComponent<Animator>();
        playerMove = GetComponent<PlayerMove>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.P)) {
            WaterTree(rb.position);
        } // press P to plant
    }

    public void PlantTree(Vector3 worldPosition, Plant plant){
        bool plantable = MapManager.instance.Plantable(worldPosition);
        if (!plantable){
            return;
        }
        StartCoroutine(PlantTreeCoroutine(worldPosition, plant)); // planting tree animation
        

    }

    private IEnumerator PlantTreeCoroutine(Vector3 worldPosition, Plant plant)
    {

        Vector3Int cellPosition = plantTilemap.WorldToCell(worldPosition);
        plant.gridPosition = cellPosition; // store position
        bool plantEligible = PlantManager.instance.AddPlant(worldPosition, plant);

        if (plantEligible==false){
            yield break;
        }



        isPlanting = true;
        string animationName;

        switch (playerMove.orientation){
            case PlayerOrientation.UP:{
                    animationName = "PlayerPlantUp";
                    break;
            }
            case PlayerOrientation.DOWN:{
                    animationName = "PlayerPlantDown";
                    break;
            }
            default:{
                    animationName = "PlayerPlantHorizontal";
                    break;
            }
        }
        animator.SetBool("idle", false);
        animator.SetTrigger("plant");
        // wait for animation to complete
        yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName)+1f);
        animator.ResetTrigger("plant");
        animator.SetBool("idle", true);

        isPlanting = false;
        plantTilemap.SetTile(cellPosition, plant.tiles[plant.currentStage]); // set plant on tilemap
    }

    public void WaterTree(Vector3 worldPosition){
        bool plantable = MapManager.instance.Plantable(worldPosition);
        if (!plantable){
            return;
        }
        bool planted = MapManager.instance.Planted(worldPosition);
        if (planted){
            return;
        }

        PlantManager.instance.WaterPlant(worldPosition);
    }

    private IEnumerator WaterTreeCoroutine(Vector3 worldPosition)
    {
        bool water = PlantManager.instance.WaterPlant(worldPosition);
        if (water==false){ // failed to water
            yield break;
        }

        isPlanting = true;
        string animationName;

        switch (playerMove.orientation){
            case PlayerOrientation.UP:{
                    animationName = "PlayerWaterUp";
                    break;
            }
            case PlayerOrientation.DOWN:{
                    animationName = "PlayerWaterDown";
                    break;
            }
            default:{
                    animationName = "PlayerWaterHorizontal";
                    break;
            }
        }
        animator.SetBool("idle", false);
        animator.SetTrigger("water");
        // wait for animation to complete
        yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName)+1f);
        animator.ResetTrigger("plant");
        animator.SetBool("idle", true);

        isPlanting = false;


    }
}
