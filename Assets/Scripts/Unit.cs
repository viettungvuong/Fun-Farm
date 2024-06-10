using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int maxHealth;
    public int damage;

    [HideInInspector] public int currentHealth;

    public virtual void Awake() // virtual for polymorphism
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int inflictedDamage)
    {
        currentHealth -= inflictedDamage;
    }
}
