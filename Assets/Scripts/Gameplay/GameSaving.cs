using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSaving : MonoBehaviour
{
    private GameObject player;
    private PlayerUnit playerUnit;
    private PlayerDefend playerDefend;
    private PlayerGun playerGun;
    private TimeManage time;
    private PlantPos plant;

    public GameObject savingPanelPrefab;

    private string gameName;

    public void NewGame(string gameName){
        this.gameName = gameName;
    }

    public string GetName(){
        return gameName;
    }

    void Start()
    {
        InitializeReferences();
    }

    private void InitializeReferences()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerUnit = player.GetComponent<PlayerUnit>();
            playerDefend = player.GetComponent<PlayerDefend>();
            playerGun = player.GetComponent<PlayerGun>();
        }

        time = TimeManage.instance;
        plant = PlantPos.instance;
    }

    bool SaveGame()
    {
        string SavePlayer()
        {
            return JsonUtility.ToJson(playerUnit);
        }

        string SavePlayerDefend()
        {
            return JsonUtility.ToJson(playerDefend);
        }

        string SavePlayerGun()
        {
            return JsonUtility.ToJson(playerGun);
        }

        string SaveTime()
        {
            return JsonUtility.ToJson(time);
        }

        string SavePlants()
        {
            plant.SetSaveTime();
            return JsonUtility.ToJson(plant);
        }

        try
        {
            string saveDirectory = Path.Combine(Application.persistentDataPath, "saves", gameName);
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            string playerFile = Path.Combine(saveDirectory, "player.data");
            File.WriteAllText(playerFile, SavePlayer());

            string playerDefendFile = Path.Combine(saveDirectory, "playerDefend.data");
            File.WriteAllText(playerDefendFile, SavePlayerDefend());

            string playerGunFile = Path.Combine(saveDirectory, "playerGun.data");
            Debug.Log(SavePlayerGun());
            File.WriteAllText(playerGunFile, SavePlayerGun());

            string timeFile = Path.Combine(saveDirectory, "time.data");
            File.WriteAllText(timeFile, SaveTime());

            string plantFile = Path.Combine(saveDirectory, "plant.data");
            File.WriteAllText(plantFile, SavePlants());
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
            transform.localScale = new Vector3(1, 1, 1);
        }
        else{
            transform.localScale = new Vector3(0, 0, 0);
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
                    Debug.Log("Saved successfully");
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