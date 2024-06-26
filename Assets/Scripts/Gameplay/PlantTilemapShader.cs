using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PlantTilemapShader: TilemapShader{
    
    private void Start() {
 
        SceneManager.sceneLoaded += OnSceneLoaded;

        InitializeMap();
    }


    private void OnDestroy()
    {
 
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeMap();
    }

    private void InitializeMap(){
        if (GameController.HomeScene()){
            tilemap = GameObject.Find("PlantTilemap").GetComponent<Tilemap>();
        }

    }

}