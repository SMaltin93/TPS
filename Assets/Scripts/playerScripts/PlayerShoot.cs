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

    // reference to the particle system
    [SerializeField] private ParticleSystem muzzleFlash;


    [SerializeField] private AudioClip _shoot;
    [SerializeField] private AudioSource _audioSource;


    private PlayerSound _playerSound;


    public void Shoot()
    {
        FireServerRpc();
    }

    private void Start()
    {
        _playerSound = GetComponent<PlayerSound>();
    }
    
    [ServerRpc]
    public void FireServerRpc() 
    {

        NetworkObject bullet = Instantiate(bulletPrefab,shootingPoint.position, shootingPoint.rotation).GetComponent<NetworkObject>();
        bullet.Spawn();
        bullet.GetComponent<BulletState>().BulletDirection.Value = shootingPoint.forward;

        // if it exists, destroy the bullet after 2 seconds
        if (bullet != null)
        {
            StartCoroutine(DestroyBullet(bullet));
        }

        FireClientRpc();
    }
  
    [ClientRpc]
    public void FireClientRpc()
    {    

        muzzleFlash = shootingPoint.GetComponentInChildren<ParticleSystem>();
        muzzleFlash.Play();
        playerShootSound();

    
    }

    IEnumerator DestroyBullet(NetworkObject bullet)
    {
        if (bullet == null) yield break;
        yield return new WaitForSeconds(ShootRate);
        if (IsServer && bullet != null && NetworkManager.Singleton.SpawnManager.SpawnedObjects.ContainsKey(bullet.NetworkObjectId))
        {
            bullet.Despawn(true);
        }
    }


    // do the shoot sound just on the local player
    private void playerShootSound()
    {
       
        this._audioSource.PlayOneShot(_shoot);
        
    }




}