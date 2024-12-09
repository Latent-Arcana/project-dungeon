using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static DungeonNarrator;

public class TeslaProjectileBehavior : ProjectileBehavior
{

    public override void Attack(bool playerAttacked)
    {
        Player_Stats.SetHP(Player_Stats.HP - 2, sourceObject: gameObject);
        Dungeon_Narrator.AddDungeonNarratorText($"The electric bolt shocks you for 2 damage.");
    }

}