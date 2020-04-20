// Вешаем на кнопку 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class typeBuilding : MonoBehaviour
{

    public GameObject prefab; // префаб постройки
    private GameObject player; // игрок который будет строить

    void Start()
    {
        /*
        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
        */
        
        this.GetComponent<Button>().onClick.AddListener(TaskOnClick);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void TaskOnClick()
    {
        //Debug.Log("You have clicked the button!");
        player.BroadcastMessage("AddBuilding", prefab);
    }
}





     

  


