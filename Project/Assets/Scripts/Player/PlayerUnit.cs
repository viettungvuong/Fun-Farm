using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerUnit : Unit
{
    #region Fields
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

    public TextMeshProUGUI textDie;

    public MoneyManager moneyManager;
    public HealthManager healthManager;
    public WaterManager waterManager;
    public DieManager dieManager;
    #endregion

    #region Serialization
    public PlayerUnitData Serialize()
    {
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
    #endregion

    #region Unity Lifecycle
    public override void Awake()
    {
        base.Awake();
        currentMoney = maxMoney;

        moneyManager = new MoneyManager(this);
        healthManager = new HealthManager(this);
        waterManager = new WaterManager(this);
        dieManager = new DieManager(this);

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

        hasRefilled = false;


    }

    private void Update() {
        if (!die){
            if (!hasRefilled && TimeManage.instance.currentMinute == 0) { // refill every hour
                waterManager.RefillWater();
            }

            if (hasRefilled && TimeManage.instance.currentMinute > 0) { // allow refill again after 0 min passes
                hasRefilled = false;
            }
        }
        if (die && Input.GetKey(KeyCode.Space)){
            GameObject canvasObject = GameObject.Find("Canvas");
            if (canvasObject != null){
                GameController.OpenMenu(); // go back to menu
            }
        }
    }

    private void LateUpdate() {
        if (coinText != null) coinText.text = currentMoney.ToString();

        if (currentHealth <= 0 || (currentMoney <= 0 && PlantManager.instance.GetNumberOfPlants() == 0)){
            if (currentMoney == 0 && PlantManager.instance.GetNumberOfPlants() == 0){
                textDie.text = "NO MONEY LEFT TO SURVIVE";
            }
            else{
                textDie.text = "YOU DIED!";
            }

            Die(); // run die animation
        }
    }
    #endregion

    #region Die Methods
    public override void Die(){
        if (!die){
            StartCoroutine(dieManager.DieCoroutine());
        }
    }

    public class DieManager
    {
        private PlayerUnit playerUnit;

        public DieManager(PlayerUnit playerUnit)
        {
            this.playerUnit = playerUnit;
        }

        public IEnumerator DieCoroutine()
        {
            playerUnit.diePanel.SetActive(true); // show die panel
            playerUnit.diePanel.transform.SetAsLastSibling();

            playerUnit.animator.Play("Die");

            // wait for animator to complete
            yield return new WaitForSeconds(GameController.GetAnimationLength(playerUnit.animator, "Die") + 2f);

            playerUnit.die = true;
        }
    }
    #endregion

    #region Money Management
    public class MoneyManager
    {
        private PlayerUnit playerUnit;

        public MoneyManager(PlayerUnit playerUnit)
        {
            this.playerUnit = playerUnit;
        }

        public void UseMoney(int amount)
        {
            playerUnit.currentMoney -= amount;
            if (playerUnit.coinJumpOutPrefab == null) {
                playerUnit.coinJumpOutPrefab = Resources.Load<GameObject>("CoinJump");
                if (playerUnit.coinJumpOutPrefab == null){
                    Debug.LogError("Coin Prefab is not found");
                }
            }
            if (playerUnit.headTransform == null)
            {
                Transform head = playerUnit.transform.Find("Head"); 
                if (head != null)
                {
                    playerUnit.headTransform = head;
                }
                else
                {
                    Debug.LogError("Head transform not found as a child of PlayerUnit.");
                }
            }
            
            Vector2 coinPosition = (Vector2) playerUnit.headTransform.position + Vector2.up * 0.5f; // Adjust the offset as needed
            GameObject coin = UnityEngine.Object.Instantiate(playerUnit.coinJumpOutPrefab, coinPosition, Quaternion.identity, playerUnit.headTransform);
        }

        public void AddMoney(int amount)
        {
            if (playerUnit.coinJumpInPrefab == null) {
                playerUnit.coinJumpInPrefab = Resources.Load<GameObject>("CoinEarn");
                if (playerUnit.coinJumpInPrefab == null){
                    Debug.LogError("Coin Prefab is not found");
                }
            }
            if (playerUnit.headTransform == null)
            {
                Transform head = playerUnit.transform.Find("Head"); 
                if (head != null)
                {
                    playerUnit.headTransform = head;
                }
                else
                {
                    Debug.LogError("Head transform not found as a child of PlayerUnit.");
                }
            }
            
            Vector2 coinPosition = (Vector2) playerUnit.headTransform.position + Vector2.up * 0.75f; // Adjust the offset as needed
            GameObject coin = UnityEngine.Object.Instantiate(playerUnit.coinJumpInPrefab, coinPosition, Quaternion.identity, playerUnit.headTransform);
            playerUnit.currentMoney += amount;
        }

        public bool SufficientMoney(int amount)
        {
            return playerUnit.currentMoney >= amount;
        }
    }
    #endregion

    #region Health Management
    public class HealthManager
    {
        private PlayerUnit playerUnit;

        public HealthManager(PlayerUnit playerUnit)
        {
            this.playerUnit = playerUnit;
        }

        public void RecoverHealth(double healthRecovered)
        {
            playerUnit.currentHealth = Math.Max(playerUnit.currentHealth + healthRecovered, playerUnit.maxHealth);
        }

        public bool EatNeeded(double healthRecovered)
        {
            return playerUnit.currentHealth + healthRecovered <= playerUnit.maxHealth;
        }

        public void HealthDamageAnimation()
        {
            if (playerUnit.heartBrokenPrefab == null) {
                playerUnit.heartBrokenPrefab = Resources.Load<GameObject>("HeartBroken");
                if (playerUnit.heartBrokenPrefab == null){
                    Debug.LogError("Heart Prefab is not found");
                }
            }
            if (playerUnit.headTransform == null)
            {
                Transform head = playerUnit.transform.Find("Head"); 
                if (head != null)
                {
                    playerUnit.headTransform = head;
                }
                else
                {
                    Debug.LogError("Head transform not found as a child of PlayerUnit.");
                }
            }
            
            Vector2 heartBrokenAnim = (Vector2) playerUnit.headTransform.position + Vector2.up * 0.5f; // Adjust the offset as needed
            GameObject heart = UnityEngine.Object.Instantiate(playerUnit.heartBrokenPrefab, heartBrokenAnim, Quaternion.identity, playerUnit.headTransform);
        }
    }
    #endregion

    #region Water Management
    public class WaterManager
    {
        private PlayerUnit playerUnit;

        public WaterManager(PlayerUnit playerUnit)
        {
            this.playerUnit = playerUnit;
        }

        public void UseWater(float amount)
        {
            if (playerUnit.remainingWater >= amount){
                playerUnit.remainingWater -= amount;
                playerUnit.waterText.text = playerUnit.remainingWater.ToString();
            }
        }

        public void RefillWater()
        {
            hasRefilled = true;
            playerUnit.remainingWater = 1f;
            playerUnit.waterText.text = playerUnit.remainingWater.ToString();
        }
    }
    #endregion
}
