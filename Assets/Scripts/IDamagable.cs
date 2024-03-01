using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public interface IDamagable 
{
    public void getHit(float amount);
    public ulong getNetworkObjectId();
}
