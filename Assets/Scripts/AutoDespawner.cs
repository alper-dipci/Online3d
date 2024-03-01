using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AutoDespawner : NetworkBehaviour
{
    public float despawnTimer;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;
        StartCoroutine(DespawnSelfCoroutine());
    }
    private IEnumerator DespawnSelfCoroutine()
    {
        yield return new WaitForSeconds(despawnTimer);
        destroySelfServerRpc();
    }
    [ServerRpc]
    private void destroySelfServerRpc() {
        GetComponent<NetworkObject>().Despawn();
    }
}
