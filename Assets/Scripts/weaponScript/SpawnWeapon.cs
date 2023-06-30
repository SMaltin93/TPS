using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// netcode
using Unity.Netcode;

public class SpawnWeapon : MonoBehaviour
{

 // refernce to the weapon prefab 

 [SerializeField] private GameObject weaponPrefab;


    void Start() {
        
            // spawn the weapon
        NetworkObject weapon = Instantiate(weaponPrefab, transform.position, transform.rotation).GetComponent<NetworkObject>();

    }





 

}
