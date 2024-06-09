using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlantButton : MonoBehaviour
{
    public Tile plantTile;
    public GameObject player;
    PlayerPlant playerPlant;
    Rigidbody2D rb;
    // public int price;

    void Start(){
        playerPlant = player.GetComponent<PlayerPlant>();
        rb = player.GetComponent<Rigidbody2D>();
    }


    public void ChoosePlant(){
        playerPlant.PlantTree(rb.position, plantTile);
    }
}
