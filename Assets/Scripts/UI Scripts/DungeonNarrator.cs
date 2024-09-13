using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DungeonNarrator : MonoBehaviour
{

    // Defining the narrator as a singleton
    public static DungeonNarrator Dungeon_Narrator { get; private set; }

    public PlayerStatsManager Player_Stats;


    private UIDocument narrator_doc;
    private TextElement text_narrator_top;
    private TextElement text_narrator;

    private TextElement hp_text;

    private void Awake()
    {
        if (Dungeon_Narrator != null && Dungeon_Narrator != this)
        {
            Destroy(this);
        }
        else
        {
            Dungeon_Narrator = this;
        }



        narrator_doc = this.GetComponent<UIDocument>();
        text_narrator = narrator_doc.rootVisualElement.Q("DungeonNarratorText") as TextElement;
        text_narrator_top = narrator_doc.rootVisualElement.Q("DungeonNarratorTextTop") as TextElement;
        hp_text = narrator_doc.rootVisualElement.Q("Health") as TextElement;

        ClearDungeonNarratorText();

    }

    private void OnEnable()
    {
        Player_Stats.OnHealthChanged += Stats_HealthChanged;
    }

    private void OnDisable()
    {
        Player_Stats.OnHealthChanged -= Stats_HealthChanged;
    }

    private void Stats_HealthChanged(object sender, PlayerStatsManager.HP_Args e)
    {
        hp_text.text = "HP: " + e.newValue;
    }

    public void SetDungeonNarratorText(string message)
    {
        text_narrator_top.text = message;
        text_narrator.text = "";
    }

    public void ClearDungeonNarratorText()
    {
        text_narrator.text = "";
        text_narrator_top.text = "";
    }

    public void AddDungeonNarratorText(string message)
    {

        //move text down from the highlighted spot
        text_narrator.text = text_narrator_top.text + "\n\n" + text_narrator.text;

        //set highlighted text to message
        text_narrator_top.text = message;

        if (text_narrator.text.Length > 2500)
        {
            text_narrator.text = text_narrator.text.Remove(2000);
        }
    }


}
