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

    [SerializeField] private LayerMask AimLayer;
    [SerializeField] private Camera playerCamera;

    [SerializeField] private Image aimImage;


    private Vector2 ScreenCenterPoint;
    
    private Transform shootingPoint;
    private Transform aimScope;


    private bool isScoped = false;
    private const float maxZoomDistance = 27f;
    private const float minZoomDistance = 2.5f;
    private const float increaseSpeed = 0.5f;


    private PlayerPickUp playerPickUp;
    private Animator anim;
    private Camera scopeCamera;


    public NetworkVariable<Vector3> AimPositionNetVar = new NetworkVariable<Vector3>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);




    private void Start()
    {
        ScreenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);

        AimPositionNetVar.OnValueChanged += (Vector3 oldPos, Vector3 newPos) =>
        {
            AimPosition.position = newPos;
        };
    
        shootingPoint = null;
        aimScope = null;
        scopeCamera = null;
        aimImage.enabled = false;

    }

    public void Aim(GameObject weapon)
    {
        if (IsOwner)
        {
     
            Ray ray = playerCamera.ScreenPointToRay(ScreenCenterPoint);
            RaycastHit hit;
            // max value of the ray is 1000 
            if (Physics.Raycast(ray, out hit, 3000, AimLayer)) 
            {
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);

                AimPositionNetVar.Value = hit.point;
            
            }

            shootingPoint = weapon.transform.Find("sniperBody").Find("shootingPoint");
            scopeCamera =  weapon.transform.Find("sniperBody").GetComponentInChildren<Camera>();
            aimScope = weapon.transform.Find("sniperBody").Find("scopePoint");
            // look just position
            shootingPoint.LookAt(AimPosition);
            scopeCamera.transform.LookAt(AimPosition);
            Debug.DrawRay(shootingPoint.position, shootingPoint.forward * 100, Color.red);
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

    private void UppdateAimPosition() {

        
    }
    public bool IsScoped() {
        return isScoped;
    }

    public Transform GetScopePosition() {
        return aimScope;
    }




}
