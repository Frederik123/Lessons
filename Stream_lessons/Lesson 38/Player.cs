// навешиваем на префаб игрока, обязательно должен лежать в папке Resources

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class Player : Photon.MonoBehaviour
{
    CharacterController CharacterController;
    Vector3 oldPos = Vector3.zero; // старая и новая позиции игрока для интерполяции
    Vector3 newPos = Vector3.zero;
    float offsetTime = 0f; // время  

    Quaternion old_rotation = Quaternion.identity;
    Quaternion new_rotation = Quaternion.identity;
    float offsetTime_rotation = 0f; // время  
     
    bool isSynch = false; // прошла ли первая синхронизация

    // Start is called before the first frame update
    void Start()
    {
        CharacterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update() // срабатывает для всех игроков которые находятся в комнате
    {
        if (photonView.isMine) // если это наш персонаж
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (v != 0) CharacterController.Move(transform.forward * v * 5f * Time.deltaTime);
            if (h != 0) transform.Rotate(Vector3.up * h);
            CharacterController.Move(Physics.gravity * Time.deltaTime);

            if (PhotonNetwork.isMasterClient) // если игрок - главный в комнате. если он выключится - перейдёт на другого игрока
            {
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    photonView.RPC("myFunc", PhotonTargets.AllBuffered); // вызываем функцию из  RPC
                }
            }
        }
        else if (isSynch) // если не наш игрок
        {
            if (Vector3.Distance(oldPos, newPos) > 3f) transform.position = oldPos = newPos; // если пролагало
            else // если всё ок 
            {
                offsetTime += Time.deltaTime * 9f;
                transform.position = Vector3.Lerp(oldPos, newPos, offsetTime);
            }

            // если угол больше 30 - поворачиваем сразу, если нет - плавно
            if (Quaternion.Angle(old_rotation, new_rotation) > 30) transform.rotation = Quaternion.Lerp(old_rotation, new_rotation, 0f);
            else
            {
                offsetTime_rotation += Time.deltaTime * 9f;
                transform.rotation = Quaternion.Lerp(old_rotation, new_rotation, offsetTime_rotation);
            }
        }
    }


    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // наша позиция и поворот
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        // сериализуем/передаём в поток нашу позицию
        stream.Serialize(ref position);
        stream.Serialize(ref rotation);

        // если сейчас вызван метод с потоком на чтение
        if (stream.isReading)
        {
            //transform.position = position;
            oldPos = transform.position;
            newPos = position;
            offsetTime = 0f; // обновили позицию и сбрасываем время
            isSynch = true;

            //transform.rotation = rotation;
            old_rotation = transform.rotation;
            new_rotation = rotation;
            offsetTime_rotation = 0f;
        }
    }

    // чтобы вызвать метод на игровом объекте у всех игроков 
    [PunRPC]
    void myFunc()
    {
        GetComponent<Renderer>().material.color = Color.red;
    }
}
