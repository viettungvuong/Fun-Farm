using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadBrowser : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject buttonParent;
    private Canvas topLevelCanvas;

    private void Start() {
        List<string> games;
        games = GameLoading.LoadGames();


        for (int i = 0; i < games.Count; i++)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonParent.transform);
            LoadButton loadButtonComponent = newButton.GetComponent<LoadButton>();
            loadButtonComponent.loadText.text = games[i];
            int index = i; // Capture the current value of i
            newButton.GetComponent<Button>().onClick.AddListener(() => SelectLoadFile(games[index])); // add on click listener
        }

        topLevelCanvas = FindTopLevelParent(gameObject).GetComponent<Canvas>();
        DontDestroyOnLoad(topLevelCanvas.gameObject); // this only works for root gameObject
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

    // private void OnEnable()
    // {


    // }

    private void SelectLoadFile(string name)
    {
        // Debug.Log("Load file name: " + name);
        LoadSceneAndAccessGameController(name);
    }

    IEnumerator LoadGameAfterDelay()
    {
        yield return new WaitForSeconds(2.0f);

        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            canvas.SetActive(true);
        }
        else
        {
            Debug.LogError("Canvas object not found.");
        }
        GameObject gameControllerObject = GameObject.Find("GameController");
        if (gameControllerObject != null)
        {
            GameController gameController = gameControllerObject.GetComponent<GameController>();
            if (gameController != null)
            {
                GameLoading gameLoading = gameController.transform.GetChild(0).GetComponent<GameLoading>();
                if (gameLoading != null)
                {
                    gameLoading.LoadGame(name);
                    Debug.Log("Load successfully");
                    Destroy(topLevelCanvas.gameObject);
                }
                else
                {
                    Debug.LogError("GameLoading component not found.");
                }
            }
            else
            {
                Debug.LogError("GameController component not found.");
            }
        }
        else
        {
            Debug.LogError("GameController object not found in the scene.");
        }
    }

    private void LoadSceneAndAccessGameController(string name)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("SceneHome");

        asyncLoad.completed += (asyncOperation) =>
        {
            StartCoroutine(LoadGameAfterDelay());
        };


    }
}
