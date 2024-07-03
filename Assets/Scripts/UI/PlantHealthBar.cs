using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlantHealthBar : MonoBehaviour
{
    public Slider healthSliderPrefab;
    [HideInInspector] public Slider healthSlider;
    private Image sliderImageFill;

    private PlantedPlant plant;
    private Tilemap plantTilemap;
    private Camera cam;
    

    
    public void Initialize(PlantedPlant plant, Tilemap plantTilemap, Slider healthSliderPrefab)
    {
        this.plant = plant;
        this.plantTilemap = plantTilemap;
        this.healthSliderPrefab = healthSliderPrefab;

        cam = GameObject.Find("Main Camera").GetComponent<Camera>();

        Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        healthSlider = Instantiate(healthSliderPrefab, canvas.transform); // create copy of health slider prefab
        healthSlider.gameObject.SetActive(true);
        healthSlider.enabled = true;
        // save as a child in canvas
    }

    private void Awake() {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void Start()
    {
        if (plant == null || plantTilemap == null || healthSliderPrefab == null)
        {
            return;
        }


        sliderImageFill = healthSlider.GetComponentsInChildren<Image>().FirstOrDefault(t => t.name == "Fill");

        // the bar shows time left until the tree is deteriorated
        healthSlider.maxValue = (float)plant.deteriorateTime;
        try
        {
            var lastTimeWatered = PlantManager.instance?.GetLastTimeWatered(plant);

            if (lastTimeWatered != null && lastTimeWatered.HasValue)
            {
                double timeDiff = (DateTime.Now-lastTimeWatered.Value).TotalSeconds;
                if (plant.lastSavedTime!=null&&plant.lastOpenedTime!=null){ // when saving to remove inaccurate calculation (idle between openings of the game)
                    if (plant.lastOpenedTime > (DateTime)lastTimeWatered)
                    {
                        double unneededDifference = Math.Abs(((DateTime)plant.lastOpenedTime - (DateTime)plant.lastSavedTime).TotalSeconds);
                        timeDiff -= unneededDifference;
                    }
                }
                
                healthSlider.value = (float)timeDiff;
            }
            else
            {
                Debug.LogWarning("Last time watered is null or not a DateTime.");
                healthSlider.gameObject.SetActive(false);
                Destroy(this);
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error calculating time difference: " + e.Message);
        }


    }


    private void FixedUpdate() {
        UpdateSliderPosition();
    }

    private void LateUpdate() {
        if (plant == null){
            return;
        }

        if (PlayerUnit.playerMode==PlayerMode.CREATIVE||plant.currentStage == plant.maxStage){
            healthSlider.gameObject.SetActive(false);
            return;
        }

        if (GameController.HomeScene()==false){
            healthSlider.gameObject.SetActive(false);
            return;
        }
        
        var lastTimeWatered = PlantManager.instance?.GetLastTimeWatered(plant);
        double timeDiff = (DateTime.Now-(DateTime)lastTimeWatered).TotalSeconds;
        if (plant.lastSavedTime!=null&&plant.lastOpenedTime!=null){ // when saving
                if (plant.lastOpenedTime > (DateTime)lastTimeWatered) // only calc if lastopen occurs before lastwatered
                {
                    double unneededDifference = Math.Abs(((DateTime)plant.lastOpenedTime - (DateTime)plant.lastSavedTime).TotalSeconds);
                    timeDiff -= unneededDifference;
                }

        }
        
        healthSlider.value = (float)timeDiff;
        float timePercentage = (float)timeDiff / (float)plant.deteriorateTime;


        if (timePercentage>=1f){ // die
            healthSlider.gameObject.SetActive(false); 
        }    
        if (timePercentage < 0.2f) {
            // Health < 20%: green
            sliderImageFill.color = Color.green;
        } else if (timePercentage >= 0.2f && timePercentage < 0.5f) {
            // Health between 20% and 50%: yellow to green
            sliderImageFill.color = Color.Lerp(Color.green, Color.yellow, (timePercentage - 0.2f) / 0.3f);
        } else if (timePercentage >= 0.5f && timePercentage < 0.8f) {
            // Health between 50% and 80%: orange to yellow
            sliderImageFill.color = Color.Lerp(Color.yellow, Color.red, (timePercentage - 0.5f) / 0.3f);
        } else if (timePercentage >= 0.8f) {
            // Health >= 80%: red
            sliderImageFill.color = Color.red;
        }


    }

    private void UpdateSliderPosition()
    {
        if (plant == null || plantTilemap == null){
            return;
        }
        if (GameController.HomeScene()==false){
            return;
        }

        Vector2 position = plantTilemap.CellToWorld(plant.gridPosition);
        Vector2 offset = new Vector2(0f, 1f);
        Vector3 screenPosition = cam.WorldToScreenPoint(position + offset); // change to screen position
        healthSlider.transform.position = screenPosition;
    }

}
