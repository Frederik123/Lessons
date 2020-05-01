// вешаем на игрока . скрипт атаки

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 30;
    private Animator m_animator;

    public float attackSpeed = 2f;
    float nextAttackTime = 0f;



    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        nextAttackTime -= Time.deltaTime;
    }


    public void Attack()
    {
        if (nextAttackTime > 0)
            return;

        // анимация
        m_animator.SetTrigger("Attack");

        // враг в радиусе 
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // дамаг по врагам в радиусе
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Ударили : " + enemy.name);
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }

        // время следующей атаки
        nextAttackTime = attackSpeed; 
    }


    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange); 
    }

}
