using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private PlayerUnit playerUnit;
    public TimeManage timeManage;


    public TextMeshProUGUI healthText;
    public static bool healthDeteriorated = false;

    private void Awake() {
        if (PlayerUnit.playerMode==PlayerMode.CREATIVE){
            healthText.gameObject.SetActive(false);
            enabled = false;
            return;
        }
    }

    void Start()
    {
        playerUnit = GetComponent<PlayerUnit>();
        Debug.Log(playerUnit.currentHealth);
        if (PlayerUnit.playerMode==PlayerMode.CREATIVE){
            healthText.gameObject.SetActive(false);
            enabled = false;
            return;
        }

        healthDeteriorated = false;
    }

    void Update()
    {
        if (PlayerUnit.playerMode==PlayerMode.CREATIVE){
            enabled = false;
            healthText.gameObject.SetActive(false);
            return;
        }

        float healthDecrease = 0.1f;
        
        if (!healthDeteriorated&&TimeManage.instance.currentMinute==0){
            playerUnit.currentHealth -= healthDecrease;
            playerUnit.HealthDamageAnimation();
            healthDeteriorated = true;

        }
        else if (TimeManage.instance.currentMinute>0){
            healthDeteriorated = false;
        }

        healthText.text = playerUnit.currentHealth.ToString("F1"); // round to 1 decimal point

    }
}
