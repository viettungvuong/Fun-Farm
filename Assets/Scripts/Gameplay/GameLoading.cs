using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameLoading : MonoBehaviour
{
    private PlayerUnit playerUnit;
    private TimeManage timeManage;
    private PlantManager plantManager;

    private string playerJsonFileName;
    private string timeJsonFileName;
    private string plantsJsonFileName;

    PlayerUnit FetchPlayer(string json)
    {
        return JsonUtility.FromJson<PlayerUnit>(json);
    }

    TimeManage FetchTime(string json)
    {
        return JsonUtility.FromJson<TimeManage>(json);
    }

    PlantManager FetchPlants(string json)
    {
        return JsonUtility.FromJson<PlantManager>(json);
    }

    void Start()
    {

        playerJsonFileName = Application.persistentDataPath + "/player.data";
        timeJsonFileName = Application.persistentDataPath + "/time.data";
        plantsJsonFileName = Application.persistentDataPath + "/plant.data";

        playerUnit = GetComponent<PlayerUnit>();
        timeManage = GetComponent<TimeManage>();
        plantManager = GetComponent<PlantManager>();

        LoadGame();
    }

    void LoadGame()
    {
        string playerJson = LoadJsonFromFile(playerJsonFileName);
        string timeJson = LoadJsonFromFile(timeJsonFileName);
        string plantsJson = LoadJsonFromFile(plantsJsonFileName);

        if (!string.IsNullOrEmpty(playerJson))
        {
            playerUnit = FetchPlayer(playerJson);
        }

        if (!string.IsNullOrEmpty(timeJson))
        {
            timeManage = FetchTime(timeJson);
        }

        if (!string.IsNullOrEmpty(plantsJson))
        {
            plantManager = FetchPlants(plantsJson);
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
