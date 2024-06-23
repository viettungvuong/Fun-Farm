using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeepCanvas : MonoBehaviour
{
    private Canvas canvas;
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeCanvas();
    }

    private void OnDestroy()
    {
 
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeCanvas();
    }

    private void InitializeCanvas()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        if (canvases.Length>=2){
            foreach (Canvas cv in canvases){
                if (cv.gameObject.scene.name!="DontDestroyOnLoad"){
                    cv.gameObject.SetActive(false);
                }
            }
        }

        canvas = canvases[0];
        DontDestroyOnLoad(canvas);
    }

}
