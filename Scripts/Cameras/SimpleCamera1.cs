// простая камера для объектов "механического" типа

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCamera1 : MonoBehaviour
{
    public Transform target; // цель за которой будем следовать
    public float speed = 4f; // скорость перемещения камеры
    public LayerMask maskObstacles; // маска слоёв которые будут ....

    private Vector3 _position; // изначальная позиция камеры относительно цели


    // Start is called before the first frame update
    void Start()
    {
        _position = target.InverseTransformPoint(transform.position);  // мировые координаты камеры в локальные относительно цели
    }

    // Update is called once per frame
    void Update()
    {
        // и делаем чтобы камера не переворачивалась вместе с целью вниз если авто перевернулось
        // для этого надо рассчитать currentPosition не зависимо от поворота target по X и Y
        var oldRotation = target.rotation;  // старый поворот
        target.rotation = Quaternion.Euler(0, oldRotation.eulerAngles.y, 0); // сбросим
        var currentPosition = target.TransformPoint(_position); // считаем текущий position
        target.rotation = oldRotation; // возвращаем старый поворот цели

        transform.position = Vector3.Lerp(transform.position, currentPosition, speed * Time.deltaTime); // плавное перемещение позиции
        var currentRotation = Quaternion.LookRotation(target.position - transform.position);  // рассчитываем направление поворота
        transform.rotation = Quaternion.Lerp(transform.rotation, currentRotation, speed * Time.deltaTime); // плавное перемещение ротации

        // пускаем луч от камеры к автобусу и если есть препятствие - то переносим камеру ближе  
        RaycastHit hit;
        if(Physics.Raycast(target.position , transform.position - target.position , out hit , Vector3.Distance(transform.position , target.position) , maskObstacles))
        {
            transform.position = hit.point;
            transform.LookAt(target);
        }
    }
}
