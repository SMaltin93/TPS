using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


public class CameraController : NetworkBehaviour
{
    // variable

    [SerializeField] private float mouseSensitivity;
    [SerializeField] private Transform weaponHolder;

    // reference
    private Transform parent;
    private CharacterController characterController;

    private Vector3 moveDirection;

    private Camera playerCamera;
    private Animator anim;
    bool isGrabbed = false;

    private float currentY = 0f;
    private float currentX = 0f;

    // awack function
    private float orginatxPos;

    private AimState aimState;
    private Transform CameraScopePosition;

    private float radius = 0;

    // breath
    Vector3 startPos;
    public float amplitude = 0.1f;
    public float period = 1f;

      private void Awake()
    {
        playerCamera = GetComponent<Camera>();


        orginatxPos = playerCamera.transform.localPosition.x;
        
        anim = transform.parent.GetComponent<Animator>();
        parent = transform.parent;

        startPos = transform.localPosition;

        aimState = parent.GetComponent<AimState>();
    }

    private void Start()
    {   
        if(IsLocalPlayer)
        {
            Cursor.lockState = CursorLockMode.Locked;
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
       isGrabbed = anim.GetBool("isGrabbed");
    }

    private void Rotate()
    {
        // get input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        currentY -= mouseY;
        currentX += mouseX;
    // currentX += mouseX;
        currentY = Mathf.Clamp(currentY, -45f, 45f);
        


        parent.Rotate(Vector3.up * mouseX);
        // allow y rotate as 2d
        transform.localRotation = Quaternion.Euler(currentY, 0f, 0f);

        // Cos of the euler angle y of the camera
        float radius = 2.5f; 
        // debug radius
        // Calculate the normalized angle between -45 and 45 degrees
        float normalizedAngle = (currentY + 45f) / 90f;
        float angleInRadians = normalizedAngle * Mathf.PI / 2f;
        float offsetY = ( radius * Mathf.Sin(angleInRadians) );
        float offsetZ = ( radius * Mathf.Cos(angleInRadians) );

        // is scoped 
        if (aimState.IsScoped()) {;
            //  float theta = Time.timeSinceLevelLoad / period;
            // float distance = amplitude * Mathf.Sin(theta);
            float distance = Mathf.Sin(Time.timeSinceLevelLoad)/1000f;
            transform.position =  aimState.GetScopePosition().position + new Vector3(0, distance, 0);
        
        } else {
            transform.localPosition = new Vector3(orginatxPos, offsetY , -1*(offsetZ + 2) ) ;
        }

    }


}
