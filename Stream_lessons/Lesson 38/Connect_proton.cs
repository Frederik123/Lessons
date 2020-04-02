// навешиваем куда-нибудь

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connect_proton : Photon.MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //PhotonNetwork.autoJoinLobby = true; 
        PhotonNetwork.ConnectUsingSettings("0.1"); // коннектимся к протону
    }

    void OnJoinedLobby() // вызывается автоматически когда мы подключаемся к серваку протону 
    {
        //PhotonNetwork.GetRoomList(); // получаем список всех комнат
        PhotonNetwork.JoinOrCreateRoom("Test_Room", new RoomOptions(), TypedLobby.Default); // создаём или подключаемся
    }

    private void OnJoinedRoom() // вызывается когда подключились к комнате
    {
        // инициализируем объект (обязательно закинуть в папку Resources)
        PhotonNetwork.Instantiate("Prefabs/Cube", Vector3.up * 10f, Quaternion.identity, 0);
    }
}
