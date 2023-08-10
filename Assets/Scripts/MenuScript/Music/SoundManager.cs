using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SoundManager : MonoBehaviour
{
    // Slider 
    [SerializeField] private Slider _slider;
    // Audio source
    [SerializeField] private AudioSource _audioSource;


    // Start is called before the first frame update

    void Start()
    {
        // Set the slider value to the current volume
        _audioSource.volume = _slider.value;
    }

    public void SetBackgroundVolume()
    {
        // Set the volume of the background music
        _audioSource.volume =  _slider.value;
    }
}
