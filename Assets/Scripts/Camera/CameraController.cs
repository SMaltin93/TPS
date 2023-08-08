using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


public class CameraController : NetworkBehaviour
{
    // variable

    [SerializeField] private float mouseSensitivity;

    [SerializeField] private Transform ScopePosition;   

    // reference
    private Transform parent;
    private Camera playerCamera;
   
    private float currentY = 0f;
    private float currentX = 0f;

    // awack function
    private float orginatxPos;

    private Vector3 orginalPos;

    private AimState aimState;


    private void Awake()
    {
        playerCamera = GetComponent<Camera>();
        aimState = transform.parent.GetComponent<AimState>();
    }

    private void Start()
    {   
        if(IsOwner)
        {
            parent = transform.parent;
            Cursor.lockState = CursorLockMode.Locked;
            orginatxPos = playerCamera.transform.localPosition.x;
            orginalPos = playerCamera.transform.localPosition;
            playerCamera.enabled = true;  // Enable camera for local player
        }
        else
        {
            playerCamera.enabled = false;  // Disable camera for other players
        }
    }

    private void Update()
    { 
        if(IsLocalPlayer)
        {
            Rotate();
        }
    }

    private void Rotate()
    {
        // get input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        currentY -= mouseY;
        currentX += mouseX;
        currentY = Mathf.Clamp(currentY, -45f, 45f);
        parent.Rotate(Vector3.up * mouseX);
        // allow y rotate as 2d
        transform.localRotation = Quaternion.Euler(currentY, 0f, 0f);

        float radius = 2.5f; 
        // debug radius
        // Calculate the normalized angle between -45 and 45 degrees
        float normalizedAngle = (currentY + 45f) / 90f;
        float angleInRadians = normalizedAngle * Mathf.PI / 2f;
        float offsetY = ( radius * Mathf.Sin(angleInRadians) );
        float offsetZ = ( radius * Mathf.Cos(angleInRadians) );
        // is scoped 
        if (aimState.IsScoped()) {
            float distance = Mathf.Sin(Time.timeSinceLevelLoad)/1000f;
            float distanceX = Mathf.Cos(Time.timeSinceLevelLoad)/1000f;
            transform.position = Vector3.Lerp(transform.position, ScopePosition.position + new Vector3(distanceX, distance, 0), Time.deltaTime * 10f);

        
        } else {
            //transform.localPosition = new Vector3(orginatxPos, offsetY , -1*(offsetZ + 2) ) ;
            transform.localPosition = new Vector3(orginalPos.x, offsetY , orginalPos.z ) ;
        }

    }
    


}
