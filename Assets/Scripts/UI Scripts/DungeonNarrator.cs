using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DungeonNarrator : MonoBehaviour
{

    // Defining the narrator as a singleton
    public static DungeonNarrator Dungeon_Narrator { get; private set; }

    public PlayerStatsManager Player_Stats;


    private UIDocument narrator_doc;

    private VisualElement dungeon_narrator_container;

    private TextElement text_narrator_top;
    private TextElement text_narrator;

    private TextElement hp_text;
    private TextElement ap_text;
    private TextElement agi_text;
    private TextElement spd_text;
    private TextElement str_text;

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
        dungeon_narrator_container = narrator_doc.rootVisualElement.Q("DungeonNarratorContainer");

        InitializeStatsUI();


        ClearDungeonNarratorText();

    }

    private void InitializeStatsUI()
    {
        hp_text = narrator_doc.rootVisualElement.Q("Health") as TextElement;
        hp_text.text = "HP: " + Player_Stats.HP + " / " + Player_Stats.MAX_HP;

        ap_text = narrator_doc.rootVisualElement.Q("Armor") as TextElement;
        ap_text.text = "AP: " + Player_Stats.AP;

        agi_text = narrator_doc.rootVisualElement.Q("Agility") as TextElement;
        agi_text.text = "AGI: " + Player_Stats.AGI;

        str_text = narrator_doc.rootVisualElement.Q("Strength") as TextElement;
        str_text.text = "STR: " + Player_Stats.STR;

        spd_text = narrator_doc.rootVisualElement.Q("Speed") as TextElement;
        spd_text.text = "SPD: " + Player_Stats.SPD;

    }

    private void OnEnable()
    {
        Player_Stats.OnHealthChanged += Stats_HealthChanged;
        Player_Stats.OnAgilityChanged += Stats_AgilityChanged;
        Player_Stats.OnStrengthChanged += Stats_StrengthChanged;
        Player_Stats.OnSpeedChanged += Stats_SpeedChanged;
        Player_Stats.OnArmorPointsChanged += Stats_ArmorPointsChanged;
        Player_Stats.OnMaxHealthChanged += Stats_MaxHealthChanged;
    }

    private void OnDisable()
    {
        Player_Stats.OnHealthChanged -= Stats_HealthChanged;
        Player_Stats.OnAgilityChanged -= Stats_AgilityChanged;
        Player_Stats.OnStrengthChanged -= Stats_StrengthChanged;
        Player_Stats.OnSpeedChanged -= Stats_SpeedChanged;
        Player_Stats.OnArmorPointsChanged -= Stats_ArmorPointsChanged;
        Player_Stats.OnMaxHealthChanged -= Stats_MaxHealthChanged;
    }

    private void Stats_HealthChanged(object sender, PlayerStatsManager.Stats_Args e)
    {
        // if we just died...
        hp_text.text = "HP: " + e.newValue;

        hp_text.text += " / " + Player_Stats.MAX_HP;
    }

    private void Stats_MaxHealthChanged(object sender, PlayerStatsManager.Stats_Args e)
    {
        string hp_text_prepend = hp_text.text.Split('/')[0];

        hp_text.text = hp_text_prepend + "/ " + e.newValue;
    }


    private void Stats_AgilityChanged(object sender, PlayerStatsManager.Stats_Args e)
    {
        agi_text.text = "AGI: " + e.newValue;
    }

    private void Stats_StrengthChanged(object sender, PlayerStatsManager.Stats_Args e)
    {
        str_text.text = "STR: " + e.newValue;
    }

    private void Stats_SpeedChanged(object sender, PlayerStatsManager.Stats_Args e)
    {
        spd_text.text = "SPD: " + e.newValue;
    }

    private void Stats_ArmorPointsChanged(object sender, PlayerStatsManager.Stats_Args e)
    {
        ap_text.text = "AP: " + e.newValue;
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

    /// <summary>
    /// Called when in the Map menu, which does not fully cover the Dungeon Narrator
    /// </summary>
    public void EnableDungeonNarrator()
    {
        dungeon_narrator_container.style.display = DisplayStyle.Flex;
    }

    /// <summary>
    /// Called when in the Map menu, which does not fully cover the Dungeon Narrator
    /// </summary>
    public void DisableDungeonNarrator()
    {
        dungeon_narrator_container.style.display = DisplayStyle.None;

    }
}
