using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerUnit : Unit
{
    public int maxMoney;
    public static PlayerUnit instance;
    public TextMeshProUGUI coinText, waterText;
    [HideInInspector] public int currentMoney;
    public static PlayerMode playerMode;
    public GameObject diePanel;

    public GameObject coinJumpOutPrefab;
    public GameObject coinJumpInPrefab; 
    public GameObject heartBrokenPrefab;

    public Transform headTransform;

    [HideInInspector] public float remainingWater = 1;

    public bool die = false;

    private static bool hasRefilled = false;

    public PlayerUnitData Serialize(){
        PlayerUnitData playerUnitData = new PlayerUnitData
        {
            currentHealth = currentHealth,
            currentMoney = currentMoney,
            playerMode = playerMode,
            nextSkeletonSpawnMin = SkeletonGenerate.nextMinuteRefill,
            nextSlimeSpawnMin = SlimeGenerate.nextMinuteRefill,
            water = remainingWater
        };
        return playerUnitData;
    }

    public void Reload(PlayerUnitData playerUnitData)
    {
        currentMoney = playerUnitData.currentMoney;
        currentHealth = playerUnitData.currentHealth;
        playerMode = playerUnitData.playerMode;
        remainingWater = playerUnitData.water;
        SkeletonGenerate.nextMinuteRefill = playerUnitData.nextSkeletonSpawnMin;
        SlimeGenerate.nextMinuteRefill = playerUnitData.nextSlimeSpawnMin;

        waterText.text = remainingWater.ToString();

    }
    
    public override void Awake()
    {
        base.Awake();
        currentMoney = maxMoney;

        if (GameController.HomeScene()){
            DontDestroyOnLoadManager.DontDestroyOnLoad(gameObject);
        }

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


        // playerMode = PlayerMode.SURVIVAL; // default is survival
    }

    private void Update() {
        if (!die){
            if (!hasRefilled&&TimeManage.instance.currentMinute==0){ // refill every hour
                RefillWater();
            }

            if (hasRefilled&&TimeManage.instance.currentMinute>0) // allow refill again after 0 min passes
            {
                hasRefilled = false;
            }
        }
        if (die&&Input.GetKey(KeyCode.Space)){
            GameObject canvasObject = GameObject.Find("Canvas");
            if (canvasObject!=null){
                GameController.OpenMenu(); // go back to menu
            }

        }
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

    public bool EatNeeded(double healthRecovered){
        return currentHealth+healthRecovered<=maxHealth;
    }

    public void UseMoney(int amount){
        currentMoney -= amount;
        if (coinJumpOutPrefab == null) {
            coinJumpOutPrefab = Resources.Load<GameObject>("CoinJump");
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
            coinJumpInPrefab = Resources.Load<GameObject>("CoinEarn");
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

    public void HealthDamageAnimation() {
        if (heartBrokenPrefab == null) {
            heartBrokenPrefab = Resources.Load<GameObject>("HeartBroken");
            if (heartBrokenPrefab == null){
                Debug.LogError("Heart Prefab is not found");

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
        
        Vector2 heartBrokenAnim = (Vector2) headTransform.position + Vector2.up * 0.5f; // Adjust the offset as needed
        GameObject heart = Instantiate(heartBrokenPrefab, heartBrokenAnim, Quaternion.identity, headTransform);
    }

    private IEnumerator DieCoroutine(){
        die = true;
        base.animator.Play("Die");

        // wait for animator to complete
        yield return new WaitForSeconds(GameController.GetAnimationLength(animator, "Die")+1f);

        diePanel.SetActive(true); // show die panel
        diePanel.transform.SetAsLastSibling();

    }

    public override void Die(){
        if (die==false){
            StartCoroutine(DieCoroutine());
        }
    }

    public void UseWater(float amount){
        if (remainingWater>=amount){
            remainingWater -= amount;
            waterText.text = remainingWater.ToString();
        }
    }

    private void RefillWater(){
        hasRefilled = true;
        remainingWater = 1f;
        waterText.text = remainingWater.ToString();
    }


}
