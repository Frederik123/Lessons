using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Car : MonoBehaviour
{
    [Tooltip("массив передних колёс c wheelcollider")]
    public WheelCollider[] frontCols; // массив передних колёс c wheelcollider
    [Tooltip("массив отображаемых передних колёс ")]
    public Transform[] dataFront; // массив отображаемых передних колёс 
    public WheelCollider[] backCol; // массив задних колёс c wheelcollider
    public Transform[] dataBack; // массив отображаемых задних колёс 
    public Transform centerOfMass;  // объект по которому будет задаваться центр массы автомобиля


    public float maxSpeed = 30f; // макс скорость
    private float sideSpeed = 30f; // скорость поворота и угол градуса поворота колёс
    public float breakSpeed = 100f; // сила торможения

    private Sounds sound;



    void Start()
    {
        // задаём объект по которому будет браться центр массы (смещяем вниз авто)
        // https://docs.unity3d.com/ScriptReference/Rigidbody-centerOfMass.html
        GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;  

        sound = gameObject.GetComponent<Sounds>(); // ссылка на скрипт со звуками
    }

    void Update()
    {
        /** Get axis **/
        float vAxis = Input.GetAxis("Vertical");
        float hAxis = Input.GetAxis("Horizontal");
        bool brakeButton = Input.GetButton("Jump");
        /** End get axis **/

        /** Motor - двигаем вперёд-назад **/
        // motorTorque - крутящий момент колеса.  
        // изменяем motorTorque, умножая её на значение полученное с вертикальной оси, вследствие чего наш автомобиль начинает ускоряться.
        frontCols[0].motorTorque = vAxis * maxSpeed;  
        frontCols[1].motorTorque = vAxis * maxSpeed;
        /** End motor **/

        /** Brake - тормоза **/
        if (brakeButton)
        {
            // brakeTorque - торможение колеса
            frontCols[0].brakeTorque = Mathf.Abs(frontCols[0].motorTorque) * breakSpeed;
            frontCols[1].brakeTorque = Mathf.Abs(frontCols[1].motorTorque) * breakSpeed;
        }
        else
        {
            frontCols[0].brakeTorque = 0;
            frontCols[1].brakeTorque = 0;
        }
        /** End brake **/

        /** Rotate **/
        // поворот влево-вправо
        // steerAngle - Угол поворота в градусах, всегда вокруг локальной оси Y.
        frontCols[0].steerAngle = hAxis * sideSpeed;
        frontCols[1].steerAngle = hAxis * sideSpeed;
        /** End rotate **/

        /** Update graphics cols **/
        // меняем графику отображаемых колёс
        // rpm - текущая скорость вращения колеса
       
        dataFront[0].Rotate(frontCols[0].rpm * Time.deltaTime, 0, 0);
        dataFront[1].Rotate(frontCols[1].rpm * Time.deltaTime, 0, 0);
        dataBack[0].Rotate(backCol[0].rpm * Time.deltaTime, 0, 0);
        dataBack[1].Rotate(backCol[1].rpm * Time.deltaTime, 0, 0);
  /*       
        dataFront[0].Rotate(0, 0, -frontCols[0].rpm * Time.deltaTime);
        dataFront[1].Rotate(frontCols[1].rpm * Time.deltaTime, 0, 0);
        dataBack[0].Rotate(backCol[0].rpm * Time.deltaTime, 0, 0);
        dataBack[1].Rotate(backCol[1].rpm * Time.deltaTime, 0, 0);
*/
        /*
        dataFront[0].rotation *= Quaternion.Euler(frontCols[0].rpm * Time.deltaTime, hAxis * sideSpeed, 0f);
        dataFront[1].rotation *= Quaternion.Euler(frontCols[1].rpm * Time.deltaTime, hAxis * sideSpeed, 0f);
        dataBack[0].rotation *= Quaternion.Euler(backCol[0].rpm * Time.deltaTime, 0f, 0f);
        dataBack[1].rotation *= Quaternion.Euler(backCol[1].rpm * Time.deltaTime, 0f, 0f);
        */



        // меняем плавно анимацию передних колёс. только по оси Y (вправо-влево)
        // не знаю почему, но колесо периодически разворачивается на 180 градусов по Z
        dataFront[0].localEulerAngles = new Vector3(dataFront[0].localEulerAngles.x, hAxis * 30f, dataFront[0].localEulerAngles.z);
        dataFront[1].localEulerAngles = new Vector3(dataFront[1].localEulerAngles.x, hAxis * sideSpeed, dataFront[1].localEulerAngles.z);

        /*
        Debug.Log(dataFront[0].localEulerAngles.z);

        if (dataFront[0].localEulerAngles.z == 180)
        {
            Debug.Log("here");
            dataFront[0].Rotate(dataFront[0].transform.rotation.x, 0, 0);
        }
        */

        /* End update graphics cols **/

        /** Skid - озвучка заноса **/
        WheelHit hit;
        // GetGroundHit - Получает данные о столкновении с землей для колеса.
        if (backCol[0].GetGroundHit(out hit))
        {
            // sidewaysSlip - боковой занос
            float vol = (Mathf.Abs(hit.sidewaysSlip) > .25f) ? hit.sidewaysSlip / 2.5f : 0;
            sound.playSkid(vol);
        }
        /** End skid **/
    }
}