using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;
using Unity.Mathematics;

public class EnemySpawnManager : NetworkBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform enemySpawnPoint;
    float timer;
    private void Update()
    {
        if (!IsServer) return;

        timer += Time.deltaTime;
        if (timer > 5f)
        {
            timer = 0;
            GameObject enemy = Instantiate(enemyPrefab, enemySpawnPoint.position, quaternion.identity);
            enemy.GetComponent<NetworkObject>().Spawn();
        }

    }
}
