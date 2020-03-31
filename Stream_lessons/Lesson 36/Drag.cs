// навешиваем на префаб ячейки инвенторя

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // содержить классы интерфейса для различных событий


public class Drag : MonoBehaviour , IBeginDragHandler , IDragHandler ,  IEndDragHandler , IPointerClickHandler 
{
    public Transform canvas; //  
    public Transform old; //  
    private GameObject player; //  
    public Item item; //
    Craft craft; // ссылка на скрипт


    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas").transform; // ищем ближайший канвас от нашего контейнера 
        player = GameObject.FindGameObjectWithTag("Player");
        craft = GameObject.Find("Craft").GetComponent<Craft>();
    }

    // начало перетаскивания 
    public void OnBeginDrag(PointerEventData eventData)
    {
        old = transform.parent; // ищем кто родитель у контейнера который мы начали таскать 
        transform.SetParent(canvas); // временно выкидываем контейнер из ячейки

        GetComponent<CanvasGroup>().blocksRaycasts = false; // отключаем рейкаст у контейнера , чтобы ячейки под ним ловили курсор 

        // для панельки крафта
        if (old.name == "ResultItem") craft.addInventory(this); // если берём из резулта крафта
        else if (old.name == "CraftItem") craft.updateCraft(); // если берём из панельки крафта 3х3
    }

    //  в процессе таскания 
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; 
    }

    // конец перетаскивания 
    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (transform.parent == canvas) // если кинули и нет ячейки под курсором 
        {
            transform.SetParent(old);
        }       
    }

    // когда кликаем на предмет
    public void OnPointerClick(PointerEventData eventData)
    {
        // если кликнули по чему-нибудь на панели крафта
        if (transform.parent.name == "ResultItem" || transform.parent.name == "CraftItem") return;


        if (eventData.button == PointerEventData.InputButton.Right) {
            player.BroadcastMessage("use", this);
        }
        else
        {
            // посылаем в игрока сообщение . this - наш контейнер на котором висит скрипт
            player.BroadcastMessage("remove_item", this);  
        }
    }
}
