using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


public class CameraController : NetworkBehaviour
{
    // variable

    [SerializeField] private float mouseSensitivity;

    // reference
    private Transform parent;
    private CharacterController characterController;

    private Vector3 moveDirection;

    public Camera playerCamera;

    // awack function

      private void Awake()
    {
        playerCamera = GetComponent<Camera>();
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
    }

    private void Rotate()
    {
        // get input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        // rotate the parent
        parent.Rotate(Vector3.up * mouseX);

    }

}
