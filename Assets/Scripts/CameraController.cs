using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // variable

    [SerializeField] private float mouseSensitivity;

    // reference
    private Transform parent;

    private void Start()
    {
        // get component
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
