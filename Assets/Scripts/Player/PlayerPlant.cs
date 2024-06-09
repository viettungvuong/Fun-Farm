using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerPlant : MonoBehaviour
{
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
        StartCoroutine(PlantTreeCoroutine(worldPosition, plant));
    }

    private IEnumerator PlantTreeCoroutine(Vector3 worldPosition, Plant plant)
    {

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

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName(animationName)) // make sure not playing the current animation
        // this ensures animation not reset when pressing
        {
            animator.SetBool("idle", false);
            animator.SetTrigger("plant");
            // đợi animation xong
            yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName)+0.5f);
            animator.SetBool("idle", true);
        }

        Vector3Int cellPosition = plantTilemap.WorldToCell(worldPosition);
        plant.gridPosition = cellPosition; // store position
        plantTilemap.SetTile(cellPosition, plant.tiles[plant.currentStage]);

        PlantManager.instance.AddPlant(worldPosition, plant);
    }


}
