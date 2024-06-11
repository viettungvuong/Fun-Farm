using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int maxHealth;
    public int damage;

    [HideInInspector] public int currentHealth;

    Animator animator;

    public virtual void Awake() // virtual for polymorphism
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int inflictedDamage)
    {
        currentHealth -= inflictedDamage;
        if (currentHealth<=0){
            Die();
        }
    }

    void Die(){
        StartCoroutine(DieCoroutine());
    }

    IEnumerator DieCoroutine(){
        string animationName = "Die";
        animator.SetTrigger("die");
        yield return new WaitForSeconds(GameController.GetAnimationLength(animator, animationName)+1f);
        GetComponent<HealthBar>().healthSlider.gameObject.SetActive(false); // delete health bar
        gameObject.SetActive(false); // delete game object
    }
}
