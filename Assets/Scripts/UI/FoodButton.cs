using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodButton : MonoBehaviour
{
    public FoodData food;
    private Image image, background, priceBg, healthRecoveredBg;
    private TextMeshProUGUI priceText, healthRecoveredText;
    private PlayerUnit player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerUnit>();

        background = GetComponent<Image>();
        priceBg = transform.GetChild(0).GetComponent<Image>();
        healthRecoveredBg = transform.GetChild(2).GetComponent<Image>();

        image = transform.GetChild(1).GetComponent<Image>();
        image.sprite = food.sprite;

        priceText = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        priceText.text = food.price.ToString();

        healthRecoveredText = transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
        healthRecoveredText.text = food.healthRecovered.ToString();
    }

    private void LateUpdate() {
        if (!player.SufficientMoney(food.price)||!player.EatNeeded(food.healthRecovered)) {
            background.color = new Color(161f / 255f, 161f / 255f, 161f / 255f); 
            Color colorBg = new Color(144f / 255f, 144f / 255f, 144f / 255f);
            priceBg.color = colorBg;
            healthRecoveredBg.color = colorBg;
        }
        else {
            background.color = new Color(176f / 255f, 166f / 255f, 97f / 255f); 
            Color colorBg = new Color(236f / 255f, 166f / 255f, 102f / 90f);
            priceBg.color = colorBg;
            healthRecoveredBg.color = colorBg;
        }
    }

    public void ChooseFood(){
        Debug.Log("Clicked food");
        IEnumerator ShakeButton() {
            float duration = 0.1f;
            float magnitude = 5f;
            Vector3 originalPosition = transform.localPosition;

            float elapsed = 0f;
            while (elapsed < duration) {
                float x = originalPosition.x + UnityEngine.Random.Range(-1f, 1f) * magnitude;
                float y = originalPosition.y + UnityEngine.Random.Range(-1f, 1f) * magnitude;

                transform.localPosition = new Vector3(x, y, originalPosition.z);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = originalPosition;
        }

        if (player.SufficientMoney(food.price)&&player.EatNeeded(food.healthRecovered)){
            player.UseMoney(food.price);

            player.RecoverHealth(food.healthRecovered);
        }
        else{
            
            StartCoroutine(ShakeButton());
        }
    }

}
