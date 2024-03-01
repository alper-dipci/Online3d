using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TrainingTarget : NetworkBehaviour,IDamagable
{
    public void getHit(float amount)
    {

    }

    public ulong getNetworkObjectId()
    {
        return NetworkObjectId;
    }

}
