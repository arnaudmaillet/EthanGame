using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon")]
    public Transform shootPoint;
    public float range = 100f;
    public int magazin = 3;
    public float fireRate = 0.5f;
    public float damage = 20f;
    [SerializeField] private int _BulletsInMagazin;
    public int bulletsleft = 1;
    public KeyCode reloadKey;

    public enum ShootMode {Auto, Semi};
    public ShootMode shootingMode;

    [Header("Bullets particules")]
    public GameObject hitParticles;
    public GameObject bulletImpact;

    [Header("Aiming")]
    private Vector3 originalPosition;
    public Vector3 amingPosition;
    public float aodSpeed = 8f;

    [Header("Animation")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem SparksEmitter;
    public ParticleSystem SparksEmitter_1;

    [Header("Sound")]
    public AudioClip shootSound;
    public AudioClip emptyMagazinSound;

    private Animator anim;
    private bool shootInput;
    private bool isReloading = false;
    private AudioSource _AudioSource;
    float fireTimer; // Temps entre les clicks gauche de la souris

    void Start()
    {
        originalPosition = transform.localPosition;
        anim = GetComponent<Animator>();
        _AudioSource = GetComponent<AudioSource>();
        _BulletsInMagazin = magazin;
    }

    void Update()
    {
        switch (shootingMode)
        {
            case ShootMode.Auto:
                shootInput = Input.GetButton("Fire1");
            break;
            case ShootMode.Semi:
                shootInput = Input.GetButtonDown("Fire1");
            break;
        }

        if (shootInput) fire();

        if (Input.GetKeyDown(reloadKey)) Reload();
        
        if (fireTimer < fireRate) 
            fireTimer += Time.deltaTime;

        AimDownSights();
    }


    void FixedUpdate()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
    }

    private void AimDownSights()
    {
        if (Input.GetButton("Fire2") && !isReloading)
            transform.localPosition = Vector3.Lerp(transform.localPosition, amingPosition, Time.deltaTime * aodSpeed);
        else
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * aodSpeed);
    }

    public void fire()
    {
        if (fireTimer < fireRate) return;
        fireTimer = 0.0f; // Réinitialisation du le temps de tir
        if (_BulletsInMagazin <= 0)
        {
            PlaySound(emptyMagazinSound, 0.5f);
            return;
        } 
        RaycastHit hit;
        if (Physics.Raycast(shootPoint.position, shootPoint.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name + " hit");
            GameObject hitParticleEffect = Instantiate(hitParticles, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));

            GameObject bulletHole = Instantiate(bulletImpact, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));

            Destroy(hitParticleEffect, 1f);
            Destroy(bulletImpact, 1f);
            if (hit.transform.GetComponent<HealthController>())
            {
                hit.transform.GetComponent<HealthController>().applyDamage(damage);
            }
        }
        anim.CrossFadeInFixedTime("Fire", 0.01f); //assigner true au parametre Fire de l'animator --> Déclenche l'animation
        muzzleFlash.Play();
        SparksEmitter.Play();
        SparksEmitter_1.Play();
        PlaySound(shootSound, 1f);
        _BulletsInMagazin--;
    }

    private void Reload()
    {
        if (bulletsleft <= 0) return;
        int bulletsToLoad = magazin - _BulletsInMagazin;
        int bulletToDeduct = (bulletsleft >= bulletsToLoad) ? bulletsToLoad : bulletsleft;
        bulletsleft -= bulletToDeduct;
        _BulletsInMagazin += bulletToDeduct;
    }

    private void PlaySound(AudioClip sound, float volume)
    {
        _AudioSource.clip = sound;
        _AudioSource.volume = volume;
        _AudioSource.Play();
    }
}
