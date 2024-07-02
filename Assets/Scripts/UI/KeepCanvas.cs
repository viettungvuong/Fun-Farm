using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                    if (cv.gameObject.scene.name!="DontDestroyOnLoad"){
                        cv.gameObject.SetActive(false);
                    }
                }
            }

            DontDestroyOnLoad(canvas);
        } catch (Exception err){
            Debug.LogError(err.Message);
        }
       
    }

}
