using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Animator anim;
    public Transform shootPoint;
    public float range = 100f;
    public int magazin = 31;
    public float fireRate = 0.5f;
    float fireTimer; // Temps entre les clicks gauche de la souris
    public int bulletsInMagazin;
    public int bulletLeft;

    void Start()
    {
        anim = GetComponent<Animator>();
        bulletsInMagazin = magazin;
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
        if (info.IsName("Fire")) anim.SetBool("Fire", false);
    }

    public void fire()
    {
        if (fireTimer < fireRate) return;
        RaycastHit hit;
        if (Physics.Raycast(shootPoint.position, shootPoint.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name + " hit");
        }
        anim.CrossFade("Fire", 0.01f); //assigner true au parametre Fire de l'animator --> Déclenche l'animation
        bulletsInMagazin--;
        fireTimer = 0.0f;   
    }
}
