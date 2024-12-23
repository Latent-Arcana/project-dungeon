using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    void Awake()
    {
        scoreController = GameObject.Find("ScoreController").GetComponent<ScoreController>();
        input = GameObject.Find("InputController").GetComponent<InputController>();
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
        if (e.newValue <= 0 && !DEBUG_GOD_MODE)
        {

            HandlePlayerDeath();
        }
        StartCoroutine(IncomingDamageFlash());
    }

    public IEnumerator IncomingDamageFlash()
    {
        gameObject.GetComponent<SpriteRenderer>().color = UnityEngine.Color.black;
        yield return new WaitForSeconds(.05f);
        gameObject.GetComponent<SpriteRenderer>().color = UnityEngine.Color.white;
    }

    public void HandlePlayerDeath()
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        Animator playerAnimator = gameObject.GetComponentInChildren<Animator>();

        PlayerMovement playerMovement = gameObject.GetComponent<PlayerMovement>();

        if (playerAnimator != null && playerMovement != null)
        {
            spriteRenderer.color = new Color(0, 0, 0, 0);
            if (playerMovement.isRightFacing)
            {
                StartCoroutine(PlayerDeath(playerAnimator, "player-death-animation"));
            }
            else
            {
                StartCoroutine(PlayerDeath(playerAnimator, "player-death-animation-mirrored"));
            }


        }
    }

    public IEnumerator PlayerDeath(Animator playerAnimator, string animationName)
    {
        playerAnimator.Play(animationName);

        float animationLength = playerAnimator.runtimeAnimatorController.animationClips.First(clip => clip.name == animationName).length;
        input.ToggleMovement();

        yield return new WaitForSeconds(animationLength);

        scoreController.SetFinalScore();
    }

}
