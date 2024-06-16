using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSliderPrefab;
    [HideInInspector] public Slider healthSlider;
    private Image sliderImageFill;

    private Unit unit;

    private float maxHealth;


    void Start()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        healthSlider = Instantiate(healthSliderPrefab, canvas.transform); // create copy of health slider prefab
        // save as a child in canvas

        DontDestroyOnLoad(canvas);

        sliderImageFill = healthSlider.GetComponentsInChildren<Image>().FirstOrDefault(t => t.name == "Fill");


        unit = GetComponent<Unit>();

        maxHealth = unit.maxHealth;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = unit.currentHealth;
    }


    private void LateUpdate() {
        healthSlider.value = unit.currentHealth;
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
        // follows player
        Vector2 position = transform.position;
        Vector2 offset = new Vector2(0f, 1f);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(position + offset); // change to screen position
        healthSlider.transform.position = screenPosition;
    }
}
