// простая камера для объектов "живого" типа

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCamera2 : MonoBehaviour
{
    public Camera cam; // камера
    public Transform target; // цель за которой будет следить
    public float speedX = 360f; // скорость камеры по X 
    public float speedY = 240f; // скорость камеры по Y
    public float limitY = 40f; // лимит камеры по Y
    public float minDistance = 1.5f; // минимальная дистанция до цели 
    private float _maxDistance; // максимальная дистанция до цели

    public float hideDistance = 2f; // дистанция на которой персонаж скрывается
    public LayerMask obstacles; // маска слоёв с препятствиями
    public LayerMask noPlayer; // все слои кроме нашего персонажа
     
    private Vector3 _localPosition; // позиция камеры в локальных координатах нашей цели
    private float _currentYRotation; // текущий  поворот по Y
    private LayerMask _camOrigin; // изначальная маска слоёв камеры

    private Vector3 _position // обертка на transform.position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    void Start()
    {
        _localPosition = target.InverseTransformPoint(_position); // локальные координаты камеры
        _maxDistance = Vector3.Distance(_position, target.position); // дистанция от камеры до цели
        _camOrigin = cam.cullingMask;
    }

    void LateUpdate()
    {
        _position = target.TransformPoint(_localPosition); // помещаем камеру сзади цели
        CameraRotation(); // поворот камеры мышью 
        ObstaclesReact(); // реагирование камеры на препятствия 
        PlayerReact(); // скрытие\показ персонажа
        _localPosition = target.InverseTransformPoint(_position); // обновляем позиция камеры после всех действий
    }

    void CameraRotation()
    {
        var mx = Input.GetAxis("Mouse X");
        var my = Input.GetAxis("Mouse Y");

        if (my != 0)
        {
            var tmp = Mathf.Clamp(_currentYRotation + my * speedY * Time.deltaTime, -limitY, limitY); // новый поворот камеры
            if (tmp != _currentYRotation)
            {
                var rot = tmp - _currentYRotation; // угол поворота
                transform.RotateAround(target.position, transform.right , rot); // поворачиваемся вокруг персонажа (вокруг цели, вверх-вниз, поворот)
                _currentYRotation = tmp; // сохраняем что получили
            }
        }
        if (mx != 0)
        {
            // здесь проверок на надо, так как по X можем поворачиваться на 360 градусов
            transform.RotateAround(target.position, Vector3.up, mx * speedX * Time.deltaTime);
        }

        transform.LookAt(target); // поворачиваем камеру на точку
    }


    void ObstaclesReact()
    {
        var distance = Vector3.Distance(_position, target.position);
        RaycastHit hit; 
        if (Physics.Raycast  ( target.position , transform.position - target.position , out hit , _maxDistance , obstacles)) // если появилось препятствие - перемещаем камеру туда
        {
            _position = hit.point; 
        }

        // если дистанция от камеры до цели меньше чем максимальная - то пуляем луч на чуть чуть назад и если нет препятствия между камерой и игроком 
        // то отодвигаем камеру назад на немного, что позволит плавно возвращать камеру на место
        else if (distance < _maxDistance && !Physics.Raycast(_position , -transform.forward, 0.1f, obstacles))
        {
            _position -= transform.forward * 0.05f;
        }
    }


    void PlayerReact()
    {
        var distance = Vector3.Distance(_position, target.position);
        if (distance < hideDistance) // если дистанция до цели меньше установленной , то камере устанавливаем маску отсечания показываемых слоёв
        {
            cam.cullingMask = noPlayer;
        }
        else
        {
            cam.cullingMask = _camOrigin;
        }
    }

}
