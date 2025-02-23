using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeObjectBehavior : MonoBehaviour
{
    public bool alreadyBuffedPlayer = false;
    public bool alreadyHealedPlayer = false;

    [SerializeField]
    public Sprite usedSprite;
    public Sprite mirroredUsedSprite;
    public int HealPlayer(int currentHP, int maximumHP)
    {
        if (!alreadyHealedPlayer)
        {
            int healAmount = (maximumHP / 6) + currentHP;

            healAmount = Math.Min(healAmount, maximumHP);

            alreadyHealedPlayer = true;

            DungeonNarrator.Dungeon_Narrator.AddDungeonNarratorText($"You rest and feel rejuvenated...");

            SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
            ObjectBehavior objectBehavior = gameObject.GetComponent<ObjectBehavior>();
            
            sr.sprite = objectBehavior.flipped ? mirroredUsedSprite : usedSprite;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.45f);

            return healAmount;

        }

        else
        {
            DungeonNarrator.Dungeon_Narrator.AddDungeonNarratorText($"You've already rested here...");

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

            SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
            ObjectBehavior objectBehavior = gameObject.GetComponent<ObjectBehavior>();
            
            sr.sprite = objectBehavior.flipped ? mirroredUsedSprite : usedSprite;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.45f);


            return buffPlayerAmount;

        }

        else
        {
            DungeonNarrator.Dungeon_Narrator.AddDungeonNarratorText($"You feel restless. The bed is cold.");

            return currentMaxHP;
        }
    }
}
