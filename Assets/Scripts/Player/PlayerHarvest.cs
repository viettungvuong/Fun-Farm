using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerHarvest : MonoBehaviour
{

    private PlantManager plantManager;
    Rigidbody2D rb;
    PlayerUnit playerUnit;

    void Start(){
        rb = GetComponent<Rigidbody2D>();
        plantManager = PlantManager.instance;
        playerUnit = GetComponent<PlayerUnit>();
    }

    void FixedUpdate(){
        if (PlantManager.instance.DetectPlantMaxStage(rb.position)){ // detect any plant
            Plant plant = PlantManager.instance.GetPlantAt(rb.position);
            
            plantManager.RemovePlant(plant, removeOnMap: true);

            // Add money to player account
            playerUnit.AddMoney(plant.harvestMoney);
        }
    }
}
