using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using static DungeonNarrator;
public class GhostProjectileBehavior : ProjectileBehavior
{

    bool awake = false;

    public int moveCountMax;
    public int currentMoveCount = 0;

    public override void Move(Vector3 currentPlayerPosition)
    {

        if (currentMoveCount >= moveCountMax)
        {
            awake = false;
            StartCoroutine(FadeOutGhost(destroyGhost: true));
        }

        else
        {
            float distance = Vector3.Distance(transform.position, currentPlayerPosition);

            // If we're just now in range of the player
            if (distance <= 5.5f && awake == false)
            {
                awake = true;
                StartCoroutine(FadeInGhost());
            }

            if (distance > 5.5f && awake == true)
            {
                awake = false;
                StartCoroutine(FadeOutGhost(destroyGhost: false));
            }


            Physics2D.SyncTransforms();

            Vector3 targetDir = GetDirectionToTarget(currentPlayerPosition);


            Vector3 potentialPosition = gameObject.transform.position + targetDir;

            Collider2D checkCollision = CheckPosition(potentialPosition);

            if (checkCollision == null || checkCollision.name == "Main Tilemap")
            {
                currentMoveCount++;
                gameObject.transform.position += targetDir;
                Physics2D.SyncTransforms();
            }


            else if (checkCollision.name == "Player")
            {
                Attack(playerAttacked: false);
                Destroy(gameObject);
            }

            else
            {
                List<Vector3> availablePositions = new List<Vector3>();

                // check all directions and return list of directions that are available
                foreach (Vector3 position in borderPositions)
                {
                    potentialPosition = (Vector2)position;

                    checkCollision = CheckPosition(potentialPosition);

                    if (checkCollision == null)
                    {
                        availablePositions.Add(potentialPosition);
                    }
                }

                // Move in one of those directions randomly
                if (availablePositions.Count > 0)
                {
                    int indexOfDirection = UnityEngine.Random.Range(0, availablePositions.Count);

                    gameObject.transform.position = availablePositions[indexOfDirection];

                    currentMoveCount++;

                }

            }
        }

    }

    public override void Attack(bool playerAttacked)
    {
        if (awake)
        {
            Player_Stats.SetHP(Player_Stats.HP - 3, sourceObjectName: "Ghost");
            HandleArmorDurability();
            Dungeon_Narrator.AddDungeonNarratorText($"Your stamina is drained by the spirit's ice-cold spectral claws.");
        }

    }


    IEnumerator FadeInGhost()
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.0f);

        spriteRenderer.enabled = true;

        yield return new WaitForSeconds(.05f);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.2f);
        yield return new WaitForSeconds(.05f);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.4f);
        yield return new WaitForSeconds(.05f);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.6f);
        yield return new WaitForSeconds(.05f);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.8f);
        yield return new WaitForSeconds(.05f);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);

        yield return new WaitForEndOfFrame();
    }

    IEnumerator FadeOutGhost(bool destroyGhost)
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
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);

        yield return new WaitForEndOfFrame();

        if (destroyGhost)
        {
            Destroy(gameObject);
        }
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {

        // if (collision.gameObject.tag == "room" && collision.gameObject.GetComponentInParent<Room>() != null && gameObject != null && behaviorState != BehaviorState.Dead)
        // {
        //     behaviorState = BehaviorState.Idle;
        //     currentRoom = collision.gameObject.GetComponentInParent<Room>().roomId;
        // }

    }

}
