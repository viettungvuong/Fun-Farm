using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSaving : MonoBehaviour
{
    public static GameSaving instance;
    private GameObject player;
    private PlayerUnitData playerUnit;
    private PlayerDefendData playerDefend;
    private PlayerGunData playerGun;
    private BuyLandData buyLand;
    private TimeData time;
    private PlantPos plant;

    public GameObject savingPanelPrefab;

    private string gameName = "Untitled";

    private Renderer renderer;


    public bool NewGame(string gameName, bool save=true){
        this.gameName = gameName;
        if (save){
            if (SaveGame()){ // save beforehand
                Debug.Log("saved successfully");
                return true;
            }
            else{
                Debug.LogError("Error when saving game");
                return false;
            }
        }
        else{
            return true;
        }


    }

    public string GetName(){
        return gameName;
    }

    void Start()
    {
        if (instance==null){
            instance = this;
        }
        else{
            Destroy(this);
        }
        DontDestroyOnLoadManager.DontDestroyOnLoad(gameObject);
        renderer = GetComponent<Renderer>();

    }

    private void Update() {
        Debug.Log(gameName);
    }

    private void InitializeReferences()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerUnit = player.GetComponent<PlayerUnit>().Serialize();
            playerDefend = player.GetComponent<PlayerDefend>().Serialize();
            playerGun = player.GetComponent<PlayerGun>().Serialize();
            buyLand = player.GetComponent<BuyLand>().Serialize();
        }

        time = TimeManage.instance.Serialize();
        plant = PlantPos.instance;
    }


    bool SaveGame()
    {
        string SavePlayer()
        {
            if (playerUnit == null)
                return null;

            return JsonUtility.ToJson(playerUnit);
        }

        string SavePlayerDefend()
        {
            if (playerDefend == null)
                return null;

            return JsonUtility.ToJson(playerDefend);
        }

        string SavePlayerGun()
        {
            if (playerGun == null)
                return null;

            return JsonUtility.ToJson(playerGun);
        }

        string SaveBuyLand()
        {
            if (buyLand == null)
                return null;

            return JsonUtility.ToJson(buyLand);
        }

        string SaveTime()
        {
            if (time == null)
                return null;

            return JsonUtility.ToJson(time);
        }

        string SavePlants()
        {
            if (plant == null)
                return null;

            return JsonUtility.ToJson(plant);
        }

        try
        {
            InitializeReferences();
            string saveDirectory = Path.Combine(Application.persistentDataPath, "saves", gameName);
            if (Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            string playerFile = Path.Combine(saveDirectory, "player.data");
            string json = SavePlayer();
            if (json != null)
            {
                Debug.Log(json);
                File.WriteAllText(playerFile, json);
            }
            else
            {
                Debug.LogError("SavePlayer() returned null. Player data not saved.");
            }

            string playerDefendFile = Path.Combine(saveDirectory, "playerDefend.data");
            json = SavePlayerDefend();
            if (json != null)
            {
                Debug.Log(json);
                File.WriteAllText(playerDefendFile, json);
            }
            else
            {
                Debug.LogError("SavePlayerDefend() returned null. Player defend data not saved.");
            }

            string playerGunFile = Path.Combine(saveDirectory, "playerGun.data");
            json = SavePlayerGun();
            if (json != null)
            {
                Debug.Log(json);
                File.WriteAllText(playerGunFile, json);
            }
            else
            {
                Debug.LogError("SavePlayerGun() returned null. Player gun data not saved.");
            }

            string buyLandFile = Path.Combine(saveDirectory, "buyLand.data");
            json = SaveBuyLand();
            if (json != null)
            {
                Debug.Log(json);
                File.WriteAllText(buyLandFile, json);
            }
            else
            {
                Debug.LogError("SaveBuyLand() returned null. Player gun data not saved.");
            }

            string timeFile = Path.Combine(saveDirectory, "time.data");
            json = SaveTime();
            if (json != null)
            {
                Debug.Log(json);
                File.WriteAllText(timeFile, json);
            }
            else
            {
                Debug.LogError("SaveTime() returned null. Time data not saved.");
            }

            string plantFile = Path.Combine(saveDirectory, "plant.data");
            json = SavePlants();
            if (json != null)
            {
                Debug.Log(json);
                File.WriteAllText(plantFile, json);
            }
            else
            {
                Debug.LogError("SavePlants() returned null. Plant data not saved.");
            }
           
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }

    private void LateUpdate() {
        if (SkeletonGenerate.skeletons <= 0 && SlimeGenerate.slimes <= 0){
            renderer.enabled = true;
        }
        else{
            renderer.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (SkeletonGenerate.skeletons <= 0 && SlimeGenerate.slimes <= 0)
            {
                if (SaveGame()) // only save when all enemies have been killed
                {
                    Debug.Log("Saved successfully "+gameName);
                    StartCoroutine(ShowSavingPanel());
                }
            }
        }
    }

    private IEnumerator ShowSavingPanel()
    {
        GameObject activeCanvas = FindActiveCanvas();
        if (activeCanvas != null)
        {
            GameObject savingPanelInstance = Instantiate(savingPanelPrefab, activeCanvas.transform);
            RectTransform rectTransform = savingPanelInstance.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(1, 0); // Bottom right corner
                rectTransform.anchorMax = new Vector2(1, 0);
                rectTransform.pivot = new Vector2(1, 0);
                rectTransform.anchoredPosition = new Vector2(-10, 10); // Adjust as needed
            }

            savingPanelInstance.SetActive(true);
            yield return new WaitForSeconds(4.0f);
            Destroy(savingPanelInstance);
        }
    }

    private GameObject FindActiveCanvas()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            if (canvas.isActiveAndEnabled && canvas.gameObject.scene.isLoaded)
            {
                return canvas.gameObject;
            }
        }
        return null;
    }
}