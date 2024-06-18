using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VillageEntrance : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name=="Player"&&TimeManage.instance.IsDay()){
            // move to village scene
                SceneManager.LoadScene("SceneVillage");
        }
    }
}
