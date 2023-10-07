using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class PlayerDeath : NetworkBehaviour
{
    private PlayerHealth playerHealth;
    private float thisHealth;

    private Animator animator;

    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<bool> fakeDeath = new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    private PlayerSound _playerSound;
    private bool playTheSoundOnce = true; 

    [SerializeField] private GameObject buttonUI;
    [SerializeField] private TextMeshProUGUI timeOfDeathText;
    [SerializeField] private TextMeshProUGUI timeOfDeathText2;


    public NetworkVariable<int> timeOfDeath = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private bool deathState = false;
    

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

         if (thisHealth <= 0 && !deathState)
         {
            UpdateDeathState(false, true);
            animator.SetBool("isDead", isDead.Value);
            playDeathSound(isDead.Value);
            buttonUI.SetActive(true);
            timeOfDeath.Value++;
            timeOfDeathText.text = "YOU LOST: " + timeOfDeath.Value;
            timeOfDeathText2.text = "YOU LOST:\n" + timeOfDeath.Value;
            
            // and dont look the mouse
            Cursor.lockState = CursorLockMode.None;
            deathState = true;
         } else if (thisHealth > 0) {
            Cursor.lockState = CursorLockMode.Locked;
            buttonUI.SetActive(false);
            deathState = false;
         }

        // if typ k is pressed and is dead respawn
        if (Input.GetKeyDown(KeyCode.K) && !isDead.Value)
        {
            UpdateDeathState(true, false);
            animator.SetBool("isDead", fakeDeath.Value);
            
        }
        // trigger standUp animation if l is pressed
        if (Input.GetKeyDown(KeyCode.L) && fakeDeath.Value)
        {
            StandUp();
        }
    }

    public bool GetIsDead()
    {
        return isDead.Value;
    }

    public void UpdateDeathState(bool fake, bool real)
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

    public void StandUp() {

        if (!IsOwner) return;
        animator.SetTrigger("standUp");
        UpdateDeathState(false, false);
        Wait(1f);
        animator.SetBool("isDead", isDead.Value);     
    }

    IEnumerator Wait(float duration)
    {
        yield return new WaitForSeconds(duration);
    }

}
