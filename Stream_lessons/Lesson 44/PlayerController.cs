// контроллер для игрока

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;
    public Animator animator;
    public float speedMove = 3f;
    public float speedRotation = 180f;
    public GunController GunController;
    
    public float minY = -25f; // манимальный угол камеры по Y
    public float maxY = 35f; // максимальный угол камеры по Y
    private float _currentY; // текущий поворот камеры


    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        _currentY = Camera.main.transform.eulerAngles.x; // получаем поворот камеры в нормальных углах, а не в кватернионах
    }

    // Update is called once per frame
    void Update()
    {
        if (characterController.isGrounded)
        {
            float vertical = Input.GetAxis("Vertical");
            //float horizontal = Input.GetAxis("Horizontal");
            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");

            if (vertical != 0) // вперед назад
            {
                characterController.Move(transform.forward * speedMove * vertical * Time.deltaTime);
                animator.SetBool("walk", true);
            }
            else
            {
                animator.SetBool("walk", false);
            }

            if (mx != 0)  // двигаем обзор по горизонтали
            {
                transform.Rotate(transform.up * mx * speedRotation * Time.deltaTime);
            }

            if (my != 0)  // двигаем обзор по вертикали
            {
                _currentY = Mathf.Clamp(_currentY - my * speedRotation * Time.deltaTime , minY , maxY); // вычисляем новый поворот камеры
                Vector3 camRotation = Camera.main.transform.rotation.eulerAngles; // получаем текущий поворот камеры

                // изменяем поворот камеры . Quaternion.Euler - позволяет вернуть из нормальных Euler-углов в кватернионы
                Camera.main.transform.rotation = Quaternion.Euler(_currentY, camRotation.y, camRotation.z); 
            }

            if (Input.GetMouseButtonDown(0))
            {
                animator.SetBool("shoot", true);
                GunController.Shoot();
            }
            else
            {
                animator.SetBool("shoot", false);
            }
        }
        characterController.Move(Physics.gravity * Time.deltaTime);
    }

    void Damage()
    {
        GameManager.instance.deadUnit(gameObject);
    }

}
