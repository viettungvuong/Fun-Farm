using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeepCanvas : MonoBehaviour
{
    private Canvas canvas;
    public static bool showCanvas = false;
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeCanvas();

        showCanvas = false;
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
        if (SceneManager.GetActiveScene().name=="SceneWelcome"){
            return;
        }
        try{
            List<Canvas> canvases = FindObjectsOfType<Canvas>().ToList();
            bool hasDontDestroyOnLoad = false;
            if (canvases.Count>=2){
                foreach (Canvas cv in canvases){
                    // if (cv.gameObject.name!="Canvas"){
                    //     cv.gameObject.SetActive(false);
                    // }

                    if (cv.gameObject.name=="Canvas"){
                        if (!hasDontDestroyOnLoad){
                            canvas = cv;
                        }

                        if (cv.gameObject.scene.name=="DontDestroyOnLoad"){
                            hasDontDestroyOnLoad = true;
                        }
                    }

                }
            }

            if (hasDontDestroyOnLoad){ // keep dontdestroyonload canvas
                foreach (Canvas cv in canvases)
                {
                    Debug.Log(cv.gameObject.scene.name);
                    if (cv.gameObject.scene.name!="DontDestroyOnLoad"){
                        Destroy(cv.gameObject);
                    }
                }
            }

            DontDestroyOnLoadManager.DontDestroyOnLoad(canvas.gameObject);
        } catch (Exception err){
            Debug.LogError(err.Message);
        }
       
    }

    private void Update() {
        if (showCanvas){
            canvas.enabled = true;
        }
        else{
            canvas.enabled = false;
        }
    }

}
