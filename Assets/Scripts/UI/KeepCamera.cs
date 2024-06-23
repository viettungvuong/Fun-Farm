using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class KeepCamera : MonoBehaviour
{
    private static KeepCamera instance;
    private Camera mainCamera;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
            return;
        }

        mainCamera = GetComponent<Camera>(); 

        SceneManager.sceneLoaded += OnSceneLoaded; 
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (GameController.HomeScene()==false)
        {
            mainCamera.enabled = false;
            GetComponent<CameraFollow>().enabled = false;
        }
        else
        {
            mainCamera.enabled = true; 
            GetComponent<CameraFollow>().enabled = true;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; 
    }
}
