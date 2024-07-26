using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreController : MonoBehaviour
{

    private int Numerator;

    private int Denominator;

    private List<GameObject> allRooms;

    //private Dictionary<int, GameObject> currentScores;

    private Dictionary<int, GameObject> currentMarks = new();
    private Dictionary<GameObject, int> currentMarksInverse = new();


    void Awake()
    {
        allRooms = GameObject.Find("DungeonGenerator").GetComponent<BSPGeneration>().allRooms;
    }


    /// <summary>
    /// Add a room marker to the list of markers. If one already exists for a room, delete the game object
    /// </summary>
    /// <param name="marker">GameObject that was placed</param>
    /// <param name="RoomId">Id of the room that the marker was placed in</param>
    public void SetRoomMark(GameObject marker, int RoomId)
    {

        //already a merker in room
        if (currentMarks.ContainsKey(RoomId))
        {

            Debug.Log("SetRoomMark: Removing existing mark for room" + RoomId );

            //remove from dictionary (inverse)
            currentMarksInverse.Remove(marker);

            //delete current marker
            Destroy(currentMarks[RoomId]);

            //remove from dictionary
            currentMarks.Remove(RoomId);
        }

        Debug.Log("SetRoomMark: Adding mark for room" + RoomId );

        //add new mark to the dictionary(x2)
        currentMarks.Add(RoomId, marker);
        currentMarksInverse.Add(marker, RoomId);

    }

    /// <summary>
    /// Mark was right clicked on the map. Remove it from the list of markers
    /// </summary>
    public void RemoveRoomMark(GameObject marker)
    {
        int removeFromRoom = currentMarksInverse[marker];

        Debug.Log("RemoveRoomMark: Removing existing mark for room" + removeFromRoom );

        //remove it from the dictionary(x2)
        currentMarks.Remove(removeFromRoom);
        currentMarksInverse.Remove(marker);

        Destroy(marker);

    }


    public void ScoreRound()
    {

        foreach (GameObject r in allRooms)
        {
            Room room1 = r.GetComponent<Room>();

            //We have a pin in the room
            if (currentMarks.ContainsKey(room1.roomId))
            {

                MapMark mark1 = currentMarks[room1.roomId].GetComponent<MapMark>();

                //pin matches the room
                if (room1.roomType == mark1.roomType)
                {
                    Numerator++;
                    Denominator++;
                }

                //pin is wrong
                else
                {
                    Denominator++;
                }
            }

            //no pin in room, but room doesn't actually have a type (null == null)
            else if (room1.roomType == Enums.RoomType.Unassigned)
            {
                Numerator++;
                Denominator++;
            }

            // No pin but there should have been one
            else
            {
                Denominator++;
            }
        }

        Debug.Log($"Current Score: {Numerator}/{Denominator}");
    }
}
