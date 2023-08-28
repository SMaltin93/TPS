using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class PlayerSound : NetworkBehaviour
{
    [SerializeField] private AudioClip[] _footsteps;

    // DeathSound 
    [SerializeField] private AudioClip _deathSound;

    // hitSound
    [SerializeField] private AudioClip _hitSound;




    private AudioSource _audioSource;

    // add listener to the footstep sound

    private AudioListener _AudioListener;

    private PlayerHealth _playerHealth;


    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

         _playerHealth = GetComponent<PlayerHealth>();

        // add one listener in every scene
        if (IsLocalPlayer) {
            _AudioListener = this.gameObject.AddComponent<AudioListener>();
        }

        // if the currentHealth.Value is cahnged play the hit sound

        _playerHealth.currentHealth.OnValueChanged += (float oldHealth, float newHealth) =>
        {
            // if the local player is hit play the hit sound
            if (!IsLocalPlayer) return;
            if (newHealth < oldHealth)
            {
                PlayHit();
            }
        };
    }
        

    public void PlayFootstep()
    {
       // _audioSource.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);

        if (IsLocalPlayer)
        {

           PlayFootstepServerRpc() ;

        }

    }

   
    public void PlayDeath()
    {

        _audioSource.PlayOneShot(_deathSound);
   
    }

    public void PlayHit()
    {
         float currentVolume = _audioSource.volume;
         Debug.Log("currentVolume: " + currentVolume);
         // increase the volume of the hit sound
         _audioSource.volume = 1f;
        _audioSource.PlayOneShot(_hitSound);
        // return the volume to the previous value
        _audioSource.volume = currentVolume;

        Debug.Log("_audioSource.volume: " + _audioSource.volume);

    }


     [ServerRpc] 
        public void PlayFootstepServerRpc()
        {
            PlayFootstepClientRpc();
        }

        [ClientRpc]
        public void PlayFootstepClientRpc()
        {
            Debug.Log("PlayFootstep called. IsLocalPlayer: " + IsLocalPlayer + " IsServer: " + IsServer);
            // if the owner true the footstep sound
            //_footstepSound.Value = true;
            
            this._audioSource.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
        }

}
