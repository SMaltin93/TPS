using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
// multi aim constraint
using UnityEngine.Animations.Rigging;

public class AimState : NetworkBehaviour
{
    
    [SerializeField] private Transform AimPosition;

    [SerializeField] private float AimSmoothSpeed = 20f;
    [SerializeField] private LayerMask AimLayer;
    [SerializeField] private Camera playerCamera;


    private Vector2 ScreenCenterPoint;
    private Animator anim;
    private bool isWeaponed = false;
    private GameObject weapon;

    private Transform shootingPoint;
    private Transform sniperBody;

    private void Start()
    {

        anim =  GetComponent<Animator>();
        ScreenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        weapon = null;
        shootingPoint = null;
        sniperBody = null;
       
    }

    private void Update()
    {
        if (IsLocalPlayer)
        {
            Aim();
            isWeaponed = anim.GetBool("isGrabbed");
        }
    }

    private void Aim()
    {
     
        Ray ray = playerCamera.ScreenPointToRay(ScreenCenterPoint);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, AimLayer))
        {
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
            AimPosition.position = Vector3.Lerp(AimPosition.position, hit.point, Time.deltaTime * AimSmoothSpeed);   
        }

        if (isWeaponed)
        {
           weapon = GameObject.FindGameObjectWithTag("Weapon");
           shootingPoint = weapon.transform.Find("sniperBody").Find("shootingPoint");
           shootingPoint.LookAt(AimPosition);
        }
        
    }


}
