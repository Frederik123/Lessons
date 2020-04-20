using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text scoresDisplay;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.scoreEvent += ScoreChange; // добавляем(подписываемся на) в делегата метод
        Cursor.visible = false; // скрываем курсор
        Cursor.lockState = CursorLockMode.Locked;  // блокируем курсор в центре экрана
    }

    void ScoreChange(int scores) // метод для отображения очков
    {
        if (scoresDisplay != null)
        {
            scoresDisplay.text = scores.ToString();
        }
    }

    void OnDestroy()
    {
        Cursor.visible = true; // показываем курсор
        Cursor.lockState = CursorLockMode.None;  // разблокируем курсор 
    }

}
