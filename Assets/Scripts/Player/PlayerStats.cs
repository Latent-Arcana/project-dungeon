using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static DungeonNarrator;
using static PlayerStatsManager;

public class PlayerStats : MonoBehaviour
{

    public PlayerStatsManager Stats_Manager;

    public bool DEBUG_GOD_MODE;

    private ScoreController scoreController;

    private InputController input;

    private Animator playerAnimator;

    private bool playerDied = false;

    void Awake()
    {
        scoreController = GameObject.Find("ScoreController").GetComponent<ScoreController>();
        input = GameObject.Find("InputController").GetComponent<InputController>();
        playerAnimator = gameObject.GetComponentInChildren<Animator>();

    }

    void OnEnable()
    {
        Stats_Manager.OnHealthChanged += Stats_ModifyHP;
    }

    void OnDisable()
    {
        Stats_Manager.OnHealthChanged -= Stats_ModifyHP;
    }

    private void Stats_ModifyHP(object sender, Stats_Args e)
    {

        if (e.newValue < e.oldValue && !DEBUG_GOD_MODE)
        {
            StartCoroutine(IncomingDamageAnimationHandler(e.newValue));
        }

    }

    public IEnumerator IncomingDamageAnimationHandler(int newValue)
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


            Debug.Log("Killing player now.");

            playerAnimator.Play(animationName);

            Debug.Log("Kill Played.");


            float animationLength = playerAnimator.runtimeAnimatorController.animationClips.First(clip => clip.name == animationName).length;

            Debug.Log("Waiting");
            yield return new WaitForSeconds(animationLength);

            playerAnimator.Play("Idle");

            Debug.Log("setting final score");
            scoreController.SetFinalScore();

        }

        // the player is still alive but took damage
        else
        {
            string animationName = "player-damage-slash-2";
            Debug.Log("player is taking damage here");

            if (!playerDied) { playerAnimator.Play(animationName); }
            float animationLength = playerAnimator.runtimeAnimatorController.animationClips.First(clip => clip.name == animationName).length / 2;

            yield return new WaitForSeconds(animationLength);


            if (!playerDied) { playerAnimator.Play("Idle"); }
        }

    }

    public IEnumerator IncomingDamageFlash()
    {
        gameObject.GetComponent<SpriteRenderer>().color = UnityEngine.Color.black;
        yield return new WaitForSeconds(.05f);
        gameObject.GetComponent<SpriteRenderer>().color = UnityEngine.Color.white;
    }

}
