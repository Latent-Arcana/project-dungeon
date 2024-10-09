using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static BSPGeneration;

public class MapMark : MonoBehaviour
{

    private ScoreController scoreController;

    [SerializeField]
    public Enums.RoomType roomType;

    void Awake()
    {
        scoreController = GameObject.Find("ScoreController").GetComponent<ScoreController>();

    }


    private void OnTriggerEnter(Collider collision)
    {
        //Debug.Log("collision between map marker and object");

        if (collision.gameObject.tag == "room")
        {
            scoreController.SetRoomMark(this.gameObject, collision.gameObject.GetComponentInParent<Room>().roomId);
        }

        else{
            Debug.Log("What am I doing out here?");
        }
    }

    // private void OnDestroy()
    // {
    //     scoreController.RemoveRoomMark(this.gameObject);
    // }
}
