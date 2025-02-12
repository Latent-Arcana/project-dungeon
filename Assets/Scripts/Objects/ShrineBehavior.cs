using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineBehavior : MonoBehaviour
{

    [SerializeField]
    public Enums.ShrineType shrineType;

    public bool hasTriggered = false;


    public void Bless(PlayerStatsManager Player_Stats, PlayerInventory playerInventory = null)
    {

        if (hasTriggered)
        {
            return;
        }

        switch (shrineType)
        {
            case Enums.ShrineType.Strength:

                Player_Stats.SetSTR(Player_Stats.STR + 1);
                DungeonNarrator.Dungeon_Narrator.AddDungeonNarratorText("Val-gro-dal grants you strength.");
                
                hasTriggered = true;
                StartCoroutine(FadeOutShrine());
                break;

            case Enums.ShrineType.Speed:
                Player_Stats.SetSPD(Player_Stats.SPD + 1);
                DungeonNarrator.Dungeon_Narrator.AddDungeonNarratorText("Zixithisus grants you speed.");
                
                hasTriggered = true;
                StartCoroutine(FadeOutShrine());
                break;

            case Enums.ShrineType.Agility:
                Player_Stats.SetAGI(Player_Stats.AGI + 1);
                DungeonNarrator.Dungeon_Narrator.AddDungeonNarratorText("Al'qunaq grants you agility.");

                hasTriggered = true;
                StartCoroutine(FadeOutShrine());
                break;

            case Enums.ShrineType.Armor:
                if (playerInventory != null)
                {
                    int equippedArmor = playerInventory.GetEquippedArmor();

                    if (equippedArmor >= 0)
                    {
                        Armor armorData = playerInventory.inventory.items[equippedArmor] as Armor;


                        if (playerInventory.GetDurability(equippedArmor) == 5)
                        {
                            DungeonNarrator.Dungeon_Narrator.AddDungeonNarratorText("Your armor does not need to be repaired.");

                        }
                        else
                        {
                            playerInventory.SetDurability(equippedArmor, armorData.DUR);
                            DungeonNarrator.Dungeon_Narrator.AddDungeonNarratorText("You repair your armor.");
                            hasTriggered = true;
                            StartCoroutine(FadeOutShrine());
                        }
                    }
                    else
                    {
                        DungeonNarrator.Dungeon_Narrator.AddDungeonNarratorText("You are not wearing any armor.");
                    }
                }
                break;

            case Enums.ShrineType.Weapon:
                if (playerInventory != null)
                {
                    int equippedWeapon = playerInventory.GetEquippedWeapon();
                    if (equippedWeapon >= 0)
                    {
                        Weapon weaponData = playerInventory.inventory.items[equippedWeapon] as Weapon;


                        if (playerInventory.GetDurability(equippedWeapon) == 5)
                        {
                            DungeonNarrator.Dungeon_Narrator.AddDungeonNarratorText("Your weapon does not need to be repaired.");

                        }

                        else
                        {
                            playerInventory.SetDurability(equippedWeapon, weaponData.DUR);
                            DungeonNarrator.Dungeon_Narrator.AddDungeonNarratorText("You repair your weapon.");
                            hasTriggered = true;
                            StartCoroutine(FadeOutShrine());
                        }
                    }
                    else
                    {
                        DungeonNarrator.Dungeon_Narrator.AddDungeonNarratorText("You are not wielding a weapon.");
                    }

                }
                break;
        }
    }

    IEnumerator FadeOutShrine()
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1.0f);
        yield return new WaitForSeconds(.05f);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.8f);
        yield return new WaitForSeconds(.05f);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.6f);
        yield return new WaitForSeconds(.05f);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.4f);
        yield return new WaitForSeconds(.05f);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.2f);
        yield return new WaitForSeconds(.05f);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.1f);

        yield return new WaitForEndOfFrame();
    }
}
