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

    public bool wasRemoved;

    void Awake()
    {
        scoreController = GameObject.Find("ScoreController").GetComponent<ScoreController>();
        wasRemoved = false;

    }


    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.tag == "room")
        {
            scoreController.SetRoomMark(this.gameObject, collision.gameObject.GetComponentInParent<Room>().roomId);
        }

        else{
            Debug.Log("MapMark, WARNING: Collision with map marker outside of map.");
        }
    }

    // private void OnDestroy()
    // {
    //     scoreController.RemoveRoomMark(this.gameObject);
    // }
}
