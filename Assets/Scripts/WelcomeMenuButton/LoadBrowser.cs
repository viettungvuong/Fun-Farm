using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadBrowser : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject buttonParent;

    private void OnEnable()
    {
        List<string> games;
        if (GameLoading.LoadGames() == null)
        {
            games = new List<string>(new string[] { "EMPTY 1", "EMPTY 2", "EMPTY 3", "EMPTY 4", "EMPTY 5", "EMPTY 6" });
        }
        else
        {
            games = GameLoading.LoadGames();
        }


        Debug.Log("Number of games loaded: " + games.Count);


        for (int i = 0; i < games.Count; i++)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonParent.transform);
            LoadButton loadButtonComponent = newButton.GetComponent<LoadButton>();
            loadButtonComponent.loadText.text = games[i];
            int index = i; // Capture the current value of i
            newButton.GetComponent<Button>().onClick.AddListener(() => SelectLoadFile(games[index]));
        }

    }

    private void SelectLoadFile(string name)
    {
        // Debug.Log("Load file name: " + name);
        StartCoroutine(LoadSceneAndAccessGameController(name));
    }

    private IEnumerator LoadSceneAndAccessGameController(string name)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("SceneHome");

        // wait until scene fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        GameObject gameControllerObject = GameObject.Find("GameController");
        if (gameControllerObject != null)
        {
            GameController gameController = gameControllerObject.GetComponent<GameController>();
            GameLoading gameLoading = gameController.transform.GetChild(0).GetComponent<GameLoading>();
            if (gameController != null)
            {
                gameLoading.LoadGame(name);
            }
            else{
                Debug.LogError("Game controller not found");
                SceneManager.LoadScene("SceneWelcome"); // go back to menu
            }
        }
        else
        {
            Debug.LogError("GameController object not found in the scene.");
        }
    }
}
