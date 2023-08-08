using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// netcode
using Unity.Netcode;

public class SpawnObject : NetworkBehaviour
{

 // refernce to the objectToSpawn prefab

    [SerializeField] private GameObject objectPrefab;

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
               NetworkObject objectToSpawn = Instantiate(objectPrefab, GetRandomPosition(), Quaternion.identity).GetComponent<NetworkObject>();
               spawnedObjects.Add(objectToSpawn);
               objectToSpawn.Spawn();
            }
        }
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-planXstart, planXstart), HeightFallFrom, Random.Range(-planZstart, planZstart));
    }
    

}
