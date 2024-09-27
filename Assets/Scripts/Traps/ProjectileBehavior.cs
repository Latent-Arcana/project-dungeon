using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
public class ProjectileBehavior : EnemyBehavior
{
    public Vector3 directionOfTravel;

    public GameObject trap;

    public int projectileId;

    public bool isAtSpawn;


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
        if (this.gameObject != null)
        {
            Physics2D.SyncTransforms();

            borderPositions[0] = gameObject.transform.position + Vector3.up;
            borderPositions[1] = gameObject.transform.position + Vector3.down;
            borderPositions[2] = gameObject.transform.position + Vector3.right;
            borderPositions[3] = gameObject.transform.position + Vector3.left;


            CheckAndAttack(e.prevPosition, e.position, e.intendedDirection);

            Move(e.position);
        }

    }


    public override void Move(Vector3 currentPlayerPosition)
    {

    }

    public override void WanderRandomly()
    {

    }

    public override void Die()
    {

    }


    public override void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "room" && collision.gameObject.GetComponentInParent<Room>() != null && gameObject != null && behaviorState != BehaviorState.Dead)
        {
            behaviorState = BehaviorState.Idle;
            currentRoom = collision.gameObject.GetComponentInParent<Room>().roomId;
        }

    }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "room" && collision.gameObject.GetComponentInParent<Room>() != null && gameObject != null && behaviorState != BehaviorState.Dead)
        {
            Die();
        }
    }




}