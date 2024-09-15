using System;
using System.Collections;
using System.Collections.Generic;
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

    void Awake(){
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
        if(e.newValue <= 0 && !DEBUG_GOD_MODE){

            StartCoroutine(PlayerDeath());
        }
        StartCoroutine(IncomingDamageFlash());
    }

    public IEnumerator IncomingDamageFlash()
    {
        gameObject.GetComponent<SpriteRenderer>().color = UnityEngine.Color.black;
        yield return new WaitForSeconds(.05f);
        gameObject.GetComponent<SpriteRenderer>().color = UnityEngine.Color.white;
    }

     public IEnumerator PlayerDeath()
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        input.ToggleMovement();
        spriteRenderer.color = new Color(255f, 255f, 255f, 1.0f);
        spriteRenderer.flipY = true;
        yield return new WaitForSeconds(.20f);
        spriteRenderer.color= new Color(255f, 255f, 255f, 0.75f);
        spriteRenderer.flipY = true;
        yield return new WaitForSeconds(.20f);
        spriteRenderer.color = new Color(255f, 255f, 255f, 0.5f);
        spriteRenderer.flipY = true;
        yield return new WaitForSeconds(.20f);
        spriteRenderer.color = new Color(255f, 255f, 255f, 0.25f);
        spriteRenderer.flipY = true;
        yield return new WaitForSeconds(.20f);
        spriteRenderer.color = new Color(255f, 255f, 255f, 0.0f);
        spriteRenderer.flipY = true;
        yield return new WaitForSeconds(.20f);
        scoreController.SetFinalScore();
    }
}
