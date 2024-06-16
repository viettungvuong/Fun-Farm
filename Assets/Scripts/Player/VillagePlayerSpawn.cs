using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagePlayerSpawn : MonoBehaviour
{
    private GameObject player;
    Rigidbody2D playerRb;   
    PlayerMove playerMove;
    void Start(){
        player = GameObject.FindGameObjectWithTag("Player");


        playerMove = player.GetComponent<PlayerMove>();
        playerRb = player.GetComponent<Rigidbody2D>();
        player.transform.position = transform.position;
        playerMove.SetOrientation(Orientation.LEFT);
    }
}
