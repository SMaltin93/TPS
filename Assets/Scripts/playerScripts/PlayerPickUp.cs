using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;


public class PlayerPickUp : MonoBehaviour
{

    [SerializeField] private Transform findWeapon;
    [SerializeField] private LayerMask pickUpLayer;
    [SerializeField] private Transform weaponHolder;

   

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            float pickUpRange = 2f;
            if (Physics.Raycast(findWeapon.position, findWeapon.forward, out hit, pickUpRange))
            {

                if (hit.transform.TryGetComponent(out ObjectGrabb objectGrabb))
                {
                    GameObject weapon = hit.transform.gameObject;
                    // set the player as the parent of the weapon
                    weapon.transform.SetParent(this.transform);
                
                    // grab the weapon
                    objectGrabb.Grab(weaponHolder);
                    
                }
                
            }
        }
    }
}
