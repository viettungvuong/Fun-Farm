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

    

    
    public void Initialize(PlantedPlant plant, Tilemap plantTilemap, Slider healthSliderPrefab)
    {
        this.plant = plant;
        this.plantTilemap = plantTilemap;
        this.healthSliderPrefab = healthSliderPrefab;
    }

    void Start()
    {
        if (plant == null || plantTilemap == null || healthSliderPrefab == null)
        {
            return;
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        healthSlider = Instantiate(healthSliderPrefab, canvas.transform); // create copy of health slider prefab
        // save as a child in canvas

        sliderImageFill = healthSlider.GetComponentsInChildren<Image>().FirstOrDefault(t => t.name == "Fill");

        // the bar shows time left until the tree is deteriorated
        healthSlider.maxValue = (float)plant.deteriorateTime;
        try
        {
            var lastTimeWatered = PlantManager.instance?.GetLastTimeWatered(plant);
            if (lastTimeWatered != null && lastTimeWatered is DateTime lastWateredTime)
            {
                double timeDiff = (DateTime.Now - lastWateredTime).TotalSeconds;
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


    private void LateUpdate() {

        if (plant == null) return;

        if (PlayerUnit.playerMode==PlayerMode.CREATIVE||GameController.HomeScene()==false||plant.currentStage == plant.maxStage){
            healthSlider.gameObject.SetActive(false);
            return;
        }

        double timeDiff = (DateTime.Now-(DateTime)PlantManager.instance.GetLastTimeWatered(plant)).TotalSeconds;

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

        UpdateSliderPosition();
    }

    private void UpdateSliderPosition()
    {
        if (plant == null || plantTilemap == null) return;

        Vector3Int gridPosition = plant.gridPosition;
        Vector3 offset = new Vector2(0f, 1f);
        Vector3 worldPosition = plantTilemap.CellToWorld(gridPosition) + offset;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        healthSlider.transform.position = screenPosition;
    }

}
