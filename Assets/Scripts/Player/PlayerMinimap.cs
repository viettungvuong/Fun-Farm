using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMinimap : MonoBehaviour
{
    public GameObject minimap;

    private void Update() {
        if (GameController.HomeScene()){
            minimap.transform.localScale = new Vector3(1, 1, 1);
        }
        else{
            minimap.transform.localScale = new Vector3(0, 0, 0);
        }
    }
}
