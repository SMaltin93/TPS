using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandelDancing : MonoBehaviour
{
    private Animator animator;

    
    [SerializeField] private Slider _slider;

    private bool DanceState = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        // listen changed value of DieAnimation
        animator.SetFloat("Sound", _slider.value);

    }

    // die animation
    public void Die()
    {
        animator.SetBool("Die", true);
        // animator.SetBool("Dance1", DanceState);
        // DanceState = !DanceState;
        // animator.SetBool("Dance2", DanceState);
    }

    public void StandUp()
    {
        animator.SetTrigger("StandUp");
        animator.SetBool("Die", false);
    }

    public void Dance()
    {
       // Sound float 
       animator.SetFloat("Sound", _slider.value);
    }
}
