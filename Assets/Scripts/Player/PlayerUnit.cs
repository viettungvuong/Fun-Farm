using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class PlayerUnit : Unit
{
    public int maxMoney;
    public static PlayerUnit instance;
    public TextMeshProUGUI coinText;
    [HideInInspector] public int currentMoney;
    public static PlayerMode playerMode;


    
    public override void Awake()
    {
        base.Awake();
        currentMoney = maxMoney;

        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        playerMode = PlayerMode.SURVIVAL; // default is survival
    }

    private void LateUpdate() {
        coinText.text = currentMoney.ToString();

        if (currentHealth<=0){
            Die(); // run die animation
        }
    }

    public bool SufficientMoney(int amount){
        return currentMoney >= amount;
    }

    public void UseMoney(int amount){
        currentMoney -= amount;
    }

    public void AddMoney(int amount){
        currentMoney += amount;
    }



    public void RecoverHealth(double healthRecovered){
        currentHealth = Math.Max(currentHealth + healthRecovered, maxHealth);
    }

    void Die(){
        // show die menu
        // code die menu here
    }


}
