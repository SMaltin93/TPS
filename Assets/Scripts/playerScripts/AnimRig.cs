using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

// parent constraint
using UnityEngine.Animations;
using Unity.Netcode;

public class AnimRig : NetworkBehaviour
{
  
    [SerializeField] private GameObject leftHandIK;
    [SerializeField] private GameObject parentConstraintRightHand;


    private PlayerPickUp playerPickUp;
    private TwoBoneIKConstraint leftHandIKConstraint;



    private MultiAimConstraint multiAimConstraintRightHand;
    private MultiAimConstraint multiAimConstraintBody;

   



    ConstraintSource constraintSource;
    ParentConstraint parentConstraint;
    // build the rig
    private RigBuilder rigBuilder;

 


    void Start()
    {

        
        if (!IsOwner) return; 
        playerPickUp = GetComponent<PlayerPickUp>();
        rigBuilder = GetComponent<RigBuilder>();  
    }

    // public void SetLeftHandTarget(Transform weapon)
    // {
       
    //     leftHandIKConstraint = transform.Find("Rig").Find("leftHandIK").GetComponent<TwoBoneIKConstraint>(); 
    //     multiAimConstraintRightHand = transform.Find("Rig").Find("bodyAim").GetComponent<MultiAimConstraint>();
    //     multiAimConstraintBody = transform.Find("Rig").Find("RHandAim").GetComponent<MultiAimConstraint>();


    //     multiAimConstraintRightHand.weight = playerPickUp.SetAnimRig.Value;
    //     multiAimConstraintBody.weight = playerPickUp.SetAnimRig.Value;

    //     leftHandIKConstraint.weight =playerPickUp.SetAnimRig.Value;
    // }

    public void SetParentConstraint(Transform Constraint) {
        parentConstraint = Constraint.GetComponent<ParentConstraint>();
        constraintSource.sourceTransform = parentConstraintRightHand.transform;
        constraintSource.weight = 1;
        parentConstraint.AddSource(constraintSource);
        parentConstraint.constraintActive = true;
    }

  
}
