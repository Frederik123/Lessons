// контроллер для оружия

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public GameObject bulletHolePrefab; // префаб на дырку от пули
    public AudioSource audioSource; // звук выстрела
    public float waitTime = 0.15f; // задержка после выстрела
    private float _wait = 0f;

    void Update()
    {
        if (_wait > 0) _wait -= Time.deltaTime;
    }

    public void Shoot()
    {
        if (_wait <= 0)
        {
            _wait = waitTime; // делаем задержку на выстрел
            audioSource.Play();

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
            {
                //Debug.Log(hit.collider.tag);

                if (hit.collider.tag == "Enemy")
                {
                    hit.transform.SendMessage("Damage");
                }
                else
                {
                    GameObject bulletHole = Instantiate<GameObject>(bulletHolePrefab); // инициализируем дырку
                    bulletHole.transform.position = hit.point + hit.normal * 0.01f + new Vector3(0, 0.05f, 0.00f);
                    bulletHole.transform.rotation = Quaternion.LookRotation(-hit.normal);
                    bulletHole.transform.SetParent(hit.transform);

                    Rigidbody r = hit.collider.GetComponent<Rigidbody>();
                    if (r != null)
                    {
                        r.AddForceAtPosition(-hit.normal * 0.25f, hit.transform.InverseTransformPoint(hit.point), ForceMode.Impulse);
                    }
                }
            }
        }
    }


}
