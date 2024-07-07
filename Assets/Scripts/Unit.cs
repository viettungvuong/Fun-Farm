using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public double maxHealth;
    public double damage;

    public double currentHealth;

    protected Animator animator;

    public PlayerUnit playerUnit;
    private HealthBar healthBar;

    public virtual void Awake() // virtual for polymorphism
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        playerUnit = FindObjectOfType<PlayerUnit>();
    }

    public void TakeDamage(double inflictedDamage)
    {
        currentHealth -= inflictedDamage;

        if(!gameObject.name.Contains("Slime") && gameObject.name.Contains("Skeleton")){
            Debug.Log("INNN DAMAGE ANIMATION");
            if(playerUnit != null){
                Debug.Log("TAKEN DAMAGE ANIMATION");
                playerUnit.HealthDamageAnimation();
            }
        }
        
        if (currentHealth<=0){
            Die();
        }
    }

    public virtual void Die(){
        if (gameObject.name.Contains("Slime")){
            SlimeGenerate.slimes--;
        }
        else if (gameObject.name.Contains("Skeleton")){
            SkeletonGenerate.skeletons--;
        }
        if (healthBar!=null){
            healthBar.Disable();
        }
        StartCoroutine(DieCoroutine());
    }

    IEnumerator DieCoroutine(){
        string animationName = "Die";
        animator.SetTrigger("die");
        yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName)+0.2f);
        GetComponent<HealthBar>().healthSlider.gameObject.SetActive(false); // delete health bar
        gameObject.SetActive(false); // delete game object
    }
}
