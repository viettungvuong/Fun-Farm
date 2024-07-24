using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantMaxShader : MonoBehaviour
{
    public PlantedPlant plant;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")){
            gameObject.SetActive(false);

            PlayerUnit playerUnit = other.gameObject.GetComponent<PlayerUnit>();

            if (plant==null){
                return;
            }
            Debug.Log("Harvest");
            PlantManager.instance.RemovePlant(plant, removeOnMap: true);

            // Add money to player account
            playerUnit.moneyManager.AddMoney(plant.harvestMoney);
        }
    }
}
