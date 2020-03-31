using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; // вызываем чтобы могли пользоваться UI 

public class Player : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    CharacterController controller;

    public Camera cam;

    public int food = 100;
    public Slider slider;

    public Transform right_hand; // правая рука для того чтобы брать вещи из инвентаря
    private Hand_item item_in_hand;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();

        slider.maxValue = slider.value = food;
        InvokeRepeating("minus_food", 2f, 2f); // повторяет метод по времени
    }

    void minus_food()
    {
        food -= 5;
    }

    public void add_food(int count)
    {
        food += count;
        if (food > 100) food = 100;
    }

    // добавление предмета в руку
    public void add_to_hand(Hand_item it)
    {
        if (item_in_hand != null)
        {
            item_in_hand.transform.SetParent(null);
            item_in_hand.gameObject.AddComponent<Rigidbody>();
        }

        it.transform.SetParent(right_hand);
        it.transform.localPosition = it.position; // позиционируем локально
        it.transform.localRotation = Quaternion.Euler(it.rotation); // позиционируем локально
        Destroy(it.GetComponent<Rigidbody>()); // удаляем так как будет тянут к земле
        item_in_hand = it;

        // запускаем анимацию сжатия руки
        animator.SetBool("isHand", true);
        animator.SetFloat("radius", it.radius);
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = food; 

        if (Input.GetMouseButtonDown(2))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition); // берем путь от клика мыши на экране в мир
            RaycastHit hit; // 
            if (Physics.Raycast(ray, out hit))
            { // out hit - внешняяя переменна которая перезаписывается (разрешили писать в функции)
                agent.destination = hit.point; // отправляем перса в место где Raycast столкнулся с миром 
            }
        }

        // Debug.Log("Text: " + agent.velocity.magnitude);
        if (agent.velocity.magnitude > 2f) // если скорость персонажа...
        {
            animator.SetBool("walk", true);
        }
        else
        {
            animator.SetBool("walk", false);
        }

    }
}
