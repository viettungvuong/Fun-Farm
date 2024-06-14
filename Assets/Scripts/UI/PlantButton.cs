using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlantButton : MonoBehaviour
{
    public Plant plant;
    public GameObject player;
    PlayerPlant playerPlant;
    Rigidbody2D rb;
    // public int price;
    private Image image;

    void Start(){
        playerPlant = player.GetComponent<PlayerPlant>();
        rb = player.GetComponent<Rigidbody2D>();
        image = transform.GetChild(0).GetComponent<Image>();
        image.sprite = plant.tiles.Last().sprite;
    }


    public void ChoosePlant(){
        var newPlant = Instantiate(plant); // create new copy of scriptable object
        newPlant.currentStage = 0;
        newPlant.health = 100;
        playerPlant.PlantTree(rb.position, newPlant);
    }
}
