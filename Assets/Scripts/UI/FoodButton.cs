using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodButton : MonoBehaviour
{
    public FoodData food;
    private Image image;
    private TextMeshProUGUI priceText, healthRecoveredText;
    private PlayerUnit player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUnit>();

        image = transform.GetChild(1).GetComponent<Image>();
        image.sprite = food.sprite;

        priceText = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        priceText.text = food.price.ToString();

        healthRecoveredText = transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
        healthRecoveredText.text = food.healthRecovered.ToString();
    }

    public void ChooseFood(){
        if (player.SufficientMoney(food.price)){
            player.UseMoney(food.price);

            player.RecoverHealth(food.healthRecovered);
        }
    }

}
