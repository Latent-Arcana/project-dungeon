using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    /// Called to enable unique logic for combat text based on the weapons that are being used during combat
    /// </summary>
    public void AddPlayerAttackText(Enums.WeaponType weaponType, Enums.EnemyType enemyType, int damageDealt)
    {

        string damageVerb;
        float damageVerbChoice = UnityEngine.Random.value;

        switch (weaponType)
        {
            case Enums.WeaponType.Default:
                damageVerb = "attack";
                break;
            case Enums.WeaponType.HeavySlash:
                damageVerb = damageVerbChoice > 0.5f ? "slash" : "cut";
                break;
            case Enums.WeaponType.HeavyBlunt:
                damageVerb = damageVerbChoice > 0.5f ? "bludgeon" : "bash";
                break;
            case Enums.WeaponType.LightSlash:
                damageVerb = damageVerbChoice > 0.5f ? "slice" : "lacerate";
                break;
            case Enums.WeaponType.HeavySpecial:
                damageVerb = damageVerbChoice > 0.5f ? "cleave" : "split open";
                break;
            case Enums.WeaponType.LightSpecial:
                damageVerb = damageVerbChoice > 0.5f ? "stab" : "puncture";
                break;
            case Enums.WeaponType.MiscSpecial:
                damageVerb = damageVerbChoice > 0.5f ? "jab" : "punch";
                break;
            default:
                damageVerb = "attack";
                break;
        }


        AddDungeonNarratorText($"You {damageVerb} the {enemyType} for {damageDealt} points of damage.");
    }

    /// <summary>
    /// Called to enable unique logic for combat text based on the weapons that are being used during combat
    /// </summary>
    public void AddEnemyAttackText(Enums.EnemyType enemyType, int damageDealt)
    {

        string damageVerb, enemyWeaponName;
        float damageVerbChoice = UnityEngine.Random.value;

        switch (enemyType)
        {
            case Enums.EnemyType.Skeleton:
                damageVerb = damageVerbChoice > 0.5f ? "jabs" : "skewers";
                enemyWeaponName = "rusty spear";
                break;
            case Enums.EnemyType.Kobold:
                damageVerb = damageVerbChoice > 0.5f ? "pokes" : "bites";
                enemyWeaponName = damageVerbChoice > 0.5f ? "shoddy dagger" : "jagged teeth";
                break;
            case Enums.EnemyType.Goblin:
                damageVerb = damageVerbChoice > 0.5f ? "cuts" : "grazes";
                enemyWeaponName = "cracked scimitar";

                break;
            case Enums.EnemyType.Bugbear:
                damageVerb = damageVerbChoice > 0.5f ? "gores" : "scratches";
                enemyWeaponName = damageVerbChoice > 0.5f ? "hulking cleaver" : "sharp claws";

                break;
            case Enums.EnemyType.Spirit:
                damageVerb = damageVerbChoice > 0.5f ? "saps" : "frightens";
                enemyWeaponName = damageVerbChoice > 0.5f ? "spectral claws" : "horrible visage";

                break;
            default:
                damageVerb = "attacks";
                enemyWeaponName = "weapon";
                break;
        }

        AddDungeonNarratorText($"The {enemyType} {damageVerb} you with its {enemyWeaponName} for {damageDealt} points of damage.");

    }


    /// <summary>
    /// Take in a source object to determine what type of death the player experienced. Print text based on what killed them.
    /// </summary>
    /// <param name="sourceObject"></param>
    public void AddPlayerDeathText(GameObject sourceObject)
    {

        ObjectBehavior objectData = sourceObject.GetComponent<ObjectBehavior>();
        EnemyBehavior enemyData = sourceObject.GetComponent<EnemyBehavior>();
        ProjectileBehavior projectileData = sourceObject.GetComponent<ProjectileBehavior>();

        if (objectData != null)
        {
            if (objectData.ObjectType == Enums.ObjectType.Spikes)
            {
                AddDungeonNarratorText($"You were impaled by the spikes. You die.");
            }
            else
            {
                AddDungeonNarratorText($"You were killed by: {objectData.ObjectType}");
            }
        }

        else if (enemyData != null)
        {
            switch (enemyData.enemyStats.EnemyType)
            {
                case Enums.EnemyType.Kobold:
                    AddDungeonNarratorText($"You were torn apart by the kobold.");
                    break;
                case Enums.EnemyType.Goblin:
                    AddDungeonNarratorText($"The goblin fatally stabbed you.");
                    break;
                case Enums.EnemyType.Bugbear:
                    AddDungeonNarratorText($"The bugbear struck you down.");
                    break;
                case Enums.EnemyType.Spirit:
                    AddDungeonNarratorText($"The spirit sapped your life away.");
                    break;
                case Enums.EnemyType.Skeleton:
                    AddDungeonNarratorText($"The skeleton cut you down.");
                    break;

                default:
                    AddDungeonNarratorText($"The {enemyData.enemyStats.EnemyType} struck you down.");
                    break;
            }
        }

        else if (projectileData != null)
        {
            GhostProjectileBehavior ghostProjectileData = projectileData as GhostProjectileBehavior;
            MushroomProjectileBehavior mushroomProjectileData = projectileData as MushroomProjectileBehavior;
            TeslaProjectileBehavior teslaProjectileData = projectileData as TeslaProjectileBehavior;

            if (ghostProjectileData != null)
            {
                AddDungeonNarratorText($"The spirit sapped your life away.");
            }

            else if (mushroomProjectileData != null)
            {
                AddDungeonNarratorText($"You choke to death on the mushroom's toxic gas.");

            }
            else if (teslaProjectileData != null)
            {
                AddDungeonNarratorText($"You were shocked to death by the electric bolt.");

            }
            else
            {
                AddDungeonNarratorText($"You were killed by: {projectileData.gameObject.name}");
            }

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
