using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
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

    public GameObject coinJumpOutPrefab; // Coin prefab
    public GameObject coinJumpInPrefab; // Coin prefab

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
    void Start()
    {
        
    }

    private void LateUpdate() {
        if (coinText != null) coinText.text = currentMoney.ToString();

        if (currentHealth<=0||(currentMoney==0&&PlantManager.instance.GetNumberOfPlants()==0)){
            Die(); // run die animation
        }
    }

    public bool SufficientMoney(int amount){
        return currentMoney >= amount;
    }

    public void UseMoney(int amount){
        currentMoney -= amount;
        if (coinJumpOutPrefab == null) {
            coinJumpOutPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/CoinJump.prefab");
            if (coinJumpOutPrefab == null){
                Debug.LogError("Coin Prefab is not found");

            }
        }
        if (headTransform == null)
        {
            Transform head = transform.Find("Head"); 
            if (head != null)
            {
                headTransform = head;
            }
            else
            {
                Debug.LogError("Head transform not found as a child of PlayerUnit.");
            }
        }
        
        Vector2 coinPosition = (Vector2) headTransform.position + Vector2.up * 0.5f; // Adjust the offset as needed
        GameObject coin = Instantiate(coinJumpOutPrefab, coinPosition, Quaternion.identity, headTransform);
    }

    public void AddMoney(int amount){
        if (coinJumpInPrefab == null) {
            coinJumpInPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/CoinEarn.prefab");
            if (coinJumpInPrefab == null){
                Debug.LogError("Coin Prefab is not found");

            }
        }
        if (headTransform == null)
        {
            Transform head = transform.Find("Head"); 
            if (head != null)
            {
                headTransform = head;
            }
            else
            {
                Debug.LogError("Head transform not found as a child of PlayerUnit.");
            }
        }
        
        Vector2 coinPosition = (Vector2) headTransform.position + Vector2.up * 0.75f; // Adjust the offset as needed
        GameObject coin = Instantiate(coinJumpInPrefab, coinPosition, Quaternion.identity, headTransform);
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
