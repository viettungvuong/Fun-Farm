using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUnit : Unit
{
    public int maxMoney;
    public TextMeshProUGUI coinText, waterText;
    [HideInInspector] public int currentMoney;
    [HideInInspector] public double waterPercentage=1f;
    
    public override void Awake()
    {
        base.Awake();
        currentMoney = maxMoney;
        waterPercentage = 1f;

        DontDestroyOnLoad(gameObject);
    }

    private void LateUpdate() {
        coinText.text = currentMoney.ToString();
        waterText.text = (waterPercentage * 100).ToString() + "%";
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
        waterPercentage += amount;
        if(waterPercentage > 1.0){
            waterPercentage = 1.0;
        }
    }
}
