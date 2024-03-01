using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Base : NetworkBehaviour, IDamagable
{
    public float maxHealth;
    public float currentHealth;
    [SerializeField] HealthBar healthBar;
    public override void OnNetworkSpawn()
    {
        currentHealth = maxHealth;
        base.OnNetworkSpawn();
    }
    public void getHit(float amount)
    {
        currentHealth -= amount;
        healthBar.updateHealthBar(currentHealth, maxHealth);
    }

    public ulong getNetworkObjectId()
    {
        return NetworkObjectId;
    }
}
