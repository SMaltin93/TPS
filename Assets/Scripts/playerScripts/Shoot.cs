using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Shoot : NetworkBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletDistance;

    void Update()
    {
        if (!IsOwner) return;
        if (Input.GetMouseButtonDown(0) && PlayerPickUp.isGrabbed) 
        {
            RequiresFireServerRpc();
        }
    }

    public void Fire() 
    {
        GameObject bullet =  Instantiate(bulletPrefab, transform.position, transform.rotation);
        bullet.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
        Destroy(bullet, bulletDistance);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequiresFireServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ExecuteFireClientRpc();
    }

    [ClientRpc]
    private void ExecuteFireClientRpc()
    {
        Fire();
    }
}