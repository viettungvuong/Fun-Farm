using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerUnit : Unit
{
    public int maxMoney;
    public static PlayerUnit instance;
    public TextMeshProUGUI coinText, waterText;
    [HideInInspector] public int currentMoney;
    [HideInInspector] public double waterPercentage=1f;
    Animator animator;
    
    public override void Awake()
    {
        base.Awake();
        currentMoney = maxMoney;
        waterPercentage = 1f;

        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        animator = GetComponent<Animator>();
    }

    private void LateUpdate() {
        coinText.text = currentMoney.ToString();
        waterText.text = (waterPercentage * 100).ToString() + "%";

        if (currentHealth<=0){
            Die();
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

    public bool SufficientWater(double amount){
        return waterPercentage >= amount;
    }
    public void UseWater(double amount){
        waterPercentage -= amount;
    }
    public void AddWater(double amount){
        waterPercentage = Math.Min(1.0, waterPercentage + amount);
    }

    public void RecoverHealth(double healthRecovered){
        currentHealth = Math.Max(currentHealth + healthRecovered, maxHealth);
    }

    void Die(){
        // show die menu
    }


}
