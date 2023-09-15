using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class PlayerDeath : NetworkBehaviour
{
    private PlayerHealth playerHealth;
    private float thisHealth;

    private Animator animator;

    NetworkVariable<bool> isDead = new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    NetworkVariable<bool> fakeDeath = new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    private PlayerSound _playerSound;
    private bool playTheSoundOnce = true; 

    

    void Start()
    {

        playerHealth = GetComponent<PlayerHealth>();
        animator = GetComponent<Animator>();

        animator.SetBool("isDead", false);

        _playerSound = GetComponent<PlayerSound>();

    }

    void Update()
    {
        

         if (!IsOwner) return;
         thisHealth = playerHealth.GetHealth();

         if (thisHealth <= 0)
         {
            UpdateDeathState(false, true);
            animator.SetBool("isDead", isDead.Value);
            playDeathSound(isDead.Value);

         }

        // if typ k is pressed and is dead respawn
        if (Input.GetKeyDown(KeyCode.K) && !isDead.Value)
        {
            Debug.Log("Current Health: " + thisHealth);
            UpdateDeathState(true, false);
            animator.SetBool("isDead", fakeDeath.Value);
            
        }
        // trigger standUp animation if l is pressed
        if (Input.GetKeyDown(KeyCode.L) && fakeDeath.Value)
        {
            animator.SetTrigger("standUp");
            UpdateDeathState(false, false);
            animator.SetBool("isDead", isDead.Value);

        }
    }

    public bool GetIsDead()
    {
        return isDead.Value;
    }

    private void UpdateDeathState(bool fake, bool real)
    {
        isDead.Value = real;
        fakeDeath.Value = fake;
    }

    private void playDeathSound(bool isDead) {
        
        if (playTheSoundOnce) {
            _playerSound.PlayDeath();
            playTheSoundOnce = !isDead;
        }
        
    }

}
