using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class Gun : NetworkBehaviour
{
    [SerializeField] GunData gunData;
    [SerializeField] ParticleSystem muzzleVFX;
    [SerializeField] GameObject Light;

    private int currentAmmo;
    private float timeSinceLastShot;
    private Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
    private bool isReloading = false;
    private bool canShoot() => !isReloading && timeSinceLastShot > 1f / (gunData.fireRate / 60f) && currentAmmo > 0;

    //JUST FOR TESTING
    [SerializeField] Transform gunTransformPoint;
    private void OnEnable()
    {
        currentAmmo = gunData.magazineSize;
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        StarterAssets.StarterAssetsInputs.reloadInput += startReload;
        base.OnNetworkSpawn();
    }
    public override void OnNetworkDespawn()
    {
        StarterAssets.StarterAssetsInputs.reloadInput -= startReload;
        base.OnNetworkDespawn();
    }
    private void Update()
    {
        if (!muzzleVFX.isPlaying)
        {
            Light.SetActive(false);
        }
        if (!IsOwner) return;
        timeSinceLastShot += Time.deltaTime;

    }
    public void startReload()
    {
        if (isReloading) return;
        StartCoroutine(reloadCoroutine());
    }
    private IEnumerator reloadCoroutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(gunData.reloadTime);
        isReloading = false;
        currentAmmo = gunData.magazineSize;
    }
    public void Shoot()
    {
        if (canShoot())
        {
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            Vector3 shootPoint;
            if (Physics.Raycast(ray, out RaycastHit raycastHit, gunData.maxDistance))
            {
                shootPoint = raycastHit.point;
                if (raycastHit.transform.TryGetComponent<IDamagable>(out IDamagable damagable))
                {
                    ulong NetworkObjectId = damagable.getNetworkObjectId();
                    HitServerRpc(NetworkObjectId, gunData.damage);
                }
            }
            else
                shootPoint = ray.GetPoint(10);
            if (Physics.Raycast(gunTransformPoint.position, shootPoint - gunTransformPoint.position, out RaycastHit hitInfo, 1f))
                shootPoint = hitInfo.point;
            if (IsServer)
                shootActions(shootPoint);
            else
                shootActionsServerRpc(shootPoint);
            currentAmmo--;
            timeSinceLastShot = 0;
        }
    }
    private void shootActions(Vector3 shootPoint)
    {
        GameObject GunVFX = Instantiate(gunData.Vfx, shootPoint, Quaternion.identity);
        GunVFX.GetComponent<NetworkObject>().Spawn();
        muzzleVFXClientRPC();
    }
    [ClientRpc]
    private void muzzleVFXClientRPC()
    {
        muzzleVFX.gameObject.SetActive(true);
        Light.SetActive(true);
        muzzleVFX.Play();
    }
    [ServerRpc]
    private void shootActionsServerRpc(Vector3 shootPoint)
    {
        shootActions(shootPoint);
    }
    [ServerRpc]
    private void HitServerRpc(ulong NetworkObjectId, float damage)
    {
        HitClientRpc(NetworkObjectId, damage);
    }
    [ClientRpc]
    private void HitClientRpc(ulong NetworkObjectId, float damage)
    {
        NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(NetworkObjectId, out NetworkObject networkObject);
        networkObject.gameObject.TryGetComponent<IDamagable>(out IDamagable damagable);
        damagable.getHit(damage);
    }


}
