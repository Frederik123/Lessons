// Вешаем на игрока

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Builder : MonoBehaviour
{
    Building build; // какой дом сейчас "несёт" для постройки
    public Ingredient inHand; // ингридиент который сейчас в руке
    bool may; // могу ли сейчас поставить модель
    public Text tip; // ссылка куда будем выводить подсказку
    bool isInstantiate = false; // инициализация модели
    public GameObject g; // префаб постройки
    bool isMayBuild = true; // могу ли в данный момент построить

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        tip.text = "";

        if (!isInstantiate && inHand == null) // если ещё не инициализировали и в руках ничё нет
        {
            // поднимаем материалы
            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    inHand = hit.collider.GetComponent<Ingredient>(); // если на коллайдере есть компонент Ingredient
                    if (inHand != null)
                    {
                        //Rigidbody rb = inHand.GetComponent<Rigidbody>();
                        Destroy(inHand.GetComponent<Rigidbody>()); // удаляем риг чтобы не глючила анимация

                        inHand.transform.SetParent(transform);
                        inHand.transform.localPosition = inHand.position;
                        inHand.transform.localRotation = Quaternion.Euler(inHand.rotation);
                    }
                }
            }
        }


        if (inHand != null) // если в руках что-то есть (бревно)
        {
            if (Input.GetKeyUp(KeyCode.Q)) // удаляем из рук
            {
                // удаляем из рук + инстациируем перед собой + риг для физики
                GameObject inHandObject = Instantiate<GameObject>(inHand.gameObject, transform.position + transform.forward + transform.up * 2, Quaternion.Euler(90, 90, 0));
                Rigidbody inHandRigidBody = inHandObject.AddComponent<Rigidbody>(); // Add the rigidbody.
                inHandRigidBody.mass = 50f;

                Destroy(inHand.gameObject);
                inHand = null;
            }
            else
            {
                Debug.DrawRay(transform.position + transform.up, transform.forward * 3);

                RaycastHit hit;
                if (Physics.Raycast(transform.position + transform.up, transform.forward * 3, out hit, 3f))
                {
                    Building b = hit.collider.GetComponent<Building>(); // если на впереди стоящем коллайдере есть компонент-скрипт Building
                    if (b != null && !b.ready)
                    {
                        //Debug.Log("Press E");
                        tip.text = "Нажмите E для постройки";
                        if (Input.GetKeyUp(KeyCode.E))
                        {
                            if (b.addIngredients(inHand))
                            {
                                Destroy(inHand.gameObject);
                                inHand = null;
                            }
                        }
                    }
                }
            }
        }
        else // если в руках ничего нет
        {
            if (Input.GetKeyUp(KeyCode.Escape)) // уничтожаем модель для постройки 
            {
                if (build != null)
                {
                    Destroy(build.gameObject);
                    build = null;
                    isInstantiate = false;
                }
            }

            if (build != null) // если держим в руках модель для постройки
            {
                // то пуляем луч с углов которые задали для проверки плоскости 
                 
                build.transform.position = transform.position + transform.forward * build.distance + transform.up * 0.5f;
                int cnt = build.points.Length;

                if (cnt > 0)
                {
                    RaycastHit[] hits = new RaycastHit[cnt];
                    Vector3 point = Vector3.zero;
                    for (int i = 0; i < cnt; i++)
                    {
                        Ray ray = new Ray(build.transform.TransformPoint(build.points[i]), -build.transform.up * 1.5f);
                        Debug.DrawRay(build.transform.TransformPoint(build.points[i]), -build.transform.up * 1.5f);

                        if (Physics.Raycast(ray, out hits[i], 2f))
                        {
                            //Debug.Log(hits[i].collider.name);

                            if (point != Vector3.zero && Mathf.Abs(point.y - hits[i].point.y) > 0.2f)
                            {
                                isMayBuild = false;
                                break;
                            }
                            else
                            {
                                isMayBuild = true;
                                point = hits[i].point;
                            }
                        }
                        else
                        {
                            isMayBuild = false;
                            break;
                        }
                    }
                }

                if (build.enterCollider) isMayBuild = false; // если объект попал на другой объект


                if (isInstantiate || isMayBuild != may)
                {
                    build.canBuild(isMayBuild);
                    may = isMayBuild;
                }

                // если можем строить
                if (isMayBuild)
                {
                    tip.text = "Нажмите E чтобы поставить объект";
                    if (Input.GetKeyUp(KeyCode.E)) // нажали Е - ставим модель на землю
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(build.transform.position, -build.transform.up, out hit, 2f))
                        {
                            //Debug.Log(build);
                            build.transform.position = new Vector3(hit.point.x, build.positionY, hit.point.z);
                            //build.preBuild = true; 
                            build = null;
                            isInstantiate = false;
                            g = null;
                            //Debug.Log(build);
                        }
                    }

                    // поворот строящегося объекта
                    if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
                    {
                        // 1-й поворот
                        build.transform.Rotate(new Vector3(0f, 10f, 0f));
                        /*
                        // 2-й поворот
                        var angles = build.transform.rotation.eulerAngles;
                        angles.y += Time.deltaTime * 1000;
                        build.transform.rotation = Quaternion.Euler(angles);

                        // 3-й поворот
                        Quaternion oldRotation = build.transform.rotation * Quaternion.Euler(0f, 10f, 0f);
                        build.transform.rotation = Quaternion.Lerp(build.transform.rotation, oldRotation, Time.deltaTime * 100f);
                        */
                    }
                    else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
                    {
                        build.transform.Rotate(new Vector3(0f, -10f, 0f));
                    }
                }
            }
        }
    }

    // инициализация строения через кнопку на панельке
    void AddBuilding(GameObject prefab)
    {
        if (!isInstantiate && inHand == null) // если ещё не инициализировали и в руках ничё нет
        {
            g = Instantiate(prefab);
            build = g.GetComponent<Building>();
            isInstantiate = true;
        }
    }
}
