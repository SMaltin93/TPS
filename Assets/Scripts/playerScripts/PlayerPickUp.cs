using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using UnityEngine.Animations.Rigging;


public class PlayerPickUp : NetworkBehaviour
{

    [SerializeField] private LayerMask pickUpLayer;

    // // Reference to the GrabbedWeapon holder
    // anim refernce 
    private Animator anim;
    private bool isGrabbed = false;
    // // Reference to the find GrabbedWeapon point
    private Transform findWeapon;
    // refernce to the "Tow Bone IK Constraint" as acomponent of the player
    // player id
    private GameObject GrabbedWeapon;

    // grabbed weapon orgina postion and rotation
    private  Vector3 GrabbedWeaponOrgPos;
    private Quaternion GrabbedWeaponOrgRot;
    // for sync animation of the body
    private Transform body;
    private Vector3 bodyOldPos;
    

    void Awake()
    {
        
        findWeapon = transform.Find("findWeapon"); //mixamorig:Hips
        // find body with tag "body"
        body = GameObject.FindWithTag("body").transform;
        GrabbedWeapon = null;
        anim = GetComponent<Animator>();
        anim.SetBool("isGrabbed", false);
        // GrabbedWeaponOrgPos constatnt

        GrabbedWeaponOrgPos = new Vector3(0.12f, 1.5161f, 0.3601f);
        GrabbedWeaponOrgRot = Quaternion.Euler(0, -90, 0);


    }
   

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (Input.GetKey(KeyCode.E) && GrabbedWeapon == null)
        {
            PickUpWeapon();
        }

        // uppdate the GrabbedWeapon position as the body depend on owen position
        if (GrabbedWeapon != null)
        {
            // body magitute
            Vector3 animationBreathing = body.localPosition - bodyOldPos;
            GrabbedWeapon.transform.localPosition = Vector3.Lerp(GrabbedWeaponOrgPos, GrabbedWeaponOrgPos + animationBreathing, 0.1f);   
        }
    }

    private void PickUpWeapon()
    {
        float pickUpRange = 4f;
        if (Physics.Raycast(findWeapon.position, findWeapon.forward, out RaycastHit hit, pickUpRange))
        {
            Debug.Log("tag: " + hit.collider.gameObject.tag);
            // findgameobject with tag GrabbedWeapon
             if (hit.transform.tag == "Weapon")
            {
                isGrabbed = true;
                anim.SetBool("isGrabbed", isGrabbed);
                // uppdate the animation
                GrabbedWeapon = hit.transform.gameObject;
                bodyOldPos = body.localPosition;
                RequestPickUpWeaponServerRpc(GrabbedWeapon.GetComponent<NetworkObject>().NetworkObjectId);
            } 
        }
    }

    private void Grab(GameObject GrabbedWeapon) {
        // GrabbedWeapon regidbody
        Rigidbody rb = GrabbedWeapon.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        // set the parent of the objectGrabbPoint
        GrabbedWeapon.transform.parent = transform;
        GrabbedWeapon.transform.localPosition = GrabbedWeaponOrgPos;
        GrabbedWeapon.transform.localRotation = GrabbedWeaponOrgRot;

    }

    [ServerRpc]
    private void RequestPickUpWeaponServerRpc(ulong weaponNetId)
    {
        var weaponObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[weaponNetId].gameObject;
       // weaponObj.GetComponent<NetworkObject>().ChangeOwnership(NetworkManager.Singleton.LocalClientId);
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