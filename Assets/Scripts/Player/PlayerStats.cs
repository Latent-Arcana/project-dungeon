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

    PlayerAnimationBehavior playerAnimationBehavior;

    void Awake()
    {
        scoreController = GameObject.Find("ScoreController").GetComponent<ScoreController>();
        input = GameObject.Find("InputController").GetComponent<InputController>();

        playerAnimationBehavior = gameObject.GetComponent<PlayerAnimationBehavior>();

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
            playerAnimationBehavior.HandleDamageAnimations(e.newValue);
        }

    }


    public IEnumerator IncomingDamageFlash()
    {
        gameObject.GetComponent<SpriteRenderer>().color = UnityEngine.Color.black;
        yield return new WaitForSeconds(.05f);
        gameObject.GetComponent<SpriteRenderer>().color = UnityEngine.Color.white;
    }

}
