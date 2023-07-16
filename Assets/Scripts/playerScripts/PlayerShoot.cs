using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    // [SerializeField] private float bulletSpeed;
   // [SerializeField] private float bulletDistance;

    // reference to the shootingpoint that is a child of the weapon
    [SerializeField] private Transform shootingPoint;

    private PlayerState playerState;

    // reference to the particle system
    [SerializeField] private ParticleSystem muzzleFlash;


    


    // start it with null 
    private void Awake()
    {
        // shootingPoint = null;
        // muzzleFlash = null;
        playerState = GetComponent<PlayerState>();

    }

    public void Shoot()
    {
        FireServerRpc();
    }
    
    [ServerRpc]
    public void FireServerRpc() 
    {

        NetworkObject bullet = Instantiate(bulletPrefab,shootingPoint.position, shootingPoint.rotation).GetComponent<NetworkObject>();
        bullet.Spawn();

        bullet.GetComponent<BulletState>().BulletDirection.Value = shootingPoint.forward;

        FireClientRpc();
    }
  
    [ClientRpc]
    public void FireClientRpc()
    {    

        muzzleFlash = shootingPoint.GetComponentInChildren<ParticleSystem>();
        muzzleFlash.Play();
    
    }

    
    

    // IEnumerator DestroyBulletAfterTime(NetworkObject bullet)
    // {
    //    yield return new WaitForSeconds(2);
    
    // // Ensure we're on the server before attempting to despawn the bullet
    //    if (IsServer && bullet != null && NetworkManager.Singleton.SpawnManager.SpawnedObjects.ContainsKey(bullet.NetworkObjectId))
    //     {
    //         bullet.Despawn(true);
    //     }
    // }




}