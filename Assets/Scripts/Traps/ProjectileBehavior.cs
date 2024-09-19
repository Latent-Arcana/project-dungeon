using System;
using System.Data.Common;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
public class ProjectileBehavior : MonoBehaviour
{
    public Vector3 directionOfTravel;

    public bool isAtSpawn;

    public void Move()
    {
        if (isAtSpawn)
        {
            isAtSpawn = false;
            return;
        }

        else
        {
            gameObject.transform.position += (Vector3)directionOfTravel;
            return;
        }


    }
}