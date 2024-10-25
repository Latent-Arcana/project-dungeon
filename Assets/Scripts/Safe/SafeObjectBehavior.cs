using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeObjectBehavior : MonoBehaviour
{
    bool alreadyHealedPlayer = false;
    public int HealPlayer(int currentHP, int maximumHP)
    {
        if (!alreadyHealedPlayer)
        {
            int healAmount = (maximumHP / 2) + currentHP;

            healAmount = Math.Min(healAmount, maximumHP);

            alreadyHealedPlayer = true;

            DungeonNarrator.Dungeon_Narrator.AddDungeonNarratorText($"You rest and feel rejuvenated...");

            return healAmount;

        }

        else
        {
            DungeonNarrator.Dungeon_Narrator.AddDungeonNarratorText($"You feel restless. The bed is cold.");

            return currentHP;
        }
    }
}
