// скрипт для спавна зомби. вешается на место респауна

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject zombiePrefab;
    public float timeSpawn = 5f; // время респауна

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Spawn", timeSpawn); // вызываем метод через определенное время
    }

    void Spawn ()
    {
        Instantiate(zombiePrefab, transform.position, transform.rotation);
        if (timeSpawn > 0.2f)
        {
            timeSpawn -= 0.2f;
        }
        Invoke("Spawn", timeSpawn);
    }
}
