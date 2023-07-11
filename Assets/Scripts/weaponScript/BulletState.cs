using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;



public class BulletState : NetworkBehaviour
{

    [SerializeField] private float bulletSpeed = 50f;

public NetworkVariable<Vector3> BulletDirection = new NetworkVariable<Vector3>(default,
    NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
       GetComponent<Rigidbody>().velocity = this.transform.forward * bulletSpeed;
    }


    // spawn the bullet on the clients




    // void Start()
    // {
    //     // destroy the bullet after 5 seconds
    //     if (IsServer)
    //     {
    //         Destroy(this.gameObject, 5f);
    //     }
    // }

    // void FixedUpdate()
    // {
    //     if (IsServer)
    //     {
    //         // move the bullet forward
    //         GetComponent<Rigidbody>().velocity = BulletVelocity.Value;
    //     }
    // }

    // private void OnCollisionEnter(Collision other)
    // {
    //     if (IsServer)
    //     {
    //         if (NetworkObject != null && NetworkObject.IsSpawned)
    //         {
    //             NetworkObject.Despawn(true);
    //         }

    //         // The bullet object will be automatically destroyed on the clients when it's despawned on the server
    //         Destroy(this.gameObject);
    //     }

    //     FireClientRpc();
    // }

    // [ClientRpc]
    // public void FireClientRpc()
    // {
    //     Debug.Log("Bullet collided with " + gameObject.name);
    // }
}
