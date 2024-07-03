using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameLoading : MonoBehaviour
{
    public static bool? hasToLoad;
    public static string gameName;
    public static List<string> LoadGames()
    {
        List<string> gameNames = new List<string>();

        string savesDirectory = Path.Combine(Application.persistentDataPath, "saves");

        if (Directory.Exists(savesDirectory))
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(savesDirectory);
            DirectoryInfo[] directories = directoryInfo.GetDirectories();

            // most recent first
            Array.Sort(directories, (dir1, dir2) => dir2.LastWriteTime.CompareTo(dir1.LastWriteTime));

            foreach (DirectoryInfo directory in directories)
            {
                gameNames.Add(directory.Name);
            }
        }
        else
        {
            Debug.LogError("Saves directory does not exist: " + savesDirectory);
        }

        return gameNames;
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

    private void Start() {
        if (hasToLoad!=null&&(bool)hasToLoad&&gameName!=null){
            bool load = LoadGame(gameName);
            if (load==false){
                Debug.LogError("Error when load game");
            }
            else{
                hasToLoad = false;
                gameName = null;
            }
        }
    }

    private void LateUpdate() {
        if (hasToLoad!=null&&(bool)hasToLoad&&gameName!=null){
            bool load = LoadGame(gameName);
            if (load==false){
                Debug.LogError("Error when load game");
            }
            else{
                hasToLoad = false;
                gameName = null;
            }
        }
    }



    public bool LoadGame(string gameName)
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
                Debug.Log(TimeManage.instance.currentMinute);
            }

            if (!string.IsNullOrEmpty(plantsJson))
            {
                FetchPlants(plantsJson);
            }

            GameSaving gameSaving = GameObject.Find("SaveGame").GetComponent<GameSaving>();
            if (gameSaving==null){
                Debug.Log("Game saving not found");
                return false;
            }
            gameSaving.NewGame(gameName, save: false);
            Debug.Log("Loaded " + gameName + " successfully.");
            return true;
        }
        catch (Exception err)
        {
            Debug.LogError(err);
            return false;
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