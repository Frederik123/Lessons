using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Craft : MonoBehaviour
{
    public Reciple[] reciples; // массив рецептов
    public Transform items; // ссылка на  панельку CraftItem
    public Transform result ;// ссылка на ячейку в которую выводиться результат крафта
    public GameObject inventory_cell_container; // ссылка на контейнер в который ложатся предметы 
    public Inventory inventory; // ссылка на класс (через нашего игрока на котором это навешано)


    [System.Serializable] // чтобы мы могли прямо из редактора заполнять
    public class Reciple // класс рецептов
    {
        [Tooltip("Cсылка на игровой объект который мы получим при крафте")]
        public GameObject item; // Cсылка на игровой объект который мы получим при крафте

        [Tooltip("ID Материалов нужных для создания")]
        public RecipleMaterials materials; //Материалы нужные для создания
    }

    [System.Serializable] //материалы из которых будем изготавливать 
    public class RecipleMaterials
    {
        public Vector3 A;
        public Vector3 B;
        public Vector3 C;
    }

    // показываем - скрываем панельку
    public void activate(bool isActivate)
    {
        gameObject.SetActive(isActivate);

        if (!isActivate)
        {
            if (result.childCount > 0) Destroy(result.GetChild(0).gameObject); // очищаем панельку резулта

            for (int i = 0; i < items.childCount; i++) // очищаем панельку крафта 
            {
                if (items.GetChild(i).childCount > 0) Destroy(items.GetChild(i).GetChild(0).gameObject);
            }
        }
    }

    public void addInventory(Drag item) // добавляем предмет в инвентарь
    {
        for (int i = 0; i < items.childCount; i++) // очищаем панельку крафта 
        {
            if (items.GetChild(i).childCount > 0 ) // проверяем есть ли в ячейке дети
            {
                GameObject g = items.GetChild(i).GetChild(0).gameObject;
                inventory.Remove_item_public(g.GetComponent<Drag>().item);
                Destroy(g);
            }
        }
        inventory.add(item.item);
    }


    public void updateCraft()
    {
        // если в результе что-то есть - удаляем
        if(result.childCount> 0) Destroy(result.GetChild(0).gameObject);

        List<int[]> list = new List<int[]>(); // лист массивов с рецептами
        int countRec = reciples.Length; // кол-во рецептов

        // заполняем лист
        for (int i = 0; i < countRec; i++)
        {
            int[] arr = new int[9] {
                (int) reciples[i].materials.A.x,
                (int) reciples[i].materials.A.y,
                (int) reciples[i].materials.A.z,
                (int) reciples[i].materials.B.x,
                (int) reciples[i].materials.B.y,
                (int) reciples[i].materials.B.z,
                (int) reciples[i].materials.C.x,
                (int) reciples[i].materials.C.y,
                (int) reciples[i].materials.C.z
            };
            list.Add(arr);
        }


        // перебираем детей панельки крафта и смотрим совпадают ли они с индексами id из листа массива рецептов
        for (int i = 0; i < items.childCount; i++)
        {
            // смотрим есть ли ребенок . если нет - ячейка пуста
            int id = 0; 
            if (items.GetChild(i).childCount > 0)
            {
                id = items.GetChild(i).GetChild(0).GetComponent<Drag>().item.id;
            }

            // перебираем лист на совпадение ID с id ребенка
            for (int j = 0; j < countRec; j++)
            {
                if (list[j] == null) break;
                if (list[j][i] != id) list[j] = null; // зануляем, а не выкидываем из массива чтобы в конце j-индекс совпадал с индексом reciples
            }
        }

        // перебираем ещё раз и смотрим остался ли хоть 1 целый List
        for (int j = 0; j < countRec; j++)
        {
            if(list[j] != null)
            {
                Reciple current = reciples[j];


                // вставляем в ячейку резулта крафта 

                Item item = current.item.GetComponent<Item>(); // получаем наш Item
                GameObject img = Instantiate(inventory_cell_container); // создаём объект (из префаба)
                img.transform.SetParent(result);// делаем дочерним нашей ячейки
                img.GetComponent<Image>().sprite = Resources.Load<Sprite>(item.sprite);//добавляем иконку.
                // выкидываем предмет
                // img.AddComponent<Button>().onClick.AddListener(() => remove_item(current_item, img));

                img.GetComponent<Drag>().item = item;


                break;
            }
        }
    }

    





}
