using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private PlayerUnit playerUnit;
    public TimeManage timeManage;
    public float maxHealthDecreasePerDay = 50f; // Maximum health decrease over a day
    private float previousTimeOfDay;

    public TextMeshProUGUI healthText;

    void Start()
    {
        playerUnit = GetComponent<PlayerUnit>();
        previousTimeOfDay = timeManage.currentHour; // start of the game
    }

    void Update()
    {
        float currentTimeOfDay = timeManage.currentHour;
        float timePassed = currentTimeOfDay - previousTimeOfDay;
            if (timePassed < 0) // start a new day
        {
            // transition from the end of the day to the start of a new day
            timePassed += 12f;
        }
        float ratio = timePassed / 24f;

        float healthDecrease = maxHealthDecreasePerDay * ratio;

        playerUnit.currentHealth -= healthDecrease;
        previousTimeOfDay = currentTimeOfDay;

        healthText.text = playerUnit.currentHealth.ToString();
    }
}
