using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VillagePlayerSpawn : MonoBehaviour
{
    private GameObject player;
    PlayerMove playerMove;
    bool playerEntered = false;
    void Start(){
        player = GameObject.FindGameObjectWithTag("Player");

        playerMove = player.GetComponent<PlayerMove>();
        player.transform.position = transform.position;
        playerMove.SetOrientation(Orientation.LEFT);
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")&&playerEntered==false){
            playerEntered = true; // player go to the scene and move out of entrance
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")&&playerEntered==true){
            GoBackHome(player.transform);
            // player.SetActive(false);
        }
    }

    public static void GoBackHome(Transform transform){
        transform.position = new Vector3(-16.56f, -4.33f);
        SceneManager.LoadScene("SceneHome"); // go back home
    }
}
