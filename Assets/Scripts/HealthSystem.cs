using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    private int health;
    private int healthMax;

    public event EventHandler OnDead;
    public event EventHandler OnDamaged;

    private void Start()
    {
        healthMax = GetComponent<UnitInfo>().GetHP();
        health = healthMax;
    }

    public void TakeDamage(int damageAmount)
    {
        health = Mathf.Max(0, health - damageAmount);
        OnDamaged?.Invoke(this, EventArgs.Empty);
        
        if (health == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDead?.Invoke(this, EventArgs.Empty);
    }

    public int GetHealth()
    {
        return health;
    }

    public float GetHealthNormalized()
    {
        return (float)health / healthMax;
    }
}
