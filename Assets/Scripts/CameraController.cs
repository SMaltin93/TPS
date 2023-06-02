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

    public Camera playerCamera;

    // awack function

    private void Awake()
    {
        playerCamera = GetComponent<Camera>();
    }

    private void Start()
    {   
        // get component
          if (!IsOwner)
        {
            playerCamera.enabled = false;
        }
        parent = transform.parent; // get the parent of the camera which is the player     
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
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        parent.Rotate(Vector3.up * mouseX); // rotate the parent of the camera which is the player

    }


}
