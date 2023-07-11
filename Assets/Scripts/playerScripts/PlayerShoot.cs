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
    private Transform shootingPoint;

    private PlayerPickUp playerPickUp;

    // reference to the particle system
    private ParticleSystem muzzleFlash;


    


    // start it with null 
    private void Awake()
    {
        shootingPoint = null;
        muzzleFlash = null;
        playerPickUp = GetComponent<PlayerPickUp>();

    }

    // Update is called once per frame
    void Update()
    {

        if (!IsOwner) return;
    
        if (Input.GetMouseButtonDown(0) && playerPickUp.GrabbedWeapon.Value != 0) {
            FireServerRpc(playerPickUp.GrabbedWeapon.Value);
        } 
        
    }
    
    [ServerRpc]
    public void FireServerRpc(ulong weaponId) 
    {
        Debug.Log("FireServerRpc");
        // Assign shootingPoint and muzzleFlash for the specific weapon
        NetworkObject grabbedWeapon = NetworkManager.Singleton.SpawnManager.SpawnedObjects[weaponId];
        GameObject weapon = grabbedWeapon.gameObject;
        shootingPoint = weapon.transform.Find("sniperBody").Find("shootingPoint");
        muzzleFlash = shootingPoint.GetComponentInChildren<ParticleSystem>();

        NetworkObject bullet = Instantiate(bulletPrefab,shootingPoint.position, shootingPoint.rotation).GetComponent<NetworkObject>();
        bullet.Spawn();

        bullet.GetComponent<BulletState>().BulletDirection.Value = shootingPoint.forward;

        FireClientRpc();
    }
  
    [ClientRpc]
    public void FireClientRpc()
    {    
        NetworkObject grabbedWeapon = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerPickUp.GrabbedWeapon.Value];
        GameObject weapon = grabbedWeapon.gameObject;
        shootingPoint = weapon.transform.Find("sniperBody").Find("shootingPoint");
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