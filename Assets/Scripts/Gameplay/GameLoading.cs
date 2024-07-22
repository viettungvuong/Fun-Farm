using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public abstract class DataSerialize{
    public static T Deserialize<T>(string json) where T : DataSerialize
    {
        return JsonUtility.FromJson<T>(json);
    }
}

[Serializable]
public class TimeData: DataSerialize
{
    public int currentHour;
    public int currentMinute;
    public int currentDay;
    public WeatherType weatherType;

    public static TimeData Deserialize(string json)
    {
        return DataSerialize.Deserialize<TimeData>(json);
    }
}

[Serializable]
public class PlayerGunData: DataSerialize
{
    public int totalBullets;
    public int bulletsInClip;
    public bool ownedGun;
    public Weapon currentWeapon;

    public static PlayerGunData Deserialize(string json)
    {
        return DataSerialize.Deserialize<PlayerGunData>(json);
    }
}

[Serializable]
public class PlayerUnitData: DataSerialize{
    public int currentMoney;
    public double currentHealth;
    public int nextSlimeSpawnMin, nextSkeletonSpawnMin;
    public float water;
    public PlayerMode playerMode;

    public static PlayerUnitData Deserialize(string json)
    {
        return DataSerialize.Deserialize<PlayerUnitData>(json);
    }
}

[Serializable]
public class PlayerDefendData: DataSerialize
{
    public SerializableDictionary<Vector3Int, FenceOrientation> fences = new SerializableDictionary<Vector3Int, FenceOrientation>();
    public int numberOfFences;
    public int nextMinuteRefill;

    public static PlayerDefendData Deserialize(string json)
    {
        return DataSerialize.Deserialize<PlayerDefendData>(json);
    }
}

[Serializable]
public class BuyLandData: DataSerialize{
    public List<Vector3Int> purchased;

    public static BuyLandData Deserialize(string json)
    {
        return DataSerialize.Deserialize<BuyLandData>(json);
    }
}

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
        void FetchPlayer(string unitJson, string defendJson, string gunJson, string buyLandJson)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            // JsonUtility.FromJsonOverwrite(unitJson, player.GetComponent<PlayerUnit>());
            // JsonUtility.FromJsonOverwrite(defendJson, player.GetComponent<PlayerDefend>());
            // JsonUtility.FromJsonOverwrite(gunJson, player.GetComponent<PlayerGun>());
            Debug.Log(defendJson);
            player.GetComponent<PlayerUnit>().Reload(PlayerUnitData.Deserialize(unitJson));
            player.GetComponent<PlayerDefend>().Reload(PlayerDefendData.Deserialize(defendJson));
            player.GetComponent<PlayerGun>().Reload(PlayerGunData.Deserialize(gunJson));
            player.GetComponent<BuyLand>().Reload(BuyLandData.Deserialize(buyLandJson));
        }

        void FetchTime(string json)
        {
            // JsonUtility.FromJsonOverwrite(json, TimeManage.instance);
            TimeManage.instance.Reload(TimeData.Deserialize(json));
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
        string buyLandJsonFileName = Path.Combine(Application.persistentDataPath, "saves", gameName, "buyLand.data");

        string playerJson = LoadJsonFromFile(playerJsonFileName);
        string playerDefendJson = LoadJsonFromFile(playerDefendJsonFileName);
        string playerGunJson = LoadJsonFromFile(playerGunJsonFileName);
        string buyLandJson = LoadJsonFromFile(buyLandJsonFileName);
        string timeJson = LoadJsonFromFile(timeJsonFileName);
        string plantsJson = LoadJsonFromFile(plantsJsonFileName);

        try
        {
            if (!string.IsNullOrEmpty(playerJson) && !string.IsNullOrEmpty(playerDefendJson))
            {
                FetchPlayer(playerJson, playerDefendJson, playerGunJson, buyLandJson);
            }

            if (!string.IsNullOrEmpty(timeJson))
            {
                FetchTime(timeJson);
            }

            if (!string.IsNullOrEmpty(plantsJson))
            {
                FetchPlants(plantsJson);
            }

            GameSaving gameSaving = GameSaving.instance;
            if (gameSaving==null){
                Debug.LogError("Game saving not found");
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