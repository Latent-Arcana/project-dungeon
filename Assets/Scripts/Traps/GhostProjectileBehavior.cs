using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using static DungeonNarrator;
public class GhostProjectileBehavior : ProjectileBehavior
{

    public override void Move(Vector3 currentPlayerPosition)
    {

        if (Vector3.Distance(transform.position, currentPlayerPosition) <= 5.0f)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;

        }

        Physics2D.SyncTransforms();

        Vector3 targetDir = GetDirectionToTarget(currentPlayerPosition);


        Vector3 potentialPosition = gameObject.transform.position + targetDir;

        Collider2D checkCollision = CheckPosition(potentialPosition);

        if (checkCollision == null)
        {
            gameObject.transform.position += targetDir;
            Physics2D.SyncTransforms();
        }


        else if (checkCollision.name == "Player")
        {
            Attack(playerAttacked: false);
            isDestroyed = true;
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

            }

        }

    }

}
