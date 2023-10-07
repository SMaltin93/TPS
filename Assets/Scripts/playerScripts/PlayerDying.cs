using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class PlayerDying : NetworkBehaviour
{
    [SerializeField] private GameObject buttonUI;
    // exite game button
    [SerializeField] private Button exitGame;
    // return to the game button
    [SerializeField] private Button returnToGame;
    [SerializeField] private GameObject player;


    //
    
    private PlayerHealth playerHealth;
    private PlayerMovment playerMovment;
    private PlayerDeath playerDeath;
    private Animator animator;

    // amount of time to player died 
    

    private bool revive = false;

    void Awake()
    {
        playerHealth = player.GetComponent<PlayerHealth>();
        playerMovment = player.GetComponent<PlayerMovment>();
        playerDeath = player.GetComponent<PlayerDeath>();
        animator = player.GetComponent<Animator>();
        // make sure Button is not active
        buttonUI.SetActive(false);
        
    }


    void Update() {
        if (!IsLocalPlayer) return;
        // if return button is pressed revive the player
        if (revive && playerHealth.currentHealth.Value > 0)
        {
            standUp();
            revive = false;
        }
    }


    public void ReturnToTheGame()
    {
        revive = true;
       // if the button is pressed and the current health is 0 
        refillHealthServerRpc();
    }


    private void standUp() {
        this.player.transform.position = playerMovment.RandomPosition();
        this.animator.SetTrigger("standUp");
        this.animator.SetBool("isDead", false);
        playerDeath.UpdateDeathState(false, false);   
    }

    [ServerRpc]
    public void refillHealthServerRpc()
    {
        playerHealth.RevivePlayer();
        //standUp();
    }

    public void ExitTheGame()
    {
        this.GetComponent<NetworkObject>().Despawn(true);
        Application.Quit();
        
    }
    

}
