using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
// multi aim constraint
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class AimState : NetworkBehaviour
{
    
    [SerializeField] private Transform AimPosition;

    [SerializeField] private float AimSmoothSpeed = 20f;
    [SerializeField] private LayerMask AimLayer;
    [SerializeField] private Camera playerCamera;

    [SerializeField] private Image aimImage;


    private Vector2 ScreenCenterPoint;
    private Animator anim;
    private bool isWeaponed = false;
    private GameObject weapon;

    private Transform shootingPoint;
    private Transform aimScope;

    private Camera scopeCamera;

    private bool isScoped = false;
    private const float maxZoomDistance = 27f;
    private const float minZoomDistance = 2.5f;
    private const float increaseSpeed = 0.5f;


    private void Start()
    {

        anim =  GetComponent<Animator>();
        ScreenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        weapon = null;
        shootingPoint = null;
        aimScope = null;
        scopeCamera = null;

        aimImage.enabled = false;
       
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
        // max value of the ray is 1000 
        if (Physics.Raycast(ray, out hit, 1000, AimLayer)) 
        {
             Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
           // AimPosition.position = Vector3.Lerp(AimPosition.position, hit.point, Time.deltaTime * AimSmoothSpeed);
            AimPosition.position = hit.point;
        }

        if (isWeaponed)
        {
           weapon = GameObject.FindGameObjectWithTag("Weapon");
           shootingPoint = weapon.transform.Find("sniperBody").Find("shootingPoint");
           scopeCamera = weapon.transform.Find("sniperBody").GetComponentInChildren<Camera>();
           aimScope = weapon.transform.Find("sniperBody").Find("scopePoint");
           // look just position
           shootingPoint.LookAt(AimPosition);
           scopeCamera.transform.LookAt(AimPosition);
           PlayerAimState();
           aimImage.enabled = isScoped;
        }
        
    }

    private void PlayerAimState() {

        // if bottin 1 down
        if (Input.GetMouseButtonDown(1))
        {
            isScoped = !isScoped;
        }

        // zoom in and out
        if (isScoped){
            // use mouse scroll to zoom in and out 
            if (Input.GetAxis("Mouse ScrollWheel") > 0f )
            {
                if (scopeCamera.fieldOfView > minZoomDistance) scopeCamera.fieldOfView -= increaseSpeed;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f )
            {
               if (scopeCamera.fieldOfView < maxZoomDistance) scopeCamera.fieldOfView += increaseSpeed;
            }
        }
    }
    public bool IsScoped() {
        return isScoped;
    }

    public Transform GetScopePosition() {
        return aimScope;
    }


}
