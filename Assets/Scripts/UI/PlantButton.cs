using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlantButton : MonoBehaviour
{
    public Plant plant;
    private GameObject player;
    PlayerPlant playerPlant;
    Rigidbody2D rb;
    // public int price;
    private Image image;
    private TextMeshProUGUI buyText, harvestText;

    void Start(){
        player = GameObject.FindGameObjectWithTag("Player");

        playerPlant = player.GetComponent<PlayerPlant>();
        rb = player.GetComponent<Rigidbody2D>();

        image = transform.GetChild(1).GetComponent<Image>();
        image.sprite = plant.tiles.Last().sprite;

        buyText = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        buyText.text = plant.buyMoney.ToString();

        harvestText = transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>();
        harvestText.text = plant.harvestMoney.ToString();
    }


    public void ChoosePlant(){
        var newPlant = Instantiate(plant); // create new copy of scriptable object
        newPlant.currentStage = 0;
        newPlant.health = 100;
        playerPlant.PlantTree(rb.position, newPlant);
    }
}
