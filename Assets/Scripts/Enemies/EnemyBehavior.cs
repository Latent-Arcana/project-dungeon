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
using static PlayerStats;
using UnityEditor.Experimental.GraphView;

public class EnemyBehavior : MonoBehaviour
{
    protected int HP;
    protected int SPD;
    protected int AGI;

    protected string type;

    public PlayerStatsManager playerStats;

    public PlayerMovement playerMovement;

    protected InputController input;
    protected DungeonNarrator dungeonNarrator;

    public int room;
    public int id;
    public bool playerInRoom;
    public Vector2Int originPoint;

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

    BehaviorState behaviorState;

    public enum BehaviorState
    {
        Hostile,
        Idle,
        Fleeing
    }


    
    // 0: Vector3 upPosition;
    // 1: Vector3 downPosition;
    // 2: Vector3 rightPosition;
    // 3: Vector3 leftPosition;

    Vector3[] borderPositions = new Vector3[4];


    public virtual void Awake()
    {

        input = GameObject.Find("InputController").GetComponent<InputController>();

        playerInRoom = false;

        behaviorState = BehaviorState.Idle;

        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();

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

    private void Input_OnPlayerMoved(object sender, PlayerMovement.MovementArgs e)
    {
        if(this.gameObject != null)
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

            else if(!playerInRoom && behaviorState != BehaviorState.Fleeing)
            {
                behaviorState = BehaviorState.Idle;
            }

            //Debug.Log($"{gameObject.name} is in the {behaviorState} state");


            CheckAndAttack(e.prevPosition);


            Move(e.position);
            Physics2D.SyncTransforms();
        }
        
    }

    private void CheckAndAttack(Vector3 prevPosition)
    {
        
        foreach(Vector3 position in borderPositions)
        {
            if(prevPosition == position)
            {
                Attack();
            }
        }
}

    private void PlayerMovement_OnRoomEnter(object sender, InputArgs e)
    {
        if(e.roomId == room && e.type == "enter")
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
        type = "slug";
        HP = 10;
        SPD = 1;
        AGI = 2;
    }
   
    public virtual void Move(Vector3 currentPlayerPosition)
    {

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

        else if(behaviorState == BehaviorState.Idle)
        {
            WanderRandomly();
        }

        else if(behaviorState == BehaviorState.Fleeing)
        {
            Vector3 originDirection = GetDirectionToTarget(new Vector3(originPoint.x, originPoint.y, 0));
            MoveEnemy(originDirection);
        }

    }

    public virtual void WanderRandomly()
    {
        // wander inside the room
        int randDir = UnityEngine.Random.Range(0, 4);

        if (randDir == 0)
        {
            MoveEnemy(Vector2.up);

        }
        else if (randDir == 1)
        {
            MoveEnemy(Vector2.right);

        }
        else if (randDir == 2)
        {
            MoveEnemy(Vector2.down);

        }
        else if (randDir == 3)
        {

            MoveEnemy(Vector2.left);

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
    public virtual void Attack()
    {


        string _enemyType = type;
        int _enemyId = id;

        int _enemyDamageDealt = UnityEngine.Random.Range(1, 5);
        int _playerDamageDealt = UnityEngine.Random.Range(1, 5);

        Die();

        //if(SPD > Player_Stats.SPD)
        //{
        //    Player_Stats.HP -= _enemyDamageDealt;
        //    HP -= _playerDamageDealt;

        //    string _combatText = "You were attacked by enemy " + id + " for " + _enemyDamageDealt + " damage. You dealt " + _playerDamageDealt + " damage to the enemy.";
        //    Dungeon_Narrator.AddDungeonNarratorText(_combatText);

        //    // TODO: Create a player took damage event that would actually register everywhere
        //    Player_Movement.StartCoroutine(Player_Movement.IncomingDamageFlash());

        //    StartCoroutine(IncomingDamageFlash());

        //}
        //else
        //{
        //    string _combatText = "You attacked enemy " + id + " for " + _enemyDamageDealt + " damage. The enemy dealt " + _enemyDamageDealt + " damage to you.";

        //    HP -= _playerDamageDealt;

        //    if (HP <= 0)
        //    {
        //        Die();
        //    }

        //    Player_Stats.HP -= _enemyDamageDealt;
        //    Dungeon_Narrator.AddDungeonNarratorText(_combatText);

        //    // TODO: Create a player took damage event that would actually register everywhere
        //    Player_Movement.StartCoroutine(Player_Movement.IncomingDamageFlash());

        //    StartCoroutine(IncomingDamageFlash());

        //}
    }

    public virtual void Die()
    {
        Debug.Log("The enemy " + id + " died.");

        Destroy(this.gameObject);

    }

    public Collider2D CheckPosition(Vector3 checkPosition)
    {

        LayerMask mask = ~(1 << LayerMask.NameToLayer("ObjectPlacementLayer")); // we want to ignore the placement layer that we used for creating objects  in each scene

        // Check at the origin point of the location we're checking (the center)
        Collider2D collision = Physics2D.OverlapCircle(checkPosition + new Vector3(0.5f, 0.5f, 0), 0.25f, mask);

        return collision;
    }

    public virtual void MoveEnemy(Vector2 direction)
    {
        Vector3 potentialPosition = (Vector2)gameObject.transform.position + direction;
        

        Collider2D checkCollision = CheckPosition(potentialPosition);

        if (checkCollision == null)
        {
            gameObject.transform.position += (Vector3)direction;
        }


        else if(checkCollision.name == "Player")
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


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "room" && collision.gameObject.GetComponentInParent<Room>() != null && gameObject != null)
        {
            behaviorState = BehaviorState.Idle;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "room" && collision.gameObject.GetComponentInParent<Room>() != null && gameObject != null)
        {
            behaviorState = BehaviorState.Fleeing;
        }
    }


}



