// вешаем на противника

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;

    public Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth; 
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0) // умирание проитвника
        {
            animator.SetTrigger("Death");
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static; // чтобы не улетало за текстуры
            GetComponent<BoxCollider2D>().enabled = false;
            this.enabled = false;
        }
    }


}
