using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;


//Master Controller for all gameplay fog. Replacing RoomFogControllers?
public class GameplayFogController : MonoBehaviour
{

    private GameObject player;

    public float laserLength = 20f; //length of the raycast

    //filters for raycasts/overlaps
    private LayerMask mask;
    private LayerMask maskFog;
    private ContactFilter2D filter;

    private List<GameObject> allRooms;
    private List<GameObject> allHallways;



    private void Start()
    {
        player = GameObject.Find("Player"); //.GetComponent<PlayerMovement>();

        mask = LayerMask.GetMask("Default");
        maskFog = LayerMask.GetMask("RoomFog");
        filter = new()
        {
            layerMask = maskFog,
            useLayerMask = true
        };

        allRooms = new();

        //allRooms = GameObject.Find("DungeonGenerator").GetComponent<BSPGeneration>().allRooms;

        //reduce rooms list to the "Room Fogbox" child GameObjects
        foreach (GameObject room in GameObject.Find("DungeonGenerator").GetComponent<BSPGeneration>().allRooms) //Room_X
        {
            foreach (Transform room_Gameplay in room.transform) //Room_Gameplay_X
            {
                foreach (Transform child in room_Gameplay.transform) //Room Fogbox
                {
                    if (child.name == "Room Fogbox")
                    {
                        allRooms.Add(child.gameObject);
                    }
                }
            }
        }

        allHallways = GameObject.Find("DungeonGenerator").GetComponent<BSPGeneration>().allHallways;

    }

    //TODO: optimize by attaching to player movement event (OnPlayerMoved), instead of checking every update?
    private void FixedUpdate()
    {

        //Raycast to find what rooms/hallways you can see into

        //centered position of player
        Vector2 posPlayerCenter = new Vector2(player.transform.position.x + 0.5f, player.transform.position.y + 0.5f);

        // //positions to start the respective rays, slightly outside the bounds of the character to avoid that collision
        // Vector2 posRight = new Vector2(posPlayerCenter.x + 0.6f, posPlayerCenter.y);
        // Vector2 posDown = new Vector2(posPlayerCenter.x, posPlayerCenter.y - 0.6f);
        // Vector2 posUp = new Vector2(posPlayerCenter.x, posPlayerCenter.y + 0.6f);
        // Vector2 posLeft = new Vector2(posPlayerCenter.x - 0.6f, posPlayerCenter.y);


        //Get the first object hit by the ray, stopping when hitting an object on the layerMask layer(s)
        RaycastHit2D hitRight = Physics2D.Raycast(posPlayerCenter, Vector2.right, laserLength, layerMask: mask);
        RaycastHit2D hitDown = Physics2D.Raycast(posPlayerCenter, Vector2.down, laserLength, layerMask: mask);
        RaycastHit2D hitUp = Physics2D.Raycast(posPlayerCenter, Vector2.up, laserLength, layerMask: mask);
        RaycastHit2D hitLeft = Physics2D.Raycast(posPlayerCenter, Vector2.left, laserLength, layerMask: mask);



        //Get the room fogs we are colliding with

        //make lists
        List<Collider2D> rightOverlaps = new();
        List<Collider2D> downOverlaps = new();
        List<Collider2D> upOverlaps = new();
        List<Collider2D> leftOverlaps = new();

        //add colliders to lists
        Physics2D.OverlapArea(posPlayerCenter, hitRight.point, contactFilter: filter, results: rightOverlaps);
        Physics2D.OverlapArea(posPlayerCenter, hitDown.point, contactFilter: filter, results: downOverlaps);
        Physics2D.OverlapArea(posPlayerCenter, hitUp.point, contactFilter: filter, results: upOverlaps);
        Physics2D.OverlapArea(posPlayerCenter, hitLeft.point, contactFilter: filter, results: leftOverlaps);

        //remove duplicates, since the room the player is in can be in up to all 4 lists
        List<Collider2D> collisionObjects = rightOverlaps.Concat(downOverlaps).Concat(upOverlaps).Concat(leftOverlaps).ToList();
        collisionObjects = collisionObjects.Distinct().ToList();



        //loop through rooms and hallways to see if the sprites should be visible or not

        foreach (GameObject hallway in allHallways)
        {

            //handles L shaped hallways that have 2+ child objects
            foreach (Transform child in hallway.transform)
            {

                //TODO here: make the entire hallway visible if any of the segment (horizontal/vertical) colliders are in list
                if (collisionObjects.Contains(child.gameObject.GetComponent<Collider2D>()))
                {
                    child.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                }
                else
                {
                    child.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
        }

        foreach (GameObject room in allRooms)
        {
            if (collisionObjects.Contains(room.GetComponent<Collider2D>()))
            {
                room.GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                room.GetComponent<SpriteRenderer>().enabled = true;
            }
        }



        //DEBUG

        //If the collider of the object hit is not NUll
        if (hitRight.collider != null)
        {
            //Hit something, print the tag of the object
            //Debug.Log($"Hitting Right:  {hitRight.collider.name} with tag {hitRight.collider.tag} at point {hitRight.point} and distance {hitRight.distance}");

        }

        if (hitDown.collider != null)
        {
            //Hit something, print the tag of the object
            //Debug.Log($"Hitting Down: {hitDown.collider.name} with tag {hitDown.collider.tag} at point {hitDown.point} and distance {hitDown.distance}");
        }

        //If the collider of the object hit is not NUll
        if (hitUp.collider != null)
        {
            //Hit something, print the tag of the object
            //Debug.Log($"Hitting Up:  {hitUp.collider.name} with tag {hitUp.collider.tag} at point {hitUp.point} and distance {hitUp.distance}");
        }

        if (hitLeft.collider != null)
        {
            //Hit something, print the tag of the object
            //Debug.Log($"Hitting Left:  {hitLeft.collider.name} with tag {hitLeft.collider.tag} at point {hitLeft.point} and distance {hitLeft.distance}");
        }

        //Method to draw the ray in scene for debug purpose
        Debug.DrawRay(posPlayerCenter, Vector2.right * hitRight.distance, UnityEngine.Color.red);
        Debug.DrawRay(posPlayerCenter, Vector2.down * hitDown.distance, UnityEngine.Color.blue);
        Debug.DrawRay(posPlayerCenter, Vector2.up * hitUp.distance, UnityEngine.Color.green);
        Debug.DrawRay(posPlayerCenter, Vector2.left * hitLeft.distance, UnityEngine.Color.yellow);

        //Debug.DrawLine(posRight, hitRight.point, Color.white);
        //Debug.DrawLine(posDown, hitDown.point, Color.white);

    }

}