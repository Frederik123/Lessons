using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reader : MonoBehaviour
{
    public LayerMask mask; // маска слоёв для рэйкаста
    bool isRead = false;  // читаем ли письмо
    private Transform letter;
    public Text txt; // ссылка куда будем выводить подсказку

    // Update is called once per frame
    void Update()
    {
        // Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward *2f , Color.red);
        if (!isRead)
        {
            txt.text = "";
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 2f, mask)) ;
            {
                if (hit.collider.tag == "letter")
                {
                    txt.text = "Нажмите E чтобы прочесть";
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        letter = hit.transform;
                        isRead = true;
                    }
                }
            }
        }
        else
        {
            // распологаем перед камерой
            letter.position = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
            letter.LookAt(Camera.main.transform);
            if (Input.GetKeyUp(KeyCode.E))
            {
                isRead = false;
                Destroy(letter.gameObject); // если не удалять то так можно переносить объекты)
            }
        }
    }
}
