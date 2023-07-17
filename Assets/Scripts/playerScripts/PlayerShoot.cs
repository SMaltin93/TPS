using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float ShootRate = 2f;

    // reference to the shootingpoint that is a child of the weapon
    [SerializeField] private Transform shootingPoint;

    private PlayerState playerState;

    // reference to the particle system
    [SerializeField] private ParticleSystem muzzleFlash;


    


    // // start it with null 
    // private void Start()
    // {
    //     // shootingPoint = null;
    //     // muzzleFlash = null;
    //     playerState = GetComponent<PlayerState>();



    // }

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

        StartCoroutine(DestroyBullet(bullet));

        FireClientRpc();
    }
  
    [ClientRpc]
    public void FireClientRpc()
    {    

        muzzleFlash = shootingPoint.GetComponentInChildren<ParticleSystem>();
        muzzleFlash.Play();
    
    }

    IEnumerator DestroyBullet(NetworkObject bullet)
    {
        yield return new WaitForSeconds(ShootRate);
        if (IsServer && bullet != null && NetworkManager.Singleton.SpawnManager.SpawnedObjects.ContainsKey(bullet.NetworkObjectId))
        {
            bullet.Despawn(true);
        }
    }



}