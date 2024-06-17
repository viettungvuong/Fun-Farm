using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterEntrance : MonoBehaviour
{
    public float waterAmount = 0.25f; // Amount of water to add

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Taking water enter");
            PlayerUnit playerUnit = other.gameObject.GetComponent<PlayerUnit>();
            playerUnit.AddWater(waterAmount);
        }
    }
     private void OnTriggerStay2D(Collider2D other) // continuously add water
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Taking water stay");
            PlayerUnit playerUnit = other.gameObject.GetComponent<PlayerUnit>();
            playerUnit.AddWater(waterAmount);
        }
    }
}
