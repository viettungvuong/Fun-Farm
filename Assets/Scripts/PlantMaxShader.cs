using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantMaxShader : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")){
            gameObject.SetActive(false);

            // PlantedPlant plant = PlantManager.instance.GetPlantAt(rb.position);
            
            // plantManager.RemovePlant(plant, removeOnMap: true);

            // // Add money to player account
            // playerUnit.AddMoney(plant.harvestMoney);
        }
    }
}
