using System.Collections;
using System.Collections.Generic; // класс list для хранения предметов инвенторя
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public List<Item> list;
    public GameObject inventory; // ссылка на объект 
    public GameObject inventory_cell_container; // ссылка на контейнер в который ложатся предметы 
    public Player player_controller;

    public Craft craft; // ссылка класс крафта

    // Start is called before the first frame update
    void Start()
    {
        list = new List<Item>(); // определяем класс
        player_controller = GetComponent<Player>(); // получаем скрипт игрока
    }

    // Update is called once per frame
    void Update()
    {
        // поднимаем предмет 
        if (Input.GetMouseButtonUp(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // пуляем луч 
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Item item = hit.collider.GetComponent<Item>(); // получаем наш item
                if (item != null) // если есть компонент Item на объекте на который кликнули , значит он подбираемый
                {
                    list.Add(item); // добавляем в лист

                    hit.collider.gameObject.SetActive(false); // делаем скрытым так как в массив засовывается ссылка на объект. 
                    // хз пока как правильно сделать 

                   // Destroy(hit.collider.gameObject); // удаляем со сцены
                }
            }
        }

        // показываем инвентарь
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventory.activeSelf)
            {
                craft.activate(false);// скрываем панельку крафта 

                inventory.SetActive(false);

                // очищаем инвентарь
                for (int i = 0; i < inventory.transform.childCount; i++)
                {
                    if (inventory.transform.GetChild(i).transform.childCount > 0)
                    {
                        Destroy(inventory.transform.GetChild(i).transform.GetChild(0).gameObject);
                    }
                }
            }
            else
            {
                inventory.SetActive(true);
                craft.activate(true);// показываем панельку крафта 

                // инициализируем инвентрь
                int count = list.Count; // кол-во вещей в инвенторе 
                for (int i = 0; i < count; i++)
                {
                    Item current_item = list[i];

                    if (inventory.transform.childCount >= i) // если количество детей в инвенторе больше инвенторя
                    {
                        GameObject img = Instantiate(inventory_cell_container); // создаём объект (из префаба)

                        // делаем дочерним нашей ячейки.
                        //чтобы сделать дочерним один элемент ко второму - надо сделать дочерний их трансформ 
                        img.transform.SetParent(inventory.transform.GetChild(i).transform);

                        //добавляем иконку.
                        img.GetComponent<Image>().sprite = Resources.Load<Sprite>(current_item.sprite);

                        // выкидываем предмет
                        // img.AddComponent<Button>().onClick.AddListener(() => remove_item(current_item, img));
                         
                        img.GetComponent<Drag>().item = current_item; 
                    }
                    else break;
                }
            }
        }
    }

    /*
    // старый метод
    void remove_item(Item current_item, GameObject img)
    {
        // добавляем объект в мир
        GameObject new_object = Instantiate<GameObject>(Resources.Load<GameObject>(current_item.prefab));
        new_object.transform.position = transform.position + transform.forward + transform.up; // наша позиция + метр вперёд + метр вверх

        // удаляем из инвенторя
        Destroy(img);

        // удаляем из листа объектов 
        list.Remove(current_item);
    }
    */

    // метод добавления 
    public void add(Item item)
    {
        // если в рюкзаке нет его - то добавляем
        if (list.FindIndex(x => x == item) == -1) list.Add(item);
    }

    // метод удаления из инвенторя 
    public void Remove_item_public(Item item)
    {
        list.Remove(item);
    }

    void remove_item(Drag drag)
    {
        //Debug.Log("here");

        Item current_item = drag.item;

        // добавляем объект в мир
        GameObject new_object = Instantiate<GameObject>(Resources.Load<GameObject>(current_item.prefab));
        new_object.transform.position = transform.position + transform.forward + transform.up; // наша позиция + метр вперёд + метр вверх
    
        list.Remove(current_item); // удаляем из листа объектов 
        Destroy(drag.gameObject); // удаляем из инвенторя
        Destroy(current_item.gameObject);  // удаляем со сцены скрытый 
    }

    // использование предметов 
    void use(Drag drag)
    {
        Item current_item = drag.item;
        bool used = false;

        if (current_item.type == "food")
        {
            player_controller.add_food(30); // вызываем метод add_food в скрипте персонажа
            used = true;
        }
        else if (current_item.type == "hand")
        {
            // создаём новый предмет
            Hand_item myitem = Instantiate<GameObject>(Resources.Load<GameObject>(drag.item.prefab)).GetComponent<Hand_item>();
            //Debug.Log("myitem 1 = " + myitem);

            player_controller.add_to_hand(myitem); // вызываем метод add_to_hand в скрипте персонажа

            used = true;
        }

        if (used)
        {
            list.Remove(current_item); // удаляем из листа объектов 
            Destroy(drag.gameObject); // удаляем из инвенторя
            Destroy(current_item.gameObject);  // удаляем со сцены скрытый 
        }
    }
}
