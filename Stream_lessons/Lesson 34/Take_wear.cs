using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Take_wear : MonoBehaviour
{
    public GameObject prefabShoes;
    public GameObject prefabJeans;
    public SkinnedMeshRenderer playerSkin;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            addWear(prefabShoes);
        }

        if (Input.GetKeyUp(KeyCode.T)) {
            addWear(prefabJeans);
        }
    }

    void addWear(GameObject prefab)
    {
        GameObject obj = Instantiate<GameObject>(prefab); // создаём объект из префаба
        obj.transform.SetParent(playerSkin.transform.parent); // делаем ей родителем нашего игрока

        // получаем все SkinnedMeshRenderer на нашей одежде. так как одежда может состоять из нескольких частей
        SkinnedMeshRenderer[] renderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();

        if (renderers.Length > 0)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                // если они у нас есть - то перебираем и присваиваем им кости нашего игрока
                renderers[i].bones = playerSkin.bones;
                renderers[i].rootBone = playerSkin.rootBone;
            }
        }
    }
}