using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageEntrance : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name=="Player"){
            // move to village scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("SceneVillage");
        }
    }
}
