using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGoStore : MonoBehaviour
{
    public GameObject marketPanel;
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name=="Entrance"){
            marketPanel.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.name=="Entrance"){
            marketPanel.gameObject.SetActive(false);
        }
    }
}
