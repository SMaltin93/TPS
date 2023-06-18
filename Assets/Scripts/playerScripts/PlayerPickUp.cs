using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;


public class PlayerPickUp : NetworkBehaviour
{

    [SerializeField] private LayerMask pickUpLayer;

    // // Reference to the weapon holder
    private Transform weaponHolder;
    // // Reference to the find weapon point
    private Transform findWeapon;
    public static bool isGrabbed = false;
    // player id
    private GameObject weapon;
    

    void Start()
    {
        findWeapon = transform.Find("findWeapon");
        weapon = null;
    }
   

    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKey(KeyCode.E) && weapon == null)
        {
            //StartCoroutine(ToggleLagIndicator());
            PickUpWeapon();
        }
    }

    private void PickUpWeapon()
    {
        float pickUpRange = 4f;
        if (Physics.Raycast(findWeapon.position, findWeapon.forward, out RaycastHit hit, pickUpRange))
        {
            Debug.Log("tag: " + hit.collider.gameObject.tag);
            // findgameobject with tag weapon
             if (hit.transform.tag == "Weapon")
            {
                weapon = hit.transform.gameObject;
                RequestPickUpWeaponServerRpc(weapon.GetComponent<NetworkObject>().NetworkObjectId);
               // pickUpWeaponClientRpc(weapon.GetComponent<NetworkObject>().NetworkObjectId);

            } 
        }
    }

    private void Grab(GameObject weapon) {
        isGrabbed = true;
        // weapon regidbody
        Rigidbody rb = weapon.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        // set the parent of the objectGrabbPoint
        weapon.transform.parent = transform;
        // set the objectGrabbPoint to the grabbPointPosition
        // set x to 0.13, y = 1.534 , z =  0.472, rotate in y -90 
        weapon.transform.localPosition = new Vector3(0.13f, 1.534f, 0.472f);
        weapon.transform.localRotation = Quaternion.Euler(0, -90, 0);
        // weapon.transform.position = weaponHolder.position;
        // // set the rotation of the objectGrabbPoint
        // weapon.transform.rotation = weaponHolder.rotation;
        // // make the parent of the weapon the owner of the weapon
    }

    [ServerRpc]
    private void RequestPickUpWeaponServerRpc(ulong weaponNetId)
    {
        var weaponObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[weaponNetId].gameObject;
        weaponObj.GetComponent<NetworkObject>().ChangeOwnership(NetworkManager.Singleton.LocalClientId);
        // Now the server will handle reparenting the object
        Grab(weaponObj);
        pickUpWeaponClientRpc(weaponNetId);
    
    }

    [ClientRpc]
    private void pickUpWeaponClientRpc (ulong weaponNetId)
    {   
        var weaponObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[weaponNetId].gameObject;
        if (weaponObj == null) return;
    }



   
}