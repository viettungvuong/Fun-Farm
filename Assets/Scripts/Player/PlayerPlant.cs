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

    void Start(){
        animator = GetComponent<Animator>();
        playerMove = GetComponent<PlayerMove>();
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

        Vector3Int cellPosition = plantTilemap.WorldToCell(worldPosition);
        plant.gridPosition = cellPosition; // store position
        plantTilemap.SetTile(cellPosition, plant.tiles[plant.currentStage]);

        PlantManager.instance.AddPlant(worldPosition, plant);
    }


}
