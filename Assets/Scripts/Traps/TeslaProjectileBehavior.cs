using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static DungeonNarrator;

public class TeslaProjectileBehavior : ProjectileBehavior
{

    // public override void Move(Vector3 currentPlayerPosition)
    // {
        
    //     Physics2D.SyncTransforms();

    //     Vector3 targetPosition = (Vector3)WanderRandomly() + gameObject.transform.position;

    //     Collider2D coll = CheckPosition(targetPosition);

    //     if (coll == null)
    //     {
    //         gameObject.transform.position = targetPosition;
    //         Physics2D.SyncTransforms();
    //     }

    //     else if (coll.name == "Player")
    //     {
    //         Attack(playerAttacked: false);
    //         isDestroyed = true;
    //         Destroy(gameObject);
    //     }

    //     else
    //     {
    //         Die();
    //     }


    // }

}