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
    private PlayerUnit playerUnit;
    PlayerPlant playerPlant;
    Rigidbody2D rb;
    // public int price;
    private Image image, background, priceBg;
    private TextMeshProUGUI buyText, harvestText;

    void Start(){
        player = GameObject.FindGameObjectWithTag("Player");
        playerUnit = player.GetComponent<PlayerUnit>();

        playerPlant = player.GetComponent<PlayerPlant>();
        rb = player.GetComponent<Rigidbody2D>();

        background = GetComponent<Image>();
        priceBg = transform.GetChild(0).GetComponent<Image>();
        if (plant==null){
            return;
        }
        image = transform.GetChild(1).GetComponent<Image>();
        image.sprite = plant.tiles.Last().sprite;

        buyText = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        buyText.text = plant.buyMoney.ToString();

        harvestText = transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>();
        harvestText.text = plant.harvestMoney.ToString();
    }

    private void LateUpdate() {
        if (plant==null){
            return;
        }
        if (!playerUnit.moneyManager.SufficientMoney(plant.buyMoney)) {
            background.color = new Color(161f / 255f, 161f / 255f, 161f / 255f); 
            priceBg.color = new Color(144f / 255f, 144f / 255f, 144f / 255f); 
        }
        else {
            background.color = new Color(176f / 255f, 97f / 255f, 97f / 255f); 
            priceBg.color = new Color(255f / 255f, 102f / 255f, 102f / 255f); 
        }
    }


    public void ChoosePlant(){
        IEnumerator ShakeButton() {
            float duration = 0.1f;
            float magnitude = 5f;
            Vector3 originalPosition = transform.localPosition;

            float elapsed = 0f;
            while (elapsed < duration) {
                float x = originalPosition.x + Random.Range(-1f, 1f) * magnitude;
                float y = originalPosition.y + Random.Range(-1f, 1f) * magnitude;

                transform.localPosition = new Vector3(x, y, originalPosition.z);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = originalPosition;
        }
        
        if (!playerUnit.moneyManager.SufficientMoney(plant.buyMoney)) {
            // shake button
            StartCoroutine(ShakeButton());
            return;
        }
        var newPlant = Instantiate(plant); // create new copy of scriptable object
        playerPlant.PlantTree(rb.position, newPlant);
    }
}
