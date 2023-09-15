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


    [SerializeField] private Camera scopeCamera;
    [SerializeField] private Transform shootingPoint;



    private Vector2 ScreenCenterPoint;
    




    private bool isScoped = false;
    private const float maxZoomDistance = 27f;
    private const float minZoomDistance = 3f;
    private const float increaseSpeed = 3f;

    private Animator anim;
   


    public NetworkVariable<Vector3> AimPositionNetVar = new NetworkVariable<Vector3>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


        public NetworkVariable<bool> IsScopedNetVar = new NetworkVariable<bool>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);





    private void Start()
    {
        

        AimPositionNetVar.OnValueChanged += (Vector3 oldPos, Vector3 newPos) =>
        {
            AimPosition.position = newPos;
        };
        aimImage.enabled = false;

        if (!IsOwner) return;
         
        ScreenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
    
        scopeCamera.enabled = false;

    }

    public void Aim(GameObject weapon)
    {
       
        if (!IsOwner) return;
        Ray ray = playerCamera.ScreenPointToRay(ScreenCenterPoint);
        RaycastHit hit;
        // max value of the ray is 1000 
        if (Physics.Raycast(ray, out hit, 2500, AimLayer)) 
        {
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
            AimPositionNetVar.Value = hit.point;
        }

        AimPosition.LookAt(AimPositionNetVar.Value);
        
        Debug.DrawRay(shootingPoint.position, shootingPoint.forward * 100, Color.red);
        PlayerAimState();
        aimImage.enabled = true;
        
    }

    private void PlayerAimState() {
        
        if (!IsOwner) return;
        // zoom in and out
        if (IsScopedNetVar.Value){
            
            scopeCamera.enabled = true;
            // use mouse scroll to zoom in and out 
            if (Input.GetAxis("Mouse ScrollWheel") > 0f )
            {
                if (scopeCamera.fieldOfView > minZoomDistance) scopeCamera.fieldOfView -= increaseSpeed;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f )
            {
               if (scopeCamera.fieldOfView < maxZoomDistance) scopeCamera.fieldOfView += increaseSpeed;
            }
        } else {
            scopeCamera.enabled = false;
        }

        if (Input.GetMouseButtonDown(1))
        {
        
        IsScopedNetVar.Value = !IsScopedNetVar.Value;
        }


    }

    public bool IsScoped() {
        return IsScopedNetVar.Value;
    }




}
