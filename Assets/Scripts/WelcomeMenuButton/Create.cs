using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Create : MonoBehaviour
{
    static string gameName;
    public TMP_InputField inputField;
    private Canvas topLevelCanvas;
    public TextMeshProUGUI outputError;

    void Start()
    {
        inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
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

    void OnInputFieldValueChanged(string newValue)
    {
        // listener to input field
        gameName = inputField.text;
    }


    void OnDestroy()
    {
        inputField.onValueChanged.RemoveListener(OnInputFieldValueChanged);
    }

    public void CreateNewGame(){
        if (GameLoading.GameNameExists(gameName)){
            Debug.Log("Game name already taken");
            // hiện error ở đây
            outputError.text = "GAME NAME ALREADY TAKEN";
            return;
        }

        Debug.Log("Creating a new game");

        PlayerMode playerMode;
        if (gameObject.name=="SurvivalMode"){
            playerMode = PlayerMode.SURVIVAL;
        }
        else{
            playerMode = PlayerMode.CREATIVE;
        }

        void LoadSceneAndAccessGameController()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("SceneHome");
            asyncLoad.completed += (asyncOperation) =>
            {
                Debug.Log("100% save");
                topLevelCanvas.enabled = false;

                StartCoroutine(SaveGameAfterDelay());
            };
        }

        IEnumerator SaveGameAfterDelay()
        {
            yield return new WaitForSeconds(1.5f); // wait 5 sécs

            Debug.Log("Delayed 1 secs");

            GameObject canvas = GameObject.Find("Canvas");
            canvas.SetActive(true); // re enable main game canvas

            Debug.Log("Creating a new game");

            GameSaving gameSaving = GameObject.Find("SaveGame").GetComponent<GameSaving>();
            if (gameSaving != null)
            {
                if (gameSaving.NewGame(gameName))
                {
                    Debug.Log("Saved new game successfully");
                    PlayerUnit.playerMode = playerMode;

                    Destroy(topLevelCanvas.gameObject);
                }
                else
                {
                    Debug.LogError("Error when creating a new game");
                }
            }
            else
            {
                Debug.LogError("GameSaving component not found on 'SaveGame' object");
            }
        }

        LoadSceneAndAccessGameController();
    }
}
