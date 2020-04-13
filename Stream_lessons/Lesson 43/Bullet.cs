using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float speed = 925f; //Скорость пули м/c
    public float windHSpeed = 0f; //Сила бокового ветра м/с
    public float windVSpeed = 0f; //Сила продольного ветра м/с
    public float temp = 15f; //Температура воздуха
    public float pressure = 750f; //Атмосферное давление

    public float rightDerivation = 0.2f; //Деривация м/с
    public float hWindCoeff = .05f; //Коэффициэнт на боковой ветер
    public float vWindCoeff = .025f; //Коэффициэнт на продольный ветер
    public float tempCoeff = .03f; //Коэффициэнт на температуру +/-1С от 15
    public float pressureCoeff = .01f; //Коэффициэнт на атмосферное давление +/1 мм.

    public GameObject bulletDecal;

    private float tempReference = 15f;
    private float pressureReference = 750f;
    private float currentSpeed;
    private float timeOfFly = 0;
    private bool destroyed = false;

    void Start()
    {
        currentSpeed = speed;
    }

    void Update()
    {
        if (!destroyed)
        {
            // движение пули в зависимости от "погодных" условий
            Vector3 oldPosition = transform.position;
            var delta = Time.deltaTime;
            var t = temp - tempReference;
            var p = pressure - pressureReference;
            var coefficient = (transform.right * rightDerivation * delta) //деривация
                + (transform.right * (windHSpeed * hWindCoeff * delta)) //боковой ветер
                + (transform.up * (windVSpeed * vWindCoeff * delta + t * tempCoeff * delta - p * pressureCoeff * delta)) // продольный ветер, температура и давление
                ;
            transform.position += transform.forward * currentSpeed * delta + coefficient;
            timeOfFly += delta;
            currentSpeed -= delta * (pressure * .5f + windVSpeed);
            RaycastHit hit;


            if (Physics.Raycast(oldPosition, transform.position - oldPosition, out hit, Vector3.Distance(oldPosition, transform.position)))
            {
                transform.position = hit.point;
                //Во что-то попали)
                if (hit.transform.name != "Terrain")
                    //Destroy(hit.transform.gameObject);

                    if (bulletDecal != null)
                    {
                        /*
                        GameObject decal = GameObject.Instantiate<GameObject>(bulletDecal);
                        decal.transform.position = hit.point + hit.normal * .003f;
                        decal.transform.rotation = Quaternion.FromToRotation(decal.transform.forward, hit.normal);
                        decal.transform.Rotate(decal.transform.forward, Random.Range(0, 360));
                        decal.transform.SetParent(hit.transform);
                        */
                        GameObject decal = Instantiate<GameObject>(bulletDecal); // инициализируем дырку
                        decal.transform.position = hit.point + hit.normal * 0.01f + new Vector3(0, 0.05f, 0.00f);
                        decal.transform.rotation = Quaternion.LookRotation(-hit.normal);
                        decal.transform.SetParent(hit.transform);
                    }

                currentSpeed = 0;
                destroyed = true;
            }

            if (timeOfFly > 3f || currentSpeed <= 0) Destroy(gameObject);
        }
    }
}
