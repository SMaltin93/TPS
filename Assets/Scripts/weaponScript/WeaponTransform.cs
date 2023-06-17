using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class WeaponTransform : NetworkBehaviour
{
    private readonly NetworkVariable<Vector3> weaponPosition = new NetworkVariable<Vector3>(writePerm: NetworkVariableWritePermission.Owner);
    private readonly NetworkVariable<Quaternion> weaponRotation = new NetworkVariable<Quaternion>(writePerm: NetworkVariableWritePermission.Owner);
    
    private Vector3 lastPos;
    private Quaternion lastRot;
    private float lerpRate = 10f;

    private void Start()
    {
        weaponPosition.Value = transform.position;
        weaponRotation.Value = transform.rotation;
        lastPos = transform.position;
        lastRot = transform.rotation;
    }

    void Update()
{
    if (IsOwner)
    {
        weaponPosition.Value = transform.position;
        weaponRotation.Value = transform.rotation;
    }
    else 
    {
        transform.position = Vector3.Lerp(lastPos, weaponPosition.Value, Time.deltaTime * lerpRate);
        transform.rotation = Quaternion.Euler(
            0,
            Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, weaponRotation.Value.eulerAngles.y, ref lastRot.y, Time.deltaTime * lerpRate),
            0
        );

        lastPos = transform.position;
        lastRot = transform.rotation;
    }
}


}
