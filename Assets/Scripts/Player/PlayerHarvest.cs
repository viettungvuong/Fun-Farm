using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerHarvest : MonoBehaviour
{
    private Tilemap plantTilemap;
    private PlantManager plantManager;
    Rigidbody2D rb;

    void Start(){
        rb = GetComponent<Rigidbody2D>();

        plantTilemap = GameObject.Find("PlantTilemap").GetComponent<Tilemap>();
        plantManager = PlantManager.instance;
    }

    void FixedUpdate(){
        if (PlantManager.instance.DetectPlantMaxStage(rb.position)){ // detect any plant
            Plant plant = PlantManager.instance.GetPlantAt(rb.position);
            
            plantManager.RemovePlant(plant, removeOnMap: true);

            // Add money to player account
        }
    }
}
