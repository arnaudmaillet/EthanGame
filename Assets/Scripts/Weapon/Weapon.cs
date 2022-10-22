using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform shootPoint;
    public float range = 100f;
    public int magazin = 31;
    public int bulletLeft;
    public float fireRate = 0.5f;
    float fireTimer; // Temps entre les clicks gauche de la souris

    public void fire()
    {
        if (fireTimer < fireRate) return;
        RaycastHit hit;
        if (Physics.Raycast(shootPoint.position, shootPoint.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name + " hit");
        }
        fireTimer = 0.0f;   
    }

    void Update()
    {
        if (Input.GetButton("Fire1")) fire();
        if (fireTimer < fireRate) 
            fireTimer += Time.deltaTime;
    }
}
