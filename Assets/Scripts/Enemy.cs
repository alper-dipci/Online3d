using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;
using System;
using Unity.Mathematics;

public class Enemy : NetworkBehaviour, IDamagable
{
    private GameObject BaseGameObject;
    private NavMeshAgent agent;
    private Vector3 LastTarget;
    private Vector3 Target;
    private float currentHealth;
    private float maxHealth = 100f;
    private float damage = 10f;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private LayerMask hittableLayer;
    [SerializeField] GameObject explosionEffect;

    [SerializeField] private float playerDetectionRange;
    [SerializeField] private float HitRange;
    [SerializeField] private float explosionRange;


    Collider[] hitCollider;
    Collider[] attackCollider;
    public override void OnNetworkSpawn()
    {
        currentHealth = maxHealth;
        BaseGameObject = GameObject.FindGameObjectWithTag("Base");
        agent = GetComponent<NavMeshAgent>();
        base.OnNetworkSpawn();

    }
    void Update()
    {
        if (!IsServer) return;
        hitCollider = Physics.OverlapSphere(transform.position, playerDetectionRange, hittableLayer);

        if (hitCollider.Length > 0)
            Target = hitCollider[0].transform.position;
        else
            Target = BaseGameObject.transform.position;

        if (LastTarget == null) LastTarget = Target;
        if (LastTarget != Target)
            SetAgentTargetClientRpc(Target);
        LastTarget = Target;
        if ((Target - transform.position).magnitude <= HitRange)
        {

            explode();
        }
    }
    [ClientRpc]
    private void HitClientRpc(ulong NetworkObjectId, float damage)
    {
        NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(NetworkObjectId, out NetworkObject networkObject);
        networkObject.gameObject.TryGetComponent<IDamagable>(out IDamagable damagable);
        damagable.getHit(damage);
    }
    [ClientRpc]
    private void SetAgentTargetClientRpc(Vector3 target)
    {
        agent.SetDestination(target);
    }

    public void getHit(float amount)
    {
        currentHealth -= amount;
        healthBar.updateHealthBar(currentHealth, maxHealth);
        if (currentHealth <= 0 && IsServer)
            NetworkObject.Despawn();
    }
    private void explode()
    {
        attackCollider = Physics.OverlapSphere(transform.position, explosionRange, hittableLayer);
        foreach (Collider hittable in attackCollider)
        {
            ulong hittableNetworkid = hittable.GetComponent<IDamagable>().getNetworkObjectId();
            HitClientRpc(hittableNetworkid, damage);
        }
        //instantiate particles
        GameObject explosion = Instantiate(explosionEffect, transform.position, quaternion.identity);
        explosion.GetComponent<NetworkObject>().Spawn();

        NetworkObject.Despawn();
    }

    public ulong getNetworkObjectId()
    {
        return NetworkObjectId;
    }
}
