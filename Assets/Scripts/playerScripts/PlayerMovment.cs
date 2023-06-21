using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


// Network 
public class PlayerMovment :  NetworkBehaviour
{
    // variable
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float backSpeed;
    [SerializeField] private float sideSpeed;

    private Transform playerTransform;


    private Vector3 moveDirection;
    private Vector3 velocity; // for gravity


    [SerializeField] private float gravity;
    [SerializeField] private float groundDistance; // means the distance between the ground and the player
    [SerializeField] private LayerMask groundMask; //to check if the player is on the ground or not
    [SerializeField] private bool isGrounded;

    [SerializeField] private float jumpHeight;

    // animation
    private bool walkForward = false;
    private bool walkBack = false;
    private bool run = false;
    private bool jump = false;
    private bool left = false;
    private bool right = false;
    private bool idle = false;


    // reference
    private CharacterController controller;
    private Animator anim;

    // animationfor grabed weapon
    private bool isWeaponed; 


    // make character controller readable of every one
    // awack function
    private void Awake()
    {
        // make character controller readable of every one
        
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        isWeaponed = anim.GetBool("isGrabbed");
        idle = true;
    }


    private void Update()
    {
        if (!IsOwner) return;
        InputPlayerState();
        Move();
    }

    private void Move()
    {
        // checkSphere(position, radius, layerMask) -> return true if the sphere overlaps any collider that is on the layerMask/
        // if the palayer is on the ground = true
        isGrounded = Physics.CheckSphere(transform.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {   // if the player is on the ground
            velocity.y = -2f;
        }
        // get input
        float move_z = Input.GetAxisRaw("Vertical");
        float move_x = Input.GetAxisRaw("Horizontal");
      
        // calculate move direction
        moveDirection = new Vector3(move_x, 0, move_z);
        moveDirection = transform.TransformDirection(moveDirection); // to make the player move in the direction of the camera
        
        // set SpeedX and SpeedZ in the animator movex and movez smooth the transition between the animation
        // if the player is running in forward direction

        Debug.Log("isWeaponed: " + isWeaponed);

        if (idle)
        {
            Idle();    
        }
        if (walkForward) {
            WalkForward();
        }
        if (run) {
            Run();
        }
        if (walkBack) {
            WalkBack();
        } 
        if (jump && isGrounded) {

            Jump();
        }      
         moveDirection *= moveSpeed; // move the player
        // debug the last position and the last rotation

        controller.Move(moveDirection * Time.deltaTime); // move the player
        // // gravity apply to the player
        velocity.y += gravity * Time.deltaTime; // calculate gravity
        controller.Move(velocity * Time.deltaTime);
        if ((jump && velocity.y < 0) || !isGrounded) 
        {
            Fall();
        }
    }


    private void WalkForward()
    {
        // move
        moveSpeed = walkSpeed;
        //anim.SetFloat("SpeedX", 0);
        anim.SetFloat("SpeedZ", 0.5f , 0.1f, Time.deltaTime);
        // is ground
        anim.SetBool("isGrounded", isGrounded);

       
    }

    private void WalkBack()
    {
        // go back
        moveSpeed = walkSpeed/2;
        // is ground
        anim.SetBool("isGrounded", isGrounded);

        if (left) {
            anim.SetFloat("SpeedX", -0.5f, 0.1f, Time.deltaTime);
            anim.SetFloat("SpeedZ", 0);
        }
        if (right) {
            anim.SetFloat("SpeedX", 0.5f, 0.1f, Time.deltaTime);
            anim.SetFloat("SpeedZ", 0);
        }

        if (!left && !right) {
            anim.SetFloat("SpeedX", 0);
            anim.SetFloat("SpeedZ", -0.5f, 0.1f, Time.deltaTime);
        }

    }
       
      

    private void Run()
    {
        // move if w is pressed
        
        moveSpeed = runSpeed;
        anim.SetFloat("SpeedX", 0);
        // increase the speed of the player to 2.0f smooth 
        anim.SetFloat("SpeedZ", 2.0f , 0.03f, Time.deltaTime);
        // is ground
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isFalling", false);
        anim.SetBool("isJumping", false);
    }

    private void Jump()
    { 
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity/2);
        anim.SetBool("isJumping", jump);
        anim.SetBool("isFalling", false);
        anim.SetBool("isGrounded", isGrounded);
    }

    private void Fall()
    {
        // fall
        anim.SetBool("isFalling", true);
        anim.SetBool("isJumping", false);
        anim.SetBool("isGrounded", isGrounded);
    }

    private void Idle()
    {
        // idle
        anim.SetFloat("SpeedX", 0 );
        anim.SetFloat("SpeedZ", 0 );
        // is ground
        anim.SetBool("isGrounded", isGrounded);
        // is falling and jumping false
        anim.SetBool("isFalling", false);
        anim.SetBool("isJumping", false);

    }

    private void InputPlayerState()
    {
        walkForward = Input.GetKey(KeyCode.W) 
        || (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W)) 
        || (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W));
        run = Input.GetKey(KeyCode.LeftShift) && walkForward;
        left = Input.GetKey(KeyCode.A) && !run;
        right = Input.GetKey(KeyCode.D) && !run;
        walkBack = Input.GetKey(KeyCode.S) || left || right;
        idle = !run && !walkBack && !jump && isGrounded && !walkForward;
        jump =  (Input.GetKey(KeyCode.Space) && run) || (Input.GetKey(KeyCode.Space) && walkForward) || (Input.GetKey(KeyCode.Space) && idle);
        //
        isWeaponed = anim.GetBool("isGrabbed");
    }

}

