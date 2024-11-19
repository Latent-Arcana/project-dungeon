using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeObjectBehavior : MonoBehaviour
{
    bool alreadyBuffedPlayer = false;
    bool alreadyHealedPlayer = false;
    public int HealPlayer(int currentHP, int maximumHP)
    {
        if (!alreadyHealedPlayer)
        {
            int healAmount = (maximumHP / 6) + currentHP;

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

    public int BuffPlayer(int currentMaxHP)
    {
        if (!alreadyBuffedPlayer)
        {
            int buffPlayerAmount = UnityEngine.Random.Range(2, 11) + currentMaxHP;

            alreadyBuffedPlayer = true;

            DungeonNarrator.Dungeon_Narrator.AddDungeonNarratorText($"The bed gives you a sense of warmth...");

            return buffPlayerAmount;

        }

        else
        {
            DungeonNarrator.Dungeon_Narrator.AddDungeonNarratorText($"You feel restless. The bed is cold.");

            return currentMaxHP;
        }
    }
}
