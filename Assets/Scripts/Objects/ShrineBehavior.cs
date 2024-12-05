using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineBehavior : MonoBehaviour
{

    [SerializeField]
    public Enums.ShrineType shrineType;

    private bool hasTriggered = false;


    public void Bless(PlayerStatsManager Player_Stats)
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

                break;

            case Enums.ShrineType.Speed:
                Player_Stats.SetSPD(Player_Stats.SPD + 1);
                DungeonNarrator.Dungeon_Narrator.AddDungeonNarratorText("Zixithisus grants you speed.");


                break;

            case Enums.ShrineType.Agility:
                Player_Stats.SetAGI(Player_Stats.AGI + 1);
                DungeonNarrator.Dungeon_Narrator.AddDungeonNarratorText("Al'qunaq grants you agility.");

                break;
        }

        hasTriggered = true;


        StartCoroutine(FadeOutShrine());
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
