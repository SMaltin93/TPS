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

    private bool isJumping;

    // animation
    private bool walkForward = false;
    private bool walkBack = false;
    private bool left = false;
    private bool right = false;
    private bool run = false;
    private bool idle = false;
    private bool jump = false;
    private bool sideWalk = false;

    // reference
    private CharacterController controller;
    private Animator anim;


    // make character controller readable of every one
    // awack function
    private void Awake()
    {
        // make character controller readable of every one
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    // start function

  
 

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        InputPlayerState();
        Move();
    }

    private void Move()
    {
        // check ground
        // checkSphere(position, radius, layerMask) -> return true if the sphere overlaps any collider that is on the layerMask/
        // if the palayer is on the ground = true
        isGrounded = Physics.CheckSphere(transform.position, groundDistance, groundMask);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isFalling", !isGrounded);
        anim.SetBool("isJumping", !isGrounded);

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

        if (isGrounded)
        {   // if the player is on the ground and move
            // is walking 
            if (walkForward && !run)
            {   // is walk forward
                Walk();
            }
            
            if (walkBack && !run)
            {   // is walk back
                WalkBack();
            }

            if (left && !run)
            {   // is walk left
                GoLeft();
            } 
            
            if (right && !run)
            {   // is walk right
                GoRight();
            }

            if (run && walkForward)
            {   // is run
                Run();
            }
          
            if (idle)
            {   // is idle
                Idle();
            }
            // is walk ba
            if (jump )
            {   // jump
                Jump();

            }
            
            moveDirection *= moveSpeed;

        }

        // debug the last position and the last rotation

        controller.Move(moveDirection * Time.deltaTime); // move the player
        // // gravity apply to the player
        velocity.y += gravity * Time.deltaTime; // calculate gravity
        controller.Move(velocity * Time.deltaTime);
       


        // if isJumping = true and the player is falling
        if ((isJumping && velocity.y < 0) || !isGrounded)
        {
            fall();
        }
    }


    private void Walk()
    {
        // move
        moveSpeed = walkSpeed;
        anim.SetFloat("SpeedX", 0f);
        anim.SetFloat("SpeedZ", 0.5f, 0.1f, Time.deltaTime);
        // is walking
        anim.SetBool("isWalking", walkForward);
        // is falling = false
        anim.SetBool("isFalling", false);
    }

    private void Run()
    {
        // move if w is pressed
        moveSpeed = runSpeed;
        anim.SetFloat("SpeedX", 0f);
        anim.SetFloat("SpeedZ", 2.0f, 0.1f, Time.deltaTime);
        anim.SetBool("isWalking", false);
        anim.SetBool("isFalling", false);
    
    }

    private void Idle()
    {
       
        anim.SetFloat("SpeedX", 0f);
        anim.SetFloat("SpeedZ", 0f);
        anim.SetBool("isWalking", idle);
        anim.SetBool("isFalling", false);

    }

    private void Jump()
    {
        // jump
        isJumping = true;
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        anim.SetBool("isJumping", jump);
        anim.SetBool("isWalking", false);
    }

    private void fall()
    {
        // fall
        anim.SetBool("isFalling", true);
        anim.SetBool("isJumping", false);
        // is moving = false
        anim.SetBool("isWalking", false);
    }

    private void WalkBack()
    {
        // go back
        moveSpeed = backSpeed;
        anim.SetFloat("SpeedX", 0f);
        anim.SetFloat("SpeedZ", -0.5f, 0.1f, Time.deltaTime);
        // is walking
        anim.SetBool("isWalking", walkBack);
        // is falling = false
        anim.SetBool("isFalling", false);

    }
    private void GoLeft()
    {
        // go left
        moveSpeed = sideSpeed;
        anim.SetFloat("SpeedX", -0.5f, 0.1f, Time.deltaTime);
        anim.SetFloat("SpeedZ", 0f);
        // is walking
        anim.SetBool("isWalking", left);
        anim.SetBool("isFalling", false);

    }

    private void GoRight()
    {
        // go right
        moveSpeed = sideSpeed;
        anim.SetFloat("SpeedX", 0.5f, 0.1f, Time.deltaTime);
        anim.SetFloat("SpeedZ", 0f);
        // is walking
        anim.SetBool("isWalking", right);
        // is falling = false
        anim.SetBool("isFalling", false);

    }

    private void GoSideWay() {
        // rotate the player a little bit smoothly and rotate back otherwise
        Quaternion newRotation = Quaternion.Euler(0,180, 0);
        if (sideWalk && walkForward){
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 5f);
            // and walk forward
        } 


    }


    private void InputPlayerState()
    {
        // set key
        walkForward = Input.GetKey(KeyCode.W);
        walkBack = Input.GetKey(KeyCode.S);
        left = Input.GetKey(KeyCode.A);
        right = Input.GetKey(KeyCode.D);
        run = Input.GetKey(KeyCode.LeftShift);
        sideWalk = (left && walkForward) || (right && walkForward) || (left && walkBack) || (right && walkBack); 
        jump = Input.GetKey(KeyCode.Space);
        idle = !walkForward && !walkBack && !left && !right && !run && !jump;
    }

}

