// контроллер противника

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Animator animator;
    private Transform _player; 

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        // карутины позволяют снизить нагрузку на вычисления без потери производительности
        StartCoroutine(FindPath()); // запускаем карутину. она выполняется параллельно с основным потоком 
        StartCoroutine(PlayerDetect());
    }

    // =====================================================================================================

    private IEnumerator PlayerDetect() // находим игрока рядом с собой
    {
        while (true)
        {
            if (_player != null)
            {
                if (Vector3.Distance(transform.position, _player.transform.position) < 1f)
                {
                    animator.SetBool("attack", true);
                    _player.SendMessage("Damage");
                }
                else
                {
                    animator.SetBool("attack", false);
                }
                yield return new WaitForSeconds(0.3f); // цикл карутины засыпает на 0.3 секунды 
            }
            else break; // если игрока не станет - то карутина прервётся
        }
    }

    // =====================================================================================================

    private IEnumerator FindPath() // находим путь до игрока 
    {
        while (true)
        {
            if (_player != null)
            {
                navMeshAgent.SetDestination(_player.position);
                yield return new WaitForSeconds(2f); // цикл карутины засыпает на 2 секунды 
            }
            else break; // если игрока не станет - то карутина прервётся
        }
    }

    // =====================================================================================================

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(navMeshAgent.velocity.magnitude);

        if (navMeshAgent.velocity.magnitude > 1.4f)
        {
            animator.SetBool("walk", true);
        }
        else
        {
            animator.SetBool("walk", false);
        }
    }

    void Damage()
    {
        StopAllCoroutines();
        navMeshAgent.enabled = false;
        gameObject.GetComponent<CapsuleCollider>().enabled = false; 
        animator.SetTrigger("dead");
        //Destroy(gameObject, 5f);
        GameManager.instance.deadUnit(gameObject);
    }

}
