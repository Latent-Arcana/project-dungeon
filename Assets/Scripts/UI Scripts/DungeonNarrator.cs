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
    private PlayerInventory playerInventory;


    private UIDocument narrator_doc;

    private VisualElement dungeon_narrator_container;

    private TextElement text_narrator_top;
    private TextElement text_narrator;

    private TextElement hp_text;
    private TextElement ap_text;
    private TextElement agi_text;
    private TextElement spd_text;
    private TextElement str_text;

    private GameStats gameStats;

    private VisualElement screenOverlay;
    float screenFadeElapsedTime = 0f;
    float fadeInDuration = 1f;
    float fadeOutDuration = .65f;
    bool screenFadeCompleted = true;

    //Durability
    private VisualElement EquippedArmorDurability;
    private VisualElement EquippedArmor;
    private VisualElement EquippedWeaponDurability;
    private VisualElement EquippedWeapon;

    // Durability Sprites //
    [SerializeField]
    Sprite dur5;
    [SerializeField]
    Sprite dur4;
    [SerializeField]
    Sprite dur3;
    [SerializeField]
    Sprite dur2;
    [SerializeField]
    Sprite dur1;


    void Start()
    {
        gameStats = GameObject.Find("GameStats").GetComponent<GameStats>();

    }

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
        playerInventory = GameObject.Find("Player").GetComponent<PlayerInventory>();

        narrator_doc = this.GetComponent<UIDocument>();
        text_narrator = narrator_doc.rootVisualElement.Q("DungeonNarratorText") as TextElement;
        text_narrator_top = narrator_doc.rootVisualElement.Q("DungeonNarratorTextTop") as TextElement;
        dungeon_narrator_container = narrator_doc.rootVisualElement.Q("DungeonNarratorContainer");

        screenOverlay = narrator_doc.rootVisualElement.Q("ScreenOverlay");
        screenOverlay.style.opacity = 1f;


        //Initialize Durability in stats bar
        EquippedArmorDurability = narrator_doc.rootVisualElement.Q("EquippedArmorDurability");
        EquippedArmor = narrator_doc.rootVisualElement.Q("EquippedArmor");
        EquippedWeaponDurability = narrator_doc.rootVisualElement.Q("EquippedWeaponDurability");
        EquippedWeapon = narrator_doc.rootVisualElement.Q("EquippedWeapon");

        InitializeStatsUI();

        ClearDungeonNarratorText();

    }

    private void Update()
    {
        if (!screenFadeCompleted)
        {
            FadeScreenOnStart();
        }

        UpdateEquipmentIcons();
    }


    public void UpdateEquipmentIcons()
    {
        //Check/Assign Equipped Item
        //Armor
        int armorindex = playerInventory.GetEquippedArmor(); //int of index 

        if (armorindex >= 0)
        {
            Sprite sprt = Resources.Load<Sprite>(playerInventory.inventory.items[armorindex].image);
            EquippedArmor.style.backgroundImage = new StyleBackground(sprt);

            Sprite s = SwapDurabilitySprite(playerInventory.GetDurability(armorindex));
            EquippedArmorDurability.style.backgroundImage = new StyleBackground(s);

        }
        else //nothing equipped
        {
            EquippedArmor.style.backgroundImage = null;
            EquippedArmorDurability.style.backgroundImage = new StyleBackground(dur5);
        }

        //Weapon
        int weaponIndex = playerInventory.GetEquippedWeapon(); //int of index 

        if (weaponIndex >= 0)
        {
            Sprite sprt = Resources.Load<Sprite>(playerInventory.inventory.items[weaponIndex].image);
            EquippedWeapon.style.backgroundImage = new StyleBackground(sprt);

            Sprite s = SwapDurabilitySprite(playerInventory.GetDurability(weaponIndex));
            EquippedWeaponDurability.style.backgroundImage = new StyleBackground(s);
        }
        else //nothing equipped
        {
            EquippedWeapon.style.backgroundImage = null;
            EquippedWeaponDurability.style.backgroundImage = new StyleBackground(dur5);

        }
    }

    Sprite SwapDurabilitySprite(int durabilityLevel)
    {
        Sprite dur;

        //case 0-5
        switch (durabilityLevel)
        {
            case 5:
                dur = dur5;
                break;
            case 4:
                dur = dur4;
                break;
            case 3:
                dur = dur3;
                break;
            case 2:
                dur = dur2;
                break;
            case 1:
                dur = dur1;
                break;
            default:
                dur = null;
                break;
        }

        return dur;
    }

    public void InitiateScreenFadeIn()
    {
        screenFadeCompleted = false;
    }
    public void FadeScreenOnStart()
    {
        screenFadeElapsedTime += Time.deltaTime;
        float normalizedTime = Mathf.Clamp01(screenFadeElapsedTime / fadeInDuration);

        // Gradually fade out the overlay
        screenOverlay.style.opacity = Mathf.Lerp(1, 0, normalizedTime);

        // Check if the fade is completed
        if (normalizedTime >= 1f)
        {
            screenFadeCompleted = true;
            screenFadeElapsedTime = 0f; // Reset for the next effect
            screenOverlay.style.display = DisplayStyle.None;
        }
    }
    public IEnumerator FadeScreenOnExit()
    {
        screenOverlay.style.display = DisplayStyle.Flex;
        float fadeTime = 0f;
        while (fadeTime < fadeOutDuration)
        {
            fadeTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(fadeTime / fadeOutDuration);

            // Gradually fade in the overlay
            screenOverlay.style.opacity = Mathf.Lerp(0, 1, normalizedTime);

            yield return null;
        }

        // SceneManager.LoadScene("Main Menu");

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

    public void AddBookshelfText(Enums.RoomType roomType, bool bookDropped)
    {
        float randChoice = UnityEngine.Random.value;

        if (bookDropped)
        {

            string bookshelfText = "";

            if (randChoice <= .2f)
            {
                bookshelfText = "You find a glowing tome. Somewhere, a room was marked";
            }

            else if (randChoice > .2f && randChoice < .5f)
            {
                bookshelfText = "You find a floating book. You sense a room somewhere, you mark it";
            }

            else
            {
                bookshelfText = "Upon the shelf sits a speaking scroll... It tells you of a room. You record it as";
            }

            if (roomType == Enums.RoomType.Danger)
            {
                bookshelfText += " danger.";
            }
            else if (roomType == Enums.RoomType.Lore)
            {
                bookshelfText += " lore.";
            }
            else if (roomType == Enums.RoomType.Safe)
            {
                bookshelfText += " safe.";
            }

            AddDungeonNarratorText(bookshelfText);
        }
        else
        {
            if (randChoice <= .4f)
            {
                AddDungeonNarratorText("The books on this shelf are all unreadable.");

            }
            else if (randChoice <= .8f)
            {
                AddDungeonNarratorText("The books here have all been destroyed.");
            }
            else
            {
                AddDungeonNarratorText("These books are all written in indecipherable languages...");
            }

        }
    }

    /// <summary>
    /// Called to enable unique logic for combat text based on the weapons that are being used during combat
    /// </summary>
    public void AddPlayerAttackText(Enums.WeaponType weaponType, Enums.EnemyType enemyType, int damageDealt)
    {

        string damageVerb;
        float damageVerbChoice = UnityEngine.Random.value;
        string pointsText = damageDealt == 1 ? "point" : "points";

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


        AddDungeonNarratorText($"You {damageVerb} the {enemyType} for {damageDealt} {pointsText} of damage.");
    }

    /// <summary>
    /// Called to enable unique logic for combat text based on the weapons that are being used during combat
    /// </summary>
    public void AddEnemyAttackText(Enums.EnemyType enemyType, int damageDealt)
    {

        string damageVerb, enemyWeaponName;
        float damageVerbChoice = UnityEngine.Random.value;

        string pointsText = damageDealt == 1 ? "point" : "points";

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

        AddDungeonNarratorText($"The {enemyType} {damageVerb} you with its {enemyWeaponName} for {damageDealt} {pointsText} of damage.");

    }

    /// <summary>
    /// Take in a source object to determine what type of death the player experienced. Print text based on what killed them.
    /// </summary>
    /// <param name="sourceObject"></param>
    public void AddPlayerDeathText(string killedBy)
    {
        string deathText;

        switch (killedBy)
        {
            case "Spikes":
                deathText = $"They were impaled by spikes.";
                break;

            case "Kobold":
                deathText = $"They were torn apart by a kobold.";
                break;

            case "Skeleton":
                deathText = $"A skeleton cut them down.";
                break;

            case "Goblin":
                deathText = $"A goblin fatally stabbed them.";
                break;

            case "Bugbear":
                deathText = $"A bugbear struck them down.";
                break;

            case "Spirit":
                deathText = $"A spirit sapped their life away.";
                break;

            case "Ghost":
                deathText = $"A ghost took their soul.";
                break;

            case "Tesla":
                deathText = $"They were shocked to death by an electric bolt.";
                break;

            case "Gas":
                deathText = $"They chocked to death on toxic gas.";
                break;

            default:
                deathText = $"They were killed by: {killedBy}";
                break;
        }

        gameStats.SetDeathText("This cartographer was lost... " + deathText);

    }

    /// <summary>
    /// Use unique text based on the type of enemy and the type of weapon that killed them
    /// </summary>
    /// <param name="enemyType"></param>
    /// <param name="weaponType"></param>
    public void AddEnemyDeathText(Enums.EnemyType enemyType, Enums.WeaponType weaponType)
    {

        string killVerb;
        string deathClause;

        switch (weaponType)
        {
            case Enums.WeaponType.Default:
                killVerb = "kill";
                break;
            case Enums.WeaponType.LightSlash:
                killVerb = "pierce";
                break;
            case Enums.WeaponType.HeavySlash:
                killVerb = "slash";
                break;
            case Enums.WeaponType.HeavyBlunt:
                killVerb = "crush";
                break;
            case Enums.WeaponType.HeavySpecial:
                killVerb = "cleave";
                break;
            case Enums.WeaponType.LightSpecial:
                killVerb = "stab";
                break;
            case Enums.WeaponType.MiscSpecial:
                killVerb = "punch";
                break;
            default:
                killVerb = "kill";
                break;
        }

        switch (enemyType)
        {
            case Enums.EnemyType.Skeleton:

                if (weaponType == Enums.WeaponType.HeavySlash || weaponType == Enums.WeaponType.HeavySpecial)
                {
                    deathClause = " in half.";
                }
                else if (weaponType == Enums.WeaponType.HeavyBlunt || weaponType == Enums.WeaponType.MiscSpecial)
                {
                    deathClause = " into dust.";
                }
                else if (weaponType == Enums.WeaponType.LightSlash || weaponType == Enums.WeaponType.LightSpecial)
                {
                    deathClause = "'s bones. It crumbles to the floor.";
                }
                else
                {
                    deathClause = ".";
                }
                break;

            case Enums.EnemyType.Kobold:

                if (weaponType == Enums.WeaponType.HeavySlash || weaponType == Enums.WeaponType.HeavySpecial)
                {
                    deathClause = " in half.";
                }
                else if (weaponType == Enums.WeaponType.HeavyBlunt || weaponType == Enums.WeaponType.MiscSpecial)
                {
                    deathClause = " into a bloody mess.";
                }
                else if (weaponType == Enums.WeaponType.LightSlash || weaponType == Enums.WeaponType.LightSpecial)
                {
                    deathClause = "'s scaly armor. It falls to the floor, shrieking.";
                }
                else
                {
                    deathClause = ".";
                }
                break;

            case Enums.EnemyType.Goblin:

                if (weaponType == Enums.WeaponType.HeavySlash || weaponType == Enums.WeaponType.HeavySpecial)
                {
                    deathClause = " in half.";
                }
                else if (weaponType == Enums.WeaponType.HeavyBlunt || weaponType == Enums.WeaponType.MiscSpecial)
                {
                    deathClause = " into a bloody mess.";
                }
                else if (weaponType == Enums.WeaponType.LightSlash || weaponType == Enums.WeaponType.LightSpecial)
                {
                    deathClause = "'s tattered armor. It falls to the floor.";
                }
                else
                {
                    deathClause = ".";
                }

                break;
            case Enums.EnemyType.Bugbear:
                if (weaponType == Enums.WeaponType.HeavySlash || weaponType == Enums.WeaponType.HeavySpecial)
                {
                    deathClause = " in half.";
                }
                else if (weaponType == Enums.WeaponType.HeavyBlunt || weaponType == Enums.WeaponType.MiscSpecial)
                {
                    deathClause = " into a bloody mess.";
                }
                else if (weaponType == Enums.WeaponType.LightSlash || weaponType == Enums.WeaponType.LightSpecial)
                {
                    deathClause = "'s furry hide. It collapses with a loud crash.";
                }
                else
                {
                    deathClause = ".";
                }
                break;

            case Enums.EnemyType.Spirit:

                if (weaponType == Enums.WeaponType.HeavySlash || weaponType == Enums.WeaponType.HeavySpecial)
                {
                    deathClause = " in half.";
                }
                else if (weaponType == Enums.WeaponType.HeavyBlunt || weaponType == Enums.WeaponType.MiscSpecial)
                {
                    deathClause = " into a bloody mess.";
                }
                else if (weaponType == Enums.WeaponType.LightSlash || weaponType == Enums.WeaponType.LightSpecial)
                {
                    deathClause = "'s furry hide. It collapses with a loud crash.";
                }
                else
                {
                    deathClause = ".";
                }

                break;
            default:
                killVerb = "kill";
                deathClause = ".";
                break;
        }

        AddDungeonNarratorText($"You {killVerb} the {enemyType}{deathClause}");

    }

    /// <summary>
    /// Use unique text based on the enemy type for how the enemy misses the player
    /// </summary>
    /// <param name="enemyType"></param>
    public void AddEnemyMissText(Enums.EnemyType enemyType)
    {

        string missClause;

        switch (enemyType)
        {
            case Enums.EnemyType.Skeleton:
                missClause = "'s rusty spear deflects off of your armor.";
                break;
            case Enums.EnemyType.Kobold:
                missClause = "'s snarling teeth fail to penetrate your armor.";
                break;
            case Enums.EnemyType.Goblin:
                missClause = "'s cracked scimitar bounces off of your armor.";
                break;
            case Enums.EnemyType.Bugbear:
                missClause = "'s hulking cleaver narrowly misses you.";
                break;
            case Enums.EnemyType.Spirit:
                missClause = "'s spectral claws dissipate before hitting you.";
                break;
            default:
                missClause = " misses you.";
                break;
        }

        AddDungeonNarratorText($"The {enemyType}{missClause}");

    }


    /// <summary>
    /// Use unique text based on the enemy type and weapon type for how the enemy misses the player
    /// </summary>
    /// <param name="enemyType"></param>
    /// <param name="weaponType"></param>
    public void AddPlayerMissText(Enums.EnemyType enemyType, Enums.WeaponType weaponType)
    {

        string weaponDescriptor;
        string enemyDescriptor;
        string missClause;

        switch (weaponType)
        {
            case Enums.WeaponType.Default:
                weaponDescriptor = "weapon";
                break;
            case Enums.WeaponType.LightSlash:
                weaponDescriptor = "sword";
                break;
            case Enums.WeaponType.HeavySlash:
                weaponDescriptor = "weapon's edge";
                break;
            case Enums.WeaponType.HeavyBlunt:
                weaponDescriptor = "mace";
                break;
            case Enums.WeaponType.HeavySpecial:
                weaponDescriptor = "heavy blade";
                break;
            case Enums.WeaponType.LightSpecial:
                weaponDescriptor = "blade";
                break;
            case Enums.WeaponType.MiscSpecial:
                weaponDescriptor = "fist";
                break;
            default:
                weaponDescriptor = "weapon";
                break;
        }

        switch (enemyType)
        {
            case Enums.EnemyType.Skeleton:

                if (weaponType == Enums.WeaponType.HeavySlash || weaponType == Enums.WeaponType.HeavySpecial)
                {
                    missClause = " skids across";
                    enemyDescriptor = "'s ribs";
                }
                else if (weaponType == Enums.WeaponType.HeavyBlunt || weaponType == Enums.WeaponType.MiscSpecial)
                {
                    missClause = " swings past";
                    enemyDescriptor = "'s skull";
                }
                else if (weaponType == Enums.WeaponType.LightSlash || weaponType == Enums.WeaponType.LightSpecial)
                {
                    missClause = " clangs against";
                    enemyDescriptor = "'s bones";
                }
                else
                {
                    enemyDescriptor = "";
                    missClause = " misses";
                }
                break;

            case Enums.EnemyType.Kobold:

                if (weaponType == Enums.WeaponType.HeavySlash || weaponType == Enums.WeaponType.HeavySpecial)
                {
                    missClause = " skids across";
                    enemyDescriptor = "'s scales";
                }
                else if (weaponType == Enums.WeaponType.HeavyBlunt || weaponType == Enums.WeaponType.MiscSpecial)
                {
                    missClause = " swings past";
                    enemyDescriptor = "'s small frame";
                }
                else if (weaponType == Enums.WeaponType.LightSlash || weaponType == Enums.WeaponType.LightSpecial)
                {
                    missClause = " clangs against";
                    enemyDescriptor = "'s scaly armor";
                }
                else
                {
                    enemyDescriptor = "";
                    missClause = " misses";
                }
                break;

            case Enums.EnemyType.Goblin:

                if (weaponType == Enums.WeaponType.HeavySlash || weaponType == Enums.WeaponType.HeavySpecial)
                {
                    missClause = " skids across";
                    enemyDescriptor = "'s tattered armor";
                }
                else if (weaponType == Enums.WeaponType.HeavyBlunt || weaponType == Enums.WeaponType.MiscSpecial)
                {
                    missClause = " swings past";
                    enemyDescriptor = "'s head";
                }
                else if (weaponType == Enums.WeaponType.LightSlash || weaponType == Enums.WeaponType.LightSpecial)
                {
                    missClause = " clangs against";
                    enemyDescriptor = "'s jagged blade";
                }
                else
                {
                    enemyDescriptor = "";
                    missClause = " misses";
                }
                break;
            case Enums.EnemyType.Bugbear:
                if (weaponType == Enums.WeaponType.HeavySlash || weaponType == Enums.WeaponType.HeavySpecial)
                {
                    missClause = " skids across";
                    enemyDescriptor = "'s furry hide";
                }
                else if (weaponType == Enums.WeaponType.HeavyBlunt || weaponType == Enums.WeaponType.MiscSpecial)
                {
                    missClause = " swings past";
                    enemyDescriptor = "'s legs";
                }
                else if (weaponType == Enums.WeaponType.LightSlash || weaponType == Enums.WeaponType.LightSpecial)
                {
                    missClause = " clangs against";
                    enemyDescriptor = "'s claws";
                }
                else
                {
                    enemyDescriptor = "";
                    missClause = " misses";
                }
                break;

            case Enums.EnemyType.Spirit:

                if (weaponType == Enums.WeaponType.HeavySlash || weaponType == Enums.WeaponType.HeavySpecial)
                {
                    missClause = " slides through";
                    enemyDescriptor = "'s ethereal figure";
                }
                else if (weaponType == Enums.WeaponType.HeavyBlunt || weaponType == Enums.WeaponType.MiscSpecial)
                {
                    missClause = " swings past";
                    enemyDescriptor = "'s glowing shape";
                }
                else if (weaponType == Enums.WeaponType.LightSlash || weaponType == Enums.WeaponType.LightSpecial)
                {
                    missClause = " clangs against";
                    enemyDescriptor = "'s glowing claws";
                }
                else
                {
                    enemyDescriptor = "";
                    missClause = " misses";
                }
                break;
            default:
                weaponDescriptor = "attack";
                missClause = " misses";
                enemyDescriptor = "";
                break;
        }

        AddDungeonNarratorText($"Your {weaponDescriptor}{missClause} the {enemyType}{enemyDescriptor}.");

    }


    public void AddPotionConsumeText(Consumable potion)
    {

        string potionBuffPointsText = $"You drink the {potion.itemName}. You gain ";

        List<string> potionBuffs = new List<string>();

        if (potion.AGI > 0)
        {
            if (potion.AGI == 1)
            {
                potionBuffs.Add(potion.AGI + " point of Agility");

            }
            else
            {
                potionBuffs.Add(potion.AGI + " points of Agility");

            }
        }

        if (potion.STR > 0)
        {
            if (potion.STR == 1)
            {
                potionBuffs.Add(potion.STR + " point of Strength");

            }
            else
            {
                potionBuffs.Add(potion.STR + " points of Strength");

            }
        }

        if (potion.SPD > 0)
        {
            if (potion.SPD == 1)
            {
                potionBuffs.Add(potion.SPD + " point of Speed");

            }
            else
            {
                potionBuffs.Add(potion.SPD + " points of Speed");

            }
        }

        if (potion.HP > 0)
        {
            if (potion.permanent)
            {
                potionBuffs.Add(potion.HP + " points of Max HP permanently");
            }
            else
            {
                potionBuffs.Add(potion.HP + " points of HP");
            }
        }

        if (potionBuffs.Count == 1)
        {
            AddDungeonNarratorText(potionBuffPointsText + potionBuffs[0] + ".");
        }

        else if (potionBuffs.Count == 2)
        {
            AddDungeonNarratorText(potionBuffPointsText + potionBuffs[0] + " and " + potionBuffs[1] + ".");
        }

        else
        {
            string narratorText = potionBuffPointsText;

            for (int i = 0; i < potionBuffs.Count; ++i)
            {
                if (i == potionBuffs.Count - 2) // if we're the second to last, we use an oxford comma
                {
                    narratorText += potionBuffs[i] + ", and ";
                }
                else
                {
                    narratorText += potionBuffs[i] + ", ";
                }
            }

            AddDungeonNarratorText(narratorText + ".");

        }

    }

    public void AddWeaponEquipText(Weapon weapon)
    {
        string splitNameArticle = weapon.itemName.Split(" ")[0];

        if (splitNameArticle != null)
        {
            if (splitNameArticle == "The")
            {
                AddDungeonNarratorText("You equip " + weapon.itemName + ".");
            }

            else
            {
                AddDungeonNarratorText("You equip the " + weapon.itemName + ".");
            }
        }


    }

    public void AddWeaponUnequipText(Weapon weapon)
    {

        string splitNameArticle = weapon.itemName.Split(" ")[0];


        if (splitNameArticle != null)
        {
            if (splitNameArticle == "The")
            {
                AddDungeonNarratorText("You put away " + weapon.itemName + ".");
            }

            else
            {
                AddDungeonNarratorText("You put away the " + weapon.itemName + ".");
            }
        }
    }

    public void AddArmorEquipText(Armor armor)
    {
        string splitNameArticle = armor.itemName.Split(" ")[0];

        if (splitNameArticle != null)
        {
            if (splitNameArticle == "The")
            {
                AddDungeonNarratorText("You put on " + armor.itemName + ".");
            }
            else
            {
                AddDungeonNarratorText("You put on the " + armor.itemName + ".");
            }
        }

    }
    public void AddArmorUnequipText(Armor armor)
    {
        string splitNameArticle = armor.itemName.Split(" ")[0];

        if (splitNameArticle != null)
        {
            if (splitNameArticle == "The")
            {
                AddDungeonNarratorText("You take off " + armor.itemName + ".");
            }
            else
            {
                AddDungeonNarratorText("You take off the " + armor.itemName + ".");
            }
        }
    }

    public void AddItemDropText(Item item)
    {

        string splitNameArticle = item.itemName.Split(" ")[0];

        if (splitNameArticle != null)
        {
            if (splitNameArticle == "The")
            {
                AddDungeonNarratorText("You drop " + item.itemName + ". The aether swallows it.");
            }

            else
            {
                AddDungeonNarratorText("You drop the " + item.itemName + ". It is swallowed by the aether.");
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
