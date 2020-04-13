using UnityEngine;
using System.Collections;

public class Sniper : MonoBehaviour
{
    [Header("Настройки камеры")]
    public Camera cam;
    [Space]

    [Header("Настройки прицела")]
    public float correctH = 0; //Горизонтальная корректировка прицела
    public float correctV = 0; //Вертикальная корректировка прицела
    public float correctStep = .01f; //Шаг корректировки прицела
    public float returnOffset = .02f; //Смещение на отдачу
    [Space]

    [Header("Ссылки на префабы и компоненты")]
    [Tooltip("Префаб пули")]
    public GameObject bullet;

    [Tooltip("Ссылка на точку в пространстве для респавна пули ")]
    public GameObject bullet_respawn;

    [Tooltip("Проигрыватель")]
    public AudioSource sounds;

    [Tooltip("Звук выстрела")]
    public AudioClip soundFire;
    [Space]

    private bool busy = false; //Занято, управление не работает (время отдачи)
    private Quaternion oldRotation; //Старый поворот (куда должна вернуться камера после отдачи)


    public GameObject sniper_rufle;
    private Vector3 sniper_pos_start;
    private Vector3 sniper_pos_end = new Vector3(-0.012f, -0.326f, 0.316f); // координаты прицела при прицеливании

    void Start()
    {
        sniper_pos_start = sniper_rufle.transform.localPosition;
    }

    void Update()
    {

        // прицеливаемся 
        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(SmoothMove(sniper_rufle, sniper_pos_start, sniper_pos_end, 0.3f));
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StartCoroutine(SmoothMove(sniper_rufle, sniper_pos_end, sniper_pos_start, 0.3f));
        }


        if (!busy && cam != null)
        {
            //Стрельба
            if (Input.GetMouseButtonDown(0))
            {
                //Звук выстрела
                if (sounds != null && soundFire != null)
                    sounds.PlayOneShot(soundFire);

                if (bullet != null)
                {
                    //Инстанциируем пулю
                    // var bulletInstance = (GameObject)GameObject.Instantiate(bullet, cam.transform.position, cam.transform.rotation);
                    var bulletInstance = (GameObject)GameObject.Instantiate(bullet, bullet_respawn.transform.position, cam.transform.rotation);

                    //Поворачиваем в зависимости от корректировки прицела
                    bulletInstance.transform.rotation *= Quaternion.Euler(correctV, correctH, 0f);

                    //Отдача
                    oldRotation = transform.rotation * Quaternion.Euler(Random.Range(-returnOffset, returnOffset), Random.Range(-returnOffset, returnOffset), 0f);
                    var newRotation = oldRotation.eulerAngles - transform.right * 1f + transform.up * .2f;
                    transform.rotation = Quaternion.Euler(newRotation);
                    busy = true;
                    StartCoroutine(ReturnRotation());
                }
            }
        }
    }

    // плавное прицеливание
    IEnumerator SmoothMove(GameObject gameObject, Vector3 startPos, Vector3 endPos, float time)
    {
        float currTime = 0;
        do
        {
            gameObject.transform.localPosition = Vector3.Lerp(startPos, endPos, currTime / time);
            currTime += Time.deltaTime;
            yield return null;
        }
        while (currTime <= time);
    }

    // Возврат после отдачи
    IEnumerator ReturnRotation()
    {
        while (true)
        {
            if (Quaternion.Angle(transform.rotation, oldRotation) <= .001f)
            {
                transform.rotation = oldRotation;
                break;
            }
            transform.rotation = Quaternion.Lerp(transform.rotation, oldRotation, Time.deltaTime * 7f);
            yield return null;
        }
        busy = false;
    }
}
