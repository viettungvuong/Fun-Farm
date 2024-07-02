using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Create : MonoBehaviour
{
    static string gameName;
    public TMP_InputField inputField;

    void Start()
    {
        inputField.onValueChanged.AddListener(OnInputFieldValueChanged);
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
            return;
        }

        PlayerMode playerMode;
        if (gameObject.name=="SurvivalMode"){
            playerMode = PlayerMode.SURVIVAL;
        }
        else{
            playerMode = PlayerMode.CREATIVE;
        }

        IEnumerator LoadSceneAndAccessGameController()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("SceneHome");

            // wait until scene fully loaded
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            GameSaving gameSaving = GameObject.Find("Savegame").GetComponent<GameSaving>();
            if (gameSaving != null)
            {
                if (gameSaving.NewGame(gameName)){
                    PlayerUnit.playerMode = playerMode;
                }
                else{
                    Debug.LogError("Error when creating a new game");
                    // hiện error ở đây
                    SceneManager.LoadScene("SceneWelcome"); // go back to menu
                }

            }
            else
            {
                Debug.LogError("GameController object not found in the scene.");
                SceneManager.LoadScene("SceneWelcome"); // go back to menu
            }
        }

        StartCoroutine(LoadSceneAndAccessGameController());
    }
}
