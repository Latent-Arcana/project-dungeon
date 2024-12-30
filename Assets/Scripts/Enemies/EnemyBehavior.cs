using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.GraphicsBuffer;
using static DungeonNarrator;
using static PlayerMovement;
using Unity.Collections;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField]
    public PlayerStatsManager Player_Stats;
    protected string type;
    private PlayerMovement playerMovement;
    private PlayerInventory playerInventory;
    protected InputController input;
    protected DungeonNarrator dungeonNarrator;

    protected GameStats gameStats;
    public int room;
    public int currentRoom;
    public int id;
    public bool playerInRoom;
    public Vector2Int originPoint;
    public bool standingOnCorpse;
    public event EventHandler<AttackArgs> OnAttack;

    [SerializeField]
    public Sprite attackSprite;

    public class AttackArgs : EventArgs
    {
        public string enemyType;
        public int enemyId;
        public string combatText;
        public int enemyDamageDealt;
        public int playerDamageDealt;
    }

    public EnemyStats enemyStats;

    private EnemyAnimationBehavior animationBehavior;

    public BehaviorState behaviorState;

    public enum BehaviorState
    {
        Hostile,
        Idle,
        Fleeing,
        Dead
    }

    // 0: Vector3 upPosition;
    // 1: Vector3 downPosition;
    // 2: Vector3 rightPosition;
    // 3: Vector3 leftPosition;

    public Vector3[] borderPositions = new Vector3[4];

    public virtual void Awake()
    {

        input = GameObject.Find("InputController").GetComponent<InputController>();

        playerInRoom = false;

        behaviorState = BehaviorState.Idle;

        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();

        currentRoom = room;
        standingOnCorpse = false;

        playerInventory = GameObject.Find("Player").GetComponent<PlayerInventory>();

        animationBehavior = gameObject.GetComponent<EnemyAnimationBehavior>();

        gameStats = GameObject.Find("GameStats").GetComponent<GameStats>();

    }

    public virtual void OnEnable()
    {
        playerMovement.OnPlayerMoved += Input_OnPlayerMoved;
        playerMovement.OnRoomEnter += PlayerMovement_OnRoomEnter;
    }

    public virtual void OnDisable()
    {
        playerMovement.OnPlayerMoved -= Input_OnPlayerMoved;
        playerMovement.OnRoomEnter -= PlayerMovement_OnRoomEnter;

    }

    public virtual void Input_OnPlayerMoved(object sender, PlayerMovement.MovementArgs e)
    {
        if (this.gameObject != null && behaviorState != BehaviorState.Dead)
        {
            Physics2D.SyncTransforms();

            Vector3 currentEnemyPosition = gameObject.transform.position;

            borderPositions[0] = currentEnemyPosition + Vector3.up;
            borderPositions[1] = currentEnemyPosition + Vector3.down;
            borderPositions[2] = currentEnemyPosition + Vector3.right;
            borderPositions[3] = currentEnemyPosition + Vector3.left;

            // Set Behavior State

            if (playerInRoom && behaviorState != BehaviorState.Fleeing)
            {
                behaviorState = BehaviorState.Hostile;
            }

            else if (!playerInRoom && behaviorState != BehaviorState.Fleeing)
            {
                behaviorState = BehaviorState.Idle;
            }

            //Debug.Log($"{gameObject.name} is in the {behaviorState} state");


            CheckAndAttack(e.prevPosition, e.position, e.intendedDirection);


            Move(e.position);
            Physics2D.SyncTransforms();
        }

    }

    public virtual void CheckAndAttack(Vector3 prevPosition, Vector3 currentPosition, Vector2 intendedDirection)
    {

        foreach (Vector3 position in borderPositions)
        {
            if (prevPosition == position)
            {

                // Identify whether the player was trying to attack the enemy too
                if (prevPosition == currentPosition && (Vector3)intendedDirection + prevPosition == transform.position)
                {
                    Attack(playerAttacked: true);
                }

                else
                {
                    Attack(playerAttacked: false);
                }

            }
        }
    }

    private void PlayerMovement_OnRoomEnter(object sender, InputArgs e)
    {
        if ((e.roomId == room || e.roomId == currentRoom) && e.type == "enter")
        {
            playerInRoom = true;
        }

        else
        {
            playerInRoom = false;
        }
    }

    public virtual void AssignStats()
    {
        enemyStats = new EnemyStats(Enums.EnemyType.Skeleton, 5, 1, 2, 1, 1);
    }

    public virtual void Move(Vector3 currentPlayerPosition)
    {
        if (behaviorState == BehaviorState.Dead)
        {
            return;
        }

        if (behaviorState == BehaviorState.Hostile)
        {
            // Get distance between ourselves and the player
            Vector3 diffVector = new Vector3(gameObject.transform.position.x - currentPlayerPosition.x, gameObject.transform.position.y - currentPlayerPosition.y);

            Vector3 targetDirection = GetDirectionToTarget(currentPlayerPosition);


            if (Mathf.Abs(diffVector.x) <= 10f && Mathf.Abs(diffVector.y) <= 10f)
            {
                MoveEnemy(targetDirection);
            }
        }

        else if (behaviorState == BehaviorState.Idle)
        {
            MoveEnemy(WanderRandomly());
        }

        else if (behaviorState == BehaviorState.Fleeing)
        {
            Vector3 originDirection = GetDirectionToTarget(new Vector3(originPoint.x, originPoint.y, 0));
            MoveEnemy(originDirection);
        }

    }

    public virtual Vector2 WanderRandomly()
    {
        // wander inside the room
        int randDir = UnityEngine.Random.Range(0, 4);

        if (randDir == 0)
        {
            return Vector2.up;

        }

        else if (randDir == 1)
        {
            return Vector2.right;

        }

        else if (randDir == 2)
        {
            return Vector2.down;

        }

        else
        {
            return Vector2.left;
        }
    }

    public virtual Vector3 GetDirectionToTarget(Vector3 target)
    {
        Vector3 targetDir = target - gameObject.transform.position;

        float upAngle = Vector3.Angle(targetDir, transform.up);
        float rightAngle = Vector3.Angle(targetDir, transform.right);
        float leftAngle = Vector3.Angle(targetDir, -transform.right);
        float downAngle = Vector3.Angle(targetDir, -transform.up);

        float minAngle = new[] { upAngle, rightAngle, leftAngle, downAngle }.Min();

        Vector2 directionToTarget = Vector2.zero;

        if (minAngle == upAngle)
        {
            directionToTarget = Vector2.up;
        }

        else if (minAngle == rightAngle)
        {
            directionToTarget = Vector2.right;
        }

        else if (minAngle == leftAngle)
        {
            directionToTarget = Vector2.left;
        }

        else if (minAngle == downAngle)
        {
            directionToTarget = Vector2.down;
        }

        return directionToTarget;
    }

    public IEnumerator IncomingDamageFlash()
    {
        gameObject.GetComponent<SpriteRenderer>().color = UnityEngine.Color.black;
        yield return new WaitForSeconds(.05f);
        gameObject.GetComponent<SpriteRenderer>().color = UnityEngine.Color.white;

    }
    public virtual void Attack(bool playerAttacked)
    {
        bool playerMissed = false;
        bool enemyMissed = false;

        // FACTOR IN STRENGTH/DAMAGE/AP, AND A SMALL AMOUNT OF RANDOM VARIABILITY
        int _enemyDamageDealt = (-1 * (enemyStats.STR + UnityEngine.Random.Range(1, 3))) + Player_Stats.AP;
        int _playerDamageDealt = (-1 * (Player_Stats.STR + UnityEngine.Random.Range(1, 3))) + enemyStats.AP;

        // FACTOR IN AGI/DODGE
        int _playerHitChance = UnityEngine.Random.Range(1, 100);
        int _enemyHitChance = UnityEngine.Random.Range(1, 100);

        int _enemyHitThreshold = Player_Stats.AGI * 5;
        int _playerHitThreshold = enemyStats.AGI * 5;

        // Set up the player weapon information so we can print the right text later
        int playerWeaponIndex = playerInventory.GetEquippedWeapon();
        Enums.WeaponType playerWeaponType = Enums.WeaponType.Default;

        if (playerWeaponIndex >= 0)
        {
            Weapon playerWeapon = playerInventory.inventory.items[playerWeaponIndex] as Weapon;
            playerWeaponType = playerWeapon.weaponType;
        }

        if (_playerHitChance <= _playerHitThreshold)
        {
            // PLAYER MISSED ENEMY
            playerMissed = true;
            Dungeon_Narrator.AddPlayerMissText(enemyStats.EnemyType, playerWeaponType);
        }

        if (_enemyHitChance <= _enemyHitThreshold)
        {
            // ENEMY MISSED PLAYER
            enemyMissed = true;
            Dungeon_Narrator.AddEnemyMissText(enemyStats.EnemyType);
        }


        if (playerAttacked)
        {
            // if the enemy attacks first
            if (enemyStats.SPD > Player_Stats.SPD)
            {
                if (!enemyMissed)
                {
                    Player_Stats.SetHP(Player_Stats.HP + _enemyDamageDealt, sourceObjectName: enemyStats.EnemyType.ToSafeString());
                }

                if (!playerMissed)
                {
                    enemyStats.HP += _playerDamageDealt;
                    animationBehavior.HandleDamageAnimations();
                   // StartCoroutine(IncomingDamageFlash());
                    // if the enemy was reduced to 0 HP, they die
                    if (enemyStats.HP <= 0)
                    {
                        Die(playerWeaponType);
                    }

                    else
                    {
                        Dungeon_Narrator.AddEnemyAttackText(enemyStats.EnemyType, Mathf.Abs(_enemyDamageDealt));
                        Dungeon_Narrator.AddPlayerAttackText(playerWeaponType, enemyStats.EnemyType, Mathf.Abs(_playerDamageDealt));

                    }
                }

            }
            // if the player attacks first
            else
            {
                if (!playerMissed)
                {
                    enemyStats.HP += _playerDamageDealt;
                    //StartCoroutine(IncomingDamageFlash());
                    animationBehavior.HandleDamageAnimations();


                    // if the enemy was reduced to 0 HP, they die
                    if (enemyStats.HP <= 0)
                    {
                        Die(playerWeaponType);
                    }

                    else
                    {
                        Dungeon_Narrator.AddPlayerAttackText(playerWeaponType, enemyStats.EnemyType, Mathf.Abs(_playerDamageDealt));
                    }
                }

                if (!enemyMissed && behaviorState != BehaviorState.Dead)
                {
                    Player_Stats.SetHP(Player_Stats.HP + _enemyDamageDealt,  sourceObjectName: enemyStats.EnemyType.ToSafeString());
                    Dungeon_Narrator.AddEnemyAttackText(enemyStats.EnemyType, Mathf.Abs(_enemyDamageDealt));

                }
            }

        }

        else
        {
            if (!enemyMissed)
            {
                Dungeon_Narrator.AddEnemyAttackText(enemyStats.EnemyType, Mathf.Abs(_enemyDamageDealt));
                Player_Stats.SetHP(Player_Stats.HP + _enemyDamageDealt, sourceObjectName: enemyStats.EnemyType.ToSafeString());
            }

        }

    }

    public virtual void Die(Enums.WeaponType killedByWeapon)
    {

        Dungeon_Narrator.AddEnemyDeathText(enemyStats.EnemyType, killedByWeapon);

        behaviorState = BehaviorState.Dead;

        gameStats.IncrementEnemiesKilled();


        gameObject.GetComponent<SpriteRenderer>().enabled = false;

        GameObject corpse = gameObject.transform.GetChild(2).gameObject; // the Enemy Corpse object


        // check to see if we're standing on a corpse. If not, it's easy
        if (!standingOnCorpse)
        {

            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            corpse.SetActive(true);

        }

        // Otherwise let's check each border position and only spawn the corpse if there's an available spot
        else
        {
            foreach (Vector3 borderPosition in borderPositions)
            {
                if (CheckPosition(borderPosition) == null)
                {
                    corpse.transform.position = borderPosition;

                    gameObject.GetComponent<BoxCollider2D>().enabled = false;

                    corpse.SetActive(true);
                }
            }
        }
    }

    public Collider2D CheckPosition(Vector3 checkPosition)
    {

        //LayerMask mask = ~(1 << LayerMask.NameToLayer("ObjectPlacementLayer")); // we want to ignore the placement layer that we used for creating objects  in each scene
        LayerMask mask = LayerMask.GetMask("Default", "Player"); //we only care about colliding on default for now, but we should add in other layers here if needed


        // Check at the origin point of the location we're checking (the center)
        Collider2D collision = Physics2D.OverlapCircle(checkPosition + new Vector3(0.5f, 0.5f, 0), 0.25f, mask);

        return collision;
    }

    public Collider2D[] CheckPositionAll(Vector3 checkPosition)
    {

        //LayerMask mask = ~(1 << LayerMask.NameToLayer("ObjectPlacementLayer")); // we want to ignore the placement layer that we used for creating objects  in each scene
        LayerMask mask = LayerMask.GetMask("Default", "Player"); //we only care about colliding on default for now, but we should add in other layers here if needed


        // Check at the origin point of the location we're checking (the center)
        Collider2D[] collisions = Physics2D.OverlapCircleAll(checkPosition + new Vector3(0.5f, 0.5f, 0), 0.25f, mask);

        return collisions;
    }

    public virtual void MoveEnemy(Vector2 direction)
    {
        if (behaviorState == BehaviorState.Dead)
        {
            return;
        }

        Vector3 potentialPosition = (Vector2)gameObject.transform.position + direction;


        Collider2D checkCollision = CheckPosition(potentialPosition);

        if (checkCollision == null)
        {
            gameObject.transform.position += (Vector3)direction;
        }


        else if (checkCollision.name == "Player" || checkCollision.tag == "enemy")
        {
            return;
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


        // else if (collision.tag == "room") { Debug.Log("hitting a room"); }

    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "room" && collision.gameObject.GetComponentInParent<Room>() != null && gameObject != null && behaviorState != BehaviorState.Dead)
        {
            behaviorState = BehaviorState.Idle;
            currentRoom = collision.gameObject.GetComponentInParent<Room>().roomId;
        }

        if (collision.gameObject.tag == "corpse")
        {
            // we are standing on a corpse. We need to move our body to another location
            standingOnCorpse = true;
        }
    }

    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "room" && collision.gameObject.GetComponentInParent<Room>() != null && gameObject != null && behaviorState != BehaviorState.Dead)
        {
            behaviorState = BehaviorState.Fleeing;
            currentRoom = collision.gameObject.GetComponentInParent<Room>().roomId;
        }

        if (collision.gameObject.tag == "corpse")
        {
            // we are standing on a corpse. We need to move our body to another location
            standingOnCorpse = false;
        }
    }

}



