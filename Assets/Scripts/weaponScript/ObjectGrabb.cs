using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;



public class ObjectGrabb : NetworkBehaviour
{

    // rigidbody reference
    private Rigidbody rb;
    private Transform objectGrabbPoint;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();  
    }

    public void Grab(Vector3 grabbPointPosition, Quaternion grabbPointRotation, Transform parent)
    {
        // set rigidbogy to parent of the objectGrabbPoint 
       // disable gravity

        rb.useGravity = false;
        rb.isKinematic = true;
        // set the objectGrabbPoint to the grabbPointPosition
        transform.position = grabbPointPosition;
        // set the rotation of the objectGrabbPoint
        transform.rotation = grabbPointRotation;
        // set the parent of the objectGrabbPoint
        //transform.parent = parent;

    }

    // drop the weapon
    public void Drop(Transform objectGrabbPoint)
    {
        // enable gravity
        rb.useGravity = true;
        rb.isKinematic = false;
        // get player regidbody
        this.objectGrabbPoint = null;
        // force the weapon to drop
        rb.AddForce(objectGrabbPoint.forward * 200f * Time.deltaTime, ForceMode.Impulse);
        // force up 
        rb.AddForce(objectGrabbPoint.up * 200f * Time.deltaTime, ForceMode.Impulse);
        // add random rotation
        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 200f * Time.deltaTime, ForceMode.Impulse);
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
