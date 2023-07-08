using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using UnityEngine.Animations.Rigging;


public class PlayerPickUp : NetworkBehaviour
{

    [SerializeField] private LayerMask pickUpLayer;
    [SerializeField] private Transform rightHandHoldPosition;



    
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
    private Transform SniperBody;
    // for sync animation of the body

    // refernce to the target two bone ik constraint
    private TwoBoneIKConstraint twoBoneIKConstraint; 
    // refernce ti Multi_aim constraint of the sniper 
  

    private LayerMask groundLayer;
    private Transform groundTransform ;

    

    void Awake()
    {
        
        findWeapon = transform.Find("findWeapon"); 
        
        GrabbedWeapon = null;
        anim = GetComponent<Animator>();
        anim.SetBool("isGrabbed", false);
        anim.SetBool("isPickingUp", false);

        GrabbedWeaponOrgPos = new Vector3(1.164f, 0.282f, 0.537f);
        GrabbedWeaponOrgRot = Quaternion.Euler(0, -90, 0);

        // ground layer
        groundLayer = LayerMask.GetMask("Ground");
        groundTransform = null;
        SniperBody = null;
        // two bone ik constraint which is child in Rig/leftHandIK ;
    }
   

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (Input.GetKey(KeyCode.E) && GrabbedWeapon == null)
        {
            PickUpWeapon();
            
        }

        if (isGrabbed) {
            if ( DistanceBeforeGrab (rightHandHoldPosition, 0.3f) ) {
                RequestPickUpWeaponServerRpc(GrabbedWeapon.GetComponent<NetworkObject>().NetworkObjectId);
                isGrabbed = !isGrabbed;
            }
        }
        if (GrabbedWeapon != null && !isGrabbed) {
            FollowTheHand();
        }
          Debug.DrawRay(rightHandHoldPosition.position, rightHandHoldPosition.up * 100f, Color.red);
    }

    private void PickUpWeapon()
    {
        float pickUpRange = 1f;
        if (Physics.Raycast(findWeapon.position, findWeapon.forward, out RaycastHit hit, pickUpRange))
        {
            Debug.Log("tag: " + hit.collider.gameObject.tag);
            // findgameobject with tag GrabbedWeapon
             if (hit.transform.tag == "Weapon")
            {
                // set the weapon as a target of the two bone ik constraint
                isGrabbed = true;
                // isPickingUp is trigger in the animation
                anim.SetTrigger("isPickingUp");
                anim.SetBool("isGrabbed", isGrabbed);
                GrabbedWeapon = hit.transform.gameObject;
                SniperBody = GrabbedWeapon.transform.Find("sniperBody");
            } 
        }
    }

    private void Grab(GameObject GrabbedWeapon) {
       Rigidbody rb = GrabbedWeapon.GetComponent<Rigidbody>();
       rb.useGravity = false;
       rb.isKinematic = true;
       GrabbedWeapon.transform.parent = transform;
    }

    private bool DistanceBeforeGrab(Transform hand, float distance) {

        float newDistanceOfHand = 100f;
        Ray ray = new Ray(hand.position, hand.up);
        bool ground = false;

        if (!ground) {
            if (Physics.Raycast(ray, out RaycastHit hit, 5f, groundLayer)) {
                groundTransform = hit.transform;
                ground = !ground;
            }
        }
        if (ground) {
            float currentDistanceOfHand = Vector3.Distance(hand.position, groundTransform.position);
            float oldDistanceOfHand = Vector3.Distance(hand.position, hand.up * currentDistanceOfHand);
            newDistanceOfHand = Mathf.Abs(oldDistanceOfHand - currentDistanceOfHand);
        }
        if (newDistanceOfHand < 0.5f) {
            return true;
        }
        return false;
    }

    private void FollowTheHand() {

        SniperBody.localPosition = GrabbedWeaponOrgPos;
        GrabbedWeapon.transform.position = rightHandHoldPosition.position;
        GrabbedWeapon.transform.rotation = rightHandHoldPosition.rotation * Quaternion.Euler(1, 1, 90);
      
    }



    [ServerRpc]
    private void RequestPickUpWeaponServerRpc(ulong weaponNetId)
    {
       var weaponObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[weaponNetId].gameObject;
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