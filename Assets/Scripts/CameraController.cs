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

    private float orginatxPos, orginatYPos, orginatZPos;

    private float radius = 0;

      private void Awake()
    {
        playerCamera = GetComponent<Camera>();


        orginatxPos = playerCamera.transform.localPosition.x;
        orginatYPos = playerCamera.transform.localPosition.y;
        orginatZPos = playerCamera.transform.localPosition.z;
        
        anim = transform.parent.GetComponent<Animator>();
        parent = transform.parent;  

       
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
        transform.localRotation = Quaternion.Euler(currentY, 0f, 0f);

        // Cos of the euler angle y of the camera
         float radius = 2.5f ; // Vector3.Distance(transform.position, parent.position);

         // debug radius
            

            // Calculate the normalized angle between -45 and 45 degrees
         float normalizedAngle = (currentY + 45f) / 90f;

            // Convert the normalized angle to radians

         float angleInRadians = normalizedAngle * Mathf.PI / 2f;

            float offsetY = ( radius * Mathf.Sin(angleInRadians) );
            float offsetZ = ( radius * Mathf.Cos(angleInRadians) );

    
            
            transform.localPosition = new Vector3(orginatxPos, offsetY , -offsetZ ) ;

 

    }

}
