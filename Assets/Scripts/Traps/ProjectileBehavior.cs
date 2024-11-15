using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static DungeonNarrator;

public class ProjectileBehavior : EnemyBehavior
{
    public Vector3 directionOfTravel;

    public GameObject trap;

    public int projectileId;

    public bool isAtSpawn;

    public bool isDestroyed = false;

    public void CheckSpawn(Vector3 playerPosition)
    {
        if (playerPosition == transform.position)
        {
            Attack(playerAttacked: false);
            isDestroyed = true;
            Destroy(gameObject);
        }
    }

    public override void CheckAndAttack(Vector3 prevPosition, Vector3 currentPosition, Vector2 intendedDirection)
    {
        foreach (Vector3 position in borderPositions)
        {
            if (prevPosition == position)
            {

                // Identify whether the player was trying to attack the enemy too
                if (prevPosition == currentPosition && (Vector3)intendedDirection + prevPosition == transform.position)
                {
                    Attack(playerAttacked: true);
                }

                else
                {
                    Attack(playerAttacked: false);
                }

            }
        }
    }

    public override void AssignStats()
    {
        //enemyStats = new EnemyStats("skeleton", 5, 1, 2, 1, 1);
    }

    public override void Input_OnPlayerMoved(object sender, PlayerMovement.MovementArgs e)
    {

        borderPositions[0] = gameObject.transform.position + Vector3.up;
        borderPositions[1] = gameObject.transform.position + Vector3.down;
        borderPositions[2] = gameObject.transform.position + Vector3.right;
        borderPositions[3] = gameObject.transform.position + Vector3.left;

        if (gameObject != null && isDestroyed == false)
        {
            //CheckAndAttack(e.prevPosition, e.position, e.intendedDirection);

            Move(e.position);
        }

    }


    public override void Move(Vector3 currentPlayerPosition)
    {
        Physics2D.SyncTransforms();

        Vector3 targetPosition = directionOfTravel + gameObject.transform.position;

        Collider2D coll = CheckPosition(targetPosition);

        if (coll == null)
        {
            gameObject.transform.position += directionOfTravel;
            Physics2D.SyncTransforms();
        }

        else if (coll.name == "Player")
        {
            Attack(playerAttacked: false);
            isDestroyed = true;
            Destroy(gameObject);
        }

        else if (coll.tag == "projectile")
        {
            isDestroyed = true;
            Destroy(gameObject);
        }

        else
        {
            Die();
        }

    }

    public override void Die()
    {
        if (gameObject != null && isDestroyed == false)
        {
            StartCoroutine(ProjectileDestroy());
        }

    }

    public IEnumerator ProjectileDestroy()
    {
        isDestroyed = true;

        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(255f, 255f, 255f, 1.0f);
        yield return new WaitForSeconds(.10f);
        spriteRenderer.color = new Color(255f, 255f, 255f, 0.75f);
        yield return new WaitForSeconds(.10f);
        spriteRenderer.color = new Color(255f, 255f, 255f, 0.5f);
        yield return new WaitForSeconds(.10f);
        spriteRenderer.color = new Color(255f, 255f, 255f, 0.25f);
        yield return new WaitForSeconds(.10f);
        spriteRenderer.color = new Color(255f, 255f, 255f, 0.0f);
        yield return new WaitForSeconds(.10f);

        yield return new WaitForEndOfFrame();
        Destroy(gameObject);

    }

    public override void Attack(bool playerAttacked)
    {
        Player_Stats.SetHP(Player_Stats.HP - 2);
        Dungeon_Narrator.AddDungeonNarratorText($"You took 2 points of damage from the trap projectile.");
    }


    public override void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "room" && collision.gameObject.GetComponentInParent<Room>() != null && gameObject != null && behaviorState != BehaviorState.Dead)
        {
            behaviorState = BehaviorState.Idle;
            currentRoom = collision.gameObject.GetComponentInParent<Room>().roomId;
        }

    }



}