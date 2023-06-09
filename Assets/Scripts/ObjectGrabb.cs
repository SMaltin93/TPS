using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ObjectGrabb : MonoBehaviour
{

    // rigidbody reference
    private Rigidbody rb;
    private Transform objectGrabbPoint;
  
   
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
      
    }

    public void Grab(Transform objectGrabbPoint)
    {
       // disable gravity
         rb.useGravity = false;
        
        rb.isKinematic = true;

         this.objectGrabbPoint = objectGrabbPoint;
         // make the weapon rotation the same as the objectGrabbPoint
         transform.rotation = objectGrabbPoint.rotation;
    }

    private void FixedUpdate()
    {
        // if the object is grabbed, it will follow the objectGrabbPoint
        if (objectGrabbPoint != null)
        {
            rb.MovePosition(objectGrabbPoint.position);
            // rotate the object as the camera rotates
        }
    }
}
