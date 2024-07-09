using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeUI : MonoBehaviour
{
    public GameObject fence, health, enemy;

    void Awake()
    {
        if (PlayerUnit.playerMode == PlayerMode.SURVIVAL){
            fence.SetActive(true);
            health.SetActive(true);
            enemy.SetActive(true);
        }
        else{
            fence.SetActive(false);
            health.SetActive(false);
            enemy.SetActive(false);
        }
    }

    void Start()
    {
        if (PlayerUnit.playerMode == PlayerMode.SURVIVAL){
            fence.SetActive(true);
            health.SetActive(true);
            enemy.SetActive(true);
        }
        else{
            fence.SetActive(false);
            health.SetActive(false);
            enemy.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerUnit.playerMode == PlayerMode.SURVIVAL){
            fence.SetActive(true);
            health.SetActive(true);
            enemy.SetActive(true);
        }
        else{
            fence.SetActive(false);
            health.SetActive(false);
            enemy.SetActive(false);
        }
    }
}
