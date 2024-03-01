using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Weapon/Gun")]
public class GunData : ScriptableObject
{
    [Header("Info")]
    public new string name;

    [Header("Shooting")]
    public float damage;
    public float maxDistance;

    [Header("Ammo")]
    public int magazineSize;
    public float fireRate;
    public float reloadTime;

    [Header("Effect")]
    public GameObject Vfx;
    public GameObject muzzleVFX;


}
