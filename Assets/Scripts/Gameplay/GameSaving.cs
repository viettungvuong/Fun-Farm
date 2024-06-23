using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSaving : MonoBehaviour
{
    private GameObject player;

    private PlayerUnit playerUnit;
    private PlayerDefend playerDefend;

    private TimeManage time;
    private PlantPos plant;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        playerUnit = player.GetComponent<PlayerUnit>();
        playerDefend = player.GetComponent<PlayerDefend>();

        time = TimeManage.instance;
        plant = PlantPos.instance;
    }

    void SaveGame(){
        string SavePlayer(){ // player health, money
            return JsonUtility.ToJson(playerUnit);
        }

        string SavePlayerDefend(){ // number of fences
            return JsonUtility.ToJson(playerDefend);
        }

        string SaveTime(){ // current time
            return JsonUtility.ToJson(time);
        }

        string SavePlants(){ // planted plant, their status
            plant.SetSaveTime();
            return JsonUtility.ToJson(plant);
        }

        string playerFile = Application.persistentDataPath + "/player.data";
        File.WriteAllText(playerFile, SavePlayer());

        string playerDefendFile = Application.persistentDataPath + "/playerDefend.data";
        File.WriteAllText(playerDefendFile, SavePlayerDefend());

        string timeFile = Application.persistentDataPath + "/time.data";
        File.WriteAllText(timeFile, SaveTime());

        string plantFile = Application.persistentDataPath + "/plant.data";
        Debug.Log(SavePlants());
        File.WriteAllText(plantFile, SavePlants());
    }



    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")){
            if (SkeletonGenerate.skeletons<=0&&SlimeGenerate.slimes<=0)
                SaveGame(); // only save when all enemies have been killed
                Debug.Log("Saved game");
        }
    }
}
