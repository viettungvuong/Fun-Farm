using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class GameLoading : MonoBehaviour
{

    private string playerJsonFileName, playerDefendJsonFileName, playerGunJsonFileName, timeJsonFileName, plantsJsonFileName;

    void FetchPlayer(string unitJson, string defendJson, string gunJson)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        JsonUtility.FromJsonOverwrite(unitJson, player.GetComponent<PlayerUnit>());
        JsonUtility.FromJsonOverwrite(defendJson, player.GetComponent<PlayerDefend>());
        JsonUtility.FromJsonOverwrite(gunJson, player.GetComponent<PlayerGun>());
    }

    void FetchTime(string json)
    {
        JsonUtility.FromJsonOverwrite(json, TimeManage.instance);
    }

    void FetchPlants(string json)
    {
        JsonUtility.FromJsonOverwrite(json, PlantPos.instance);
        PlantPos.instance.Deserialize();
        PlantManager.instance.Load(); // load information about plant
    }

    void Start()
    {

        playerJsonFileName = Application.persistentDataPath + "/player.data";
        playerDefendJsonFileName = Application.persistentDataPath + "/playerDefend.data";
        playerGunJsonFileName = Application.persistentDataPath + "/playerGun.data"; 
        timeJsonFileName = Application.persistentDataPath + "/time.data";
        plantsJsonFileName = Application.persistentDataPath + "/plant.data";

        LoadGame();
    }

    void LoadGame()
    {
        string playerJson = LoadJsonFromFile(playerJsonFileName);
        string playerDefendJson = LoadJsonFromFile(playerDefendJsonFileName);
        string playerGunJson = LoadJsonFromFile(playerGunJsonFileName);
        string timeJson = LoadJsonFromFile(timeJsonFileName);
        string plantsJson = LoadJsonFromFile(plantsJsonFileName);

        try{
            if (!string.IsNullOrEmpty(playerJson)&&!string.IsNullOrEmpty(playerDefendJson))
            {
                FetchPlayer(playerJson, playerDefendJson, playerGunJson);
            }

            if (!string.IsNullOrEmpty(timeJson))
            {
                FetchTime(timeJson);
            }

            if (!string.IsNullOrEmpty(plantsJson))
            {
                FetchPlants(plantsJson);
            }
        } catch (Exception err){
            Debug.LogError(err);
        }


    }

    string LoadJsonFromFile(string fileName)
    {
        try{
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            else
            {
                Debug.LogError("File not found: " + filePath);
                return null;
            }
        } catch (Exception err){
            Debug.LogError(err);
            return null;
        }

    }
}
