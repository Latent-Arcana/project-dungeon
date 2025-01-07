using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreController : MonoBehaviour
{

    /// <summary>
    /// Handles the stats for the current floor
    /// Manages the map markers to score them
    /// Spawns a portal when all rooms have been visited once
    /// </summary>

    private int Numerator;
    private int Denominator;

    private List<GameObject> allRooms;

    private int[] roomsVisited;
    private GameStats gameStats;
    public MapMarker mapMarker;

    private PlayerMovement playerMovement;

    private Dictionary<int, GameObject> currentMarks = new();
    private Dictionary<GameObject, int> currentMarksInverse = new();

    bool portalsSpawned = false;

    void Awake()
    {
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    void Start()
    {
        allRooms = GameObject.Find("DungeonGenerator").GetComponent<BSPGeneration>().allRooms;
        gameStats = GameObject.Find("GameStats").GetComponent<GameStats>();
        mapMarker = GameObject.Find("MapController").GetComponent<MapMarker>();


        //rooms visited array
        roomsVisited = new int[allRooms.Count]; //init to 0's
        roomsVisited[0] = 1; //always set first room to 1 just in case

        //Spawn a marker in room 0
        mapMarker.PlacePresetMarker(allRooms[0].GetComponent<Room>());

    }

    private void OnEnable()
    {
        //subscribe MapController to the PlayerMovement script's OnRoomEnter event
        playerMovement.OnRoomEnter += Portals_OnRoomEnter;

        playerMovement.PortalEntered += Portals_OnPortalEntered;

    }

    private void OnDisable()
    {
        playerMovement.OnRoomEnter -= Portals_OnRoomEnter;
        playerMovement.PortalEntered -= Portals_OnPortalEntered;
    }

    void Update()
    {
        //TODO: Delete all this, it's DEBUG

        if (Input.GetKeyUp(KeyCode.Delete))
        {
            GameObject.Find("Player").GetComponentInChildren<PlayerStats>().DEBUG_DIE();
        }
        else if (Input.GetKeyUp(KeyCode.P))
        {
            SpawnPortal();
        }
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

            Debug.Log("SetRoomMark: Removing existing mark for room" + RoomId);

            //remove from dictionary (inverse)
            currentMarksInverse.Remove(marker);

            //delete current marker
            Destroy(currentMarks[RoomId]);

            //remove from dictionary
            currentMarks.Remove(RoomId);
        }

        Debug.Log("SetRoomMark: Adding mark for room" + RoomId);

        //add new mark to the dictionary(x2)
        currentMarks.Add(RoomId, marker);
        currentMarksInverse.Add(marker, RoomId);

    }

    /// <summary>
    /// Mark was right clicked on the map. Remove it from the list of markers
    /// </summary>
    public void RemoveRoomMark(GameObject marker)
    {

        MapMark mark = marker.GetComponent<MapMark>();

        if (!mark.wasRemoved)
        {
            string clipName = "";
            mark.wasRemoved = true;
            Animator animator = marker.GetComponent<Animator>();

            switch (mark.roomType)
            {
                case Enums.RoomType.Danger:
                    clipName = "danger-animation-erase";
                    break;

                case Enums.RoomType.Safe:
                    clipName = "safe-animation-erase";
                    break;

                case Enums.RoomType.Lore:
                    clipName = "lore-animation-erase";
                    break;
            }

            animator.Play(clipName);
            StartCoroutine(RemoveMarkAfterAnimation(marker, animator, clipName));

        }
    }

    private IEnumerator RemoveMarkAfterAnimation(GameObject marker, Animator animator, string animationName)
    {

        int removeFromRoom = currentMarksInverse[marker];

        //remove it from the dictionary(x2)
        currentMarks.Remove(removeFromRoom);
        currentMarksInverse.Remove(marker);

        Debug.Log("RemoveRoomMark: Removing existing mark for room" + removeFromRoom);

        // Wait for the duration of the animation
        float animationLength = animator.runtimeAnimatorController.animationClips.First(clip => clip.name == animationName).length / 3;
        yield return new WaitForSeconds(animationLength);

        Debug.Log("waiting for animation");

        //marker.SetActive(false);

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

                // we have a pin but the room is unassigned
                // We don't care what we marked the room, we're just going to give them this free point
                else if (room1.roomType == Enums.RoomType.Unassigned)
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

        //pass the stats to the object that won't be destroyed
        gameStats.AddToScore(Numerator, Denominator);

        gameStats.UpdateCurrentRoomAndDungeonData();

    }

    /// <summary>
    /// Called on Death. 
    /// Sets the score in the GameStats object (DontDestroy) which is passed to the "Game Over" scene
    /// </summary>
    public void SetFinalScore()
    {
        //Score current round
        ScoreRound();

        GameObject.Find("BackgroundAudio").GetComponent<BackgroundMusicController>().ChangeSongForScene("GameOver");

        //call game over scene
        SceneManager.LoadScene("GameOver");

    }

    private void Portals_OnPortalEntered(object sender, EventArgs e)
    {
        ScoreRound();
    }

    /// <summary>
    /// Thrown by PlayerMovement. Caught here to remove the room from the list of rooms to visit
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Portals_OnRoomEnter(object sender, PlayerMovement.InputArgs e)
    {
        if (e.type == "enter")
        {
            SetRoomAsEntered(e.roomId);
        }
    }

    public void SetRoomAsEntered(int roomId)
    {
        roomsVisited[roomId] = 1;
        CheckIfPortalShouldSpawn();
    }

    private void CheckIfPortalShouldSpawn()
    {
        bool allRoomsAreVisited = true;

        for (int i = 0; i < roomsVisited.Length; i++)
        {
            //Debug.Log($"Room {i}: {roomsVisited[i]}");
            if (roomsVisited[i] == 0)  //not visited yet
            {
                allRoomsAreVisited = false;
                break;
            }
        }

        if (allRoomsAreVisited && !portalsSpawned)
        {
            SpawnPortal();
        }
    }

    private void SpawnPortal()
    {
        foreach (GameObject room in allRooms)
        {
            //not every room will have a portal spawn
            if (room.transform.Find("Portal(Clone)") != null)
            {
                GameObject portalObject = room.transform.Find("Portal(Clone)").gameObject;

                StartCoroutine(FadeInPortal(portalObject));
            }
        }

        DungeonNarrator.Dungeon_Narrator.AddDungeonNarratorText("The aether stirs. The portals have opened...");

        portalsSpawned = true;
    }


    private IEnumerator FadeInPortal(GameObject portalObject)
    {
        portalObject.GetComponent<BoxCollider2D>().enabled = true;
        SpriteRenderer sr = portalObject.GetComponent<SpriteRenderer>();

        sr.enabled = true;

        yield return new WaitForSeconds(.1f);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .1f);
        yield return new WaitForSeconds(.1f);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .2f);
        yield return new WaitForSeconds(.1f);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .3f);
        yield return new WaitForSeconds(.1f);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .4f);
        yield return new WaitForSeconds(.1f);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .5f);
        yield return new WaitForSeconds(.1f);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .6f);
        yield return new WaitForSeconds(.1f);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .7f);
        yield return new WaitForSeconds(.1f);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .8f);
        yield return new WaitForSeconds(.1f);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, .9f);
        yield return new WaitForSeconds(.1f);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
    }
}
