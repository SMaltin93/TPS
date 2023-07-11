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

    [SerializeField] private Transform Target;

    [SerializeField] private float pickUpRange = 1f;

    
    // anim refernce 
    private Animator anim;
    private bool isGrabbed = false;
  


    // // Reference to the find GrabbedWeapon point
    private Transform findWeapon;
    // refernce to the "Tow Bone IK Constraint" as acomponent of the player
    // player id


    private Transform SniperBody;


    // refernce multi aim constraint 
    private MultiAimConstraint multiAimConstraintRHand, multiAimConstraintBody;

  
    // body Aim constraint
    private LayerMask groundLayer;
    private Transform groundTransform ;

    private  AnimRig animRig;
    private AimState aimState;
    private bool Aim = false;
    // make a network variable write of Owner
    public  NetworkVariable<ulong> GrabbedWeapon = new NetworkVariable<ulong>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    // write premision of owner

    public NetworkVariable<int> SetAnimRig = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private int weight = 0;


    [SerializeField] private Rig constraintRig;
    

    void Start()
    {

        SetAnimRig.OnValueChanged += (previousValue, newValue) =>
        {
            constraintRig.weight = newValue; 
        };
 
        SniperBody = null;
        anim = GetComponent<Animator>();

        if (!IsOwner) return;
        
        findWeapon = transform.Find("findWeapon"); 
        anim.SetBool("isGrabbed", false);
        anim.SetBool("isPickingUp", false);
        animRig = GetComponent<AnimRig>();
        aimState = GetComponent<AimState>();
    }



   

    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKey(KeyCode.E) && GrabbedWeapon.Value == 0)
        {
            PickUpWeapon();
            
        }

    //    if (Input.GetKeyDown(KeyCode.F) )  {
    //         SetAnimRig.Value =  constraintRig.weight == 0 ? 1 : 0;
    //    }

        if (Aim) {
            aimState.Aim(NetworkManager.Singleton.SpawnManager.SpawnedObjects[GrabbedWeapon.Value].gameObject);
            animRig.SetParentConstraint(NetworkManager.Singleton.SpawnManager.SpawnedObjects[GrabbedWeapon.Value].gameObject.transform);
            animRig.SetLeftHandTarget(NetworkManager.Singleton.SpawnManager.SpawnedObjects[GrabbedWeapon.Value].gameObject.transform);
            SetAnimRig.Value =  1;
        } 

        Debug.DrawRay(rightHandHoldPosition.position, rightHandHoldPosition.up * 100f, Color.red);
    }

    private void PickUpWeapon()
    {
       
        if (Physics.Raycast(findWeapon.position, findWeapon.forward, out RaycastHit hit, pickUpRange))
        {
            Debug.Log("tag: " + hit.collider.gameObject.tag);
            // findgameobject with tag GrabbedWeapon

             if (hit.transform.tag == "Weapon")
            {
                SetParentServerRpc(hit.transform.gameObject.GetComponent<NetworkObject>().NetworkObjectId); 
            } 
        }
    }

    private void Grab(GameObject Weapon, ulong weaponId) {
        Debug.Log("GrabbedWeapon: IS NOT NULL");
        Rigidbody rb = Weapon.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        SniperBody = Weapon.transform.Find("sniperBody");
        isGrabbed = true;
        anim.SetTrigger("pickUp");
        anim.SetBool("isGrabbed", isGrabbed);
        GameObject weaponObjec = NetworkManager.Singleton.SpawnManager.SpawnedObjects[weaponId].gameObject;
        MakeParent(transform);
        Aim = true;
    }



    private void MakeParent(Transform isParent) {
        if (GrabbedWeapon.Value == 0) return;
        var weapon = NetworkManager.SpawnManager.SpawnedObjects[GrabbedWeapon.Value].gameObject;
        weapon.transform.parent = isParent;
        
    }

    private Vector3 StartRandomPosition() {
        return new Vector3(-2f,   10f ,  2f);
    }


    [ServerRpc]
    private void SetParentServerRpc(ulong weaponId, ServerRpcParams rpcParams = default) {
        var weapon = NetworkManager.SpawnManager.SpawnedObjects[weaponId].gameObject;
        Debug.Log("request server rpc");
        weapon.GetComponent<NetworkObject>().ChangeOwnership(rpcParams.Receive.SenderClientId);
        GrabbedWeapon.Value = weaponId;
        SetParentClientRpc(weaponId, rpcParams.Receive.SenderClientId);
        Grab(weapon, weaponId); 

    }

    [ClientRpc]
    private void SetParentClientRpc(ulong weaponId,ulong clientId) {
        if (NetworkManager.Singleton.LocalClientId != clientId) return;

        var weapon = NetworkManager.SpawnManager.SpawnedObjects[weaponId].gameObject;
        if (weapon == null || GrabbedWeapon.Value != 0) return;

        Grab(weapon, weaponId);
    }

}