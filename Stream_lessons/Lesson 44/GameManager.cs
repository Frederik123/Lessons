using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class GameManager
{
    private int _userScore = 0; 
    private static GameManager _instance; // переменная хранит ссылку на экземпляр класса
    public static GameManager instance // публичная переменная для обращения 
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameManager(); // то создаём экземпляр класса
            }
            return _instance;
        }
    }
    private GameManager() { } // приватный конструктор класса, чтобы никто не мог создать экземпляр класса кроме него самого 
    public delegate void EventScoreHandler(int scores); // делегат который хранит все методы которые подписаны на события 
    public event EventScoreHandler scoreEvent = delegate { }; // событие с типом делегата для подписки . + определяем пустым для потокобезопасности

    public void deadUnit(GameObject unit) // юнит которого убили
    {
        switch (unit.tag)
        {
            case "Player":
                _userScore = 0;
                GameOver();
                break;
            case "Enemy":
                _userScore += 100;
                scoreEvent(_userScore);
                break;
        }
    }

    private void GameOver()
    {
        scoreEvent = delegate { }; // сбрасываем делегата (всех подписанных на него). 
        SceneManager.LoadScene(0);
    }

}
