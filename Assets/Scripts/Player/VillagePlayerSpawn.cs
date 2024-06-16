using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagePlayerSpawn : MonoBehaviour
{
    public GameObject player;    
    PlayerMove playerMove;
    void Start(){
        playerMove = player.GetComponent<PlayerMove>();

        playerMove.SetOrientation(Orientation.LEFT);
    }
}
