using System.Collections.Generic;
using UnityEngine;

public class SnakeTail : MonoBehaviour
{
    public Transform SnakeHead; // ссылка на "голову змейки"
    public float CircleDiameter; // диаметр головы

    private List<Transform> snakeCircles = new List<Transform>(); // список трансформов кружков из которых состоит хвост
    private List<Vector2> positions = new List<Vector2>(); // список позиций этих трансформов

    private void Awake()
    {
        positions.Add(SnakeHead.position);
    }

    private void Update()
    {
        // расстояние между текущим положениемм головы и последним кружком
        float distance = ((Vector2) SnakeHead.position - positions[0]).magnitude;

        // плавно двигаем змейку
        if (distance > CircleDiameter)
        {
            // Направление от старого положения головы, к новому
            Vector2 direction = ((Vector2) SnakeHead.position - positions[0]).normalized;

            // вставляем новую позицию головы
            positions.Insert(0, positions[0] + direction * CircleDiameter);
            // удаляем последий круг
            positions.RemoveAt(positions.Count - 1);

            distance -= CircleDiameter;
        }

        // проходимся по кружкам и задаём новую позицию
        for (int i = 0; i < snakeCircles.Count; i++)
        {
            snakeCircles[i].position = Vector2.Lerp(positions[i + 1], positions[i], distance / CircleDiameter);
        }
    }

    // добавление кружка хвоста в конец змейки 
    public void AddCircle()
    {
        Transform circle = Instantiate(SnakeHead, positions[positions.Count - 1], Quaternion.identity, transform);
        snakeCircles.Add(circle);
        positions.Add(circle.position);
    }

    // удаление кружка хвоста
    public void RemoveCircle()
    {
        Destroy(snakeCircles[0].gameObject);
        snakeCircles.RemoveAt(0);
        positions.RemoveAt(1);
    }
}
