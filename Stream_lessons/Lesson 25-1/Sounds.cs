using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public AudioClip motor;
    public AudioClip speed;
    public AudioClip skid;

    private AudioSource motorS; // звук мотора 
    private AudioSource speedS; // мотор при движении
    private AudioSource skidS; // звук заноса

    private Rigidbody r;
    // Use this for initialization
    void Awake()
    {
        r = GetComponent<Rigidbody>();
        motorS = gameObject.AddComponent<AudioSource>();
        motorS.clip = motor;
        motorS.loop = true;
        motorS.volume = .5f;
        motorS.pitch = .5f;
        //motorS.Play();

        speedS = gameObject.AddComponent<AudioSource>();
        speedS.clip = speed;
        speedS.loop = true;
        speedS.volume = .6f;
        speedS.pitch = 0;
       // speedS.Play();

        skidS = gameObject.AddComponent<AudioSource>();
        skidS.clip = skid;
        skidS.loop = true;
        //skidS.pitch = 0;
        skidS.volume = 0;
        //skidS.Play();
    }

    // Update is called once per frame
    void Update()
    {
        // pitch - скорость воспроизведения звука
        // здесь идёт зависит от скорости движения автомобиля
        motorS.pitch = (float)System.Math.Round(Mathf.Lerp(.5f, .5f + Mathf.Abs(Input.GetAxis("Vertical")), Time.deltaTime * r.velocity.magnitude * 2), 2);
        speedS.pitch = (float)System.Math.Round(Mathf.Lerp(0, 1 + Mathf.Abs(Input.GetAxis("Vertical")), Time.deltaTime * r.velocity.magnitude * 2), 2);
    }

    public void playSkid(float volume)
    {
        skidS.volume = Mathf.Abs(volume);
    }
}











