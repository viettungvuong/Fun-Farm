using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGoStore : MonoBehaviour
{
    public GameObject marketPanel, gunPanel;
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name=="StoreEntrance"){
            marketPanel.gameObject.SetActive(true);
            gunPanel.gameObject.SetActive(false);
        }
        else if (other.gameObject.name=="GunEntrance"){
            Debug.Log("Gun");
            marketPanel.gameObject.SetActive(false);
            gunPanel.gameObject.SetActive(true); 
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.name=="StoreEntrance"){
            marketPanel.gameObject.SetActive(false);
        }
        else if (other.gameObject.name=="GunEntrance"){
            gunPanel.gameObject.SetActive(false);
        }
    }
}
