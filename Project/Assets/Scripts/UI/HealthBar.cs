using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSliderPrefab;
    [HideInInspector] public Slider healthSlider;
    private Image sliderImageFill;

    private Unit unit;

    private double maxHealth;

    private Camera cam;

   
    void Start()
    {
        if (PlayerUnit.playerMode==PlayerMode.CREATIVE){
            gameObject.SetActive(false);
            enabled = false;
            return;
        }
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        healthSlider = Instantiate(healthSliderPrefab, canvas.transform); // create copy of health slider prefab
        healthSlider.transform.SetSiblingIndex(0); // render first to make it under everything UI
        // save as a child in canvas
        // healthSlider.gameObject.SetActive(false);
        sliderImageFill = healthSlider.GetComponentsInChildren<Image>().FirstOrDefault(t => t.name == "Fill");


        unit = GetComponent<Unit>();

        maxHealth = unit.maxHealth;

        healthSlider.maxValue = (float)maxHealth;
        healthSlider.value = (float)unit.currentHealth;
    }


    private void LateUpdate() {
        if (PlayerUnit.playerMode==PlayerMode.CREATIVE){
            gameObject.SetActive(false);
            enabled = false;
            return;
        }
        if (GameController.HomeScene()){
            healthSlider.gameObject.transform.localScale = new Vector3(1, 1, 1); // only show health bar in scene home
        }
        else{
            healthSlider.gameObject.transform.localScale = new Vector3(0, 0, 0);
        }
        healthSlider.value = (float)unit.currentHealth;
        float healthPercentage = (float)unit.currentHealth / (float)unit.maxHealth;
    
        if (healthPercentage >= 0.8f&&healthPercentage<=1f) {
            // Health >= 80%: green
            sliderImageFill.color = Color.green;
        } else if (healthPercentage >= 0.5f) {
            // Health between 50% and 80%: yellow to green
            sliderImageFill.color = Color.Lerp(Color.yellow, Color.green, (healthPercentage - 0.5f) / 0.3f);
        } else if (healthPercentage >= 0.2f) {
            // Health between 20% and 50%: orange to yellow
            sliderImageFill.color = Color.Lerp(Color.red, Color.yellow, (healthPercentage - 0.2f) / 0.3f);
        } else if (healthPercentage < 0.2f) {
            // Health < 20%: red
            sliderImageFill.color = Color.red;
        }
    }


    private void FixedUpdate() {
        if (GameController.HomeScene()==false){
            return;
        }
        // follows object
        Vector2 position = transform.position;
        Vector2 offset = new Vector2(0f, 1f);
        Vector3 screenPosition = cam.WorldToScreenPoint(position + offset); // change to screen position
        healthSlider.transform.position = screenPosition;
    }

    // public void Enable(){
    //     if (healthSlider==null){
    //         Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    //         healthSlider = Instantiate(healthSliderPrefab, canvas.transform); // create copy of health slider prefab
    //     }
    //     healthSlider.gameObject.SetActive(true);
    // }

    public void Disable(){
        healthSlider.gameObject.SetActive(false);
        Destroy(this);
    }
}
