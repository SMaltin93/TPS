using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PlayerControl :  NetworkBehaviour
{
   CharacterController controller;
  


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
   private  void Update()
    {


        if (!IsOwner)
        {
            return;
            
        }
        Vector3 move = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            move.z += 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            move.z -= 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            move.x -= 1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            move.x += 1f;
        }
        float speed = 5f;

        controller.Move(move * speed * Time.deltaTime);
        // walk 
     
    }   
}
