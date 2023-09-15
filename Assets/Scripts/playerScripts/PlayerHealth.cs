using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;




public class PlayerHealth : NetworkBehaviour
{


    
    

    [SerializeField] private float maxHealth;



    public NetworkVariable<float> currentHealth = new NetworkVariable<float>(150f,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    private float playerHealth;

    private float chipSpeed = 5f;
    private float lerpTime = 0f;

    [SerializeField] private GameObject playerUI;

    [SerializeField] private Image frontHealthBar;
    [SerializeField] private Image backHealthBar;

    // define the damage part 
    [SerializeField] private float BodyDamage = 30f;
    [SerializeField] private float HeadDamage = 74f;
    [SerializeField] private float LegDamage = 20f;
    [SerializeField] private float ArmDamage = 20f;


    // PlayerSound hit sound 



    void Start()
    {

       if (IsServer) currentHealth.Value = maxHealth;

       currentHealth.OnValueChanged += (float oldHealth, float newHealth) =>
       {
           lerpTime = 0;
       };


        if (IsLocalPlayer)
        {
            playerUI.SetActive(true);
        }
        else
        {
            playerUI.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (!IsOwner) return;

       if (IsServer) currentHealth.Value  = Mathf.Clamp(currentHealth.Value , 0, maxHealth);

        
        UpdateHealthBar();

        //Debug.Log("playerHealth: " + playerHealth);
        
    }

    private void UpdateHealthBar()
    {

        if (!IsOwner) return;

        
        float fillFront = frontHealthBar.fillAmount;
        float fillBack = backHealthBar.fillAmount;

        float healthPercent = currentHealth.Value / maxHealth;

        if  (fillBack > healthPercent)
        {
            frontHealthBar.fillAmount = healthPercent;
            backHealthBar.color = Color.red;
            lerpTime += Time.deltaTime;
            float precentComplete = lerpTime / chipSpeed;
            backHealthBar.fillAmount = Mathf.Lerp(fillBack, healthPercent, precentComplete);
        }
      
    }

    public void TakeDamage(string bodyPart)
    {
        if (!IsServer) return;
        // switch case for body part
        switch (bodyPart)
        {
            case "PlayerHead":
                 currentHealth.Value -= HeadDamage;
                break;
            case "PlayerBody":
                currentHealth.Value -= BodyDamage;
                break;
            case "PlayerLeg": 
                currentHealth.Value -= LegDamage;

                break;
            case "PlayerArm":
                currentHealth.Value -= ArmDamage;
                break;
            default:
                break;
        }


    }

    public void Heal(float healAmount)
    {
        if (!IsOwner) return;
        currentHealth.Value += healAmount;
        lerpTime = 0;

    }

    public float GetHealth()
    {
        return currentHealth.Value;
    }





}
