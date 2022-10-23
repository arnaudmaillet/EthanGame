using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon")]
    public Transform shootPoint;
    public float range = 100f;
    public int magazin = 31;
    public float fireRate = 0.5f;
    float fireTimer; // Temps entre les clicks gauche de la souris

    [Header("Animations")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem SparksEmitter;
    public ParticleSystem SparksEmitter_1;

    [Header("Sound")]
    public AudioClip shootSound;

    private int _BulletsInMagazin;
    private Animator anim;
    private AudioSource _AudioSource;

    void Start()
    {
        anim = GetComponent<Animator>();
        _AudioSource = GetComponent<AudioSource>();
        _BulletsInMagazin = magazin;
    }

    void Update()
    {
        if (Input.GetButton("Fire1")) fire();
        if (fireTimer < fireRate) 
            fireTimer += Time.deltaTime;
    }

    void FixedUpdate()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
    }

    public void fire()
    {
        if (fireTimer < fireRate || magazin <= 0) return;
        RaycastHit hit;
        if (Physics.Raycast(shootPoint.position, shootPoint.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name + " hit");
        }
        anim.CrossFadeInFixedTime("Fire", 0.01f); //assigner true au parametre Fire de l'animator --> Déclenche l'animation
        muzzleFlash.Play();
        SparksEmitter.Play();
        SparksEmitter_1.Play();
        PlayShootSound();
        _BulletsInMagazin--;
        fireTimer = 0.0f;   
    }

    private void PlayShootSound()
    {
        _AudioSource.clip = shootSound;
        _AudioSource.Play();
    }
}
