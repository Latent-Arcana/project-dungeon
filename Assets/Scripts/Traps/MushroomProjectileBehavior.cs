using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static DungeonNarrator;

public class MushroomProjectileBehavior : ProjectileBehavior
{

    public override void Attack(bool playerAttacked)
    {
        Player_Stats.SetHP(Player_Stats.HP - 3, sourceObjectName: "Gas");
        HandleArmorDurability();
        Dungeon_Narrator.AddDungeonNarratorText($"The mushroom's toxic gas chokes you for 3 damage.");
    }

}