using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameLoading : MonoBehaviour
{

    private string playerJsonFileName, playerDefendJsonFileName, timeJsonFileName, plantsJsonFileName;

    void FetchPlayer(string unitJson, string defendJson)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        JsonUtility.FromJsonOverwrite(unitJson, player.GetComponent<PlayerUnit>());
        JsonUtility.FromJsonOverwrite(defendJson, player.GetComponent<PlayerDefend>());
    }

    void FetchTime(string json)
    {
        JsonUtility.FromJsonOverwrite(json, TimeManage.instance);
    }

    void FetchPlants(string json)
    {
        JsonUtility.FromJsonOverwrite(json, PlantPos.instance);
        PlantPos.instance.LoadToTilemap();
    }

    void Start()
    {

        playerJsonFileName = Application.persistentDataPath + "/player.data";
        playerDefendJsonFileName = Application.persistentDataPath + "/playerDefend.data";
        timeJsonFileName = Application.persistentDataPath + "/time.data";
        plantsJsonFileName = Application.persistentDataPath + "/plant.data";

        LoadGame();
    }

    void LoadGame()
    {
        string playerJson = LoadJsonFromFile(playerJsonFileName);
        string playerDefendJson = LoadJsonFromFile(playerDefendJsonFileName);
        string timeJson = LoadJsonFromFile(timeJsonFileName);
        string plantsJson = LoadJsonFromFile(plantsJsonFileName);

        if (!string.IsNullOrEmpty(playerJson)&&!string.IsNullOrEmpty(playerDefendJson))
        {
            FetchPlayer(playerJson, playerDefendJson);
        }

        if (!string.IsNullOrEmpty(timeJson))
        {
            FetchTime(timeJson);
        }

        if (!string.IsNullOrEmpty(plantsJson))
        {
            FetchPlants(plantsJson);
        }
    }

    string LoadJsonFromFile(string fileName)
    {
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
    }
}
