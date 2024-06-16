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
    PlayerUnit playerUnit;

    void Start(){
        animator = GetComponent<Animator>();
        playerMove = GetComponent<PlayerMove>();
        rb = GetComponent<Rigidbody2D>();
        playerUnit = GetComponent<PlayerUnit>();
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.W)) {
            WaterTree(rb.position);
        } // press W to water
    }

    public void PlantTree(Vector3 worldPosition, Plant plant){
        bool plantable = MapManager.instance.Plantable(worldPosition);
        if (!plantable){
            return;
        }

        if (playerUnit.SufficientMoney(plant.buyMoney)==false){
            return; // not enough money to buy
        }

        playerUnit.UseMoney(plant.buyMoney);

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

        Orientation tileToPlayer()
        {
            // Calculate the direction vector from the tile to the player
            Vector3 direction = (Vector3)rb.position - worldPosition;
            
            // Normalize the direction vector to get the direction in terms of unit vectors
            direction.Normalize();

            // Determine the orientation based on the direction vector
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
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


        isPlanting = true;
        string animationName;

        switch (tileToPlayer()){
            case Orientation.UP:{
                    animationName = "PlayerPlantUp";
                    break;
            }
            case Orientation.DOWN:{
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

    const double waterUsage = 0.15;

    public void WaterTree(Vector3 worldPosition){


        bool plantable = MapManager.instance.Plantable(worldPosition);
        if (!plantable){
            return;
        }
        bool planted = PlantManager.instance.Planted(worldPosition);
        if (!planted){
            return;
        }

        // check sufficient water
        if (playerUnit.SufficientWater(waterUsage)==false){
            return; // insufficient water
        }


        // PlantManager.instance.WaterPlant(worldPosition);
        StartCoroutine(WaterTreeCoroutine(worldPosition));
    }

    private IEnumerator WaterTreeCoroutine(Vector3 worldPosition)
    {
        bool water = PlantManager.instance.WaterPlant(worldPosition);
        if (water==false){ // failed to water
            yield break;
        }

        playerUnit.UseWater(waterUsage);

        isPlanting = true;
        string animationName;
        

        switch (playerMove.orientation){
            case Orientation.UP:{
                    animationName = "PlayerWaterUp";
                    break;
            }
            case Orientation.DOWN:{
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
        animator.ResetTrigger("water");
        animator.SetBool("idle", true);

        isPlanting = false;


    }
}
