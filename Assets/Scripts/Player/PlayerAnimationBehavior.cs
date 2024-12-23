using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAnimationBehavior : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Animator animator;

    private bool playerDied = false;

    private ScoreController scoreController;
    private InputController input;

    void Awake()
    {
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        animator = gameObject.GetComponentInChildren<Animator>();

        scoreController = GameObject.Find("ScoreController").GetComponent<ScoreController>();
        input = GameObject.Find("InputController").GetComponent<InputController>();
    }

    public void Sleep()
    {
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
        else if (buffType == Enums.ShrineType.Speed)
        {
            animationName = "speed-buff";
        }
        else if (buffType == Enums.ShrineType.Strength)
        {
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

    public void HandleDamageAnimations(int newValue)
    {

        StartCoroutine(IncomingDamageAnimationHandler(newValue));
    }

    private IEnumerator IncomingDamageAnimationHandler(int newValue)
    {

        if (playerDied) { yield break; }

        // if the player died
        if (newValue <= 0)
        {

            playerDied = true;
            input.enabled = false;

            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

            PlayerMovement playerMovement = gameObject.GetComponent<PlayerMovement>();


            spriteRenderer.color = new Color(0, 0, 0, 0);

            string animationName = "player-death-animation";

            if (!playerMovement.isRightFacing) // if the player is facing left, die with the left animation
            {
                animationName = "player-death-animation-mirrored";
            }

            playerMovement.enabled = false;


            animator.Play(animationName);


            float animationLength = animator.runtimeAnimatorController.animationClips.First(clip => clip.name == animationName).length;

            yield return new WaitForSeconds(animationLength);

            animator.Play("Idle");

            scoreController.SetFinalScore();

        }

        // the player is still alive but took damage
        else
        {
            string animationName;

            float slashChoice = UnityEngine.Random.value;

            if (slashChoice > .5f)
            {
                animationName = "player-damage-slash-1";
            }
            else
            {
                animationName = "player-damage-slash-2";
            }

            if (!playerDied) { animator.Play(animationName); }
            float animationLength = animator.runtimeAnimatorController.animationClips.First(clip => clip.name == animationName).length / 2;

            yield return new WaitForSeconds(animationLength);

            if (!playerDied) { animator.Play("Idle"); } // we have to do this because coroutines are annoying and I don't want to re-code the animation system right now
        }

    }

}
