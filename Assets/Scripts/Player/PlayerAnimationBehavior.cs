using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAnimationBehavior : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Animator animator;

    void OnAwake()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        animator = gameObject.GetComponentInChildren<Animator>();
    }

    public void Sleep(){
        animator.Play("sleep");
        StartCoroutine(ResetToIdle("sleep"));
    }
    public void BuffStats(Enums.ShrineType buffType)
    {

        string animationName = "None";

        if (buffType == Enums.ShrineType.Agility)
        {
            animationName = "agility-buff";
        }
        else if(buffType == Enums.ShrineType.Speed){
            animationName = "speed-buff";
        }
        else if(buffType == Enums.ShrineType.Strength){
            animationName = "strength-buff";
        }

        animator.Play(animationName);

        StartCoroutine(ResetToIdle(animationName));

    }


     private IEnumerator ResetToIdle(string animationName)
    {
        // Wait for the duration of the animation
        float animationLength = animator.runtimeAnimatorController.animationClips.First(clip => clip.name == animationName).length;
        yield return new WaitForSeconds(animationLength);

        // Play the idle state or any default animation
        animator.Play("None");
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
