using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BulletState : NetworkBehaviour
{
    void Start()
    {
        // destroy the bullet after 5 seconds
        if (IsServer) {
            Destroy(this.gameObject, 5f);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // destroy the bullet if it collides with something 
        Debug.Log("Bullet collided with " + other.gameObject.name);
        if (IsLocalPlayer) {
            RequestDespawnServerRpc(NetworkObject.NetworkObjectId);
        }
    }

    [ServerRpc]
    public void RequestDespawnServerRpc(ulong networkObjectId, ServerRpcParams rpcParams = default)
    {
        NetworkObject networkObject;
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out networkObject))
        {
            networkObject.Despawn(true);
        }
    }
}