using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;
using static BSPGeneration;
using static DungeonNarrator;

public class PlayerMovement : MonoBehaviour
{

    public GameObject player;

    public PlayerCameraBehavior playerCameraBehavior;

    public InputController input;

    public event EventHandler<InputArgs> OnRoomEnter;

    public event EventHandler<MovementArgs> OnPlayerMoved;

    public event EventHandler OnPlayerTookDamage;
    public class MovementArgs : EventArgs
    {
        public Vector3 position;
        public Vector3 prevPosition;
    }


    //public Vector2 moveDirection = Vector2.zero;

    void Awake()
    {
        player = gameObject;

        playerCameraBehavior = GameObject.Find("Main Camera").GetComponent<PlayerCameraBehavior>();

        input = GameObject.Find("InputController").GetComponent<InputController>();

    }


    public class InputArgs : EventArgs
    {
        public int roomId;
        public string type;
    }

    private void OnEnable()
    {
        input.OnInput += Input_OnInput;
    }

    private void OnDisable()
    {
        input.OnInput -= Input_OnInput;
    }

    private void Input_OnInput(object sender, InputController.InputArgs e)
    {
        if(this.gameObject != null)
        {
            MovePlayer(e.direction);
        }
    }



    private void LateUpdate()
    {
        playerCameraBehavior.MoveCameraFollowPlayer();

    }

    public IEnumerator IncomingDamageFlash()
    {
        gameObject.GetComponent<SpriteRenderer>().color = UnityEngine.Color.red;
        yield return new WaitForSeconds(.05f);
        gameObject.GetComponent<SpriteRenderer>().color = UnityEngine.Color.white;

    }

    public void MovePlayer(Vector2 direction)
    {
        Vector2 checkPosition = (Vector2)player.transform.position + new Vector2(0.5f, 0.5f) + new Vector2(direction.x, direction.y);

        LayerMask mask = ~(1 << LayerMask.NameToLayer("ObjectPlacementLayer")); // we want to ignore the placement layer that we used for creating objects  in each scene

        Collider2D collision = Physics2D.OverlapCircle(checkPosition, 0.1f, mask);

        Vector3 previousPosition = player.transform.position;


        if (collision == null)
        {
            //player.transform.position = Vector3Int.FloorToInt(player.transform.position + direction);

            //player.GetComponent<Rigidbody2D>().MovePosition((Vector2)(player.transform.position) + direction);
            player.transform.position += (Vector3)direction;


        }
        // else if (collision.tag == "room") { Debug.Log("hitting a room"); }


        if (OnPlayerMoved != null)
        {
            OnPlayerMoved.Invoke(this, new MovementArgs { position = gameObject.transform.position, prevPosition = previousPosition});
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "room")
        {
            OnRoomEnter.Invoke(this, new InputArgs
            {
                type = "enter",
                roomId = collision.gameObject.GetComponentInParent<Room>().roomId
            });
        }

        else if (collision.gameObject.tag == "hallway")
        {

            // Vector2 checkPosition = (Vector2)player.transform.position + new Vector2(0.5f, 0.5f);
            // Collider2D[] collision2 = Physics2D.OverlapCircleAll(checkPosition, 0.1f);

            // OnRoomEnter.Invoke(this, new InputArgs
            // {
            //     type = "hallway",
            //     room1Id = collision.gameObject.GetComponent<Hallway>().room1Id,
            //     room2Id = collision.gameObject.GetComponent<Hallway>().room2Id
            // });


            //Debug.Log("Player entered " + collision.gameObject.name);

            SpriteRenderer[] ar = collision.gameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer r in ar)
            {
                r.enabled = false;
            }

        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        //Debug.Log("Player entered " + collision.gameObject.name);


        // if the player quits the game (or maybe changes scene) when they are colliding with a room,
        // it triggers onTriggerExit, but the Room component will get get deleted before it finishes
        // so we check to make sure it is still existing before invoking the event to prevent error
        if (collision.gameObject.tag == "room" && collision.gameObject.GetComponentInParent<Room>() != null && player.gameObject != null)
        {
            Dungeon_Narrator.AddDungeonNarratorText("You entered " +  collision.gameObject.GetComponentInParent<Room>().name);
            
            OnRoomEnter.Invoke(this, new InputArgs
            {
                type = "exit",
                roomId = collision.gameObject.GetComponentInParent<Room>().roomId
            });
        }

        else if (collision.gameObject.tag == "hallway")
        {

            // Vector2 checkPosition = (Vector2)player.transform.position + new Vector2(0.5f, 0.5f);
            // Collider2D[] collision2 = Physics2D.OverlapCircleAll(checkPosition, 0.1f);

            // OnRoomEnter.Invoke(this, new InputArgs
            // {
            //     type = "hallway",
            //     room1Id = collision.gameObject.GetComponent<Hallway>().room1Id,
            //     room2Id = collision.gameObject.GetComponent<Hallway>().room2Id
            // });


            SpriteRenderer[] ar = collision.gameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer r in ar)
            {
                r.enabled = true;
            }

        }
    }
}
