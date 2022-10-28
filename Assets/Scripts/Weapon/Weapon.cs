using Knife.ScifiEffects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon")]
    public GameObject arms;
    public Transform shootPoint;
    public float range = 100f;
    public int magazin = 5;
    public float fireRate = 0.5f;
    public float damage = 20f;
    public int bulletsInMagazin;
    public int bulletsleft = 1;
    public KeyCode reloadKey;

    public enum ShootMode {Auto, Semi};
    public ShootMode shootingMode;

    [Header("projectile")]
    public GameObject[] projectiles;
    public float projectileSpeed = 30f;
    private Vector3 Destination;
    public float ProjectileTimeAnimation;

    [Header("Bullets particules")]
    [SerializeField] private ParticleGroupEmitter[] shotEmitters;
    public GameObject hitParticles;
    public GameObject bulletImpact;

    [Header("Aiming")]
    private Vector3 armsOriginalPosition;
    private Vector3 originalPosition;
    public Vector3 amingPosition;
    public float aodSpeed = 8f;

    [Header("Animations")]
    public GameObject arm;
    public GameObject shoulder;
    private Quaternion originalArmRotation;
    private Quaternion originalShoulderRotation;
    public Quaternion armRotation;
    public Quaternion shoulderRotation;
    public float ReloadTimeAnimation;

    [Header("Sound")]
    public AudioClip shootSound;
    public AudioClip emptyMagazinSound;
    private Animator anim;
    private bool shootInput;
    private bool isReloading = false;
    public bool isShooting = false;
    private AudioSource _AudioSource;
    public float fireTimer; // Temps entre les clicks gauche de la souris

    void Start()
    {
        armsOriginalPosition = arms.transform.localPosition;
        originalPosition = transform.localPosition;
        originalArmRotation = arm.transform.localRotation;
        originalShoulderRotation= shoulder.transform.localRotation;
        anim = GetComponent<Animator>();
        _AudioSource = GetComponent<AudioSource>();
        bulletsInMagazin = magazin;
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
        {
            arms.transform.localPosition = Vector3.Lerp(arms.transform.localPosition, amingPosition, Time.deltaTime * aodSpeed);
            // transform.localPosition = Vector3.Lerp(transform.localPosition, amingPosition, Time.deltaTime * aodSpeed);
        }
        else
        {
            arms.transform.localPosition = Vector3.Lerp(arms.transform.localPosition, armsOriginalPosition, Time.deltaTime * aodSpeed);
            // transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * aodSpeed);
        }
    }

    public void fire()
    {
        if (fireTimer < fireRate) return;
        fireTimer = 0.0f; // Réinitialisation du le temps de tir
        if (bulletsInMagazin <= 0)
        {
            PlaySound(emptyMagazinSound, 0.5f);
            return;
        } 
        RaycastHit hit;
        if (Physics.Raycast(shootPoint.position, shootPoint.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name + " hit");
            if (hitParticles)
            {
                GameObject hitParticleEffect = Instantiate(hitParticles, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                Destroy(hitParticleEffect, 0.1f);
            }
            
            // GameObject bulletHole = Instantiate(bulletImpact, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
            PlaySound(shootSound, 1f);
            // Destroy(bulletImpact, 1f);
            if (hit.transform.GetComponent<HealthController>())
            {
                hit.transform.GetComponent<HealthController>().applyDamage(damage);
            }
        }
        StartCoroutine(ShootProjectiles(ProjectileTimeAnimation));
        anim.CrossFadeInFixedTime("Fire", 0.01f); //assigner true au parametre Fire de l'animator --> Déclenche l'animation
        PlaySound(shootSound, 100f);
        PlayFX();
        bulletsInMagazin--;
        isShooting = true;
    }

    private void Reload()
    {
        if (bulletsleft <= 0) return;
        StartCoroutine(Reloading(ReloadTimeAnimation));
        arm.transform.localRotation = Quaternion.Lerp(arm.transform.localRotation, originalArmRotation, Time.deltaTime * aodSpeed);
        shoulder.transform.localRotation = Quaternion.Lerp(shoulder.transform.localRotation, originalShoulderRotation, Time.deltaTime * aodSpeed);
    }

    private void PlayFX()
    {
        if (shotEmitters != null)
        {
            foreach (var e in shotEmitters)
                e.Emit(1);
        }
    }

    private void PlaySound(AudioClip sound, float volume)
    {
        _AudioSource.clip = sound;
        _AudioSource.volume = volume;
        _AudioSource.Play();
    }
    IEnumerator ShootProjectiles(float timeToWait) {
        foreach(var projectile in projectiles)
        {
            Instantiate(projectile, shootPoint.position, shootPoint.rotation);
            yield return new WaitForSeconds(timeToWait);
        }
    }

    IEnumerator Reloading(float timeToWait)
    {
        int bulletsToLoad = magazin - bulletsInMagazin;
        int bulletToDeduct = (bulletsleft >= bulletsToLoad) ? bulletsToLoad : bulletsleft;
        bulletsleft -= bulletToDeduct;
        bulletsInMagazin += bulletToDeduct;
        arm.transform.localRotation = Quaternion.Lerp(arm.transform.localRotation, armRotation, Time.deltaTime * aodSpeed);
        shoulder.transform.localRotation = Quaternion.Lerp(shoulder.transform.localRotation, shoulderRotation, Time.deltaTime * aodSpeed);
        yield return new WaitForSeconds(timeToWait);
    }
}
