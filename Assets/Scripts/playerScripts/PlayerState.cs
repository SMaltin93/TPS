using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using UnityEngine.Animations.Rigging;



public class PlayerState : NetworkBehaviour
{

    [SerializeField] private LayerMask pickUpLayer;
    [SerializeField] private Transform rightHandHoldPosition;

    [SerializeField] private Transform Target;

    [SerializeField] private float pickUpRange = 1f;

    [SerializeField] private Transform Sniper;

    private PlayerShoot playerShoot;
    
    // anim refernce 
    private Animator anim;
    private bool isGrabbed = false;
  


    // // Reference to the find GrabbedWeapon point
    private Transform findWeapon;
    // refernce to the "Tow Bone IK Constraint" as acomponent of the player
    // player id

    private AimState aimState;
    private bool Aim = false;

    // make a network variable write of Owne
    // write premision of owner

    public NetworkVariable<int> SetAnimRig = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    public NetworkVariable<bool> IsWeaponActive = new NetworkVariable<bool>(false,
    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);



    [SerializeField] private Rig constraintRig;
    

    void Start()
    {

        SetAnimRig.OnValueChanged += (previousValue, newValue) =>
        {
            constraintRig.weight = newValue; 
        };

        Sniper.gameObject.SetActive(IsWeaponActive.Value);

        IsWeaponActive.OnValueChanged += (previousValue, newValue) =>
        {
            Sniper.gameObject.SetActive(newValue);
        };

        // desable visiblity of the sniper
 
        anim = GetComponent<Animator>();

        if (!IsOwner) return;
        constraintRig.weight = 0;
        findWeapon = transform.Find("findWeapon"); 
        anim.SetBool("isGrabbed", false);
        aimState = GetComponent<AimState>();
        playerShoot = GetComponent<PlayerShoot>();
    }


    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKey(KeyCode.E) && isGrabbed == false)
        {
            PickUpWeapon();
        }

        if (isGrabbed )
        {
            if (Aim)
            {
                aimState.Aim(Sniper.gameObject);
            }

            if (Input.GetMouseButtonDown(0))
            {
                playerShoot.Shoot();
            }
          
        }
        Debug.DrawRay(rightHandHoldPosition.position, rightHandHoldPosition.up * 100f, Color.red);

    }


    private void PickUpWeapon()
    {
       
        if (Physics.Raycast(findWeapon.position, findWeapon.forward, out RaycastHit hit, pickUpRange))
        {
            Debug.Log("tag: " + hit.collider.gameObject.tag);

             if (hit.transform.tag == "Weapon")
            {
                Debug.Log("GrabbedWeapon: IS NULL");
                isGrabbed = true;
                StartCoroutine(WaitForAnimation());
                anim.SetTrigger("pickUp");
                anim.SetBool("isGrabbed", isGrabbed);
                Aim = true;
                // set the sniper to be active
                IsWeaponActive.Value = true;

            } 
        }
    }

    IEnumerator WaitForAnimation()
    {
        yield return new WaitForSeconds(1f);
        SetAnimRig.Value = 1;
    }

}