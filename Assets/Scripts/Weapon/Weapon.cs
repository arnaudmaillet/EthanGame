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
    public float damage = 20f;
    [SerializeField] private int _BulletsInMagazin;
    public int bulletsleft = 200;
    public KeyCode reloadKey;

    public enum ShootMode {Auto, Semi};
    public ShootMode shootingMode;

    [Header("Bullets particules")]
    public GameObject hitParticles;
    public GameObject bulletImpact;

    [Header("Animation")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem SparksEmitter;
    public ParticleSystem SparksEmitter_1;

    [Header("Sound")]
    public AudioClip shootSound;

    private Animator anim;
    private bool shootInput;
    private AudioSource _AudioSource;
    float fireTimer; // Temps entre les clicks gauche de la souris

    void Start()
    {
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

        if (shootInput) 
        {
            if (_BulletsInMagazin > 0) fire();
        }

        if (Input.GetKeyDown(reloadKey)) Reload();
        
        if (fireTimer < fireRate) 
            fireTimer += Time.deltaTime;
    }


    void FixedUpdate()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
    }

    public void fire()
    {
        if (fireTimer < fireRate || _BulletsInMagazin <= 0) return;
        RaycastHit hit;
        if (Physics.Raycast(shootPoint.position, shootPoint.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name + " hit");
            GameObject hitParticleEffect = Instantiate(hitParticles, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            hitParticleEffect.transform.SetParent(hit.transform);

            GameObject bulletHole = Instantiate(bulletImpact, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
            bulletHole.transform.SetParent(hit.transform);

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
        PlayShootSound();
        _BulletsInMagazin--;
        fireTimer = 0.0f;   
    }

    private void Reload()
    {
        if (bulletsleft <= 0) return;
        int bulletsToLoad = magazin - _BulletsInMagazin;
        int bulletToDeduct = (bulletsleft >= bulletsToLoad) ? bulletsToLoad : bulletsleft;
        bulletsleft -= bulletToDeduct;
        _BulletsInMagazin += bulletToDeduct;
    }

    private void PlayShootSound()
    {
        _AudioSource.clip = shootSound;
        _AudioSource.Play();
    }
}
