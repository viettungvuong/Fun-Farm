using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : MonoBehaviour
{
    private void Start() {
        DontDestroyOnLoad(gameObject);
    }
    
    private void Update() {
        if (GameController.HomeScene()){
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
        else{
            gameObject.transform.localScale = new Vector3(0, 0, 0); // hide
        }
    }
}
