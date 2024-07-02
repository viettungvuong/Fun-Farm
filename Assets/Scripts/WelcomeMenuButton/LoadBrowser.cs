using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        Debug.Log("Load file name: " + name);
    }
}
