using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameLoading : MonoBehaviour
{
    public static List<string> LoadGames()
    {
        // Fetch subfolders from the saves folder
        List<string> gameNames = new List<string>();

        string savesDirectory = Path.Combine(Application.persistentDataPath, "saves");

        if (Directory.Exists(savesDirectory))
        {
            string[] directories = Directory.GetDirectories(savesDirectory);
            return directories.ToList();
        }

        return null;

    }

    public static bool GameNameExists(string gameName)
    {
        List<string> gameNames = LoadGames();

        if (gameNames != null && gameNames.Contains(gameName))
        {
            return true;
        }

        return false;
    }



    public void LoadGame(string gameName)
    {
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
            PlantManager.instance.Load(); // Load information about plant
        }

        GameSaving gameSaving = transform.parent.GetComponent<GameSaving>();
        gameSaving.NewGame(gameName);

        string playerJsonFileName = Path.Combine(Application.persistentDataPath, "saves", gameName, "player.data");
        string playerDefendJsonFileName = Path.Combine(Application.persistentDataPath, "saves", gameName, "playerDefend.data");
        string playerGunJsonFileName = Path.Combine(Application.persistentDataPath, "saves", gameName, "playerGun.data");
        string timeJsonFileName = Path.Combine(Application.persistentDataPath, "saves", gameName, "time.data");
        string plantsJsonFileName = Path.Combine(Application.persistentDataPath, "saves", gameName, "plant.data");

        string playerJson = LoadJsonFromFile(playerJsonFileName);
        string playerDefendJson = LoadJsonFromFile(playerDefendJsonFileName);
        string playerGunJson = LoadJsonFromFile(playerGunJsonFileName);
        string timeJson = LoadJsonFromFile(timeJsonFileName);
        string plantsJson = LoadJsonFromFile(plantsJsonFileName);

        try
        {
            if (!string.IsNullOrEmpty(playerJson) && !string.IsNullOrEmpty(playerDefendJson))
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
        }
        catch (Exception err)
        {
            Debug.LogError(err);
        }
    }



    private string LoadJsonFromFile(string fileName)
    {
        try
        {
            if (File.Exists(fileName))
            {
                return File.ReadAllText(fileName);
            }
            else
            {
                Debug.LogError("File not found: " + fileName);
                return null;
            }
        }
        catch (Exception err)
        {
            Debug.LogError(err);
            return null;
        }
    }
}