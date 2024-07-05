using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System;

public class LoadBrowser : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject buttonParent;
    private Canvas topLevelCanvas;

    private void Start()
    {
        InitializeButtons();
    }

    // Method to initialize the buttons
    private void InitializeButtons()
    {
        // Clear existing buttons first
        foreach (Transform child in buttonParent.transform)
        {
            Destroy(child.gameObject);
        }

        List<string> games = GameLoading.LoadGames();

        for (int i = 0; i < games.Count; i++)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonParent.transform);
            LoadRemoveButton loadButtonComponent = newButton.GetComponent<LoadRemoveButton>();
            loadButtonComponent.mainButton.loadText.text = games[i];
            int index = i; // Capture the current value of i

            // Add on click listener to the main button to load the game
            loadButtonComponent.mainButton.GetComponent<Button>().onClick.AddListener(() => SelectLoadFile(games[index]));

            // Add on click listener to the remove button to delete the save and reinitialize the buttons
            loadButtonComponent.removeButton.GetComponent<Button>().onClick.AddListener(() => {
                RemoveSave(games[index]);
                InitializeButtons(); // Re-initialize buttons after deleting a save
            });
        }

        // Find the top-level canvas and mark it as not to be destroyed on load
        topLevelCanvas = FindTopLevelParent(gameObject).GetComponent<Canvas>();
        DontDestroyOnLoad(topLevelCanvas.gameObject); // This only works for root gameObject
    }


    private GameObject FindTopLevelParent(GameObject obj)
    {
        Transform parent = obj.transform;
        while (parent.parent != null)
        {
            parent = parent.parent;
        }
        return parent.gameObject;
    }

    private void RemoveSave(string name)
    {
        Debug.Log("Remove file name: " + name);
        // Construct the full file path
        string savesDirectory = Path.Combine(Application.persistentDataPath, "saves", name);
        savesDirectory = savesDirectory.Replace("\\", "/");

        // Check if the file exists before attempting to delete it
        if (System.IO.Directory.Exists(savesDirectory))
        {
            // Delete all files in the directory
            foreach (string file in System.IO.Directory.GetFiles(savesDirectory))
            {
                System.IO.File.Delete(file);
            }

            // Delete all subdirectories and their contents
            foreach (string dir in System.IO.Directory.GetDirectories(savesDirectory))
            {
                System.IO.Directory.Delete(dir, true);
            }

            // Finally, delete the directory itself
            System.IO.Directory.Delete(savesDirectory);
            Debug.Log("Save directory removed successfully: " + savesDirectory);
        }
        else
        {
            Debug.LogWarning("Directory not found: " + savesDirectory);
        }
    }

    private void SelectLoadFile(string name)
    {
        // Debug.Log("Load file name: " + name);
        LoadSceneAndAccessGameController(name);
    }

    // IEnumerator LoadGameAfterDelay(string name)
    // {
    //     yield return new WaitForSeconds(0.0f);

    //     GameObject gameControllerObject = GameObject.Find("GameController");
    //     if (gameControllerObject != null)
    //     {
    //         GameLoading gameLoading = gameControllerObject.transform.GetChild(0).GetComponent<GameLoading>();
    //         if (gameLoading != null)
    //         {
    //             bool load = gameLoading.LoadGame(name);
    //             if (load){
    //                 Debug.Log("Load successfully "+name);
    //                 Destroy(topLevelCanvas.gameObject);
    //             }


    //         }
    //         else
    //         {
    //             Debug.LogError("GameLoading component not found.");
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogError("GameController object not found in the scene.");
    //     }
    // }

    private void LoadSceneAndAccessGameController(string name)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("SceneHome");

        asyncLoad.completed += (asyncOperation) =>
        {
            topLevelCanvas.enabled = false;

            GameLoading.hasToLoad = true;
            GameLoading.gameName = name;
        };


    }
}
