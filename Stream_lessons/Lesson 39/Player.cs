using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    CharacterController CharacterController;

    public float speed = 5f;
    Animator animator;
    public Transform pistol; // ссылка на трансформ пистолета
    public GameObject holes; // ссылка на префаб дырки от пули

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        CharacterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (v != 0) CharacterController.Move(transform.forward * v * 5f * Time.deltaTime);
        if (h != 0) transform.Rotate(Vector3.up * h);
        CharacterController.Move(Physics.gravity * Time.deltaTime);
        */

        //Vector3 pistol_barrel = new Vector3(pistol.position.x - 0.1f, pistol.position.y + 0.1f, pistol.position.z + 0.15f);
        Vector3 aim_offset = new Vector3(2, 0, 0); // смещение вектора прицела
        Debug.DrawRay(pistol.position, pistol.forward * 10f + aim_offset, Color.red);

        if (Input.GetMouseButtonDown(1))
        {
            animator.SetBool("Aim", true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool("Aim", false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(pistol.position, pistol.forward * 10f + aim_offset);
            RaycastHit info;
            if (Physics.Raycast(ray, out info))
            {
                GameObject inst_holes = Instantiate<GameObject>(holes); // инициализируем дырку
                inst_holes.transform.position = info.point + info.normal * 0.01f;
                inst_holes.transform.rotation = Quaternion.LookRotation(-info.normal);
                inst_holes.transform.SetParent(info.transform);

                Rigidbody rig = info.transform.gameObject.GetComponent<Rigidbody>();
                if (rig != null)
                {
                    rig.AddForceAtPosition(-info.normal * 1000f, info.point);
                    // rig.AddForce(-info.normal * 10f, ForceMode.Impulse);
                }
            }
        }
    }
}
