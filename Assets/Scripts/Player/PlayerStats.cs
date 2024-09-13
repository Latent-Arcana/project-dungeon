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

    private ScoreController scoreController;

    void Awake(){
        scoreController = GameObject.Find("ScoreController").GetComponent<ScoreController>();
    }

    void OnEnable()
    {
        Stats_Manager.OnHealthChanged += Stats_ModifyHP;
    }

    void OnDisable()
    {
        Stats_Manager.OnHealthChanged -= Stats_ModifyHP;
    }

    private void Stats_ModifyHP(object sender, HP_Args e)
    {
        if(e.newValue <= 0){
            scoreController.SetFinalScore();
        }
        StartCoroutine(IncomingDamageFlash());
    }

    public IEnumerator IncomingDamageFlash()
    {
        gameObject.GetComponent<SpriteRenderer>().color = UnityEngine.Color.black;
        yield return new WaitForSeconds(.05f);
        gameObject.GetComponent<SpriteRenderer>().color = UnityEngine.Color.white;
    }
}
