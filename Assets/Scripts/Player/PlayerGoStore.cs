using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGoStore : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name=="Entrance"){
            StoreEntrance entrance = other.gameObject.GetComponent<StoreEntrance>();
            entrance.OpenStore();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.name=="Entrance"){
            StoreEntrance entrance = other.gameObject.GetComponent<StoreEntrance>();
            entrance.CloseStore();
        }
    }
}
