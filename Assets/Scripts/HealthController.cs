using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    public void applyDamage(float damage)
    {
        health -= damage;
        if (health <= 0f) Destroy(gameObject);
    }
}
