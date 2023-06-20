using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletDistance;

    // reference to the shootingpoint that is a child of the weapon
    private Transform shootingPoint;

    // reference to the particle system
    private ParticleSystem muzzleFlash;

    // start it with null 
    private void Awake()
    {
        shootingPoint = null;
        muzzleFlash = null;
    }

    // Update is called once per frame
    void Update()
    {
       if (shootingPoint == null)
        {
            foreach (Transform child in transform)
            {
                if (child.tag == "Weapon")
                {
                    shootingPoint = child.Find("shootingPoint");
                    // shootingPoint has particle system as child
                    muzzleFlash = shootingPoint.GetComponentInChildren<ParticleSystem>();
                    break;  // Stop the loop as we found the shooting point
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && shootingPoint != null && IsLocalPlayer)
        {
            FireServerRpc();
            // start muzzle flash
            // start muzzle flash on local player only
           // if (IsLocalPlayer)
           // {
           // }
        }
    }

    // public void Fire() 
    // {
    //     GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
    //     bullet.GetComponent<NetworkObject>().Spawn();
    //     bullet.GetComponent<Rigidbody>().velocity = shootingPoint.forward * bulletSpeed;
    //     //StartCoroutine(DestroyBulletAfterTime(bullet));

    // }

    
    [ServerRpc]
    public void FireServerRpc(ServerRpcParams rpcParams = default)
    {
        Debug.Log("FireServerRpc");
        // Instantiate and spawn the bullet on the server
        GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
        bullet.GetComponent<NetworkObject>().Spawn();

        bullet.GetComponent<Rigidbody>().velocity = shootingPoint.forward * bulletSpeed;
        //StartCoroutine(DestroyBulletAfterTime(bullet));
        FireClientRpc();
    }

    [ClientRpc]
    public void FireClientRpc()
    {
        
        muzzleFlash.Play();

       
    }

    IEnumerator DestroyBulletAfterTime(NetworkObject bullet)
    {
        yield return new WaitForSeconds(bulletDistance);
        RequestDespawnServerRpc(bullet.NetworkObjectId);
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