using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGoStore : MonoBehaviour
{
    public GameObject marketPanel, gunPanel, bulletPanel;
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name=="StoreEntrance"){
            gunPanel.gameObject.SetActive(false);

            marketPanel.gameObject.SetActive(true);
            marketPanel.transform.SetAsLastSibling(); // bottom of canvas hierarchy



        }
        else if (other.gameObject.name=="GunEntrance"){
            marketPanel.gameObject.SetActive(false);
            gunPanel.gameObject.SetActive(true);
            gunPanel.transform.SetAsLastSibling();

            bulletPanel.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.name=="StoreEntrance"){
            marketPanel.gameObject.SetActive(false);
        }
        else if (other.gameObject.name=="GunEntrance"){
            gunPanel.gameObject.SetActive(false);

            bulletPanel.SetActive(false);
        }
    }
}
