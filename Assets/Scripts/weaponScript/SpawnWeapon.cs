using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// netcode
using Unity.Netcode;

public class SpawnWeapon : NetworkBehaviour
{

 // refernce to the weapon prefab

    [SerializeField] private GameObject weaponPrefab;

    [SerializeField] private int planXstart;
    [SerializeField] private int planZstart;
    [SerializeField] private int HeightFallFrom;
    [SerializeField] private int numOfbjects;

    // list to store the spawned objects NetworkIdentities

    private List<NetworkObject> spawnedObjects = new List<NetworkObject>();


    public void SpawnedObjects()
    {
        if(IsServer)
        {
            for(int i = 0; i < numOfbjects; i++)
            {
               NetworkObject weapon = Instantiate(weaponPrefab, GetRandomPosition(), Quaternion.identity).GetComponent<NetworkObject>();
               spawnedObjects.Add(weapon);
               weapon.Spawn();
            }
        }
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-planXstart, planXstart), HeightFallFrom, Random.Range(-planZstart, planZstart));
    }

}
