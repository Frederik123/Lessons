// навешиваем на префаб ячейки которую будет таскать

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// чтобы сделать "приёмник" - надо реализовать интерфейс IDropHandler
public class Drop : MonoBehaviour , IDropHandler
{
    
    public void OnDrop(PointerEventData eventData)
    {
        Drag drag = eventData.pointerDrag.GetComponent<Drag>(); // берём объект который мы таскали и кинули
        if (drag != null)
        {
            //Debug.Log("drag = " + drag);
            // меняем местами 2 предмета в инвенторе при перетаскивании одного на второй
            if (transform.childCount > 0)
            {
                transform.GetChild(0).SetParent(drag.old);
            }

            drag.transform.SetParent(transform); // SetParent(transform) - объекта на который у нас навешан скрипт 


            // если ячейка на которую перенесли называется CraftItem
            if (transform.name == "CraftItem")
            {
                //player.BroadcastMessage("remove_item", this); // посылает игровому объекту и его детям
                SendMessageUpwards("updateCraft"); // посылает игровому объекту и его родителям (на родителе у нас висит Craft.cs)
            }
        }
    }
}
