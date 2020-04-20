// Вешаем на префаб строения

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [System.Serializable]
    public class Part // класс одной запчасти модели которую будем строить
    {
        public bool ready = false; // готова ли она
        public int needType; // нужный тип ингридиента
        public GameObject result; // какой игровой объект(кусочек) мы сделаем в результате.
        public Material resultMaterial; // какой материал будет наложен после завершения постройки
    }


    public Part[] parts; // массив кусочков для заполнения
    public bool ready = false; // построено ли строение полностью
    public Vector3[] points; // позиция углов строения. пуляем с них лучи для проверки плоскости 
    public float distance; // расстояние от центра модели до героя
    public float positionY; // позиция по Y когда объект поставили. немного вниз чтобы был на земле
    //public bool isCross = false;
    //public bool preBuild = false; // когда мы поставили но ещё не полностью построили

    public bool enterCollider = false;   // когда объект попал в коллизию 

    // Start is called before the first frame update
    void Start()
    {
        if (ready) activate(); // активация самой постройки
        for (int i = 0; i < parts.Length; i++) activateParts(parts[i]); // активация её частей
    }

    public void canBuild(bool can) // перекрашиваем строение если можем\не можем его построить
    {
        Color color = (can) ? Color.green : Color.red;
        color.a = 0.5f;
        for (int i = 0; i < parts.Length; i++)
        {
            if (!parts[i].ready)
            {
                //parts[i].result.GetComponent<Renderer>().sharedMaterial.color = color;
                parts[i].result.GetComponent<Renderer>().material.color = color;
            }
        }
    }


    public bool addIngredients(Ingredient ingredient) // метод добавления ингридиента
    {
        if (!ready) // если модель не готова
        {
            bool isFindedPart = false; // нашли ли часть в которую можно положить 
            bool isReadyFull = true; // стала ли часть готова после того как положили
            for (int i = 0; i < parts.Length; i++) // перебираем все части
            {
                Part p = parts[i];
                if (!p.ready) // если часть не активирована
                {
                    if (!isFindedPart && p.needType == ingredient.type) // если ещё не построили и тип совпадает 
                    {
                        // часть готова и активируем её 
                        isFindedPart = true;
                        p.ready = true;
                        activateParts(p);
                    }
                    else
                    {
                        isReadyFull = false;
                    }
                }
                if (!isReadyFull && isFindedPart) break; // если часть найдена
            }
            if (isReadyFull) // модель полностью готова
            {
                ready = true;
                activate();
            }
            return isFindedPart;
        }
        return false;
    }

    void activateParts(Part part) // активируем часть постройки
    {
        if (part.ready && part.result != null && part.resultMaterial != null)
        {
            // part.result.GetComponent<Renderer>().sharedMaterial = part.resultMaterial; // применяем к ней финальный материал
            part.result.GetComponent<Renderer>().material = part.resultMaterial; // применяем к ней финальный материал
            part.result.GetComponent<CapsuleCollider>().enabled = true; // активируем коллайдер чтобы не проходили сквозь готовые блоки
        }
    }

    void activate() // активируем коллайдеры на объекте (постройка полностью готова)
    {
        Collider[] cols = gameObject.GetComponents<BoxCollider>();
        for (int i = 0; i < cols.Length; i++)
        {
            if (!cols[i].isTrigger) cols[i].enabled = true;
            else cols[i].enabled = false;
        }
    }

    
    void OnTriggerEnter(Collider collider)
    {
        enterCollider = true;
    }

    void OnTriggerStay(Collider other)
    {
        enterCollider = true;
    }

    void OnTriggerExit(Collider other)
    {
        enterCollider = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("123");
    }


}
