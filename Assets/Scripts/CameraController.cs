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
        parent = transform.parent; // get the parent of the camera which is the player   

    }

    private void Start()
    {   
        // get component
          if (!IsOwner)
        {
            playerCamera.enabled = false;
        } 
        Cursor.lockState = CursorLockMode.Locked; // lock the cursor in the center of the screen
    }

    private void Update()
    { 
        Rotate();
     
    }

    private void Rotate()
    {
        // get input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        // rotate the parent
        parent.Rotate(Vector3.up * mouseX);

    }

}
