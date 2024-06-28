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
    public GameObject diePanel;

    public GameObject coinPrefab; // Coin prefab
    public Transform headTransform; 


    
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

        if (currentHealth<=0||(currentMoney==0&&PlantManager.instance.GetNumberOfPlants()==0)){
            Die(); // run die animation
        }
    }

    public bool SufficientMoney(int amount){
        return currentMoney >= amount;
    }

    public void UseMoney(int amount){
        currentMoney -= amount;
        Vector2 coinPosition = (Vector2) headTransform.position + Vector2.up * 0.5f; // Adjust the offset as needed
        GameObject coin = Instantiate(coinPrefab, coinPosition, Quaternion.identity, headTransform);
    }

    public void AddMoney(int amount){
        currentMoney += amount;
    }



    public void RecoverHealth(double healthRecovered){
        currentHealth = Math.Max(currentHealth + healthRecovered, maxHealth);
    }

    private IEnumerator DieCoroutine(){
        base.animator.SetBool("die", true);
        yield return new WaitForSeconds(GameController.GetAnimationLength(animator, "Die"));
        diePanel.SetActive(false);
    }

    public override void Die(){
        // show die menu
        StopAllCoroutines();
        StartCoroutine(DieCoroutine());

    }


}
