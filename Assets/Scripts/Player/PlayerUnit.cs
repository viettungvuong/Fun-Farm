using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : MonoBehaviour
{
    public int maxMoney;
    public int maxHealth;
    [HideInInspector] public int currentMoney;
    [HideInInspector] public int currentHealth;

    void Awake(){
        currentMoney = maxMoney;
        currentHealth = maxHealth;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
