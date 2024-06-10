using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSliderPrefab;
    private Slider healthSlider;

    private Unit unit;

    private float maxHealth;

    void Start()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        healthSlider = Instantiate(healthSliderPrefab, canvas.transform); // create copy of health slider prefab
        // save as a child in canvas

        unit = GetComponent<Unit>();

        maxHealth = unit.maxHealth;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = unit.currentHealth;
    }

    private void LateUpdate() {
        healthSlider.value = unit.currentHealth;
        Debug.Log(gameObject.name + " " + healthSlider.value);
    }


    private void FixedUpdate() {
        // follows
        Vector2 position = transform.position;
        Vector2 offset = new Vector2(0f, 1f);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(position + offset); // change to screen position
        healthSlider.transform.position = screenPosition;
    }
}
