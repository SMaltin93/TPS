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

      private void Awake()
    {
        playerCamera = GetComponent<Camera>();
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
        currentY = Mathf.Clamp(currentY, -80f, 80f);
        //currentX = Mathf.Clamp(currentX, -30f, 30f);
        
        // rotate camera

    // in input c rotate the player

    // if (isGrabbed)
    // {
    //     transform.localRotation = Quaternion.Euler(currentY, currentX, 0f);
    // } 
    // else {
        parent.Rotate(Vector3.up * mouseX);
        transform.localRotation = Quaternion.Euler(currentY, 0f, 0f);
   // }

    

    }

}
