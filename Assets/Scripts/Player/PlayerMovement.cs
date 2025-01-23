using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BSPGeneration;
using static DungeonNarrator;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{

    public GameObject player;

    public PlayerCameraBehavior playerCameraBehavior;

    public InputController input;

    public event EventHandler<InputArgs> OnRoomEnter;

    public event EventHandler<MovementArgs> OnPlayerMoved;

    public event EventHandler OnContainerOpen;

    public event EventHandler PortalEntered;

    public event EventHandler OnPlayerTookDamage;
    public class MovementArgs : EventArgs
    {
        public Vector3 position;
        public Vector3 prevPosition;
        public Vector3 intendedDirection;
    }

    public PlayerInventory playerInventory;

    [SerializeField]
    public PlayerStatsManager Player_Stats;


    [SerializeField]
    private Sprite rightFacingSprite;

    [SerializeField]
    private Sprite leftFacingSprite;
    public bool isRightFacing = true;

    private SpriteRenderer spriteRenderer;

    private PlayerAnimationBehavior playerAnimationBehavior;

    private MenuAudioController menuAudioController;
    private BackgroundMusicController backgroundMusicController;
    private AmbientAudioController ambientAudioController;

    private GameStats gameStats;

    void Awake()
    {
        player = gameObject;

        playerCameraBehavior = GameObject.Find("Main Camera").GetComponent<PlayerCameraBehavior>();

        input = GameObject.Find("InputController").GetComponent<InputController>();

        playerInventory = gameObject.GetComponent<PlayerInventory>();

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        playerAnimationBehavior = gameObject.transform.GetComponentInChildren<PlayerAnimationBehavior>();

        menuAudioController = GameObject.Find("Audio").GetComponentInChildren<MenuAudioController>();
        backgroundMusicController = GameObject.Find("Audio").GetComponentInChildren<BackgroundMusicController>();
        ambientAudioController = GameObject.Find("Audio").GetComponentInChildren<AmbientAudioController>();
    }

    void Start()
    {
        gameStats = GameObject.Find("GameStats").GetComponent<GameStats>();
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
        if (this.gameObject != null)
        {
            FlipDirection(e.direction);
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
        Debug.Log(playerInventory.inventory.currentDurability.Count);
        int tick = 0;
        foreach (int durability in playerInventory.inventory.currentDurability)
        {
            Debug.Log("current durability at " + tick + " = " + durability);
            ++tick;
        }

        Vector2 checkPosition = (Vector2)player.transform.position + new Vector2(0.5f, 0.5f) + new Vector2(direction.x, direction.y);

        //LayerMask mask = ~(1 << LayerMask.NameToLayer("ObjectPlacementLayer")); // we want to ignore the placement layer that we used for creating objects  in each scene

        LayerMask mask = LayerMask.GetMask("Default"); //we only care about colliding on default for now, but we should add in other layers here if needed

        Collider2D collision = Physics2D.OverlapCircle(checkPosition, 0.1f, mask);

        Vector3 previousPosition = player.transform.position;


        if (collision == null)
        {
            //player.transform.position = Vector3Int.FloorToInt(player.transform.position + direction);

            //player.GetComponent<Rigidbody2D>().MovePosition((Vector2)(player.transform.position) + direction);
            player.transform.position += (Vector3)direction;


        }


        // We collided with an object
        else if (collision.tag == "object")
        {

            // Check to see if we collided with something that should be opened
            ContainerBehavior container = collision.gameObject.GetComponent<ContainerBehavior>();
            if (container)
            {
                ambientAudioController.PlayAudioClip("Chest");

                if (container.items.Count == 0)
                {
                    Dungeon_Narrator.AddDungeonNarratorText("There's nothing in there...");
                }
                else
                {
                    List<Item> itemsToRemove = new List<Item>();
                    foreach (Item item in container.items)
                    {
                        if (playerInventory.inventory.items.Count < playerInventory.maxItemCount)
                        {
                            playerInventory.inventory.items.Add(item);
                            itemsToRemove.Add(item);

                            Weapon weapon = item as Weapon;
                            Armor armor = item as Armor;

                            if (weapon != null)
                            {
                                playerInventory.inventory.currentDurability.Add(weapon.DUR);
                            }

                            else if (armor != null)
                            {
                                playerInventory.inventory.currentDurability.Add(armor.DUR);
                            }

                            else
                            {
                                playerInventory.inventory.currentDurability.Add(-1); // we need a dummy here
                            }

                            Dungeon_Narrator.AddDungeonNarratorText("You picked up the " + item.itemName + ".");
                        }

                        else
                        {
                            Dungeon_Narrator.AddDungeonNarratorText("You do not have space to pick up the " + item.itemName + ".");
                        }
                    }

                    // Remove items from the container if any were removed
                    foreach (Item item in itemsToRemove)
                    {
                        container.RemoveItem(item);
                    }
                }
                //check to see if the sprite needs to be swapped to open-empty or open-full
                container.SwapSprite();
            }

            ObjectBehavior objectBehavior = collision.gameObject.GetComponent<ObjectBehavior>();

            if (objectBehavior.ObjectType == Enums.ObjectType.Spikes)
            {
                player.transform.position += (Vector3)direction;
                Player_Stats.SetHP(Player_Stats.HP - 1, sourceObjectName: "Spikes");
                Dungeon_Narrator.AddDungeonNarratorText($"You take 1 damage from stepping on the spikes.");
            }

            else if (objectBehavior.ObjectType == Enums.ObjectType.BedHospital)
            {
                player.transform.position += (Vector3)direction;

                SafeObjectBehavior bedBehavior = collision.gameObject.GetComponent<SafeObjectBehavior>();

                if (!bedBehavior.alreadyHealedPlayer)
                {
                    ambientAudioController.PlayAudioClip("Rest");
                    playerAnimationBehavior.Sleep();
                }

                int healAmount = bedBehavior.HealPlayer(Player_Stats.HP, Player_Stats.MAX_HP);
                Player_Stats.SetHP(healAmount, collision.gameObject.name);
            }

            else if (objectBehavior.ObjectType == Enums.ObjectType.Bed)
            {

                player.transform.position += (Vector3)direction;

                SafeObjectBehavior bedBehavior = collision.gameObject.GetComponent<SafeObjectBehavior>();

                if (!bedBehavior.alreadyBuffedPlayer)
                {
                    ambientAudioController.PlayAudioClip("Rest");
                    playerAnimationBehavior.Sleep();

                }

                int buffAmount = bedBehavior.BuffPlayer(Player_Stats.MAX_HP);
                int tempMax = Player_Stats.MAX_HP;
                Player_Stats.SetMaxHP(buffAmount);
                Player_Stats.SetHP(Player_Stats.HP + (Player_Stats.MAX_HP - tempMax), sourceObjectName: collision.gameObject.name);
            }

            else if (objectBehavior.ObjectType == Enums.ObjectType.Shrine)
            {
                ShrineBehavior shrineBehavior = collision.gameObject.GetComponent<ShrineBehavior>();

                if (!shrineBehavior.hasTriggered)
                {
                    ambientAudioController.PlayAudioClip("Bless");
                    playerAnimationBehavior.BuffStats(shrineBehavior.shrineType);
                    shrineBehavior.Bless(Player_Stats);
                }

            }

            else if (objectBehavior.ObjectType == Enums.ObjectType.Bookshelf)
            {
                BookshelfBehavior bookshelfBehavior = collision.gameObject.GetComponent<BookshelfBehavior>();

                ambientAudioController.PlayAudioClip("Bookshelf");


                bookshelfBehavior.RevealRoom();
            }


        }

        else if (collision.tag == "portal")
        {
            backgroundMusicController.StopAudio();
            ambientAudioController.PlayAudioClip("Portal");

            PortalEntered.Invoke(this, EventArgs.Empty);

            PortalAnimationBehavior portalAnimationBehavior = collision.gameObject.GetComponent<PortalAnimationBehavior>();

            portalAnimationBehavior.AnimatePortal();

            //SceneManager.LoadScene("BSP");
        }


        if (OnPlayerMoved != null)
        {
            OnPlayerMoved.Invoke(this, new MovementArgs { position = gameObject.transform.position, prevPosition = previousPosition, intendedDirection = direction });
        }
    }

    public void FlipDirection(Vector2 direction)
    {
        if ((direction == Vector2.right && !isRightFacing) || (direction == Vector2.left && isRightFacing))
        {
            spriteRenderer.sprite = (spriteRenderer.sprite == rightFacingSprite) ? leftFacingSprite : rightFacingSprite;
            isRightFacing = !isRightFacing;
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


            Room collisionRoom = collision.gameObject.GetComponentInParent<Room>();

            if (!collisionRoom.discovered)
            {
                collisionRoom.discovered = true;

                if (gameStats != null)
                {
                    gameStats.IncrementRoomsVisited();
                }
            }
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

        else if (collision.gameObject.tag == "corpse")
        {
            // Check to see if we collided with something that should be opened
            EnemyCorpseBehavior corpse = collision.gameObject.GetComponent<EnemyCorpseBehavior>();
            if (corpse)
            {

                //if we want to sprite swap for full/empty enemy corpses like we do with chests, do it around here

                if (corpse.items.Count == 0)
                {
                    Dungeon_Narrator.AddDungeonNarratorText("There's nothing on this body...");
                }
                else
                {
                    List<Item> itemsToRemove = new List<Item>();
                    foreach (Item item in corpse.items)
                    {
                        if (playerInventory.inventory.items.Count < playerInventory.maxItemCount)
                        {
                            playerInventory.inventory.items.Add(item);
                            itemsToRemove.Add(item);

                            Weapon weapon = item as Weapon;
                            Armor armor = item as Armor;

                            if (weapon != null)
                            {
                                playerInventory.inventory.currentDurability.Add(weapon.DUR);
                            }

                            else if (armor != null)
                            {
                                playerInventory.inventory.currentDurability.Add(armor.DUR);
                            }

                            else
                            {
                                playerInventory.inventory.currentDurability.Add(-1); // we need a dummy here
                            }


                            Dungeon_Narrator.AddDungeonNarratorText("You picked up the " + item.itemName);
                        }

                        else
                        {
                            Dungeon_Narrator.AddDungeonNarratorText("You do not have space to pick up the " + item.itemName);
                        }
                    }

                    // Remove items from the container if any were removed
                    foreach (Item item in itemsToRemove)
                    {
                        corpse.RemoveItem(item);
                    }
                }

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
            OnRoomEnter.Invoke(this, new InputArgs
            {
                type = "exit",
                roomId = collision.gameObject.GetComponentInParent<Room>().roomId
            });
        }

        else if (collision.gameObject.tag == "hallway")
        {
            SpriteRenderer[] ar = collision.gameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer r in ar)
            {
                r.enabled = true;
            }

        }

    }
}
