using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSaving : MonoBehaviour
{
    private GameObject player;

    private PlayerUnit playerUnit;

    private TimeManage time;
    private PlantManager plantManager;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        playerUnit = player.GetComponent<PlayerUnit>();

        time = TimeManage.instance;
        plantManager = PlantManager.instance;
    }

    void SaveGame(){
        string SavePlayer(){ // player health, money
            return JsonUtility.ToJson(playerUnit);
        }

        string SaveTime(){ // current time
            return JsonUtility.ToJson(time);
        }

        string SavePlants(){ // planted plant, their status
            return JsonUtility.ToJson(plantManager);
        }

        string playerFile = Application.persistentDataPath + "/player.data";
        File.WriteAllText(playerFile, SavePlayer());

        string timeFile = Application.persistentDataPath + "/time.data";
        File.WriteAllText(timeFile, SaveTime());

        string plantFile = Application.persistentDataPath + "/plant.data";
        File.WriteAllText(plantFile, SavePlants());
    }



    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")){
            if (SkeletonGenerate.skeletons<=0&&SlimeGenerate.slimes<=0)
                SaveGame(); // only save when all enemies have been killed
        }
    }
}
